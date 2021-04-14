using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct MSExchangeUMTags
	{
		public const int ServiceStart = 0;

		public const int Service = 1;

		public const int ServiceStop = 2;

		public const int VoipPlatform = 3;

		public const int CallSession = 4;

		public const int StackIf = 5;

		public const int StateMachine = 6;

		public const int Greetings = 7;

		public const int Authentication = 8;

		public const int VoiceMail = 9;

		public const int Calendar = 10;

		public const int Email = 11;

		public const int Xso = 12;

		public const int Fax = 13;

		public const int AutoAttendant = 14;

		public const int DirectorySearch = 15;

		public const int Util = 16;

		public const int ClientAccess = 17;

		public const int Diagnostic = 18;

		public const int Outdialing = 19;

		public const int SpeechAutoAttendant = 20;

		public const int AsrContacts = 21;

		public const int AsrSearch = 22;

		public const int PromptProvisioning = 23;

		public const int PFDUMCallAcceptance = 24;

		public const int UMCertificate = 25;

		public const int OCSNotifEvents = 26;

		public const int PersonalAutoAttendant = 27;

		public const int FindMe = 28;

		public const int MWI = 29;

		public const int UMPartnerMessage = 30;

		public const int FaultInjection = 31;

		public const int SipPeerManager = 32;

		public const int Rpc = 33;

		public const int UCMA = 34;

		public const int UMReports = 35;

		public const int MobileSpeechReco = 36;

		public const int UMGrammarGenerator = 37;

		public const int UMCallRouter = 38;

		public static Guid guid = new Guid("321b4079-df13-45c3-bbc9-2c610013dff4");
	}
}
