using System;

namespace Microsoft.Exchange.Data
{
	internal sealed class DatabaseAvailabilityGroupNetworkSchema : SimpleProviderObjectSchema
	{
		private static SimpleProviderPropertyDefinition MakeProperty(string name, ExchangeObjectVersion versionAdded, Type type, PropertyDefinitionFlags flags, object defaultValue, PropertyDefinitionConstraint[] readConstraints, PropertyDefinitionConstraint[] writeConstraints)
		{
			return new SimpleProviderPropertyDefinition(name, versionAdded, type, flags, defaultValue, readConstraints, writeConstraints);
		}

		private static SimpleProviderPropertyDefinition MakeProperty(string name, Type type, object defaultValue)
		{
			return DatabaseAvailabilityGroupNetworkSchema.MakeProperty(name, ExchangeObjectVersion.Exchange2010, type, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		private static SimpleProviderPropertyDefinition MakeProperty(string name, Type type)
		{
			return DatabaseAvailabilityGroupNetworkSchema.MakeProperty(name, type, null);
		}

		public static readonly SimpleProviderPropertyDefinition Name = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("Name", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
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

		public static readonly SimpleProviderPropertyDefinition Description = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("Description", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new StringLengthConstraint(0, 256)
		});

		public static readonly SimpleProviderPropertyDefinition Subnets = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("Subnets", ExchangeObjectVersion.Exchange2010, typeof(DatabaseAvailabilityGroupNetworkSubnet), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly SimpleProviderPropertyDefinition ReplicationEnabled = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("ReplicationEnabled", typeof(bool), true);

		public static readonly SimpleProviderPropertyDefinition IgnoreNetwork = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("IgnoreNetwork", typeof(bool), false);

		public static readonly SimpleProviderPropertyDefinition MapiAccessEnabled = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("MapiAccessEnabled", typeof(bool), true);

		public new static readonly SimpleProviderPropertyDefinition Identity = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("Identity", typeof(DagNetworkObjectId));

		public static readonly SimpleProviderPropertyDefinition Interfaces = DatabaseAvailabilityGroupNetworkSchema.MakeProperty("Interfaces", ExchangeObjectVersion.Exchange2010, typeof(DatabaseAvailabilityGroupNetworkInterface), PropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
