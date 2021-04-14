using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ConversationItemsDiagnostics")]
	[Serializable]
	public class GetConversationItemsDiagnosticsResponseType
	{
		[DataMember(Name = "ConversationId", IsRequired = true, Order = 1)]
		public ItemId ConversationId { get; set; }

		[DataMember(Name = "Recipients", IsRequired = true, Order = 2)]
		public SingleRecipientType[] Recipients { get; set; }

		[DataMember(Name = "ConversationNodeMetadatum", EmitDefaultValue = false, Order = 3)]
		public ConversationNodeMetadata[] ConversationNodeMetadatum
		{
			get
			{
				if (this.conversationNodeMetadatum != null && this.conversationNodeMetadatum.Count > 0)
				{
					return this.conversationNodeMetadatum.ToArray();
				}
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.conversationNodeMetadatum = null;
					return;
				}
				this.conversationNodeMetadatum = new List<ConversationNodeMetadata>(value);
			}
		}

		[DataMember(Name = "CanDelete", IsRequired = true, Order = 4)]
		public bool CanDelete { get; set; }

		private List<ConversationNodeMetadata> conversationNodeMetadatum;
	}
}
