using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Data
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class DatabaseSizeInfo
	{
		[IgnoreDataMember]
		public ByteQuantifiedSize AvailableWhitespace
		{
			get
			{
				return ByteQuantifiedSize.FromBytes(this.avaialbleWhitespaceBytes);
			}
			set
			{
				this.avaialbleWhitespaceBytes = value.ToBytes();
			}
		}

		[IgnoreDataMember]
		public ByteQuantifiedSize CurrentPhysicalSize
		{
			get
			{
				return ByteQuantifiedSize.FromBytes(this.currentSizeBytes);
			}
			set
			{
				this.currentSizeBytes = value.ToBytes();
			}
		}

		[DataMember]
		private ulong avaialbleWhitespaceBytes;

		[DataMember]
		private ulong currentSizeBytes;
	}
}
