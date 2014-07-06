using System;

namespace ircontrol {

	public interface ArduinoManagerListener {
		void errorComPort();
		void errorTimedOut();
		void response(string message);
	}
}

