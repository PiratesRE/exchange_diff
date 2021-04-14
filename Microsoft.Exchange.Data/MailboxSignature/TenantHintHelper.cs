using System;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal static class TenantHintHelper
	{
		public static byte[] ParseTenantHint(MailboxSignatureSectionMetadata metadata, byte[] buffer, ref int offset)
		{
			byte[] array = new byte[metadata.Length];
			Buffer.BlockCopy(buffer, offset, array, 0, metadata.Length);
			offset += metadata.Length;
			return array;
		}

		public static int SerializeTenantHint(byte[] tenantHintBlob, byte[] buffer, int offset)
		{
			if (buffer != null && tenantHintBlob != null)
			{
				Buffer.BlockCopy(tenantHintBlob, 0, buffer, offset, tenantHintBlob.Length);
			}
			if (tenantHintBlob == null)
			{
				return 0;
			}
			return tenantHintBlob.Length;
		}

		public const short RequiredTenantHintVersion = 1;
	}
}
