using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public CharacterCustomization characterCustomization;
}

[Serializable]
public class Part
{
    public string name;
    public Position pos;
    public List<Sprite> sprites;
    public float scale;

    public GameObject instance;
}


[Serializable]
public class Position
{
    public float y;
    public Position pos;
}
[Serializable]
public class CharacterCustomization
{
    public List<Part> parts;
}
