using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class SavedInputPin
{
	public string name;
	// An input pin receives its input from one of the output pins of some chip (called the parent chip)
	// The chipIndex is the chip's index in the array of chips being written to file
	public int parentChipIndex;
	public int parentChipOutputIndex;
	public bool isCylic;


}