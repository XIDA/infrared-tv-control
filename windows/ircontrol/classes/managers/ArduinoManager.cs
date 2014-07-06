using System;
using System.Threading;
using CommandMessenger;
using CommandMessenger.TransportLayer;

/// <summary>
/// Arduino manager connects to the arduino and sends your message
/// uses: https://commandlinearguments.codeplex.com/
/// uses: http://playground.arduino.cc/Code/CmdMessenger
/// </summary>
namespace ircontrol {

	// Commands
	enum Command
	{
		SendSamsung,
		SendSamsungResponse,
		LEDEffect,
		LEDEffectResponse,
		LEDOnForMillis,
		LEDOnForMillisResponse,
		LEDOn,
		LEDOnResponse,
		LEDOnRange,
		LEDOnRangeResponse,
		LEDOff,
		LEDOffResponse,
		LEDBlink,
		LEDBlinkResponse,
		Response,
		Status
	};


	public class ArduinoManager {

		private bool setupDone = false;

		private string _comPort;
		private int _comPortSpeed = 9600;

		private ArduinoManagerListener listener;

		public bool RunLoop { get; set; }
		private SerialTransport _serialTransport;
		private CmdMessenger _cmdMessenger;

		public ArduinoManager (ArduinoManagerListener listener, String comPort) {
			this.listener = listener;

			// if there is a problem setting up the com port, then we exit
			if (!setupComPort (comPort)) {
				return;
			}

			/*
			if (Globals.arguments().verbose) {
				Console.WriteLine ("Com port is: " + _comPort);
			}
			*/

			setupConnection();

			setupDone = true;
		}

		bool setupComPort(String comPort) {

			if (comPort == null) {
				//C onsole.WriteLine("search ini");
				_comPort = SettingsManager.get ("comport");
			} else {
				_comPort = comPort;
			}

			if (_comPort != "") {
				return true;
			}

			_comPort = ComPortManager.getPort();
			if (_comPort == null) {
				listener.errorComPort();
				return false;
			}

			return true;
		}

		void setupConnection() {
			// Create Serial Port object
			_serialTransport = new SerialTransport();
			_serialTransport.CurrentSerialSettings.PortName = _comPort;    // Set com port
			_serialTransport.CurrentSerialSettings.BaudRate = _comPortSpeed;     // Set baud rate
			_serialTransport.CurrentSerialSettings.DtrEnable = false;     // For some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.

			// Initialize the command messenger with the Serial Port transport layer
			_cmdMessenger = new CmdMessenger(_serialTransport);

			// Tell CmdMessenger if it is communicating with a 16 or 32 bit Arduino board
			_cmdMessenger.BoardType = BoardType.Bit16;

			// Attach the callbacks to the Command Messenger
			AttachCommandCallBacks();

			// Start listening
			_cmdMessenger.StartListening();   
		}

		/// Attach command call backs. 
		private void AttachCommandCallBacks() {
			_cmdMessenger.Attach(OnUnknownCommand);
			_cmdMessenger.Attach((int)Command.Status, OnStatus);
			_cmdMessenger.Attach((int)Command.Response, OnResponse);
		}

		/// Executes when an unknown command has been received.
		void OnUnknownCommand(ReceivedCommand arguments) {
			if (Globals.arguments ().verbose) {
				Console.WriteLine ("Command without attached callback received");
			}
		}

		// this is called when the arduino starts
		void OnStatus(ReceivedCommand arguments) {
			int cStatus = arguments.ReadInt16Arg ();
			if (Globals.arguments ().verbose) {
				Console.WriteLine ("Command OnStatus received with status " + cStatus);
			}
		}

		//we use this for general response debugging
		void OnResponse(ReceivedCommand arguments) {
			int cStatus = arguments.ReadInt16Arg ();
			string cMessage = arguments.ReadStringArg ();
			if (Globals.arguments ().verbose) {
				Console.WriteLine ("Command OnResponse received with status " + cStatus + " and message: " + cMessage);
				Console.WriteLine (arguments.RawString);
			}
		}

		public bool sendSamsung(String value) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.SendSamsung, (int)Command.SendSamsungResponse, Globals.arguments().timeout);
			command.AddArgument (value);            

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnSendSamsungResponse(responseCommand, value);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		public bool sendLEDEffect(int effectIndex, int repetitions) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.LEDEffect, (int)Command.LEDEffectResponse, Globals.arguments().timeout);
			command.AddArgument (effectIndex); 
			command.AddArgument (repetitions);            

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnLEDEffectResponse(responseCommand);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		public bool sendLEDOnForMillis (int ledIndex, int specialcommandparameter) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.LEDOnForMillis, (int)Command.LEDOnForMillisResponse, Globals.arguments().timeout);
			command.AddArgument (ledIndex); 
			command.AddArgument (specialcommandparameter);            

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnLEDOnForMillisResponse(responseCommand);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		public bool sendLEDOn (int ledIndex) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.LEDOn, (int)Command.LEDOnResponse, Globals.arguments().timeout);
			command.AddArgument (ledIndex);     

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnLEDOnResponse(responseCommand);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		public bool sendLEDOnRange(int ledStartIndex, int ledEndIndex, int millisToKeepOn) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.LEDOnRange, (int)Command.LEDOnRangeResponse, Globals.arguments().timeout);
			command.AddArgument(ledStartIndex);     
			command.AddArgument(ledEndIndex);     
			command.AddArgument(millisToKeepOn);     

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnLEDOnRangeResponse(responseCommand);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		public bool sendLEDOff (int ledIndex) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.LEDOff, (int)Command.LEDOffResponse, Globals.arguments().timeout);
			command.AddArgument (ledIndex);     

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnLEDOnResponse(responseCommand);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		public bool sendLEDBlink (int ledIndex, int blinkMillis, int repetitions) {
			if (!setupDone) {
				return false;
			}

			var command = new SendCommand((int)Command.LEDBlink, (int)Command.LEDBlinkResponse, Globals.arguments().timeout);
			command.AddArgument (ledIndex);     
			command.AddArgument (blinkMillis);  
			command.AddArgument (repetitions);  

			_cmdMessenger.SendCommand(command);
			var responseCommand = _cmdMessenger.SendCommand(command);

			// Check if received a (valid) response
			if (responseCommand.Ok) {
				OnLEDBlinkResponse(responseCommand);
				return true;
			} else {
				listener.errorTimedOut();
				return false;
			}
		}

		void OnSendSamsungResponse(ReceivedCommand responseCommand, String value) {

			int success = responseCommand.ReadInt16Arg();
			String cValue = responseCommand.ReadStringArg();

			listener.response (cValue);
		}		

		void OnLEDEffectResponse (ReceivedCommand responseCommand) {
			int success = responseCommand.ReadInt16Arg();
			int effectIndex = responseCommand.ReadInt16Arg();
			listener.response ("" + effectIndex);
		}

		void OnLEDOnForMillisResponse (ReceivedCommand responseCommand) {
			int success = responseCommand.ReadInt16Arg();
			int ledIndex = responseCommand.ReadInt16Arg();
			listener.response ("" + ledIndex);
		}

		void OnLEDOnResponse (ReceivedCommand responseCommand) {
			int success = responseCommand.ReadInt16Arg();
			int ledIndex = responseCommand.ReadInt16Arg();
			listener.response ("" + ledIndex);
		}

		void OnLEDOnRangeResponse (ReceivedCommand responseCommand) {
			int success = responseCommand.ReadInt16Arg();
			int ledStartIndex = responseCommand.ReadInt16Arg();
			listener.response ("" + ledStartIndex);
		}

		void OnLEDOffResponse (ReceivedCommand responseCommand) {
			int success = responseCommand.ReadInt16Arg();
			int ledIndex = responseCommand.ReadInt16Arg();
			listener.response ("" + ledIndex);
		}

		void OnLEDBlinkResponse (ReceivedCommand responseCommand) {
			int success = responseCommand.ReadInt16Arg();
			int ledIndex = responseCommand.ReadInt16Arg();
			listener.response ("" + ledIndex);
		}
	}
}

