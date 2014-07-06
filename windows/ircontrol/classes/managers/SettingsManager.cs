using System;
using IniFileLib;

namespace ircontrol {

	public class SettingsManager {

        private static IniFile settingsIni;

        private const string SECTION = "ircontrol";

        static void setup() {
            settingsIni = new IniFile(FilesManager.settingsFilePath());
        }

        public static string get(string value) {
            if(settingsIni == null) {
              setup();
            }

          return settingsIni.Read(value, SECTION);
        }
	}
}

