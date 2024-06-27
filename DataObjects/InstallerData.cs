using Godot;
using System;

[GlobalClass, Tool]
public partial class InstallerData : Resource
{
	[Export] public string installPath = "";

	[Export] public bool KARDontShouldInstall = false;
	[Export] public bool GCAdapterDriversShouldInstall = false;
	[Export] public bool KWShouldAutoRun = true;
}
