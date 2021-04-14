using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ProtocolAnalysisTags
	{
		public const int Factory = 0;

		public const int Database = 1;

		public const int CalculateSrl = 2;

		public const int OnMailFrom = 3;

		public const int OnRcptTo = 4;

		public const int OnEOD = 5;

		public const int Reject = 6;

		public const int Disconnect = 7;

		public static Guid guid = new Guid("A0F3DC2A-7FD4-491E-C176-4857EAF2D7EF");
	}
}
