using Godot;
using System;
using System.IO;
using System.Diagnostics;

public partial class DownloadManager : Node2D
{
	[Export] InstallerData installerData;

	Vector2 lastPos; //the last position of a text notification

	//the various font color items
	[Export] Theme itemHasFinished_FontColor;
	[Export] Theme itemIsDownloading_FontColor;
	[Export] Theme systemEventInProgess_FontColor;

	//Legacy Dolphin download
	string KARphin_Legacy_KWQI = "KWQI/KARphinLegacy.KWQI";

	//modern KARphin downloads
	string KARphin_Windows_DownloadURL = "";
	string KARphin_Mac_DownloadURL = "";

	//KAR Workshop downloads
	string KARWorkshop_Windows_DownloadURL = "";
	string KARWorkshop_Mac_DownloadURL = "";

	//KARDon't
	string KARDont_DownloadURL = "https://github.com/SeanMott/KARDont/releases/download/1.0.0-stable/KARDont.7z";

	//Hack Pack
	string HP_KWQI = "KWQI/HP.KWQI";

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

		//we create a texture pack folder for all texture packs
		if (!Directory.Exists(installerData.installPath + "/TexturePacks"))
			Directory.CreateDirectory(installerData.installPath + "/TexturePacks");

		//we create a audio pack folder for all audio packs
		if (!Directory.Exists(installerData.installPath + "/AudioPacks"))
			Directory.CreateDirectory(installerData.installPath + "/AudioPacks");

		//we also create a KWQI folder for all kWQI files
		if (!Directory.Exists(installerData.installPath + "/KWQI"))
			Directory.CreateDirectory(installerData.installPath + "/KWQI");
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
		GenerateFolderStructure();
		//write KAR Workshop settings and other files
		GenerateKWSettings();

		//check what platform we are on, execute KARphin legacy
		KWQI KARphinLegacy_KWQIData = KWQI.LoadKWQI(KARphin_Legacy_KWQI);

		//runs Duma for legecy KARphin
		Process p;
		KWQIPackaging.DownloadContent_Archive_Windows(out p, "SupportProgs/Duma.exe", KARphinLegacy_KWQIData.displayName, KARphinLegacy_KWQIData.ContentDownloadURL_Windows,
		installerData.installPath + "/Clients");
		AddTextNotification("KARphin Legacy download started...", itemIsDownloading_FontColor);

		//doesn't matter the platform, get HP
		KWQI HP_KWQIData = KWQI.LoadKWQI(HP_KWQI);

		//runs Duma for Hack Pack
		Process hp;
		KWQIPackaging.DownloadContent_ROM_Windows(out hp, "SupportProgs/Duma.exe", HP_KWQIData.displayName, HP_KWQIData.ContentDownloadURL_Windows,
		installerData.installPath + "/ROMs");
		AddTextNotification("HP download started...", itemIsDownloading_FontColor);

		//runs Duma for KARDon't

		//checks for each of the Downloads
		bool doneDownloading = false;
		bool KARphinIsDone = false, HPIsDone = false, KARDontIsDone = false;
		
		//HPIsDone = true;

		while(!doneDownloading)
		{
			//if KARphin is done
			if(!KARphinIsDone && p.HasExited)
			{
				//validate the file completed
				AddTextNotification("KARphin Legacy done downloading, unpacking...", itemHasFinished_FontColor);

				//unpacks KARphin
				KWQIPackaging.UnpackArchive_Windows(installerData.installPath + "/Clients", KARphinLegacy_KWQIData.displayName, installerData.installPath, true);
				//p = new Process();
				//p.StartInfo.FileName = installerData.installPath + "/Clients/" + KARphinLegacy_KWQIData.displayName + ".exe";
				//p.StartInfo.Arguments = "-o " + installerData.installPath + " -y";
				//p.StartInfo.WorkingDirectory = installerData.installPath;
				//p.Start();
				//p.WaitForExit();

				//deletes the 7zip unpacking exe
				//System.IO.File.Delete(installerData.installPath + "/Clients/KARphin_Legacy.exe");
				AddTextNotification("KARphin Legacy aquired", itemHasFinished_FontColor);

				KARphinIsDone = true;
			}

			//if Hack Pack is done
			if(!HPIsDone && hp.HasExited)
			{
				//validate the file completed
				AddTextNotification("HP done downloading, unpacking...", itemHasFinished_FontColor);

				//unpacks HP
				KWQIPackaging.UnpackROM_Windows("SupportProgs/brotli.exe", installerData.installPath + "/ROMs", HP_KWQIData.displayName, true);
				//hp = new Process();
				//hp.StartInfo.FileName = "SupportProgs/brotli.exe";
				//hp.StartInfo.Arguments = "--decompress -o HP_101.iso HP_101.br";
				//hp.StartInfo.WorkingDirectory = installerData.installPath + "/ROMs";
				//hp.Start();
				//hp.WaitForExit();
				
				//deletes the brotil format
				//System.IO.File.Delete(installerData.installPath + "/ROMs/HP_101.br");
				AddTextNotification("HP aquired", itemHasFinished_FontColor);

				HPIsDone = true;

			}

			//if both are done
			if(KARphinIsDone && HPIsDone)
				break;
		}

		//configure Dolphin Legacy
		string dolphinINI = "[General]\nLastFilename = C:/Users/Jas/Desktop/ROMs/GC/Kirby Air Ride [GKYE01]/game.iso\nShowLag = False\nShowFrameCount = False\nISOPaths = 1\nRecursiveISOPaths = False\nNANDRootPath = \nDumpPath = \nWirelessMac = \nWiiSDCardPath = \nISOPath0 = ";
		dolphinINI += installerData.installPath + "/ROMs";
		dolphinINI += "\n[Interface]\nConfirmStop = False\nUsePanicHandlers = True\nOnScreenDisplayMessages = True\nHideCursor = True\nAutoHideCursor = False\nMainWindowPosX = 918\nMainWindowPosY = 251\nMainWindowWidth = 660\nMainWindowHeight = 444\nLanguageCode = \nShowToolbar = True\nShowStatusbar = True\nShowLogWindow = False\nShowLogConfigWindow = False\nExtendedFPSInfo = False\nThemeName = Clean Pink\nPauseOnFocusLost = False\nDisableTooltips = False\n[Display]\nFullscreenResolution = Auto\nFullscreen = False\nRenderToMain = False\nRenderWindowXPos = 900\nRenderWindowYPos = 62\nRenderWindowWidth = 822\nRenderWindowHeight = 961\nRenderWindowAutoSize = False\nKeepWindowOnTop = False\nProgressiveScan = False\nPAL60 = False\nDisableScreenSaver = True\nForceNTSCJ = False\n[GameList]\nListDrives = False\nListWad = True\nListElfDol = True\nListWii = True\nListGC = True\nListJap = True\nListPal = True\nListUsa = True\nListAustralia = True\nListFrance = True\nListGermany = True\nListItaly = True\nListKorea = True\nListNetherlands = True\nListRussia = True\nListSpain = True\nListTaiwan = True\nListWorld = True\nListUnknown = True\nListSort = 3\nListSortSecondary = 0\nColorCompressed = True\nColumnPlatform = True\nColumnBanner = True\nColumnNotes = True\nColumnFileName = True\nColumnID = True\nColumnRegion = True\nColumnSize = True\nColumnState = False\n[Core]\nHLE_BS2 = False\nTimingVariance = 8\nCPUCore = 1\nFastmem = True\nCPUThread = True\nDSPHLE = True\nSyncOnSkipIdle = True\nSyncGPU = False\nSyncGpuMaxDistance = 200000\nSyncGpuMinDistance = -200000\nSyncGpuOverclock = 1.00000000\nFPRF = False\nAccurateNaNs = False\nDefaultISO = \nDVDRoot = \nApploader = \nEnableCheats = True\nSelectedLanguage = 0\nOverrideGCLang = False\nDPL2Decoder = False\nTimeStretching = False\nRSHACK = False\nLatency = 0\nMemcardAPath = F:/Documents/Kirby Workshop/ARGC-win32-shipping/FM 5.9F (Current)/FM-v5.9-Slippi-r10-Win/User/GC/MemoryCardA.USA.raw\nMemcardBPath = F:/Documents/Kirby Workshop/ARGC-win32-shipping/FM 5.9F (Current)/FM-v5.9-Slippi-r10-Win/User/GC/MemoryCardB.USA.raw\nAgpCartAPath = \nAgpCartBPath = \nSlotA = 255\nSlotB = 10\nSerialPort1 = 5\nBBA_MAC = 00:09:bf:1d:22:25\nSIDevice0 = 7\nAdapterRumble0 = False\nSimulateKonga0 = False\nSIDevice1 = 0\nAdapterRumble1 = False\nSimulateKonga1 = False\nSIDevice2 = 0\nAdapterRumble2 = False\nSimulateKonga2 = False\nSIDevice3 = 0\nAdapterRumble3 = False\nSimulateKonga3 = False\nWiiSDCard = False\nWiiKeyboard = False\nWiimoteContinuousScanning = False\nWiimoteEnableSpeaker = False\nRunCompareServer = False\nRunCompareClient = False\nEmulationSpeed = 1.00000000\nFrameSkip = 0x00000000\nOverclock = 1.00000000\nOverclockEnable = False\nGFXBackend = \nGPUDeterminismMode = auto\nPerfMapDir = \nEnableCustomRTC = False\nCustomRTCValue = 0x386d4380\nAllowAllNetplayVersions = True\nQoSEnabled = True\nAdapterWarning = True\nShownLagReductionWarning = False\n[Movie]\nPauseMovie = False\nAuthor = \nDumpFrames = False\nDumpFramesSilent = False\nShowInputDisplay = False\nShowRTC = False\n[DSP]\nEnableJIT = True\nDumpAudio = False\nDumpAudioSilent = False\nDumpUCode = False\nBackend = Cubeb\nVolume = 25\nCaptureLog = False\n[Input]\nBackgroundInput = False\n[FifoPlayer]\nLoopReplay = True\n[Analytics]\nID = a6857848fbbd3db6a6bac458d4c559c3\nEnabled = False\nPermissionAsked = True\n[Network]\nSSLDumpRead = False\nSSLDumpWrite = False\nSSLVerifyCert = False\nSSLDumpRootCA = False\nSSLDumpPeerCert = False\n[BluetoothPassthrough]\nEnabled = False\nVID = -1\nPID = -1\nLinkKeys = \n[Sysconf]\nSensorBarPosition = 1\nSensorBarSensitivity = 50331648\nSpeakerVolume = 88\nWiimoteMotor = True\nWiiLanguage = 1\nAspectRatio = 1\nScreensaver = 0\n[NetPlay]\nSelectedHostGame = Kirby Air Ride Hack Pack v1.01 (KHPE01)\nTraversalChoice = direct\nNickname = kirby 1\nHostCode = 72e6152e\nConnectPort = 2626\nHostPort = 2626\nListenPort = 0\nNetWindowPosX = 672\nNetWindowPosY = 129\nNetWindowWidth = 1149\nNetWindowHeight = 726\nAddress = 127.0.0.1\nUseUPNP = True";
		System.IO.StreamWriter file = new System.IO.StreamWriter(installerData.installPath + "/Clients/kar-netplay-win/User/Config/Dolphin.ini");
		file.WriteLine(dolphinINI);
		file.Close();
		AddTextNotification("Configured KARphin Legacy", systemEventInProgess_FontColor);
	
		//runs Dolphin
		var dolphin = new Process();
		dolphin.StartInfo.FileName = installerData.installPath + "/Clients/kar-netplay-win/Dolphin.exe";
		dolphin.StartInfo.WorkingDirectory = installerData.installPath + "/Clients/kar-netplay-win";
		dolphin.Start();
		AddTextNotification("Installing is done, feel free to close the installer. Welcome to The City :3", itemHasFinished_FontColor);
	}
}
