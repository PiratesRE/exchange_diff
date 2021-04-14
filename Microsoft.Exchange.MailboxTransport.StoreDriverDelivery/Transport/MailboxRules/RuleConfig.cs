using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.MailboxRules;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	internal class RuleConfig : IRuleConfig
	{
		public static RuleConfig Instance
		{
			get
			{
				return RuleConfig.instance;
			}
		}

		public object SCLJunkThreshold
		{
			get
			{
				return this.sclJunkThreshold;
			}
		}

		public static void Load()
		{
			lock (RuleConfig.configLock)
			{
				GlobalConfigurationBase<UceContentFilter, RuleConfig.UceContentFilterConfiguration>.Start();
			}
		}

		public static void UnLoad()
		{
			lock (RuleConfig.configLock)
			{
				GlobalConfigurationBase<UceContentFilter, RuleConfig.UceContentFilterConfiguration>.Stop();
			}
		}

		internal static void SetSCLJunkThreshold(int threshold)
		{
			RuleConfig.instance.sclJunkThreshold = threshold;
		}

		private const int DefaultSCLJunkThreshold = 4;

		private static object configLock = new object();

		private static RuleConfig instance = new RuleConfig();

		private object sclJunkThreshold = 4;

		private sealed class UceContentFilterConfiguration : GlobalConfigurationBase<UceContentFilter, RuleConfig.UceContentFilterConfiguration>
		{
			protected override string ConfigObjectName
			{
				get
				{
					return "UceContentFilter";
				}
			}

			protected override string ReloadFailedString
			{
				get
				{
					return "Failed to load UceContentFilter config";
				}
			}

			protected override ADObjectId GetObjectId(IConfigurationSession session)
			{
				ADObjectId relativePath = new ADObjectId("CN=UCE Content Filter,CN=Message Delivery,CN=Global Settings");
				return session.GetOrgContainerId().GetDescendantId(relativePath);
			}

			protected override void HandleObjectLoaded()
			{
				RuleConfig.SetSCLJunkThreshold(base.ConfigObject.SCLJunkThreshold);
			}

			protected override bool HandleObjectNotFound()
			{
				RuleConfig.SetSCLJunkThreshold(4);
				return true;
			}
		}
	}
}
