using System;

/// <summary>
/// Handle CommandLineArguments
/// </summary>
namespace ircontrol {

	public class ArgumentsManager {

		public static CommandLineArguments setup (string[] args) {
			if (Globals.isDebug()) { 
				if (args.Length != 1) {
					args = new string[] { "-c 0xE0E040BF", "-v", "-t 5000"};
				}
			}

			CommandLineArguments commandLineArguments = new CommandLineArguments();
			if (CommandLine.Parser.Default.ParseArguments (args, commandLineArguments)) {

				if (validateArguments (commandLineArguments)) {
					return commandLineArguments;
				} 

				return null;

			} else {
				//required argument missing
				return null;
			}
		}


		private static bool validateArguments(CommandLineArguments cArguments) {

			//check the tv brand
			String tvBrand = cArguments.tvbrand;
			if (tvBrand != null) {
				Console.WriteLine ("cArguments.tvbrand >" + tvBrand + "<") ;
				if (!tvBrand.Equals("samsung")) {
					Console.WriteLine("ERROR: Currently only Samsung TVs are supported");
					return false;
				}
			}

			return true;
		}
	}
}

