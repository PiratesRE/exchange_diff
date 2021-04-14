using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	internal class MRSRecurrentOperationConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "MRSScripts";
			}
		}

		[ConfigurationProperty("TargetAddressOnMailboxRecoveryWorkflowIsEnabled", DefaultValue = false)]
		public bool TargetAddressRecoveryEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("TargetAddressOnMailboxRecoveryWorkflowIsEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "TargetAddressOnMailboxRecoveryWorkflowIsEnabled");
			}
		}

		[ConfigurationProperty("MidsetDeletedRecoveryE15WorkflowIsEnabled", DefaultValue = false)]
		public bool MidsetRecoveryEnabled
		{
			get
			{
				return this.InternalGetConfig<bool>("MidsetDeletedRecoveryE15WorkflowIsEnabled");
			}
			set
			{
				this.InternalSetConfig<bool>(value, "MidsetDeletedRecoveryE15WorkflowIsEnabled");
			}
		}

		public MRSRecurrentOperationConfigSchema()
		{
			if (CommonUtils.IsMultiTenantEnabled())
			{
				base.SetDefaultConfigValue<bool>("TargetAddressOnMailboxRecoveryWorkflowIsEnabled", true);
				base.SetDefaultConfigValue<bool>("MidsetDeletedRecoveryE15WorkflowIsEnabled", true);
			}
		}

		[Serializable]
		public static class Setting
		{
			public const string TargetAddressRecoveryIsEnabled = "TargetAddressOnMailboxRecoveryWorkflowIsEnabled";

			public const string MidsetRecoveryIsEnabled = "MidsetDeletedRecoveryE15WorkflowIsEnabled";
		}
	}
}
