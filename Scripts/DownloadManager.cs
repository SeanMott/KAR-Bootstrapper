using Godot;
using System;
using System.IO;
using System.Diagnostics;
using System.Data.Common;

public partial class DownloadManager : Node2D
{
	[Export] InstallerData installerData;

	Vector2 lastPos; //the last position of a text notification

	//the various font color items
	[Export] Theme itemHasFinished_FontColor;
	[Export] Theme itemIsDownloading_FontColor;
	[Export] Theme systemEventInProgess_FontColor;

	KWQI KWStructureBlob = new KWQI();
	KWQI KARphinBlob = new KWQI();

	KWQI SkinPackBlob = new KWQI();

	KWQI ROMBlob = new KWQI();

	KWQI KARDontBlob = new KWQI();
	KWQI GCAdapterBlob = new KWQI();

	//KARDon't
	string KARDont_DownloadURL = "https://github.com/SeanMott/KARDont/releases/download/1.0.0-stable/KARDont.7z";

	string ROMBlobURL = "https://github.com/SeanMott/KAR-Workshop/releases/download/KAR-Installs_Netplay_1/ROMs.tar.br"; //the URL for the ROMs on Windows

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		VisibilityChanged += StartDownloads;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//generates the folder structure
	void GenerateFolderStructure()
	{
		//---whereever KARphin and Workshop are chosen to download---
		
		//generate a folder for all the settings
		if (!Directory.Exists(installerData.installPath + "/KWData"))
			Directory.CreateDirectory(installerData.installPath + "/KWData");

		//we generate a Dolphin client's folder to put all KARphin versions in
		if (!Directory.Exists(installerData.installPath + "/Clients"))
			Directory.CreateDirectory(installerData.installPath + "/Clients");

		//we generate a Mod Tools folder to put any Mod tools in.
		//ie Blender, DATManager
		if (!Directory.Exists(installerData.installPath + "/Tools"))
			Directory.CreateDirectory(installerData.installPath + "/Tools");

		//we also generate a Replay folder for all replays
		if (!Directory.Exists(installerData.installPath + "/Replays"))
			Directory.CreateDirectory(installerData.installPath + "/Replays");

		//---whereever the mod folder is chosen---

		//we create a ROMs folder for all ROMs
		if (!Directory.Exists(installerData.installPath + "/ROMs"))
			Directory.CreateDirectory(installerData.installPath + "/ROMs");

		//we also create a KWQI folder for all kWQI files
		if (!Directory.Exists(installerData.installPath + "/KWQI"))
			Directory.CreateDirectory(installerData.installPath + "/KWQI");

		//we create a Mods  folder for all Mods
		if (!Directory.Exists(installerData.installPath + "/Mods"))
			Directory.CreateDirectory(installerData.installPath + "/Mods");

		//we create a Mods  folder for all Mods
		if (!Directory.Exists(installerData.installPath + "/Mods"))
			Directory.CreateDirectory(installerData.installPath + "/Mods");
	}

	//generates the settings files and lets KARphin and Workshop know about eachother
	//and the location of ROMs
	void GenerateKWSettings()
	{
		//user settings
		Godot.Collections.Dictionary KWSettings_User = new Godot.Collections.Dictionary();
		KWSettings_User.Add("displayName", "Hey Now Star");
		KWSettings_User.Add("discordHandle", "Hey Now");
		KWSettings_User.Add("geoLocation", "NA");
		
		System.IO.StreamWriter file = new System.IO.StreamWriter(installerData.installPath + "/KWData/User.KWU");
		file.WriteLine(Json.Stringify(KWSettings_User));
		file.Close();

		//mod settings
		Godot.Collections.Dictionary KWSettings_Mod = new Godot.Collections.Dictionary();
		KWSettings_Mod.Add("RomDir", installerData.installPath + "/ROMs");
		KWSettings_Mod.Add("TexturePackDir", installerData.installPath + "/TexturePacks");
		KWSettings_Mod.Add("AudioPackDir", installerData.installPath + "/AudioPacks");
		KWSettings_Mod.Add("KWQIDir", installerData.installPath + "/KWQI");
		KWSettings_Mod.Add("ToolsDir", installerData.installPath + "/Tools");

		file = new System.IO.StreamWriter(installerData.installPath + "/KWData/ModSettings.KWM");
		file.WriteLine(Json.Stringify(KWSettings_Mod));
		file.Close();

		//netplay settings
		Godot.Collections.Dictionary KWSettings_Netplay = new Godot.Collections.Dictionary();
		KWSettings_Netplay.Add("user", installerData.installPath + "/KWData/user.KWU");
		KWSettings_Netplay.Add("RomDir",installerData.installPath + "/ROMs");
		KWSettings_Netplay.Add("NetClientsDir", installerData.installPath + "/Clients");
		KWSettings_Netplay.Add("ReplaysDir", installerData.installPath + "/Replays");
		KWSettings_Netplay.Add("KWQIDir", installerData.installPath + "/KWQI");

		file = new System.IO.StreamWriter(installerData.installPath + "/KWData/Netplay.KWN");
		file.WriteLine(Json.Stringify(KWSettings_Netplay));
		file.Close();
	}

	//adds a new text notification
	private void AddTextNotification(string text, Theme theme)
	{
		//creates a text
		Label textNotify = new Label();
		textNotify.Position = lastPos;
		textNotify.Text = text;
		textNotify.Theme = theme;
		AddChild(textNotify);

		//increment position
		lastPos.Y += 20.0f;
	}

	//starts downloading
	public void StartDownloads()
	{
		//generate folder structures
		//GenerateFolderStructure();
		//write KAR Workshop settings and other files
		//GenerateKWSettings();

		//download the base KAR Netplay blob package
		bool structureBlobIsDone = false;
		Process p;
		//KWStructureBlob.ContentDownloadURL_Windows = "https://github.com/SeanMott/KAR-Workshop/releases/download/KAR-Installs_Netplay_1/KWStructure.tar.br";
		//KWStructureBlob.author = "Jas";
		//KWStructureBlob.contentVersion = "1.0.0";
		//KWStructureBlob.date = "08-09-2024";
		//KWStructureBlob.displayName = "KW Structure Blob";
		//KWStructureBlob.internalName = "KWStructureBlob";
		//KWStructureBlob.doesItOnlyWorkOnASpecificOS = false;
		//KWQI.WriteKWQI(installerData.installPath, "KWStructureBlob", KWStructureBlob);
		KWStructureBlob = KWQI.LoadKWQI("KWQI/KWStructureBlob.KWQI");
		KWQIPackaging.DownloadContent_Archive_Windows(out p, "SupportProgs/Windows/Duma.exe", KWStructureBlob.internalName, KWStructureBlob.ContentDownloadURL_Windows,
		installerData.installPath);
		AddTextNotification("Netplay Boilerplate Blob download started...", itemIsDownloading_FontColor);

		//extra goodies download flags
		bool skinPackIsDone = false;
		bool ROMsIsDone = false;
		bool KARDontIsDone = false;
		bool GCAdapterDriverIsDone = false;
		Process skinPack = new Process();
		Process ROMs = new Process();
		Process KARDont = new Process();
		Process GCAdapterDriver = new Process();

		//process each of the downloads
		bool doneDownloading = false;
		while(!doneDownloading)
		{
			//if structure blob is done
			if(!structureBlobIsDone && p.HasExited)
			{
				//validate the file completed
				AddTextNotification("Netplay Boilerplate Blob done downloading, unpacking...", itemHasFinished_FontColor);

				//unpacks the blob
				KWQIPackaging.UnpackArchive_Windows(installerData.installPath, KWStructureBlob.internalName, installerData.installPath, true,
				"SupportProgs/Windows/brotli.exe");

				AddTextNotification("Netplay Boilerplate Blob aquired", itemHasFinished_FontColor);

				//copy the contents into the install directory and delete the package folder
				string KWStructurePackageFolder = installerData.installPath + "/KWStructure";
				string[] entries = Directory.GetFileSystemEntries(KWStructurePackageFolder);
				foreach (string fileOrDir in entries)
				{
					Directory.Move(fileOrDir, Path.Combine(installerData.installPath, 
					fileOrDir.Substring(KWStructurePackageFolder.Length + 1)));
				}
				Directory.Delete(KWStructurePackageFolder);

				AddTextNotification("Folder Structure generated", systemEventInProgess_FontColor);

				structureBlobIsDone = true;

				//kicks off the other downloads

				///kicks off the ROMs since they're the largest item to down
				if(installerData.downloadRoms)
				{
					//KWQIPackaging.DownloadContent_Archive_Windows(out ROMs, "SupportProgs/Windows/Duma.exe", "ROMs", ROMBlobURL,
					//installerData.installPath);

					ROMsIsDone = true;
				}
				else
				{
					ROMsIsDone = true;
				}

				//kicks off any skin packs
				if(installerData.downloadSkinPacks)
				{
					//SkinPackBlob.ContentDownloadURL_Windows = "https://github.com/SeanMott/KAR-Workshop/releases/download/KAR-Installs_Netplay_1/SkinPacks.tar.br";
					//SkinPackBlob.author = "Jas";
					//SkinPackBlob.contentVersion = "1.0.0";
					//SkinPackBlob.date = "08-09-2024";
					//SkinPackBlob.displayName = "Backside Skin Packs";
					//SkinPackBlob.internalName = "SkinPacks";
					//SkinPackBlob.doesItOnlyWorkOnASpecificOS = false;
					//KWQI.WriteKWQI(installerData.installPath, "BacksideSkinPacks", SkinPackBlob);
					SkinPackBlob = KWQI.LoadKWQI(installerData.installPath + "/KWQI/BacksideSkinPacks.KWQI");

					KWQIPackaging.DownloadContent_Archive_Windows(out skinPack, "SupportProgs/Windows/Duma.exe", SkinPackBlob.internalName, SkinPackBlob.ContentDownloadURL_Windows,
					installerData.installPath);

					//skinPackIsDone = true;
				}
				else
				{
					skinPackIsDone = true;
				}

				//GC Adapter Driver
				if(installerData.GCAdapterDriversShouldInstall)
				{
					//KWQIPackaging.DownloadContent_Archive_Windows(out ROMs, "SupportProgs/Windows/Duma.exe", "ROMs", ROMBlobURL,
					//installerData.installPath);
					GCAdapterDriverIsDone = true;
				}
				else
				{
					GCAdapterDriverIsDone = true;
				}

				//KARdont
				if(installerData.KARDontDownload)
				{
					//KWQIPackaging.DownloadContent_Archive_Windows(out ROMs, "SupportProgs/Windows/Duma.exe", "ROMs", ROMBlobURL,
					//installerData.installPath);
					KARDontIsDone = true;
				}
				else
				{
					KARDontIsDone = true;
				}
			}

			//if ROMs are done
			/*if(structureBlobIsDone && !ROMsIsDone && ROMs.HasExited)
			{
				//validate the file completed
				AddTextNotification("ROMs Downloaded, unpacking...", itemHasFinished_FontColor);

				//unpacks the blob
				KWQIPackaging.UnpackArchive_Windows(installerData.installPath, "ROMs-package", installerData.installPath, true,
				"SupportProgs/Windows/brotli.exe");

				AddTextNotification("ROMs aquired", itemHasFinished_FontColor);

				//copy the contents into the install directory and delete the package folder
				string KWStructurePackageFolder = installerData.installPath + "/ROMs-package";
				string[] entries = Directory.GetFileSystemEntries(KWStructurePackageFolder);
				foreach (string fileOrDir in entries)
				{
					Directory.Move(fileOrDir, Path.Combine(installerData.installPath + "/ROMs", 
					fileOrDir.Substring(KWStructurePackageFolder.Length + 1)));
				}
				Directory.Delete(KWStructurePackageFolder);

				AddTextNotification("ROMs Gotten", systemEventInProgess_FontColor);

				ROMsIsDone = true;
			}*/

			//if Skin Packs are done
			if(structureBlobIsDone && !skinPackIsDone && skinPack.HasExited)
			{
				//validate the file completed
				AddTextNotification("Skins Downloaded, unpacking...", itemHasFinished_FontColor);

				//unpacks the blob
				KWQIPackaging.UnpackArchive_Windows(installerData.installPath, SkinPackBlob.internalName, installerData.installPath, true,
				"SupportProgs/Windows/brotli.exe");

				AddTextNotification("Skins aquired", itemHasFinished_FontColor);

				//copy the contents into the install directory and delete the package folder
				string KWStructurePackageFolder = installerData.installPath + "/SkinPacks";
				string[] entries = Directory.GetFileSystemEntries(KWStructurePackageFolder);
				foreach (string fileOrDir in entries)
				{
					Directory.Move(fileOrDir, Path.Combine(installerData.installPath + "/Mods/SkinPacks", 
					fileOrDir.Substring(KWStructurePackageFolder.Length + 1)));
				}
				Directory.Delete(KWStructurePackageFolder);

				AddTextNotification("Skin Pack Gotten", systemEventInProgess_FontColor);

				skinPackIsDone = true;
			}

			//if both are done
			if(structureBlobIsDone && skinPackIsDone && ROMsIsDone)
				break;
		}

		//run the Updater to get the latest Dolphin client

		//overwrite the User data folder with the one from the legacy KARphin
		if(installerData.isMigrating)
		{
			string userFolder = installerData.R10Path + "/User";
			string target = installerData.installPath + "/Clients/User";
			Directory.Delete(target); Directory.CreateDirectory(installerData.installPath + "/Clients/User");
			string[] entries = Directory.GetFileSystemEntries(userFolder);
			foreach (string fileOrDir in entries)
			{
				Directory.Move(fileOrDir, Path.Combine(target, 
				fileOrDir.Substring(userFolder.Length + 1)));
			}

			AddTextNotification("Migrated KARphin Legacy data to KARphin", systemEventInProgess_FontColor);
		}
		else //write the data for the ROM locates
		{
			
		}

		//runs Dolphin
		var dolphin = new Process();
		dolphin.StartInfo.FileName = installerData.installPath + "/Clients/KARphin_Legacy";
		dolphin.StartInfo.WorkingDirectory = installerData.installPath + "/Clients";
		dolphin.Start();
		AddTextNotification("Installing is done, feel free to close the installer. Welcome to The City :3", itemHasFinished_FontColor);
	}
}
