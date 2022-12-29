using System.Collections;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
// Composite chip is a custom chip made up from other chips ("components")
public class SavedChip {

	public string name;
	public int creationIndex;
	public DoubleColor colour;
	public DoubleColor nameColour;

	// Names of all chips used as components in this new chip (each name appears only once)
	public string[] componentNameList;
	// Data about all the chips used as components in this chip (positions, connections, etc)
	// Array is ordered: first come input signals, then output signals, then remaining component chips
	public SavedComponentChip[] savedComponentChips;


}
public class DoubleColor
{
	public double r { get; set; }
	public double g { get; set; }
	public double b { get; set; }
	public double a { get; set; }
}