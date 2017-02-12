using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_LevelDefinition : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		LevelDefinition data = (LevelDefinition)obj;

        // Add your writer.Write calls here.

        // Write the row data
        for (int i = data.levelGrid.Length - 1; i >= 0; --i) {
            writer.Write(new string(data.levelGrid[i]));
        }
        writer.Write("###");
	}
	
	public override object Read(ES2Reader reader)
	{
		LevelDefinition data = new LevelDefinition();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		LevelDefinition data = (LevelDefinition)c;

        // Add your reader.Read calls here to read the data into the object.
        List<string> levelRows = new List<string>();
        string row = reader.Read<string>();
        while (row != "###" && row != null) {
            levelRows.Insert(0, row);
            row = reader.Read<string>();
        }

        // Convert the strings to tile characters
        data.levelGrid = new char[levelRows.Count][];
        for (int i = 0; i < levelRows.Count; ++i) {
            data.levelGrid[i] = levelRows[i].ToCharArray();
        }        
    }
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_LevelDefinition():base(typeof(LevelDefinition)){}
}