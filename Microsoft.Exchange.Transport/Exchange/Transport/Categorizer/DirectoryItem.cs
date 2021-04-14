using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal abstract class DirectoryItem : RecipientItem
	{
		public DirectoryItem(MailRecipient recipient) : base(recipient)
		{
		}

		public static CachedProperty[] AllCachedProperties
		{
			get
			{
				if (DirectoryItem.allCachedProperties == null)
				{
					List<CachedProperty> list = new List<CachedProperty>();
					list.AddRange(DirectoryItemSchema.CachedProperties);
					list.AddRange(RestrictedItemSchema.CachedProperties);
					list.AddRange(DeliverableItemSchema.CachedProperties);
					list.AddRange(MailboxItemSchema.CachedProperties);
					list.AddRange(ContactItemSchema.CachedProperties);
					list.AddRange(GroupItemSchema.CachedProperties);
					list.AddRange(ForwardableItemSchema.CachedProperties);
					list.AddRange(PublicFolderItemSchema.CachedProperties);
					list.AddRange(PublicDatabaseItemSchema.CachedProperties);
					list.AddRange(ReroutableItemSchema.CachedProperties);
					list.AddRange(SenderSchema.CachedProperties);
					DirectoryItem.allCachedProperties = list.ToArray();
				}
				return DirectoryItem.allCachedProperties;
			}
		}

		public RecipientType RecipientType
		{
			get
			{
				return base.GetProperty<RecipientType>("Microsoft.Exchange.Transport.DirectoryData.RecipientType");
			}
		}

		public RecipientTypeDetails? RecipientTypeDetails
		{
			get
			{
				long? property = base.GetProperty<long?>("Microsoft.Exchange.Transport.DirectoryData.RecipientTypeDetailsRaw");
				if (property != null && Enum.IsDefined(typeof(RecipientTypeDetails), property))
				{
					return new RecipientTypeDetails?((RecipientTypeDetails)property.Value);
				}
				return null;
			}
		}

		public new static DirectoryItem Create(MailRecipient recipient)
		{
			RecipientType recipientType;
			if (!recipient.ExtendedProperties.TryGetValue<RecipientType>("Microsoft.Exchange.Transport.DirectoryData.RecipientType", out recipientType))
			{
				return null;
			}
			switch (recipientType)
			{
			case RecipientType.Invalid:
				return new InvalidItem(recipient);
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
			{
				string text;
				if (ContactItem.TryGetTargetAddress(recipient, out text))
				{
					return new ContactItem(recipient);
				}
				return new ForwardableItem(recipient);
			}
			case RecipientType.Contact:
			case RecipientType.MailContact:
				return new ContactItem(recipient);
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
			{
				string text;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TargetAddressRoutingForRemoteGroupMailbox.Enabled && ContactItem.TryGetTargetAddress(recipient, out text) && DirectoryItem.IsRecipientRemoteUnifiedGroup(recipient))
				{
					return new ContactItem(recipient);
				}
				return new GroupItem(recipient);
			}
			case RecipientType.PublicFolder:
			{
				bool flag = false;
				if (recipient.ExtendedProperties.TryGetValue<bool>("Microsoft.Exchange.Transport.IsRemoteRecipient", out flag) && flag)
				{
					return new ContactItem(recipient);
				}
				bool flag2 = false;
				if (recipient.ExtendedProperties.TryGetValue<bool>("Microsoft.Exchange.Transport.IsOneOffRecipient", out flag2) && flag2)
				{
					return null;
				}
				return new PublicFolderItem(recipient);
			}
			case RecipientType.PublicDatabase:
				return new PublicDatabaseItem(recipient);
			case RecipientType.SystemAttendantMailbox:
			case RecipientType.SystemMailbox:
				return new MailboxItem(recipient);
			case RecipientType.MicrosoftExchange:
				return new MicrosoftExchangeItem(recipient);
			default:
				throw new InvalidOperationException("Unknown recipient type: " + recipientType);
			}
		}

		public static void Set(ADRawEntry entry, MailRecipient recipient)
		{
			RecipientType recipientType = (RecipientType)entry[ADRecipientSchema.RecipientType];
			switch (recipientType)
			{
			case RecipientType.Invalid:
				DirectoryItemSchema.Set(entry, recipient);
				return;
			case RecipientType.User:
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
				if (entry[ADRecipientSchema.ExternalEmailAddress] != null)
				{
					ContactItemSchema.Set(entry, recipient);
					return;
				}
				ForwardableItemSchema.Set(entry, recipient);
				return;
			case RecipientType.Contact:
			case RecipientType.MailContact:
				ContactItemSchema.Set(entry, recipient);
				return;
			case RecipientType.Group:
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Transport.TargetAddressRoutingForRemoteGroupMailbox.Enabled && entry[ADRecipientSchema.ExternalEmailAddress] != null && (RecipientTypeDetails)entry[ADRecipientSchema.RecipientTypeDetailsRaw] == Microsoft.Exchange.Data.Directory.Recipient.RecipientTypeDetails.RemoteGroupMailbox)
				{
					ContactItemSchema.Set(entry, recipient);
					return;
				}
				GroupItemSchema.Set(entry, recipient);
				return;
			case RecipientType.PublicFolder:
				PublicFolderItemSchema.Set(entry, recipient);
				return;
			case RecipientType.PublicDatabase:
				PublicDatabaseItemSchema.Set(entry, recipient);
				return;
			case RecipientType.SystemAttendantMailbox:
			case RecipientType.SystemMailbox:
				MailboxItemSchema.Set(entry, recipient);
				return;
			case RecipientType.MicrosoftExchange:
				ForwardableItemSchema.Set(entry, recipient);
				return;
			default:
				throw new InvalidOperationException("Unknown recipient type: " + recipientType);
			}
		}

		public static string GetPrimarySmtpAddress(TransportMiniRecipient entry)
		{
			SmtpAddress primarySmtpAddress = entry.PrimarySmtpAddress;
			if (primarySmtpAddress == SmtpAddress.Empty)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug(0L, "no primary SMTP address");
				return null;
			}
			if (!primarySmtpAddress.IsValidAddress)
			{
				ExTraceGlobals.ResolverTracer.TraceDebug<string>(0L, "Invalid Smtp Address {0}", primarySmtpAddress.ToString());
				return null;
			}
			return primarySmtpAddress.ToString();
		}

		public override void PostProcess(Expansion expansion)
		{
			OofRestriction.InternalUserOofCheck(expansion, base.Recipient);
		}

		protected Guid ObjectGuid
		{
			get
			{
				return base.GetProperty<Guid>("Microsoft.Exchange.Transport.DirectoryData.ObjectGuid", Guid.Empty);
			}
		}

		private static bool IsRecipientRemoteUnifiedGroup(MailRecipient recipient)
		{
			long? value = recipient.ExtendedProperties.GetValue<long?>("Microsoft.Exchange.Transport.DirectoryData.RecipientTypeDetailsRaw", null);
			return value != null && Enum.IsDefined(typeof(RecipientTypeDetails), value) && value.Value == 8796093022208L;
		}

		private static CachedProperty[] allCachedProperties;
	}
}
