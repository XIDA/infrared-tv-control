using System;
using System.Globalization;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;      

/// <summary>
/// COM port manager searches for the first com port
/// </summary>
namespace ircontrol {

	public class ComPortManager {

		private const string DEVICE_NAME = "Arduino";

		public static string getPort() {

			// Get a list of serial port names. 
			string[] portnames = SerialPort.GetPortNames();

			using (var searcher = new ManagementObjectSearcher ("SELECT * FROM WIN32_SerialPort")) {

				var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
				var tList = (
						from n in portnames
						join p in ports on n equals p["DeviceID"].ToString()
						select n + " - " + p["Caption"]
				).ToList();

				CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;

				foreach (string s in tList) {
					if(s.Contains(DEVICE_NAME))  {
						string[] portArray = s.Split (new string[] { " - " }, StringSplitOptions.None);
						//C onsole.WriteLine (portArray[0]);
						return portArray [0];
					}
				}
			}

			//so no port found, use the first one
			if (portnames.Length > 0) {
				return portnames [0];
			}
			return null;
		}
	}
}

