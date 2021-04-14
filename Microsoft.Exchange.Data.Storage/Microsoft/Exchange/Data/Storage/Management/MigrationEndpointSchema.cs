using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management.Migration;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationEndpointSchema : ObjectSchema
	{
		public static readonly ProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2012, typeof(MigrationEndpointId), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition EndpointType = new SimpleProviderPropertyDefinition("EndpointType", ExchangeObjectVersion.Exchange2012, typeof(MigrationType), PropertyDefinitionFlags.TaskPopulated, MigrationType.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MaxConcurrentMigrations = new SimpleProviderPropertyDefinition("MaxConcurrentMigrations", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MaxConcurrentIncrementalSyncs = new SimpleProviderPropertyDefinition("MaxConcurrentIncrementalSyncs", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition RemoteServer = new SimpleProviderPropertyDefinition("RemoteServer", ExchangeObjectVersion.Exchange2012, typeof(Fqdn), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Username = new SimpleProviderPropertyDefinition("Username", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Port = new SimpleProviderPropertyDefinition("Port", ExchangeObjectVersion.Exchange2012, typeof(int?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition AuthenticationMethod = new SimpleProviderPropertyDefinition("AuthenticationMethod", ExchangeObjectVersion.Exchange2012, typeof(AuthenticationMethod?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Security = new SimpleProviderPropertyDefinition("Security", ExchangeObjectVersion.Exchange2012, typeof(IMAPSecurityMechanism?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition RPCProxyServer = new SimpleProviderPropertyDefinition("RPCProxyServer", ExchangeObjectVersion.Exchange2012, typeof(Fqdn), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ExchangeServer = new SimpleProviderPropertyDefinition("ExchangeServer", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition NspiServer = new SimpleProviderPropertyDefinition("NspiServer", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MailboxPermission = new SimpleProviderPropertyDefinition("MailboxPermission", ExchangeObjectVersion.Exchange2012, typeof(MigrationMailboxPermission), PropertyDefinitionFlags.TaskPopulated, MigrationMailboxPermission.Admin, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition UseAutoDiscover = new SimpleProviderPropertyDefinition("UseAutoDiscover", ExchangeObjectVersion.Exchange2012, typeof(bool?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly PropertyDefinition SourceMailboxLegacyDN = new SimpleProviderPropertyDefinition("SourceMailboxLegacyDN", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static PropertyDefinition PublicFolderDatabaseServerLegacyDN = new SimpleProviderPropertyDefinition("PublicFolderDatabaseServerLegacyDN", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ObjectState = UserConfigurationObjectSchema.ObjectState;

		public static readonly ProviderPropertyDefinition ExchangeVersion = UserConfigurationObjectSchema.ExchangeVersion;
	}
}
