using System;

namespace Microsoft.Exchange.Data.Providers
{
	internal sealed class AlternateMailboxSchema : SimpleProviderObjectSchema
	{
		private static SimpleProviderPropertyDefinition MakeProperty(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints)
		{
			return new SimpleProviderPropertyDefinition(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints);
		}

		private static SimpleProviderPropertyDefinition MakeProperty(string name, Type type, object defaultValue)
		{
			return AlternateMailboxSchema.MakeProperty(name, ExchangeObjectVersion.Exchange2010, type, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		private static SimpleProviderPropertyDefinition MakeProperty(string name, Type type)
		{
			return AlternateMailboxSchema.MakeProperty(name, type, null);
		}

		public static readonly SimpleProviderPropertyDefinition Name = AlternateMailboxSchema.MakeProperty("Name", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new MandatoryStringLengthConstraint(1, 127),
			new CharacterConstraint(new char[]
			{
				'\\',
				'/',
				'=',
				';',
				'\0',
				'\n'
			}, false)
		});

		public static readonly SimpleProviderPropertyDefinition UserDisplayName = AlternateMailboxSchema.MakeProperty("UserDisplayName", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new CharacterConstraint(new char[]
			{
				'\\',
				'/',
				'=',
				';',
				'\0',
				'\n'
			}, false)
		});

		public static readonly SimpleProviderPropertyDefinition RetentionPolicyEnabled = AlternateMailboxSchema.MakeProperty("RetentionPolicyEnabled", typeof(bool), false);

		public static readonly SimpleProviderPropertyDefinition Type = AlternateMailboxSchema.MakeProperty("Type", typeof(AlternateMailbox.AlternateMailboxFlags), AlternateMailbox.AlternateMailboxFlags.Unknown);

		public new static readonly SimpleProviderPropertyDefinition Identity = AlternateMailboxSchema.MakeProperty("Identity", typeof(AlternateMailboxObjectId));

		public static readonly SimpleProviderPropertyDefinition DatabaseGuid = AlternateMailboxSchema.MakeProperty("DatabaseGuid", typeof(Guid), Guid.Empty);

		public static readonly SimpleProviderPropertyDefinition MailboxGuid = AlternateMailboxSchema.MakeProperty("Guid", typeof(Guid), Guid.Empty);
	}
}
