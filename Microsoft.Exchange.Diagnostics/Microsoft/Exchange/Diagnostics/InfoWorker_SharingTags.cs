using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InfoWorker_SharingTags
	{
		public const int SharingEngine = 0;

		public const int AppointmentTranslator = 1;

		public const int ExchangeService = 2;

		public const int LocalFolder = 3;

		public const int SharingKeyHandler = 4;

		public static Guid guid = new Guid("A15553C6-31A1-4a7a-8526-8FABE6841235");
	}
}
