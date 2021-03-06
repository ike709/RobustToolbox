﻿using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Physics
{
    [Serializable, NetSerializable]
    public class PhysShapeRect : IPhysShape
    {
        private int _collisionLayer;
        private int _collisionMask;
        private Box2 _rectangle = Box2.UnitCentered;

        public Box2 Rectangle
        {
            get => _rectangle;
            set => _rectangle = value;
        }

        /// <inheritdoc />
        [ViewVariables(VVAccess.ReadWrite)]
        public int CollisionLayer
        {
            get => _collisionLayer;
            set => _collisionLayer = value;
        }

        /// <inheritdoc />
        [ViewVariables(VVAccess.ReadWrite)]
        public int CollisionMask
        {
            get => _collisionMask;
            set => _collisionMask = value;
        }

        public void ExposeData(ObjectSerializer serializer)
        {
            serializer.DataField(ref _collisionLayer, "layer", 0);
            serializer.DataField(ref _collisionMask, "mask", 0);
            serializer.DataField(ref _rectangle, "bounds", Box2.UnitCentered);
        }

        public Box2 CalculateLocalBounds(Angle rotation)
        {
            var rect = new Box2Rotated(_rectangle, rotation);
            return rect.CalcBoundingBox();
        }
    }
}
