using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MessageOptionsConfiguration : MessagingConfigurationBase
	{
		public MessageOptionsConfiguration(MailboxMessageConfiguration mailboxMessageConfiguration) : base(mailboxMessageConfiguration)
		{
		}

		[DataMember]
		public string AfterMoveOrDeleteBehavior
		{
			get
			{
				return base.MailboxMessageConfiguration.AfterMoveOrDeleteBehavior.ToString();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public int NewItemNotification
		{
			get
			{
				return (int)base.MailboxMessageConfiguration.NewItemNotification;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool EmptyDeletedItemsOnLogoff
		{
			get
			{
				return base.MailboxMessageConfiguration.EmptyDeletedItemsOnLogoff;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool CheckForForgottenAttachments
		{
			get
			{
				return base.MailboxMessageConfiguration.CheckForForgottenAttachments;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
