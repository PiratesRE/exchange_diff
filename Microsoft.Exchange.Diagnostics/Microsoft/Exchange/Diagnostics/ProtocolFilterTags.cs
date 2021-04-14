using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ProtocolFilterTags
	{
		public const int SenderFilterAgent = 0;

		public const int RecipientFilterAgent = 1;

		public const int Other = 2;

		public static Guid guid = new Guid("0C5B72B3-290E-4c06-BE9D-D4727DF5FC0D");
	}
}
