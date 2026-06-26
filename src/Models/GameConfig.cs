using System.Xml.Serialization;

namespace LR2Nexus.Models
{
	[XmlRoot("config")]
	public class GameConfig
	{
		[XmlElement("system")] public Lr2System System { get; set; } = new();
		[XmlElement("jukebox")] public Lr2Jukebox Jukebox { get; set; } = new();


		[XmlRoot("system")]
		public class Lr2System
		{
			[XmlElement("screenmode")]
			public int Screenmode { get; set; } = 1;

			[XmlElement("vsync")]
			public int VSync { get; set; } = 0;

			[XmlElement("directdraw")]
			public int DirectDraw { get; set; } = 0;


			[XmlElement("maindisplay")]
			public int MainDisplay { get; set; } = 0;

			[XmlElement("highcolor")]
			public int HighColor { get; set; } = 0;

			[XmlElement("autoreload")]
			public int AutoReload { get; set; } = 0;

			[XmlElement("customfolder")]
			public int CustomFolder { get; set; } = 246;

			[XmlElement("mainsleep")]
			public int MainSleep { get; set; } = 3;

			[XmlElement("bmssleep")]
			public int BmsSleep { get; set; } = 3;

			[XmlElement("screenexrate")]
			public int ScreenExrate { get; set; } = 100;

			[XmlElement("inputinterval")]
			public int InputInterval { get; set; } = 16;

			[XmlElement("disablesystemkey")]
			public int DisableSystemKey { get; set; } = 0;

			[XmlElement("outputlog")]
			public int OutputLog { get; set; } = 0;

			[XmlElement("thread")]
			public int Thread { get; set; } = 0;

			[XmlElement("eventmode")]
			public int Eventmode { get; set; } = 0;

			[XmlElement("disableskinpreview")]
			public int DisableSkinPreview { get; set; } = 0;

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
	}
}
