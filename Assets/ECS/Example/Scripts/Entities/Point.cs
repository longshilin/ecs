﻿using ME.ECS;
using UnityEngine;

public struct Point : IEntity {

    public Entity entity { get; set; }

    public Color color;
    public Vector3 position;
    public Vector3 scale;
    public float increaseRate;
    public float unitsCount;
    
}
