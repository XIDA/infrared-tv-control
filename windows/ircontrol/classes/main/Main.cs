using System;
using CommandLine;
using CommandLine.Text;
using System.Resources;
using System.Globalization;
using System.Reflection;
namespace ircontrol {

	public class Main:ArduinoManagerListener {

		private ResourceManager _resman;

		private ArduinoManager _arduinoManager;

		// we need to know how many messages should be send
		private int _messagesToSend;

		// and how many response we got so for
		private int _responsesReceived;

		// storing the responses, so we can write them to a text file when all responses arrived
		private string[] _responsesLog;

		public Main(string[] args) {

			setupResources();
			setupGlobals();
			setupArguments(args);

			setupArduino();

			if (!sendMessage ()) {
				if (Globals.arguments ().verbose) {
					Console.WriteLine(_resman.GetString("error_send"));
				}

				if (Globals.arguments ().responsesFile != null) {
					System.IO.File.WriteAllText(Globals.arguments().responsesFile.Trim(), _resman.GetString("error_send"));
				}

				if (Globals.isDebug ()) {
					Console.ReadLine ();
				}
			}
			Environment.Exit (0);
		}

		void setupResources () {
			_resman = new ResourceManager("IRControl.resources.en", Assembly.GetExecutingAssembly());
		}

		void setupGlobals () {
			Globals.setup();
		}

		void setupArguments (string[] args) {
			CommandLineArguments cArguments = ArgumentsManager.setup(args);
			if (cArguments == null) {
				if (Globals.isDebug()) {
					Console.ReadLine ();
				}
				Environment.Exit (0);
			}

			Globals.setArguments(cArguments);
		}

		void setupArduino() {
			_arduinoManager = new ArduinoManager(this, Globals.arguments().comport); 
		}

		// ArduinoManagerListener START
		void ArduinoManagerListener.errorComPort() {
			if (Globals.arguments ().verbose) {
				Console.WriteLine (_resman.GetString("error_nocomport"));
			}

			if (Globals.arguments ().responsesFile != null) {
				System.IO.File.WriteAllText(Globals.arguments().responsesFile.Trim(), _resman.GetString("error_nocomport"));
			}

			if (Globals.isDebug()) {
				Console.ReadLine ();
			}
			Environment.Exit (0);
		}

		void ArduinoManagerListener.errorTimedOut() {
			if (Globals.arguments ().verbose) {
				Console.WriteLine (_resman.GetString("error_timedout"));
			}

			if (Globals.arguments ().responsesFile != null) {
				System.IO.File.WriteAllText(Globals.arguments().responsesFile.Trim(), _resman.GetString("error_timedout"));
			}

			if (Globals.isDebug()) {
				Console.ReadLine ();
			}
			Environment.Exit (0);
		}

		void ArduinoManagerListener.response(string message) {
			if (Globals.arguments ().verbose) {
				Console.WriteLine ("Response >" + message + "<");
			}

			if (_responsesLog == null) {
				_responsesLog = new string[_messagesToSend];
			}

			_responsesLog[_responsesReceived] = message;

			_responsesReceived++;

			if (_responsesReceived >= _messagesToSend) {
				if (Globals.isDebug ()) {
					Console.ReadLine ();
				}

				if (Globals.arguments ().responsesFile != null) {
					System.IO.File.WriteAllLines (Globals.arguments ().responsesFile.Trim(), _responsesLog);
				}
				Environment.Exit (0);
			} else {
				// we are still waiting for more responses
				//C onsole.WriteLine("Waiting for " + _messagesToSend + " responses");
			}

		}
		// ArduinoManagerListener END

		bool sendMessage() {

			_messagesToSend = 1;
			_responsesReceived = 0;

			if (Globals.arguments ().specialcommand != null) {
				switch (Globals.arguments ().specialcommand) {
					case "ledeffect":
						_arduinoManager.sendLEDEffect(Convert.ToInt32(Globals.arguments ().command), Globals.arguments ().specialcommandparameter2);
					break;
					case "ledonformillis":
						_arduinoManager.sendLEDOnForMillis(Convert.ToInt32(Globals.arguments ().command), Globals.arguments ().specialcommandparameter);
					break;	
					case "ledon":
						_arduinoManager.sendLEDOn(Convert.ToInt32(Globals.arguments ().command));
					break;
					case "ledonrange":
					_arduinoManager.sendLEDOnRange(Convert.ToInt32(Globals.arguments ().command), Globals.arguments ().specialcommandparameter, Globals.arguments ().specialcommandparameter2);
					break;
					case "ledoff":
						_arduinoManager.sendLEDOff(Convert.ToInt32(Globals.arguments ().command));
					break;	
					case "ledblink":
					_arduinoManager.sendLEDBlink(Convert.ToInt32(Globals.arguments ().command), Globals.arguments ().specialcommandparameter, Globals.arguments ().specialcommandparameter2);
					break;
				}

			} else {
				_arduinoManager.sendSamsung(Globals.arguments ().command.Trim ());
			}

			return true;
		}
	}
}

