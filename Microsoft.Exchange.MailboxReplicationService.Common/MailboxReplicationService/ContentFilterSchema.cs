using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ContentFilterSchema : SimpleProviderObjectSchema
	{
		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Sender = new ContentFilterSchema.ContentFilterPropertyDefinition("Sender", typeof(string), null, PropTag.SearchSender, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildRecipientRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition To = new ContentFilterSchema.ContentFilterPropertyDefinition("To", typeof(string), null, PropTag.SearchRecipientsTo, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildRecipientRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition BCC = new ContentFilterSchema.ContentFilterPropertyDefinition("BCC", typeof(string), null, PropTag.SearchRecipientsBcc, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildRecipientRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition CC = new ContentFilterSchema.ContentFilterPropertyDefinition("CC", typeof(string), null, PropTag.SearchRecipientsCc, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildRecipientRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Participants = new ContentFilterSchema.ContentFilterPropertyDefinition("Participants", typeof(string), null, PropTag.SearchRecipients, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildRecipientRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Subject = new ContentFilterSchema.ContentFilterPropertyDefinition("Subject", typeof(string), null, PropTag.Subject, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Body = new ContentFilterSchema.ContentFilterPropertyDefinition("Body", typeof(string), null, PropTag.Body, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Sent = new ContentFilterSchema.ContentFilterPropertyDefinition("Sent", typeof(DateTime), DateTime.MinValue, PropTag.ClientSubmitTime, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Received = new ContentFilterSchema.ContentFilterPropertyDefinition("Received", typeof(DateTime), DateTime.MinValue, PropTag.MessageDeliveryTime, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Attachment = new ContentFilterSchema.ContentFilterPropertyDefinition("Attachment", typeof(string), null, PropTag.SearchAttachments, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildAttachmentRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition MessageKind = new ContentFilterSchema.ContentFilterPropertyDefinition("MessageKind", typeof(MessageKindEnum), MessageKindEnum.Email, PropTag.MessageClass, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildMessageKindRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition PolicyTag = new ContentFilterSchema.ContentFilterPropertyDefinition("PolicyTag", typeof(string), null, PropTag.PolicyTag, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPolicyTagRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Expires = new ContentFilterSchema.ContentFilterPropertyDefinition("Expires", typeof(DateTime), DateTime.MinValue, PropTag.RetentionDate, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition IsFlagged = new ContentFilterSchema.ContentFilterPropertyDefinition("IsFlagged", typeof(bool), false, (PropTag)277872643U, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildIsFlaggedRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition IsRead = new ContentFilterSchema.ContentFilterPropertyDefinition("IsRead", typeof(bool), false, PropTag.MessageFlags, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildIsReadRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Category = new ContentFilterSchema.ContentFilterPropertyDefinition("Category", typeof(string), null, new NamedPropData(WellKnownPropertySet.PublicStrings, "Keywords"), PropType.StringArray, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Importance = new ContentFilterSchema.ContentFilterPropertyDefinition("Importance", typeof(ImportanceEnum), ImportanceEnum.Normal, PropTag.Importance, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition Size = new ContentFilterSchema.ContentFilterPropertyDefinition("Size", typeof(ByteQuantifiedSize), ByteQuantifiedSize.MinValue, PropTag.MessageSize, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition HasAttachment = new ContentFilterSchema.ContentFilterPropertyDefinition("HasAttachment", typeof(bool), false, PropTag.Hasattach, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition All = new ContentFilterSchema.ContentFilterPropertyDefinition("All", typeof(string), null, PropTag.SearchAllIndexedProps, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public static readonly ContentFilterSchema.ContentFilterPropertyDefinition MessageLocale = new ContentFilterSchema.ContentFilterPropertyDefinition("MessageLocale", typeof(CultureInfo), null, PropTag.LocaleId, new ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate(ContentFilterBuilder.BuildPropertyRestriction));

		public class ContentFilterPropertyDefinition : SimpleProviderPropertyDefinition
		{
			public ContentFilterPropertyDefinition(string propertyName, Type propertyType, object defaultValue, PropTag propTagToSearch, ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate convertToRestriction) : base(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, (defaultValue == null) ? PropertyDefinitionFlags.None : PropertyDefinitionFlags.PersistDefaultValue, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
			{
				this.propTagToSearch = propTagToSearch;
				this.namedPropToSearch = null;
				this.convertToRestriction = convertToRestriction;
			}

			public ContentFilterPropertyDefinition(string propertyName, Type propertyType, object defaultValue, NamedPropData namedPropToSearch, PropType namedPropType, ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate convertToRestriction) : base(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, (defaultValue == null) ? PropertyDefinitionFlags.None : PropertyDefinitionFlags.PersistDefaultValue, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None)
			{
				this.propTagToSearch = PropTag.Null;
				this.namedPropToSearch = namedPropToSearch;
				this.namedPropType = namedPropType;
				this.convertToRestriction = convertToRestriction;
			}

			public PropTag PropTagToSearch
			{
				get
				{
					return this.propTagToSearch;
				}
			}

			public NamedPropData NamedPropToSearch
			{
				get
				{
					return this.namedPropToSearch;
				}
			}

			public PropType NamedPropType
			{
				get
				{
					return this.namedPropType;
				}
			}

			public ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate ConvertToRestriction
			{
				get
				{
					return this.convertToRestriction;
				}
			}

			private PropTag propTagToSearch;

			private NamedPropData namedPropToSearch;

			private PropType namedPropType;

			private ContentFilterSchema.ContentFilterPropertyDefinition.ConvertToRestrictionDelegate convertToRestriction;

			public delegate Restriction ConvertToRestrictionDelegate(SinglePropertyFilter filter, IFilterBuilderHelper mapper);
		}
	}
}
