using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Drawing;
using System.Numerics;


// See https://aka.ms/new-console-template for more information
Console.WriteLine("Digital Logic Sim v0 to v1 converter");
Console.WriteLine("By Loganh4005");
Console.WriteLine("This program will convert every project you made, to a new build");
//Console.WriteLine("0: Official Version, 1:Community Version*"); // Community will come later


// Convert Chip.
string[] oldChips = { @"C:\Users\logan\AppData\LocalLow\Sebastian Lague\Digital Logic Sim\SaveData\ConvertProject\NAND.json", ""}; // Will be text in real this, set to json for debugging.
string[] oldWires = { @"C:\Users\logan\AppData\LocalLow\Sebastian Lague\Digital Logic Sim\SaveData\ConvertProject\WireLayout\NAND.json", "" };

// Get Chip Data
string oldSaveChipJson = System.IO.File.ReadAllText(oldChips[0]);
SavedChip oldSaveChip = JsonConvert.DeserializeObject<SavedChip>(oldSaveChipJson);
string oldSaveWireJson = System.IO.File.ReadAllText(oldWires[0]);
SavedWireLayout oldSaveWire = JsonConvert.DeserializeObject<SavedWireLayout>(oldSaveWireJson);

NewChip exportChip = new NewChip();
exportChip.Name = oldSaveChip.name;
exportChip.Colour = string.Format("#{0:X2}{1:X2}{2:X2}", (int)Math.Round(oldSaveChip.colour.r * 255), (int)Math.Round(oldSaveChip.colour.g * 255), (int)Math.Round(oldSaveChip.colour.b * 255));
// Setup Components
exportChip.InputPins = new List<InputPin>();
exportChip.OutputPins = new List<OutputPin>();
exportChip.SubChips = new List<SubChip>();



foreach (SavedComponentChip component in oldSaveChip.savedComponentChips)
{
    if (component.chipName == "SIGNAL IN")
    {
        InputPin convertedComponent = new InputPin();
        convertedComponent.Name = "Pin";
        if (component.outputPinNames[0] != "")
        {
            convertedComponent.Name = component.outputPinNames[0];
        }
        
        convertedComponent.ID = new Random().Next(); // In actual sim code, is set to instance id of object, which is a unity thing that i don't have idk id yet but soon maybe.
        convertedComponent.PositionY = component.posY;
        convertedComponent.ColourThemeName = "Red"; // Maybe customizable later
        exportChip.InputPins.Add(convertedComponent);
        component.newId = convertedComponent.ID;


    } else
    if (component.chipName == "SIGNAL OUT")
    {
        OutputPin convertedComponent = new OutputPin();
        convertedComponent.Name = component.inputPins[0].name;
        convertedComponent.ID = new Random().Next();
        convertedComponent.PositionY = component.posY;
        convertedComponent.ColourThemeName = "Red"; // Maybe customizable later
        exportChip.OutputPins.Add(convertedComponent);
        component.newId = convertedComponent.ID;

    } else // has to be chip
    {
        SubChip convertedComponent = new SubChip();
        convertedComponent.Name = component.chipName;
        convertedComponent.ID = new Random().Next();
        convertedComponent.Points = new List<Vector2>();
        convertedComponent.Points.Add(new Vector2((float)component.posX, (float)component.posY));
        convertedComponent.Data = null; // idk what data is, and idk if it's important.
        exportChip.SubChips.Add(convertedComponent);
        component.newId = convertedComponent.ID;
    }
}
/*

Converting wires

Pin Types:

Unassigned = 0,
ChipInputPin = 1,
ChipOutputPin = 2,
SubChipInputPin = 3,
SubChipOutputPin = 4
*/

exportChip.Connections = new List<Connection>();
for (int i = 0; i < oldSaveWire.serializableWires.Length; i++)
{
    SavedWire wire = oldSaveWire.serializableWires[i];
    Connection convertedWire = new Connection();
    convertedWire.WirePoints = wire.anchorPoints.ToList<Vector2>();
    // Create Source
    convertedWire.Source = new Source();
    if (oldSaveChip.savedComponentChips[wire.parentChipIndex].chipName == "SIGNAL IN")
    {
        convertedWire.Source.PinType = 1;
        convertedWire.Source.SubChipID = 0;
        convertedWire.Source.PinID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;
        
    } else
    if (oldSaveChip.savedComponentChips[wire.parentChipIndex].chipName == "SIGNAL OUT")
    {
        convertedWire.Source.PinType = 2;
        convertedWire.Source.SubChipID = 0;
        convertedWire.Source.PinID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;
    } else
    if (oldSaveChip.savedComponentChips[wire.parentChipIndex].inputPins[wire.parentChipOutputIndex] != null)
    {
        convertedWire.Source.PinType = 3;
        convertedWire.Source.SubChipID = wire.parentChipOutputIndex;
        convertedWire.Source.PinID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;
    } else
    {
        convertedWire.Source.PinType = 4;
        convertedWire.Source.SubChipID = wire.parentChipOutputIndex;
        convertedWire.Source.PinID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;
    }
    // Create Target
    convertedWire.Target = new Target();
    if (oldSaveChip.savedComponentChips[wire.parentChipIndex].chipName == "SIGNAL IN")
    {
        convertedWire.Target.PinType = 1;
        convertedWire.Target.PinID = 0;
        convertedWire.Target.SubChipID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;

    }
    else
    if (oldSaveChip.savedComponentChips[wire.parentChipIndex].chipName == "SIGNAL OUT")
    {
        convertedWire.Target.PinType = 2;
        convertedWire.Target.PinID = 0;
        convertedWire.Source.SubChipID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;
    }
    else
    if (oldSaveChip.savedComponentChips[wire.parentChipIndex].inputPins[wire.parentChipOutputIndex] != null)
    {
        convertedWire.Target.PinType = 3;
        convertedWire.Target.PinID = wire.parentChipOutputIndex;
        convertedWire.Target.SubChipID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;

    } else
    {
        convertedWire.Target.PinType = 4;
        convertedWire.Target.PinID = wire.parentChipOutputIndex;
        convertedWire.Target.SubChipID = oldSaveChip.savedComponentChips[wire.parentChipIndex].newId;
    }
    convertedWire.ColourThemeName = "Red";
    exportChip.Connections.Add(convertedWire);
    
}


string chipJson = JsonConvert.SerializeObject(exportChip);
Console.Write(chipJson);

// C:\Users\logan\Downloads\ExportedProject
System.IO.File.WriteAllText(@"C:\Users\logan\Downloads\ExportedProject\ConvertedChip.json", chipJson);

Console.ReadKey();


