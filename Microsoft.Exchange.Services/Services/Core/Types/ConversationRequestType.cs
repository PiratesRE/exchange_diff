using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ConversationRequestType")]
	[Serializable]
	public class ConversationRequestType
	{
		[XmlElement("ConversationId", IsNullable = false)]
		[DataMember(Name = "ConversationId", IsRequired = true, Order = 1)]
		public ItemId ConversationId { get; set; }

		[XmlElement(DataType = "base64Binary")]
		[IgnoreDataMember]
		public byte[] SyncState { get; set; }

		[DataMember(Name = "SyncState", EmitDefaultValue = false, Order = 2)]
		[XmlIgnore]
		public string SyncStateString
		{
			get
			{
				byte[] syncState = this.SyncState;
				if (syncState == null)
				{
					return null;
				}
				return Convert.ToBase64String(syncState);
			}
			set
			{
				this.SyncState = (string.IsNullOrEmpty(value) ? null : Convert.FromBase64String(value));
			}
		}

		[DataMember(Name = "ConversationFamilyId", IsRequired = false, Order = 3)]
		public ItemId ConversationFamilyId { get; set; }
	}
}
