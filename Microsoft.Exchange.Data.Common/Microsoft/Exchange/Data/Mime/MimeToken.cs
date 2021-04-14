using System;

namespace Microsoft.Exchange.Data.Mime
{
	internal struct MimeToken
	{
		public MimeToken(MimeTokenId id, int length)
		{
			this.Id = id;
			this.Length = (short)length;
		}

		public MimeTokenId Id;

		public short Length;
	}
}
