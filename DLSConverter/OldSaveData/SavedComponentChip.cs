using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SavedComponentChip {
	public string chipName;
	public double posX;
	public double posY;

	public SavedInputPin[] inputPins;
	public string[] outputPinNames;
	public int newId;


}