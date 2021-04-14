using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ConversationsConfiguration : MessagingConfigurationBase
	{
		public ConversationsConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
		}

		[DataMember]
		public string ConversationSortOrder
		{
			get
			{
				return base.MailboxMessageConfiguration.ConversationSortOrder.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ShowConversationAsTree
		{
			get
			{
				return base.MailboxMessageConfiguration.ShowConversationAsTree.ToString().ToLowerInvariant();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool HideDeletedItems
		{
			get
			{
				return base.MailboxMessageConfiguration.HideDeletedItems;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
