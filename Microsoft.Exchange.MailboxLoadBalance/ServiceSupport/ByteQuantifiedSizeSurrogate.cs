using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ByteQuantifiedSizeSurrogate
	{
		public ByteQuantifiedSizeSurrogate(ByteQuantifiedSize byteQuantifiedSize)
		{
			this.canonical = byteQuantifiedSize.ToString();
		}

		public ByteQuantifiedSize ToByteQuantifiedSize()
		{
			return ByteQuantifiedSize.Parse(this.canonical);
		}

		[DataMember]
		private readonly string canonical;
	}
}
