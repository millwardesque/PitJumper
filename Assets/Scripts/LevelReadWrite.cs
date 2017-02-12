using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class LevelReadWrite {
    public static List<LevelDefinition> ReadLevelDefinitions(string levelFilename) {
        List<LevelDefinition> levels = new List<LevelDefinition>();

        List<char[]> levelData = new List<char[]>();

        Regex endLevelPattern = new Regex(@"^###");
        Regex commentPattern = new Regex(@"^#");
        TextAsset levelText = Resources.Load<TextAsset>(levelFilename);
        if (!levelText) {
            Debug.LogError("Error: Unable to load level from '" + levelFilename + "': Resource doesn't exist");
            return levels;
        }
        string rawLevelString = levelText.text;
        string[] rows = rawLevelString.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rows.Length; ++i) {
            string row = rows[i];
            if (endLevelPattern.IsMatch(row)) {
                levels.Add(new LevelDefinition(levelData.ToArray()));
                levelData = new List<char[]>();
            }
            else if (commentPattern.IsMatch(row)) {
                // Do nothing.
            }
            else {
                char[] levelRow = row.ToCharArray();
                levelData.Insert(0, levelRow);
            }
        }

        return levels;
    }

    public static void WriteLevelDefinition(string levelFilename, LevelDefinition level, int levelIndex) {
        string taggedFilename = levelFilename + "?tag=Level" + levelIndex;
        ES2.Save<LevelDefinition>(level, taggedFilename);
    }

    public static void WriteAllLevelDefinitions(string levelFilename, List<LevelDefinition> levels) {
        for (int i = 0; i < levels.Count; ++i) {
            ES2.Save<LevelDefinition>(levels[i], levelFilename + "?tag=Level" + i);
        }
    }
}
