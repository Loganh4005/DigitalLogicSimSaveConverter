using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;


// pls opensource v1.0
[System.Serializable]
public class Connection
{
    public Source Source { get; set; }
    public Target Target { get; set; }
    public List<Vector2> WirePoints { get; set; }
    public string ColourThemeName { get; set; }
}

[System.Serializable]
public class InputPin
{
    public string Name { get; set; }
    public int ID { get; set; }
    public double PositionY { get; set; }
    public string ColourThemeName { get; set; }
}

[System.Serializable]
public class OutputPin
{
    public string Name { get; set; }
    public int ID { get; set; }
    public double PositionY { get; set; }
    public string ColourThemeName { get; set; }
}

[System.Serializable]
public class NewChip
{
    public string Name { get; set; }
    public string Colour { get; set; }
    public List<InputPin> InputPins { get; set; }
    public List<OutputPin> OutputPins { get; set; }
    public List<SubChip> SubChips { get; set; }
    public List<Connection> Connections { get; set; }
}

[System.Serializable]
public class Source
{
    public int PinType { get; set; }
    public int SubChipID { get; set; }
    public int PinID { get; set; }
}

[System.Serializable]
public class SubChip
{
    public string Name { get; set; }
    public int ID { get; set; }
    public List<Vector2> Points { get; set; }
    public object Data { get; set; }
}

[System.Serializable]
public class Target
{
    public int PinType { get; set; }
    public int SubChipID { get; set; }
    public int PinID { get; set; }
}



