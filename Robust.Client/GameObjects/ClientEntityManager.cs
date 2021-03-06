﻿using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Client.Interfaces.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.GameObjects.Components;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Client.GameObjects
{
    /// <summary>
    /// Manager for entities -- controls things like template loading and instantiation
    /// </summary>
    public sealed class ClientEntityManager : EntityManager, IClientEntityManager, IDisposable
    {
#pragma warning disable 649
        [Dependency] private readonly IMapManager _mapManager;
#pragma warning restore 649

        private int NextClientEntityUid = EntityUid.ClientUid + 1;

        public IEnumerable<IEntity> GetEntitiesInRange(GridCoordinates position, float range)
        {
            var aabb = new Box2(position.Position - new Vector2(range / 2, range / 2), position.Position + new Vector2(range / 2, range / 2));
            return GetEntitiesIntersecting(_mapManager.GetGrid(position.GridID).ParentMapId, aabb);
        }

        public IEnumerable<IEntity> GetEntitiesIntersecting(MapId mapId, Box2 position)
        {
            foreach (var entity in GetEntities())
            {
                var transform = entity.Transform;
                if (transform.MapID != mapId)
                    continue;

                if (entity.TryGetComponent<ICollidableComponent>(out var component))
                {
                    if (position.Intersects(component.WorldAABB))
                        yield return entity;
                }
                else
                {
                    if (position.Contains(transform.WorldPosition))
                    {
                        yield return entity;
                    }
                }
            }
        }

        public IEnumerable<IEntity> GetEntitiesIntersecting(MapId mapId, Vector2 position)
        {
            foreach (var entity in GetEntities())
            {
                var transform = entity.Transform;
                if (transform.MapID != mapId)
                    continue;

                if (entity.TryGetComponent<ICollidableComponent>(out var component))
                {
                    if (component.WorldAABB.Contains(position))
                        yield return entity;
                }
                else
                {
                    if (FloatMath.CloseTo(transform.GridPosition.X, position.X) && FloatMath.CloseTo(transform.GridPosition.Y, position.Y))
                    {
                        yield return entity;
                    }
                }
            }
        }

        public bool AnyEntitiesIntersecting(MapId mapId, Box2 box)
        {
            foreach (var entity in GetEntities())
            {
                var transform = entity.Transform;
                if (transform.MapID != mapId)
                    continue;

                if (entity.TryGetComponent<ICollidableComponent>(out var component))
                {
                    if (box.Intersects(component.WorldAABB))
                        return true;
                }
                else
                {
                    if (box.Contains(transform.WorldPosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void Startup()
        {
            base.Startup();

            if (Started)
            {
                throw new InvalidOperationException("Startup() called multiple times");
            }

            EntitySystemManager.Initialize();
            Started = true;
        }

        public void ApplyEntityStates(List<EntityState> curEntStates, IEnumerable<EntityUid> deletions, List<EntityState> nextEntStates)
        {
            var toApply = new Dictionary<IEntity, (EntityState, EntityState)>();
            var toInitialize = new List<Entity>();
            deletions = deletions ?? new EntityUid[0];

            if (curEntStates != null && curEntStates.Count != 0)
            {
                foreach (var es in curEntStates)
                {
                    //Known entities
                    if (Entities.TryGetValue(es.Uid, out var entity))
                    {
                        toApply.Add(entity, (es, null));
                    }
                    else //Unknown entities
                    {
                        var metaState = (MetaDataComponentState)es.ComponentStates.First(c => c.NetID == NetIDs.META_DATA);
                        var newEntity = CreateEntity(metaState.PrototypeId, es.Uid);
                        toApply.Add(newEntity, (es, null));
                        toInitialize.Add(newEntity);
                    }
                }
            }

            if (nextEntStates != null && nextEntStates.Count != 0)
            {
                foreach (var es in nextEntStates)
                {
                    if (Entities.TryGetValue(es.Uid, out var entity))
                    {
                        if (toApply.TryGetValue(entity, out var state))
                        {
                            toApply[entity] = (state.Item1, es);
                        }
                        else
                        {
                            toApply[entity] = (null, es);
                        }
                    }
                }
            }

            // Make sure this is done after all entities have been instantiated.
            foreach (var kvStates in toApply)
            {
                var ent = kvStates.Key;
                Entity entity = (Entity)ent;
                HandleEntityState(entity.EntityManager.ComponentManager, entity, kvStates.Value.Item1, kvStates.Value.Item2);
            }

            foreach (var id in deletions)
            {
                DeleteEntity(id);
            }

            foreach (var entity in toInitialize)
            {
                InitializeEntity(entity);
            }

            foreach (var entity in toInitialize)
            {
                StartEntity(entity);
            }
        }

        public void Dispose()
        {
            Shutdown();
        }

        public override IEntity CreateEntityUninitialized(string prototypeName)
        {
            return CreateEntity(prototypeName);
        }

        public override IEntity CreateEntityUninitialized(string prototypeName, GridCoordinates coordinates)
        {
            var newEntity = CreateEntity(prototypeName, NewClientEntityUid());
            if (coordinates.GridID != GridId.Invalid)
            {
                var gridEntityId = _mapManager.GetGrid(coordinates.GridID).GridEntityId;
                newEntity.Transform.AttachParent(GetEntity(gridEntityId));
                newEntity.Transform.LocalPosition = coordinates.Position;
            }
            return newEntity;
        }

        public override IEntity CreateEntityUninitialized(string prototypeName, MapCoordinates coordinates)
        {
            var newEntity = CreateEntity(prototypeName, NewClientEntityUid());
            if (coordinates.MapId != MapId.Nullspace)
            {
                newEntity.Transform.AttachParent(_mapManager.GetMapEntity(coordinates.MapId));
                newEntity.Transform.WorldPosition = coordinates.Position;
            }
            return newEntity;
        }

        public override IEntity SpawnEntity(string protoName, GridCoordinates coordinates)
        {
            return SpawnEntityNoMapInit(protoName, coordinates);
        }

        public override IEntity SpawnEntityNoMapInit(string protoName, GridCoordinates coordinates)
        {
            var newEnt = CreateEntityUninitialized(protoName, coordinates);
            InitializeAndStartEntity((Entity)newEnt);
            return newEnt;
        }

        public override IEntity SpawnEntityAt(string entityType, GridCoordinates coordinates)
        {
            return SpawnEntity(entityType, coordinates);
        }

        /// <inheritdoc />
        public override IEntity SpawnEntityAt(string entityType, MapCoordinates coordinates)
        {
            var grid = _mapManager.FindGridAt(coordinates);
            var gridCoords = new GridCoordinates(grid.WorldToLocal(coordinates.Position), grid);

            return SpawnEntityAt(entityType, gridCoords);
        }

        private EntityUid NewClientEntityUid()
        {
            return new EntityUid(NextClientEntityUid++);
        }

        private static void HandleEntityState(IComponentManager compMan, IEntity entity, EntityState curState,
            EntityState nextState)
        {
            var compStateWork = new Dictionary<uint, (ComponentState curState, ComponentState nextState)>();
            var entityUid = entity.Uid;

            if(curState?.ComponentChanges != null)
            {
                foreach (var compChange in curState.ComponentChanges)
                {
                    if (compChange.Deleted)
                    {
                        if (compMan.TryGetComponent(entityUid, compChange.NetID, out var comp))
                        {
                            compMan.RemoveComponent(entityUid, comp);
                        }
                    }
                    else
                    {
                        if (compMan.HasComponent(entityUid, compChange.NetID))
                            continue;

                        var newComp = (Component) IoCManager.Resolve<IComponentFactory>().GetComponent(compChange.ComponentName);
                        newComp.Owner = entity;
                        compMan.AddComponent(entity, newComp, true);
                    }
                }
            }

            if(curState?.ComponentStates != null)
            {
                foreach (var compState in curState.ComponentStates)
                {
                    compStateWork[compState.NetID] = (compState, null);
                }
            }

            if(nextState?.ComponentStates != null)
            {
                foreach (var compState in nextState.ComponentStates)
                {
                    if (compStateWork.TryGetValue(compState.NetID, out var state))
                    {
                        compStateWork[compState.NetID] = (state.curState, compState);
                    }
                    else
                    {
                        compStateWork[compState.NetID] = (null, compState);
                    }
                }
            }

            foreach (var kvStates in compStateWork)
            {
                if (!compMan.TryGetComponent(entityUid, kvStates.Key, out var component))
                {
                    DebugTools.Assert("Component does not exist for state.");
                    continue;
                }

                DebugTools.Assert(kvStates.Value.curState == null
                                  || kvStates.Value.curState.GetType() == component.StateType,
                    "Component state is of the wrong type.");

                component.HandleComponentState(kvStates.Value.curState, kvStates.Value.nextState);
            }
        }
    }
}
