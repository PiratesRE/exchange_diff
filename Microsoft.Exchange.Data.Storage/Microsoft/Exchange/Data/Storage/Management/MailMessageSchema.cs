using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailMessageSchema : XsoMailboxConfigurationObjectSchema
	{
		public static readonly XsoDriverPropertyDefinition Subject = new XsoDriverPropertyDefinition(ItemSchema.Subject, "Subject", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, string.Empty, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition RawFrom = new XsoDriverPropertyDefinition(ItemSchema.From, "From", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition RawSender = new XsoDriverPropertyDefinition(ItemSchema.Sender, "Sender", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly XsoDriverPropertyDefinition InternalMessageIdentity = new XsoDriverPropertyDefinition(ItemSchema.Id, "InternalMessageIdentity", ExchangeObjectVersion.Exchange2003, PropertyDefinitionFlags.None, null, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Bcc = new SimpleProviderPropertyDefinition("Bcc", ExchangeObjectVersion.Exchange2003, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Cc = new SimpleProviderPropertyDefinition("Cc", ExchangeObjectVersion.Exchange2003, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2003, typeof(MailboxStoreObjectId), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated | PropertyDefinitionFlags.Mandatory, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailMessageSchema.InternalMessageIdentity,
			XsoMailboxConfigurationObjectSchema.MailboxOwnerId
		}, null, new GetterDelegate(MailMessage.IdentityGetter), null);

		public static readonly SimpleProviderPropertyDefinition From = new SimpleProviderPropertyDefinition("From", ExchangeObjectVersion.Exchange2003, typeof(ADRecipientOrAddress), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailMessageSchema.RawFrom
		}, null, new GetterDelegate(MailMessage.FromGetter), null);

		public static readonly SimpleProviderPropertyDefinition Sender = new SimpleProviderPropertyDefinition("Sender", ExchangeObjectVersion.Exchange2003, typeof(ADRecipientOrAddress), PropertyDefinitionFlags.ReadOnly | PropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			MailMessageSchema.RawSender
		}, null, new GetterDelegate(MailMessage.SenderGetter), null);

		public static readonly SimpleProviderPropertyDefinition To = new SimpleProviderPropertyDefinition("To", ExchangeObjectVersion.Exchange2003, typeof(ADRecipientOrAddress[]), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
