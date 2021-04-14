using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ForefrontActiveMonitoringTags
	{
		public const int FaultInjection = 0;

		public const int SMTP = 1;

		public const int SMTPConnection = 2;

		public const int SMTPMonitor = 3;

		public const int WebService = 4;

		public const int HTTP = 5;

		public const int Responder = 6;

		public const int DNS = 7;

		public const int AntiSpam = 8;

		public const int Background = 10;

		public const int DAL = 11;

		public const int Deployment = 12;

		public const int Monitoring = 14;

		public const int Provisioning = 15;

		public const int Transport = 16;

		public const int WebStore = 17;

		public const int Cmdlet = 18;

		public const int GenericHelper = 19;

		public const int Datamining = 20;

		public const int ShadowRedundancy = 21;

		public const int RWS = 22;

		public const int MessageTracing = 23;

		public const int AsyncEngine = 24;

		public const int FFOMigration1415 = 25;

		public const int QueueDigest = 26;

		public const int CentralAdmin = 27;

		public const int RPS = 28;

		public static Guid guid = new Guid("94FBFACE-D4CE-4A9F-B2C6-64646394868F");
	}
}
