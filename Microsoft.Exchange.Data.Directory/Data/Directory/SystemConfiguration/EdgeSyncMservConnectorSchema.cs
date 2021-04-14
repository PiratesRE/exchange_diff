using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class EdgeSyncMservConnectorSchema : EdgeSyncConnectorSchema
	{
		public static readonly ADPropertyDefinition ProvisionUrl = new ADPropertyDefinition("ProvisionUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchEdgeSyncMservProvisionUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition SettingUrl = new ADPropertyDefinition("SettingUrl", ExchangeObjectVersion.Exchange2007, typeof(Uri), "msExchEdgeSyncMservSettingUrl", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, new PropertyDefinitionConstraint[]
		{
			new UriKindConstraint(UriKind.Absolute)
		}, null, null);

		public static readonly ADPropertyDefinition LocalCertificate = new ADPropertyDefinition("LocalCertificate", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncMservLocalCertificate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RemoteCertificate = new ADPropertyDefinition("RemoteCertificate", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncMservRemoteCertificate", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PrimaryLeaseLocation = new ADPropertyDefinition("PrimaryLeaseLocation", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncMservPrimaryLeaseLocation", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BackupLeaseLocation = new ADPropertyDefinition("BackupLeaseLocation", ExchangeObjectVersion.Exchange2007, typeof(string), "msExchEdgeSyncMservBackupLeaseLocation", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
