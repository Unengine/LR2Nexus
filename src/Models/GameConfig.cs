using System.Xml.Serialization;

namespace LR2Nexus.Models
{
	[XmlRoot("config")]
	public class GameConfig
	{
		[XmlElement("system")] public Lr2System System { get; set; } = new();
		[XmlElement("jukebox")] public Lr2Jukebox Jukebox { get; set; } = new();
		[XmlElement("play")] public Lr2Play Play { get; set; } = new();
		[XmlElement("select")] public Lr2Select Select { get; set; } = new();

		[XmlRoot("system")]
		public class Lr2System
		{
			[XmlElement("screenmode")]
			public int Screenmode { get; set; } = 1;

			private int _vsync = 0;
			[XmlElement("vsync")]
			public int VSync
			{
				get => _vsync;
				set
				{
					var clamped = int.Clamp(value, 0, 1);
					_vsync = clamped;
				}
			}

			[XmlElement("directdraw")]
			public int DirectDraw { get; set; } = 0;

			[XmlElement("maindisplay")]
			public int MainDisplay { get; set; } = 0;

			[XmlElement("highcolor")]
			public int HighColor { get; set; } = 0;

			private int _autoReload = 1;
			[XmlElement("autoreload")]
			public int AutoReload
			{
				get => _autoReload;
				set
				{
					var clamped = int.Clamp(value, 0, 2);
					_autoReload = clamped;
				}
			}

			private int _customFolder = 0;
			[XmlElement("customfolder")]
			public int CustomFolder
			{
				get => _customFolder;
				set
				{
					var clamped = int.Clamp(value, 0, 255);
					_customFolder = clamped;
				}
			}

			[XmlElement("mainsleep")]
			public int MainSleep { get; set; } = 3;

			[XmlElement("bmssleep")]
			public int BmsSleep { get; set; } = 3;

			[XmlElement("screenexrate")]
			public int ScreenExrate { get; set; } = 100;

			private int _inputInterval = 16;
			[XmlElement("inputinterval")]
			public int InputInterval
			{
				get => _inputInterval;
				set
				{
					var clamped = int.Clamp(value, 8, 16);
					_inputInterval = clamped;
				}
			}

			[XmlElement("disablesystemkey")]
			public int DisableSystemKey { get; set; } = 0;

			private int _outputLog = 0;
			[XmlElement("outputlog")]
			public int OutputLog
			{
				get => _outputLog;
				set
				{
					var clamped = int.Clamp(value, 0, 1);
					_outputLog = clamped;
				}
			}

			[XmlElement("thread")]
			public int Thread { get; set; } = 0;

			[XmlElement("eventmode")]
			public int Eventmode { get; set; } = 0;

			private int _disableSkinPreview = 0;
			[XmlElement("disableskinpreview")]
			public int DisableSkinPreview
			{
				get => _disableSkinPreview;
				set
				{
					var clamped = int.Clamp(value, 0, 1);
					_disableSkinPreview = clamped;
				}
			}

			[XmlElement("newsongfolder")]
			public string NewSongFolder { get; set; } = @"NEW SONG\";

			[XmlElement("titleflash")]
			public int TitleFlash { get; set; } = 24;

			[XmlElement("hptimer")]
			public int HpTimer { get; set; } = 0;

			[XmlElement("disablebmsthread")]
			public int DisableBmsThread { get; set; } = 0;

			[XmlElement("disablefolderthread")]
			public int DisableFolderThread { get; set; } = 0;

			[XmlElement("language")]
			public int Language { get; set; } = 0;

			private int _windowSizeX = 1280;
			[XmlElement("windowsize_x")]
			public int WindowSizeX
			{
				get => _windowSizeX;
				set
				{
					var clamped = int.Clamp(value, 640, 1920);
					_windowSizeX = clamped;
				}
			}

			private int _windowSizeY = 720;
			[XmlElement("windowsize_y")]
			public int WindowSizeY
			{
				get => _windowSizeY;
				set
				{
					var clamped = int.Clamp(value, 480, 1080);
					_windowSizeY = clamped;
				}
			}

			[XmlElement("softwarerendering")]
			public int SoftwareRendering { get; set; } = 0;
		}

		[XmlRoot("jukebox")]
		public class Lr2Jukebox
		{
			[XmlElement("path")]
			public List<string> Paths { get; set; } = [];
		}

		[XmlRoot("play")]
		public class Lr2Play
		{
			private int _hsMax = 900;
			[XmlElement("hsmax")]
			public int HsMax
			{
				get => _hsMax;
				set
				{
					var clamped = int.Clamp(value, 1, 10000);
					_hsMax = clamped;
				}
			}

			private int _hsMin = 10;
			[XmlElement("hsmin")]
			public int HsMin
			{
				get => _hsMin;
				set
				{
					var clamped = int.Clamp(value, 1, 10000);
					_hsMin = clamped;
				}
			}

			private int _baseSpeed = 100;
			[XmlElement("basespeed")]
			public int BaseSpeed
			{
				get => _baseSpeed;
				set
				{
					var clamped = int.Clamp(value, 1, 10000);
					_baseSpeed = clamped;
				}
			}

			private int _hsStep = 10;
			[XmlElement("hsmargin")]
			public int HsStep
			{
				get => _hsStep;
				set
				{
					var clamped = int.Clamp(value, 1, 10000);
					_hsStep = clamped;
				}
			}

			private int _sudhidStep = 10;
			[XmlElement("shuttermargin")]
			public int SudHidStep
			{
				get => _sudhidStep;
				set
				{
					var clamped = int.Clamp(value, 1, 100);
					_sudhidStep = clamped;
				}
			}

			private int _misslayerDuration = 500;
			[XmlElement("poorbga")]
			public int MisslayerDuration
			{
				get => _misslayerDuration;
				set
				{
					var clamped = int.Clamp(value, 0, 5000);
					_misslayerDuration = clamped;
				}
			}
		}

		[XmlRoot("select")]
		public class Lr2Select
		{
			private int _folderLamp = 1;
			[XmlElement("folderlamp")]
			public int FolderLamp
			{
				get => _folderLamp;
				set
				{
					var clamped = int.Clamp(value, 0, 1);
					_folderLamp = clamped;
				}
			}

			private int _searchMax = 900;
			[XmlElement("searchmax")]
			public int SearchMax
			{
				get => _searchMax;
				set
				{
					var clamped = int.Clamp(value, 0, 10000);
					_searchMax = clamped;
				}
			}

			private int _preview = 1;
			[XmlElement("preview")]
			public int Preview
			{
				get => _preview;
				set
				{
					var clamped = int.Clamp(value, 0, 1);
					_preview = clamped;
				}
			}
		}

		public const int BitRandomSelect = 1 << 0;
		public const int BitFavoriteFolder = 1 << 1;
		public const int BitTop10Playcount = 1 << 2;
		public const int BitLevelFolder = 1 << 3;
		public const int BitClearTypeFolder = 1 << 4;
		public const int BitPlayrank = 1 << 5;
		public const int BitIgnoredFolder = 1 << 6;
		public const int BitInsaneBMSFolder = 1 << 7;
	}
}
