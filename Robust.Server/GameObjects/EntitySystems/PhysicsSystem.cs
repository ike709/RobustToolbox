﻿using System.Collections.Generic;
using System.Linq;
using Robust.Server.Interfaces.Timing;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Map;
using Robust.Shared.Interfaces.Physics;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Robust.Server.GameObjects.EntitySystems
{
    internal class PhysicsSystem : EntitySystem
    {
#pragma warning disable 649
        [Dependency] private readonly IPauseManager _pauseManager;
        [Dependency] private readonly IPhysicsManager _physicsManager;
        [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager;
        [Dependency] private readonly IMapManager _mapManager;
#pragma warning restore 649

        private const float Epsilon = 1.0e-6f;

        public PhysicsSystem()
        {
            EntityQuery = new TypeEntityQuery(typeof(PhysicsComponent));
        }

        /// <inheritdoc />
        public override void Update(float frameTime)
        {
            var entities = EntityManager.GetEntities(EntityQuery);
            _physicsManager.BuildCollisionGrid();
            foreach (var entity in entities)
            {
                if (_pauseManager.IsEntityPaused(entity))
                {
                    continue;
                }

                HandleMovement(entity, frameTime);
            }

            foreach (var entity in entities)
            {
                DoMovement(entity, frameTime);
            }
        }

        private void HandleMovement(IEntity entity, float frameTime)
        {
            if (entity.Deleted)
            {
                // Ugh let's hope this fixes the crashes.
                return;
            }

            var velocity = entity.GetComponent<PhysicsComponent>();
            if (velocity.DidMovementCalculations)
            {
                velocity.DidMovementCalculations = false;
                return;
            }

            if (velocity.AngularVelocity == 0 && velocity.LinearVelocity == Vector2.Zero)
            {
                return;
            }

            var transform = entity.Transform;
            if (ContainerHelpers.IsInContainer(transform.Owner))
            {
                transform.Parent.Owner.SendMessage(transform, new RelayMovementEntityMessage(entity));
                velocity.LinearVelocity = Vector2.Zero;
                return;
            }

            var velocityConsumers = velocity.GetVelocityConsumers();
            var initialMovement = velocity.LinearVelocity;
            int velocityConsumerCount;
            float totalMass;
            Vector2 lowestMovement;
            do
            {
                velocityConsumerCount = velocityConsumers.Count;
                totalMass = 0;
                lowestMovement = initialMovement;
                var copy = new List<Vector2>(velocityConsumers.Count);
                foreach (var consumer in velocityConsumers)
                {
                    totalMass += consumer.Mass;
                    var movement = lowestMovement * velocity.Mass / totalMass;
                    consumer.AngularVelocity = velocity.AngularVelocity;
                    consumer.LinearVelocity = movement;
                    copy.Add(CalculateMovement(consumer, frameTime, consumer.Owner) / frameTime);
                }

                copy.Sort(LengthComparer);
                lowestMovement = copy[0];
                velocityConsumers = velocity.GetVelocityConsumers();
            } while (velocityConsumers.Count != velocityConsumerCount);

            velocity.ClearVelocityConsumers();

            foreach (var consumer in velocityConsumers)
            {
                consumer.LinearVelocity = lowestMovement;
                consumer.DidMovementCalculations = true;
            }

            velocity.DidMovementCalculations = false;
        }

        private static void DoMovement(IEntity entity, float frameTime)
        {
            // TODO: Terrible hack to fix bullets crashing the server.
            // Should be handled with deferred physics events instead.
            if (entity.Deleted)
            {
                return;
            }

            var velocity = entity.GetComponent<PhysicsComponent>();

            if (velocity.LinearVelocity.LengthSquared < Epsilon && velocity.AngularVelocity < Epsilon)
                return;

            float angImpulse = 0;
            if (velocity.AngularVelocity > Epsilon)
            {
                angImpulse = velocity.AngularVelocity * frameTime;
            }

            var transform = entity.Transform;
            transform.LocalRotation += angImpulse;
            transform.WorldPosition += velocity.LinearVelocity * frameTime;
        }

        private Vector2 CalculateMovement(PhysicsComponent velocity, float frameTime, IEntity entity)
        {
            if (velocity.Deleted)
            {
                // Help crashes.
                return default;
            }

            var movement = velocity.LinearVelocity * frameTime;
            if (movement.LengthSquared <= Epsilon)
            {
                return Vector2.Zero;
            }

            //Check for collision
            if (entity.TryGetComponent(out CollidableComponent collider))
            {
                var collided = collider.TryCollision(movement, true);

                if (collided)
                {
                    if (velocity.EdgeSlide)
                    {
                        //Slide along the blockage in the non-blocked direction
                        var xBlocked = collider.TryCollision(new Vector2(movement.X, 0));
                        var yBlocked = collider.TryCollision(new Vector2(0, movement.Y));

                        movement = new Vector2(xBlocked ? 0 : movement.X, yBlocked ? 0 : movement.Y);
                    }
                    else
                    {
                        //Stop movement entirely at first blockage
                        movement = new Vector2(0, 0);
                    }
                }

                if (movement != Vector2.Zero && collider.IsScrapingFloor)
                {
                    var location = entity.Transform;
                    var grid = _mapManager.GetGrid(location.GridPosition.GridID);
                    var tile = grid.GetTileRef(location.GridPosition);
                    var tileDef = _tileDefinitionManager[tile.Tile.TypeId];
                    if (tileDef.Friction != 0)
                    {
                        movement -= movement * tileDef.Friction;
                        if (movement.LengthSquared <= velocity.Mass * Epsilon / (1 - tileDef.Friction))
                        {
                            movement = Vector2.Zero;
                        }
                    }
                }
            }

            return movement;
        }

        private static readonly IComparer<Vector2> LengthComparer =
            Comparer<Vector2>.Create((a, b) => a.LengthSquared.CompareTo(b.LengthSquared));
    }
}
