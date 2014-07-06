using System;

namespace ircontrol {

	public class Globals {

		private static bool _isDebug;
		private static CommandLineArguments _arguments;

		public static void setup() {
			_isDebug = System.Diagnostics.Debugger.IsAttached;

		}

		public static bool isDebug() {
			return _isDebug;
		}

		public static void setArguments (CommandLineArguments arguments) {
			_arguments = arguments;
		}

		public static CommandLineArguments arguments() {
			return _arguments;
		}
	}
}

