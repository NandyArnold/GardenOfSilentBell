using System.Collections.Generic;

public class GameStateData
{
    public List<string> unlockedCharacters;
    public Dictionary<string, CharacterSaveData> characterStates;
    public Dictionary<string, bool> puzzleStates; // e.g. "Level1_LeverA" = true
    public string currentScene;
}

