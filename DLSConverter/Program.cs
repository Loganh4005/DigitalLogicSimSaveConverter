using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Drawing;
using System.Numerics;
using System.IO;

string buildVersion = "1.0"; // DO NOT CHANGE UNLESS UPDATING PROGRAM


string colorSelected;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Digital Logic Sim v0 to v1 converter");
Console.WriteLine("By Loganh4005");
Console.WriteLine("This program will convert every project you made, to a new build");
Console.WriteLine("Choose Line Color:");
Console.WriteLine($"R: Red, Y: Yellow, G: Green, B: Blue, I: Indigo");

ConsoleKey colorKeyPressed = Console.ReadKey().Key;
Console.WriteLine();

if (colorKeyPressed == ConsoleKey.R)
{
    colorSelected = "Red";
    Console.WriteLine("Red Selected");
} else
if (colorKeyPressed == ConsoleKey.Y)
{
    colorSelected = "Yellow";
    Console.WriteLine("Yellow Selected");
}
else
if (colorKeyPressed == ConsoleKey.G)
{
    colorSelected = "Green";
    Console.WriteLine("Green Selected");
}
else
if (colorKeyPressed == ConsoleKey.B)
{
    colorSelected = "Blue";
    Console.WriteLine("Blue Selected");
}
else
if (colorKeyPressed == ConsoleKey.I)
{
    colorSelected = "Indigo";
    Console.WriteLine("Indigo Selected");
}
else
{
    colorSelected = "Red";
    Console.WriteLine("No color selected, defaulting to red");
}

Console.WriteLine("Converting...");
string[] projects = Directory.GetDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..\\LocalLow\\Sebastian Lague\\Digital Logic Sim\\SaveData"));
string newData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..\\LocalLow\\SebastianLague\\Digital Logic Sim\\V1\\Projects");


foreach (string projectPath in projects)
{
    ProjectSettings projectSettings = new ProjectSettings();
    projectSettings.ProjectName = Path.GetFileName(projectPath);
    Console.WriteLine("Converting Project " + projectSettings.ProjectName);
    projectSettings.BuildVersion = buildVersion;
    projectSettings.CreationTime = new FileInfo(projectPath).CreationTime;
    string[] chips = Directory.GetFiles(projectPath);
    List<string> convertedChips = new List<string>();
    string newProjectDirectory = Path.Combine(newData, projectSettings.ProjectName);
    Directory.CreateDirectory(newProjectDirectory + "/Chips/");
    foreach (string chip in chips)
    {
        Console.WriteLine($"Converting Chip {Path.GetFileName(chip)}");
        string json = ConvertChip(chip, Path.Combine(projectPath, $"WireLayout/{Path.GetFileName(chip)}"));
        convertedChips.Add(Path.GetFileNameWithoutExtension(chip));
        Console.WriteLine("Saving Chip...");
        File.WriteAllText(newProjectDirectory + "/Chips/" + Path.GetFileNameWithoutExtension(chip) + ".json", json);

    }
    
    projectSettings.AllCreatedChips = convertedChips;
    convertedChips.Insert(0, "AND");
    convertedChips.Insert(1, "NOT");
    projectSettings.StarredChips = convertedChips;

    projectSettings.DisplayOptions = new DisplayOptions();
    projectSettings.DisplayOptions.MainChipPinNameDisplayMode = DisplayOptions.PinNameDisplayMode.Hover;
    projectSettings.DisplayOptions.SubChipPinNameDisplayMode = DisplayOptions.PinNameDisplayMode.Hover;
    projectSettings.DisplayOptions.ShowCursorGuide = DisplayOptions.ToggleState.On;

    Console.WriteLine($"Saving Project...");
    File.WriteAllText(newProjectDirectory + "/ProjectSettings.json", JsonConvert.SerializeObject(projectSettings, Formatting.Indented));

}

//System.IO.File.WriteAllText(@"C:\Users\logan\Downloads\ExportedProject\ConvertedChip.json", );

//Console.WriteLine("0: Official Version, 1:Community Version*"); // Community will come later



string ConvertChip(string chipPath, string wirePath)
{
    // Get Chip Data
    string oldSaveChipJson = System.IO.File.ReadAllText(chipPath);
    string oldSaveWireJson = System.IO.File.ReadAllText(wirePath);
    SavedChip oldSaveChip = JsonConvert.DeserializeObject<SavedChip>(oldSaveChipJson);
    SavedWireLayout oldSaveWire = JsonConvert.DeserializeObject<SavedWireLayout>(oldSaveWireJson);

    // Define Chip
    NewChip exportChip = new NewChip();

    // General Conversions
    exportChip.Name = oldSaveChip.name;
    exportChip.Colour = string.Format("#{0:X2}{1:X2}{2:X2}", (int)Math.Round(oldSaveChip.colour.r * 255), (int)Math.Round(oldSaveChip.colour.g * 255), (int)Math.Round(oldSaveChip.colour.b * 255));

    // Setup Components
    exportChip.InputPins = new List<InputPin>();
    exportChip.OutputPins = new List<OutputPin>();
    exportChip.SubChips = new List<SubChip>();


    //Convert Components (Chips, I/O)

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
            convertedComponent.ColourThemeName = colorSelected; // Maybe customizable later
            exportChip.InputPins.Add(convertedComponent);
            component.newId = convertedComponent.ID;


        } else
        if (component.chipName == "SIGNAL OUT")
        {
            OutputPin convertedComponent = new OutputPin();
            convertedComponent.Name = component.inputPins[0].name;
            convertedComponent.ID = new Random().Next();
            convertedComponent.PositionY = component.posY;
            convertedComponent.ColourThemeName = colorSelected; // Maybe customizable later
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
        convertedWire.ColourThemeName = colorSelected;
        exportChip.Connections.Add(convertedWire);
        
    }
    return JsonConvert.SerializeObject(exportChip, Formatting.Indented);
}


//Console.Write(chipJson);

// C:\Users\logan\Downloads\ExportedProject

Console.WriteLine("Done! Pray that it worked and didn't wipe your old things");
Console.ReadKey();


