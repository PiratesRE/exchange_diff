using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types.Conversations
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ThreadedConversation")]
	[Serializable]
	public class ThreadedConversationResponseType : IConversationResponseType
	{
		[DataMember(Name = "ConversationId", IsRequired = true, Order = 1)]
		public ItemId ConversationId { get; set; }

		[IgnoreDataMember]
		public byte[] SyncState { get; set; }

		[DataMember(Name = "SyncState", EmitDefaultValue = false, Order = 2)]
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

		[DataMember(Name = "ConversationThreads", EmitDefaultValue = false, Order = 3)]
		public ConversationThreadType[] ConversationThreads
		{
			get
			{
				if (this.threads != null && this.threads.Count > 0)
				{
					return this.threads.ToArray();
				}
				return null;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.threads = null;
					return;
				}
				this.threads = new List<ConversationThreadType>(value);
			}
		}

		[DataMember(Name = "TotalThreadCount", IsRequired = false, Order = 4)]
		public int TotalThreadCount { get; set; }

		private List<ConversationThreadType> threads;
	}
}
