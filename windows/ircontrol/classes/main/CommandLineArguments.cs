using System;
using CommandLine;
using CommandLine.Text;

namespace ircontrol {
	public class CommandLineArguments {
		[Option('c', "command", Required = true, HelpText = "Command to send (like 0xE0E040BF)")]
		public string command { get; set; }

		[Option('t', "tvbrand", DefaultValue = "samsung", HelpText = "The brand of tv you have, currently only samsung is supported")]
		public string tvbrand { get; set; }

		[Option('p', "comport", HelpText = "set the com port, if no com port is set, the first com port device with a name of Arduino will be used")]
		public string comport { get; set; }

		[Option('r', "response", HelpText = "save the response in a text file instead of just echoeing, add the response text file name like output.txt")]
		public string responsesFile { get; set; }

		[Option('v', "verbose", DefaultValue = false, HelpText = "Print details during execution.")]
		public bool verbose { get; set; }

		[Option('t', "timeout", DefaultValue = 500, HelpText = "Sending time out in ms")]
		public int timeout { get; set; }

		[Option('s', "specialcommand", HelpText = "Special Command to send: 'ledeffect'  'ledonformillis', 'ledon', 'ledoff', 'ledonrange")]
		public string specialcommand { get; set; }

		[Option('l', "specialcommandparameter", DefaultValue = 1, HelpText = "Used in combination with the special command")]
		public int specialcommandparameter { get; set; }

		[Option('m', "specialcommandparameter2", DefaultValue = 0, HelpText = "Used in combination with the special command")]
		public int specialcommandparameter2 { get; set; }
	
		[HelpOption]
		public string GetUsage() {
			return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
		}
	}
}

