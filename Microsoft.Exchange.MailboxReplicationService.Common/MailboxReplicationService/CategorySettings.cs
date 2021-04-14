using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal sealed class CategorySettings
	{
		[DataMember]
		public ushort Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ColorCode { get; set; }

		[DataMember]
		public int AttributeFlagsInt { get; set; }

		public OlcCategoryAttributeFlags AttributeFlags
		{
			get
			{
				return (OlcCategoryAttributeFlags)this.AttributeFlagsInt;
			}
			set
			{
				this.AttributeFlagsInt = (int)value;
			}
		}

		[DataMember]
		public byte ViewTypeByte { get; set; }

		[DataMember]
		public DateTime LastWrite { get; set; }
	}
}
