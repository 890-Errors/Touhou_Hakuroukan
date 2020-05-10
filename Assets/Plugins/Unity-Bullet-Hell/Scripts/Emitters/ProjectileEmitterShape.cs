﻿using UnityEngine;
using System.Collections.Generic;

namespace BulletHell
{
    // Emitter to fire pre-defined shape patterns
    public class ProjectileEmitterShape : ProjectileEmitterBase
    {
        [SerializeField]
        public GameObject ShapeTemplate;
        private List<Vector3> TemplatePositions;

        new void Awake()
        {
            base.Awake();

            if (ShapeTemplate == null)
            {
                ShapeTemplate = Resources.Load<GameObject>(@"ShapeTemplates\CircleShape");
            }

            TemplatePositions = new List<Vector3>();
            foreach (Transform child in ShapeTemplate.transform)
            {
                TemplatePositions.Add(child.transform.position);
            }
        }

        public override Pool<ProjectileData>.Node FireProjectile(Vector2 direction, float leakedTime)
        {
            Pool<ProjectileData>.Node node = new Pool<ProjectileData>.Node();

            if (Projectiles.AvailableCount >= TemplatePositions.Count)
            {
                for (int n = 0; n < TemplatePositions.Count; n++)
                {
                    node = Projectiles.Get();

                    node.Item.Position = transform.position + TemplatePositions[n];
                    node.Item.Scale = Scale;
                    node.Item.TimeToLive = TimeToLive - leakedTime;
                    node.Item.Velocity = Speed * Direction.normalized;
                    node.Item.Position += node.Item.Velocity * leakedTime;
                    node.Item.Color = new Color(0.6f, 0.7f, 0.6f, 1);
                    node.Item.Acceleration = Acceleration;
                }

                Direction = Rotate(Direction, RotationSpeed);
            }

            return node;
        }

        public new void UpdateEmitter(float tick)
        {
            base.UpdateEmitter(tick);
        }

        protected override void UpdateProjectile(ref Pool<ProjectileData>.Node node, float tick)
        {
            throw new System.NotImplementedException();
        }

        protected override void UpdateProjectiles(float tick)
        {
            throw new System.NotImplementedException();
        }
    }
}