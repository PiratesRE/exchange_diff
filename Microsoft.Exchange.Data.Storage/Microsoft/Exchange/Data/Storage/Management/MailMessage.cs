using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class MailMessage : XsoMailboxConfigurationObject
	{
		internal override XsoMailboxConfigurationObjectSchema Schema
		{
			get
			{
				return MailMessage.schema;
			}
		}

		public ADRecipientOrAddress[] Bcc
		{
			get
			{
				return (ADRecipientOrAddress[])this[MailMessageSchema.Bcc];
			}
			private set
			{
				this[MailMessageSchema.Bcc] = value;
			}
		}

		public ADRecipientOrAddress[] Cc
		{
			get
			{
				return (ADRecipientOrAddress[])this[MailMessageSchema.Cc];
			}
			private set
			{
				this[MailMessageSchema.Cc] = value;
			}
		}

		public ADRecipientOrAddress From
		{
			get
			{
				return (ADRecipientOrAddress)this[MailMessageSchema.From];
			}
		}

		public ADRecipientOrAddress Sender
		{
			get
			{
				return (ADRecipientOrAddress)this[MailMessageSchema.Sender];
			}
		}

		public string Subject
		{
			get
			{
				return (string)this[MailMessageSchema.Subject];
			}
		}

		public ADRecipientOrAddress[] To
		{
			get
			{
				return (ADRecipientOrAddress[])this[MailMessageSchema.To];
			}
			private set
			{
				this[MailMessageSchema.To] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return (MailboxStoreObjectId)this[MailMessageSchema.Identity];
			}
		}

		internal VersionedId InternalMessageIdentity
		{
			get
			{
				return (VersionedId)this[MailMessageSchema.InternalMessageIdentity];
			}
		}

		private static ADRecipientOrAddress[] GetSpecifiedRecipients(RecipientCollection recipients, RecipientItemType recipientType)
		{
			if (recipients == null || recipients.Count <= 0)
			{
				return null;
			}
			List<ADRecipientOrAddress> list = new List<ADRecipientOrAddress>(recipients.Count);
			for (int i = 0; i < recipients.Count; i++)
			{
				if (recipients[i].RecipientItemType == recipientType)
				{
					list.Add(new ADRecipientOrAddress(recipients[i].Participant));
				}
			}
			if (list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		internal static object FromGetter(IPropertyBag propertyBag)
		{
			Participant participant = (Participant)propertyBag[MailMessageSchema.RawFrom];
			if (null != participant)
			{
				return new ADRecipientOrAddress(participant);
			}
			return null;
		}

		internal static object IdentityGetter(IPropertyBag propertyBag)
		{
			ADObjectId mailboxOwnerId = (ADObjectId)propertyBag[XsoMailboxConfigurationObjectSchema.MailboxOwnerId];
			VersionedId versionedId = (VersionedId)propertyBag[MailMessageSchema.InternalMessageIdentity];
			if (versionedId != null)
			{
				return new MailboxStoreObjectId(mailboxOwnerId, (versionedId == null) ? null : versionedId.ObjectId);
			}
			return null;
		}

		internal static object SenderGetter(IPropertyBag propertyBag)
		{
			Participant participant = (Participant)propertyBag[MailMessageSchema.RawSender];
			if (null != participant)
			{
				return new ADRecipientOrAddress(participant);
			}
			return null;
		}

		internal void SetRecipients(RecipientCollection recipients)
		{
			this.Bcc = MailMessage.GetSpecifiedRecipients(recipients, RecipientItemType.Bcc);
			this.Cc = MailMessage.GetSpecifiedRecipients(recipients, RecipientItemType.Cc);
			this.To = MailMessage.GetSpecifiedRecipients(recipients, RecipientItemType.To);
		}

		public override string ToString()
		{
			if (this.Subject != null)
			{
				return this.Subject;
			}
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return base.ToString();
		}

		private static MailMessageSchema schema = new MailMessageSchema();
	}
}
