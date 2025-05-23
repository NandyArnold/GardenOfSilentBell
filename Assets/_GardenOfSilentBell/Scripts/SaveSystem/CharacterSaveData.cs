using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CharacterSaveData
{
    public Vector3 position;
    public string sceneName;
    public List<string> skills;
    public int chargesLeft;
    public float cooldownRemaining;
}
