using Godot;
using System;

public partial class CheckBoxUpdater : Node2D
{
	[Export] InstallerData installerData;

	private void _on_gc_adapter_checkbox_toggled(bool toggled_on)
	{
		installerData.GCAdapterDriversShouldInstall = toggled_on;
	}


	private void _on_kar_dont_checkbox_toggled(bool toggled_on)
	{
		installerData.KARDontShouldInstall = toggled_on;
	}


	private void _on_auto_start_checkbox_toggled(bool toggled_on)
	{
		installerData.KWShouldAutoRun = toggled_on;
	}

}
