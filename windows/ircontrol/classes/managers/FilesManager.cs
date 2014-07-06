using System;

namespace ircontrol {
	public class FilesManager {

		private const string ASSETS_DIR = "assets\\";

		private const string SETTINGS_FILE = "settings.ini";

		public static string assetsDir() {
			return ASSETS_DIR;
		}

		public static string settingsFilePath() {
			return assetsDir() + SETTINGS_FILE;
		}
	}
}

