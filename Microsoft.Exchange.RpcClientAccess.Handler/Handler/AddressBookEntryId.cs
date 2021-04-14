using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class AddressBookEntryId : AddressEntryId
	{
		internal AddressBookEntryId(byte[] entryId) : base(entryId)
		{
		}

		public static bool IsAddressBookEntryId(Reader reader, uint sizeEntry)
		{
			if (sizeEntry < 29U || reader.Length - reader.Position < (long)((ulong)sizeEntry))
			{
				return false;
			}
			long position = reader.Position;
			bool result;
			try
			{
				reader.ReadArraySegment(4U);
				result = reader.ReadGuid().Equals(AddressBookEntryId.ExchangeProviderGuid);
			}
			finally
			{
				reader.Position = position;
			}
			return result;
		}

		public const int MinimalSize = 29;

		private static readonly Guid ExchangeProviderGuid = new Guid("{c840a7dc-42c0-1a10-b4b9-08002b2fe182}");
	}
}
