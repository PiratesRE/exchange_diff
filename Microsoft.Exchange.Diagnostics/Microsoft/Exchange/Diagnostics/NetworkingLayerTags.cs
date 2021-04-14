using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct NetworkingLayerTags
	{
		public const int DNS = 0;

		public const int Network = 1;

		public const int Authentication = 2;

		public const int Certificate = 3;

		public const int DirectoryServices = 4;

		public const int ProcessManager = 5;

		public const int HttpClient = 6;

		public const int ProtocolLog = 7;

		public const int RightsManagement = 8;

		public const int LiveIDAuthenticationClient = 9;

		public const int DeltaSyncClient = 11;

		public const int DeltaSyncResponseHandler = 12;

		public const int LanguagePackInfo = 13;

		public const int WSTrust = 14;

		public const int EwsClient = 15;

		public const int Configuration = 16;

		public const int SmtpClient = 17;

		public const int XropServiceClient = 18;

		public const int XropServiceServer = 19;

		public const int Claim = 20;

		public const int Facebook = 21;

		public const int LinkedIn = 22;

		public const int MonitoringWebClient = 23;

		public const int RulesBasedHttpModule = 24;

		public const int AADClient = 25;

		public const int AppSettings = 26;

		public const int Common = 27;

		public static Guid guid = new Guid("351632BC-3F4E-4C79-A368-F8E54DCE4A2E");
	}
}
