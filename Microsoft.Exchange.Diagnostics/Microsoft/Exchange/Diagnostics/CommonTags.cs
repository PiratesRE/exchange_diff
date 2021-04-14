using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct CommonTags
	{
		public const int Common = 0;

		public const int EventLog = 1;

		public const int ScheduleInterval = 2;

		public const int CertificateValidation = 3;

		public const int Authorization = 4;

		public const int Tracing = 5;

		public const int FaultInjection = 6;

		public const int Rpc = 7;

		public const int Sqm = 8;

		public const int TracingConfiguration = 9;

		public const int FaultInjectionConfiguration = 10;

		public const int AppConfigLoader = 11;

		public const int WebHealth = 12;

		public const int VariantConfiguration = 13;

		public const int ClientAccessRules = 14;

		public const int ConcurrencyGuard = 15;

		public static Guid guid = new Guid("5948f08f-9d8f-11da-9575-00e08161165f");
	}
}
