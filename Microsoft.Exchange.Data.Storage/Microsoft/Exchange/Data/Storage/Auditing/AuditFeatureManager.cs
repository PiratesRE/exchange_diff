using System;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage.Auditing
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AuditFeatureManager
	{
		public static bool IsPartitionedMailboxLogEnabled(IExchangePrincipal exchangePrincipal)
		{
			if (AuditFeatureManager.partitionedMailboxLogOverride != null)
			{
				return AuditFeatureManager.partitionedMailboxLogOverride();
			}
			if (AuditFeatureManager.isPartitionedMailboxLogEnabled == null)
			{
				bool? flag = AuditFeatureManager.ReadTertiaryValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters", "PartitionedMailboxLogEnabled");
				if (flag != null)
				{
					AuditFeatureManager.isPartitionedMailboxLogEnabled = flag;
				}
				else
				{
					AuditFeatureManager.isPartitionedMailboxLogEnabled = new bool?(VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.PartitionedMailboxAuditLogs.Enabled);
				}
			}
			return AuditFeatureManager.isPartitionedMailboxLogEnabled.Value;
		}

		public static bool IsPartitionedAdminLogEnabled(IExchangePrincipal exchangePrincipal)
		{
			return AuditFeatureManager.IsEnabled(AuditFeatureManager.partitionedAuditLogOverride, AuditFeatureManager.isPartitionedAdminLogEnabled, exchangePrincipal, (VariantConfigurationSnapshot.IpaedSettingsIni settings) => settings.PartitionedAdminAuditLogs);
		}

		public static bool IsAdminAuditCmdletBlockListEnabled()
		{
			if (AuditFeatureManager.adminAuditCmdletBlockListOverride != null)
			{
				return AuditFeatureManager.adminAuditCmdletBlockListOverride();
			}
			return VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.AdminAuditCmdletBlockList.Enabled;
		}

		public static bool IsAdminAuditEventLogThrottlingEnabled()
		{
			if (AuditFeatureManager.AdminAuditEventLogThrottlingOverride != null)
			{
				return AuditFeatureManager.AdminAuditEventLogThrottlingOverride();
			}
			return VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.AdminAuditEventLogThrottling.Enabled;
		}

		public static bool IsAdminAuditLocalQueueEnabled(IExchangePrincipal exchangePrincipal)
		{
			return AuditFeatureManager.IsEnabled(AuditFeatureManager.AdminAuditLocalQueueOverride, AuditFeatureManager.isAdminAuditLocalQueueEnabled, exchangePrincipal, (VariantConfigurationSnapshot.IpaedSettingsIni settings) => settings.AdminAuditLocalQueue);
		}

		public static bool IsMailboxAuditLocalQueueEnabled(IExchangePrincipal exchangePrincipal)
		{
			return AuditFeatureManager.IsEnabled(AuditFeatureManager.MailboxAuditLocalQueueOverride, AuditFeatureManager.isMailboxAuditLocalQueueEnabled, exchangePrincipal, (VariantConfigurationSnapshot.IpaedSettingsIni settings) => settings.MailboxAuditLocalQueue);
		}

		public static bool IsExternalAccessCheckOnDedicatedEnabled()
		{
			if (AuditFeatureManager.ExternalAccessCheckOnDedicatedEnabledOverride != null)
			{
				return AuditFeatureManager.ExternalAccessCheckOnDedicatedEnabledOverride();
			}
			return VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.AdminAuditExternalAccessCheckOnDedicated.Enabled;
		}

		public static bool IsAuditConfigFromUCCPolicyEnabled(MailboxSession mailboxSession, IExchangePrincipal exchangePrincipal)
		{
			if (AuditFeatureManager.AuditConfigFromUCCPolicyOverride != null)
			{
				return AuditFeatureManager.AuditConfigFromUCCPolicyOverride();
			}
			if (AuditFeatureManager.isAuditConfigFromUCCPolicyInRegistry)
			{
				bool? flag = AuditFeatureManager.ReadTertiaryValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters", "AuditConfigFromUCCPolicyEnabled");
				AuditFeatureManager.isAuditConfigFromUCCPolicyInRegistry = (flag != null);
				if (flag != null)
				{
					return flag.Value;
				}
			}
			if (mailboxSession != null)
			{
				return mailboxSession.IsAuditConfigFromUCCPolicyEnabled;
			}
			if (exchangePrincipal != null)
			{
				VariantConfigurationSnapshot configuration = exchangePrincipal.GetConfiguration();
				if (configuration != null)
				{
					return configuration.Ipaed.AuditConfigFromUCCPolicy.Enabled;
				}
			}
			return false;
		}

		public static bool IsFolderBindExtendedThrottlingEnabled()
		{
			if (AuditFeatureManager.folderBindExtendedThrottlingOverride != null)
			{
				return AuditFeatureManager.folderBindExtendedThrottlingOverride();
			}
			return VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.FolderBindExtendedThrottling.Enabled;
		}

		private static AuditFeatureManager.Tertiary IsFeatureEnabled(string registrySubkeyPath, string valueName)
		{
			AuditFeatureManager.Tertiary result = AuditFeatureManager.Tertiary.Unknown;
			try
			{
				object value = RegistryReader.Instance.GetValue<object>(Registry.LocalMachine, registrySubkeyPath, valueName, AuditFeatureManager.sentinelDefault);
				if (value != null && value is int)
				{
					result = (((int)value > 0) ? AuditFeatureManager.Tertiary.True : AuditFeatureManager.Tertiary.False);
				}
			}
			catch (IOException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return result;
		}

		private static bool? ReadTertiaryValueFromRegistry(string registrySubkeyPath, string valueName)
		{
			switch (AuditFeatureManager.IsFeatureEnabled(registrySubkeyPath, valueName))
			{
			case AuditFeatureManager.Tertiary.False:
				return new bool?(false);
			case AuditFeatureManager.Tertiary.True:
				return new bool?(true);
			default:
				return null;
			}
		}

		private static bool IsEnabled(Func<bool> overrideCallback, Lazy<bool?> cachedValue, IExchangePrincipal exchangePrincipal, Func<VariantConfigurationSnapshot.IpaedSettingsIni, IFeature> getFlightFeature)
		{
			if (overrideCallback != null)
			{
				return overrideCallback();
			}
			bool? value = cachedValue.Value;
			bool result;
			if (value != null)
			{
				result = value.Value;
			}
			else
			{
				VariantConfigurationSnapshot configuration = exchangePrincipal.GetConfiguration();
				result = getFlightFeature(configuration.Ipaed).Enabled;
			}
			return result;
		}

		internal static void TestOnlyResetPartitionedAdminLogEnabled()
		{
			AuditFeatureManager.isPartitionedAdminLogEnabled = new Lazy<bool?>(() => AuditFeatureManager.ReadTertiaryValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters", "PartitionedAdminLogEnabled"), LazyThreadSafetyMode.ExecutionAndPublication);
		}

		public const string AuditKeyBase = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters";

		public const string PartitionedMailboxLogEnabled = "PartitionedMailboxLogEnabled";

		public const string PartitionedAdminLogEnabled = "PartitionedAdminLogEnabled";

		public const string AdminAuditLocalQueueEnabled = "AdminAuditLocalQueueEnabled";

		public const string MailboxAuditLocalQueueEnabled = "MailboxAuditLocalQueueEnabled";

		public const string AuditConfigFromUCCPolicyEnabled = "AuditConfigFromUCCPolicyEnabled";

		private static Func<bool> partitionedMailboxLogOverride = null;

		private static Func<bool> partitionedAuditLogOverride = null;

		private static Func<bool> adminAuditCmdletBlockListOverride = null;

		private static Func<bool> AdminAuditEventLogThrottlingOverride = null;

		private static Func<bool> ExternalAccessCheckOnDedicatedEnabledOverride = null;

		private static Func<bool> AdminAuditLocalQueueOverride = null;

		private static Func<bool> MailboxAuditLocalQueueOverride = null;

		private static Func<bool> AuditConfigFromUCCPolicyOverride = null;

		private static Func<bool> folderBindExtendedThrottlingOverride = null;

		private static bool? isPartitionedMailboxLogEnabled = null;

		private static Lazy<bool?> isPartitionedAdminLogEnabled = new Lazy<bool?>(() => AuditFeatureManager.ReadTertiaryValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters", "PartitionedAdminLogEnabled"), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<bool?> isAdminAuditLocalQueueEnabled = new Lazy<bool?>(() => AuditFeatureManager.ReadTertiaryValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters", "AdminAuditLocalQueueEnabled"), LazyThreadSafetyMode.ExecutionAndPublication);

		private static Lazy<bool?> isMailboxAuditLocalQueueEnabled = new Lazy<bool?>(() => AuditFeatureManager.ReadTertiaryValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Audit\\Parameters", "MailboxAuditLocalQueueEnabled"), LazyThreadSafetyMode.ExecutionAndPublication);

		private static bool isAuditConfigFromUCCPolicyInRegistry = true;

		private static readonly object sentinelDefault = new object();

		private enum Tertiary
		{
			False,
			True,
			Unknown
		}
	}
}
