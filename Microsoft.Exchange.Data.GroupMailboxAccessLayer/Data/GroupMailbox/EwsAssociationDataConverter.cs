using System;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EwsAssociationDataConverter
	{
		internal static MailboxAssociationType Convert(MailboxAssociation association)
		{
			ArgumentValidator.ThrowIfNull("association", association);
			return new MailboxAssociationType
			{
				User = EwsAssociationDataConverter.Convert(association.User),
				Group = EwsAssociationDataConverter.Convert(association.Group),
				IsMember = association.IsMember,
				IsMemberSpecified = true,
				IsPin = association.IsPin,
				IsPinSpecified = true,
				JoinDate = (DateTime)association.JoinDate,
				JoinDateSpecified = true,
				JoinedBy = association.JoinedBy
			};
		}

		internal static MailboxAssociation Convert(MailboxAssociationType associationType, IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNull("associationType", associationType);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			return new MailboxAssociation
			{
				Group = EwsAssociationDataConverter.Convert(associationType.Group, adSession),
				User = EwsAssociationDataConverter.Convert(associationType.User, adSession),
				IsMember = associationType.IsMember,
				IsPin = associationType.IsPin,
				JoinDate = (ExDateTime)associationType.JoinDate,
				JoinedBy = associationType.JoinedBy
			};
		}

		internal static GroupLocatorType Convert(GroupMailboxLocator locator)
		{
			ArgumentValidator.ThrowIfNull("locator", locator);
			return new GroupLocatorType
			{
				ExternalDirectoryObjectId = locator.ExternalId,
				LegacyDn = locator.LegacyDn
			};
		}

		internal static GroupMailboxLocator Convert(GroupLocatorType locatorType, IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNull("locatorType", locatorType);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			return new GroupMailboxLocator(adSession, locatorType.ExternalDirectoryObjectId, locatorType.LegacyDn);
		}

		internal static UserLocatorType Convert(UserMailboxLocator locator)
		{
			ArgumentValidator.ThrowIfNull("locator", locator);
			return new UserLocatorType
			{
				ExternalDirectoryObjectId = locator.ExternalId,
				LegacyDn = locator.LegacyDn
			};
		}

		internal static UserMailboxLocator Convert(UserLocatorType locatorType, IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNull("locatorType", locatorType);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			return new UserMailboxLocator(adSession, locatorType.ExternalDirectoryObjectId, locatorType.LegacyDn);
		}

		internal static MailboxLocatorType Convert(IMailboxLocator locator)
		{
			ArgumentValidator.ThrowIfNull("locator", locator);
			GroupMailboxLocator groupMailboxLocator = locator as GroupMailboxLocator;
			if (groupMailboxLocator != null)
			{
				return EwsAssociationDataConverter.Convert(groupMailboxLocator);
			}
			UserMailboxLocator userMailboxLocator = locator as UserMailboxLocator;
			if (userMailboxLocator != null)
			{
				return EwsAssociationDataConverter.Convert(userMailboxLocator);
			}
			throw new NotImplementedException(string.Format("Conversion of '{0}' is not yet supported.", locator.GetType()));
		}

		internal static MailboxLocator Convert(MailboxLocatorType locatorType, IRecipientSession adSession)
		{
			ArgumentValidator.ThrowIfNull("locatorType", locatorType);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			GroupLocatorType groupLocatorType = locatorType as GroupLocatorType;
			if (groupLocatorType != null)
			{
				return EwsAssociationDataConverter.Convert(groupLocatorType, adSession);
			}
			UserLocatorType userLocatorType = locatorType as UserLocatorType;
			if (userLocatorType != null)
			{
				return EwsAssociationDataConverter.Convert(userLocatorType, adSession);
			}
			throw new InvalidOperationException("Unsupported type of Mailbox Locator");
		}

		internal static ModernGroupTypeType Convert(ModernGroupObjectType type)
		{
			switch (type)
			{
			case ModernGroupObjectType.Private:
				return ModernGroupTypeType.Private;
			case ModernGroupObjectType.Secret:
				return ModernGroupTypeType.Secret;
			case ModernGroupObjectType.Public:
				return ModernGroupTypeType.Public;
			}
			return ModernGroupTypeType.None;
		}

		internal static ModernGroupObjectType Convert(ModernGroupTypeType type)
		{
			switch (type)
			{
			case ModernGroupTypeType.Private:
				return ModernGroupObjectType.Private;
			case ModernGroupTypeType.Secret:
				return ModernGroupObjectType.Secret;
			case ModernGroupTypeType.Public:
				return ModernGroupObjectType.Public;
			}
			return ModernGroupObjectType.None;
		}

		private static readonly Trace Tracer = ExTraceGlobals.AssociationReplicationTracer;
	}
}
