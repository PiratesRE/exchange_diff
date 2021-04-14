using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Data.Providers
{
	internal sealed class MailMessageSchema : SimpleProviderObjectSchema
	{
		private static SimpleProviderPropertyDefinition MakeProperty(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints)
		{
			return new SimpleProviderPropertyDefinition(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints);
		}

		private static SimpleProviderPropertyDefinition MakeProperty(string name, Type type, object defaultValue)
		{
			return MailMessageSchema.MakeProperty(name, ExchangeObjectVersion.Exchange2010, type, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		private static SimpleProviderPropertyDefinition MakeProperty(string name, Type type)
		{
			return MailMessageSchema.MakeProperty(name, type, null);
		}

		public static readonly SimpleProviderPropertyDefinition Subject = MailMessageSchema.MakeProperty("Subject", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 255)
		});

		public static readonly SimpleProviderPropertyDefinition Body = MailMessageSchema.MakeProperty("Body", typeof(string), string.Empty);

		public static readonly SimpleProviderPropertyDefinition BodyFormat = MailMessageSchema.MakeProperty("BodyFormat", typeof(MailBodyFormat), MailBodyFormat.PlainText);

		public new static readonly SimpleProviderPropertyDefinition Identity = MailMessageSchema.MakeProperty("Identity", typeof(StoreObjectId));
	}
}
