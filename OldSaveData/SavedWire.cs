using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Drawing;


[System.Serializable]
public class SavedWire {
	public int parentChipIndex;
	public int parentChipOutputIndex;
	public int childChipIndex;
	public int childChipInputIndex;
	public Vector2[] anchorPoints;


}