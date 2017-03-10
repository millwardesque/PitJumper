using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class LevelReadWrite {
    public static List<LevelDefinition> ReadLevelDefinitions(string levelFilename) {
        List<LevelDefinition> levels = new List<LevelDefinition>();

        List<string[]> levelData = new List<string[]>();
		string levelName = "<unnamed level>";
		Color levelAmbientColour = Color.cyan;
		Color levelBackgroundColour = Color.cyan;
		float playerLightSize = 1.0f;

        Regex endLevelPattern = new Regex(@"^###");
        Regex commentPattern = new Regex(@"^#");
		Regex metaBlockPattern = new Regex (@"^\[meta\]");
		Regex tilesBlockPattern = new Regex (@"^\[tiles\]");
        TextAsset levelText = Resources.Load<TextAsset>(levelFilename);

		string section = "";
        if (!levelText) {
            Debug.LogError("Error: Unable to load level from '" + levelFilename + "': Resource doesn't exist");
            return levels;
        }
        string rawLevelString = levelText.text;
        string[] rows = rawLevelString.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < rows.Length; ++i) {
            string row = rows[i];
			if (endLevelPattern.IsMatch (row)) {
				// Create the new level definition based on the collected data.
				LevelDefinition level = new LevelDefinition (levelData.ToArray ());
				level.name = levelName;
				level.ambientLightColour = levelAmbientColour;
				level.backgroundColour = levelBackgroundColour;
				level.playerLightSize = playerLightSize;
				levels.Add (level);

				// Reset the file data variables.
				section = "";
				levelData = new List<string[]> ();
				levelName = "<unnamed level>";
				levelAmbientColour = Color.cyan;
				levelBackgroundColour = Color.cyan;
				playerLightSize = 1.0f;
			} else if (commentPattern.IsMatch (row)) {
				// Do nothing.
			} else if (metaBlockPattern.IsMatch (row)) {
				section = "meta";
			} else if (tilesBlockPattern.IsMatch (row)) {
				section = "tiles";
			}
            else {
				Regex whitespacePattern = new Regex(@"\s+");
				row = whitespacePattern.Replace (row, " ");

				if (section == "tiles") {
					string[] levelRow = row.Split (new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
					levelData.Insert (0, levelRow);
				} else if (section == "meta") {
					string[] metaRow = row.Split (new string[] { ":" }, System.StringSplitOptions.RemoveEmptyEntries);
					switch (metaRow [0]) {
					case "name":
						levelName = metaRow [1];
						break;
					case "ambientLightColour":
						string[] ambientComponents = metaRow [1].Split (new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
						levelAmbientColour = new Color (float.Parse(ambientComponents [0]), float.Parse(ambientComponents [1]), float.Parse(ambientComponents [2]), float.Parse(ambientComponents [3]));
						break;
					case "backgroundColour":
						string[] backgroundComponents = metaRow [1].Split (new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
						levelBackgroundColour = new Color (float.Parse(backgroundComponents [0]), float.Parse(backgroundComponents [1]), float.Parse(backgroundComponents [2]), float.Parse(backgroundComponents [3]));
						break;
					case "playerLightSize":
						playerLightSize = float.Parse (metaRow [1]);
						break;
					default:
						Debug.Log ("Unknown meta configuration directive: '" + row + "'");
						break;
					}
				}
			}
        }

        return levels;
    }
}
