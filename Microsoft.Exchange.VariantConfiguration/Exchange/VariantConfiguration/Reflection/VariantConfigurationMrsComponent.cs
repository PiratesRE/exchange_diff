using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationMrsComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationMrsComponent() : base("Mrs")
		{
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "MigrationMonitor", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "PublicFolderMailboxesMigration", typeof(IFeature), true));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "UseDefaultValueForCheckInitialProvisioningForMovesParameter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "SlowMRSDetector", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "CheckProvisioningSettings", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "TxSyncMrsImapExecute", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "TxSyncMrsImapCopy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Mrs.settings.ini", "AutomaticMailboxLoadBalancing", typeof(IFeature), false));
		}

		public VariantConfigurationSection MigrationMonitor
		{
			get
			{
				return base["MigrationMonitor"];
			}
		}

		public VariantConfigurationSection PublicFolderMailboxesMigration
		{
			get
			{
				return base["PublicFolderMailboxesMigration"];
			}
		}

		public VariantConfigurationSection UseDefaultValueForCheckInitialProvisioningForMovesParameter
		{
			get
			{
				return base["UseDefaultValueForCheckInitialProvisioningForMovesParameter"];
			}
		}

		public VariantConfigurationSection SlowMRSDetector
		{
			get
			{
				return base["SlowMRSDetector"];
			}
		}

		public VariantConfigurationSection CheckProvisioningSettings
		{
			get
			{
				return base["CheckProvisioningSettings"];
			}
		}

		public VariantConfigurationSection TxSyncMrsImapExecute
		{
			get
			{
				return base["TxSyncMrsImapExecute"];
			}
		}

		public VariantConfigurationSection TxSyncMrsImapCopy
		{
			get
			{
				return base["TxSyncMrsImapCopy"];
			}
		}

		public VariantConfigurationSection AutomaticMailboxLoadBalancing
		{
			get
			{
				return base["AutomaticMailboxLoadBalancing"];
			}
		}
	}
}
