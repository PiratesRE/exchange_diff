using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public static class CorrelationIdHelper
	{
		public static Guid GetCorrelationId(int mailboxNumber, long fid)
		{
			return new Guid(mailboxNumber, 0, 0, (byte)(fid & 255L), (byte)(fid >> 8 & 255L), (byte)(fid >> 16 & 255L), (byte)(fid >> 24 & 255L), (byte)(fid >> 32 & 255L), (byte)(fid >> 40 & 255L), (byte)(fid >> 48 & 255L), (byte)(fid >> 56 & 255L));
		}
	}
}
