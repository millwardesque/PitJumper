using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class LevelReadWrite {
    public static List<LevelDefinition> ReadLevelDefinitions(string levelFilename) {
        List<LevelDefinition> levels = new List<LevelDefinition>();

        List<string[]> levelData = new List<string[]>();

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
                levelData = new List<string[]>();
            }
            else if (commentPattern.IsMatch(row)) {
                // Do nothing.
            }
            else {
				Regex whitespacePattern = new Regex(@"\s+");
				row = whitespacePattern.Replace (row, " ");
				string[] levelRow = row.Split (new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
                levelData.Insert(0, levelRow);
            }
        }

        return levels;
    }
}
