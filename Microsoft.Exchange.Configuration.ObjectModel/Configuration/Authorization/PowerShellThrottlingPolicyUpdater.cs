using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Authorization
{
	internal static class PowerShellThrottlingPolicyUpdater
	{
		static PowerShellThrottlingPolicyUpdater()
		{
			foreach (ADPropertyDefinition adpropertyDefinition in PowerShellThrottlingPolicyUpdater.PowerShellThrottlingPolicySettings)
			{
				PowerShellThrottlingPolicyUpdater.PowerShellThrottlingPolicyPropertyDictionary[adpropertyDefinition.Name] = adpropertyDefinition;
			}
		}

		internal static bool RevertExpiredThrottlingPolicyIfNeeded(IPowerShellBudget budget)
		{
			if (budget == null)
			{
				return false;
			}
			bool result;
			using (new MonitoredScope("PowerShellThrottlingPolicyUpdater", "RevertExpiredThrottlingPolicyIfNeeded", AuthZLogHelper.AuthZPerfMonitors))
			{
				ThrottlingPolicy throttlingPolicy = null;
				try
				{
					throttlingPolicy = ((EffectiveThrottlingPolicy)budget.ThrottlingPolicy).ThrottlingPolicy;
					Match match = Regex.Match(throttlingPolicy.Name, "^\\[(?<expiredtime>[0-9]{4}(-[0-9]{2}){2}T([0-9]{2}:){2}[0-9]{2})\\](?<orginalname>.+)", RegexOptions.Compiled);
					DateTime t;
					if (!match.Success)
					{
						result = false;
					}
					else if (!DateTime.TryParse(match.Groups["expiredtime"].Value, out t))
					{
						AuthZLogger.SafeAppendGenericInfo("PowerShellThrottlingPolicyUpdater", string.Format("Unrecognized time format in throttling policy '{0}'.", throttlingPolicy.Name));
						result = false;
					}
					else if (t > DateTime.UtcNow)
					{
						AuthZLogger.SafeAppendGenericInfo("PowerShellThrottlingPolicyUpdater", string.Format("Throttlling policy '{0}' is not expired yet.", throttlingPolicy.Name));
						result = false;
					}
					else
					{
						IConfigurationSession configuationSession = PowerShellThrottlingPolicyUpdater.GetConfiguationSession(throttlingPolicy.OrganizationId);
						ThrottlingPolicy writableThrottlingPolicy = PowerShellThrottlingPolicyUpdater.GetWritableThrottlingPolicy(configuationSession, throttlingPolicy);
						if (writableThrottlingPolicy == null || writableThrottlingPolicy.Name != throttlingPolicy.Name)
						{
							AuthZLogger.SafeAppendGenericInfo("PowerShellThrottlingPolicyUpdater", string.Format("Throttlling policy '{0}' is updated and don't need to be expired.", throttlingPolicy.Name));
							result = false;
						}
						else
						{
							string arg;
							if (PowerShellThrottlingPolicyUpdater.RevertBackupThrottlingSettings(writableThrottlingPolicy, out arg))
							{
								writableThrottlingPolicy.Name = match.Groups["orginalname"].Value;
								configuationSession.Save(writableThrottlingPolicy);
								AuthZLogger.SafeAppendGenericInfo("PowerShellThrottlingPolicyUpdater", string.Format("Revert throttling policy '{0}' to name: {1} and restore backup throttling value: {2}.", throttlingPolicy.Name, writableThrottlingPolicy.Name, arg));
							}
							else
							{
								PowerShellThrottlingPolicyUpdater.ClearThrottlingPolicyAssoicate(PowerShellThrottlingPolicyUpdater.GetRecipientSession(throttlingPolicy.OrganizationId), throttlingPolicy);
								configuationSession.Delete(writableThrottlingPolicy);
								AuthZLogger.SafeAppendGenericInfo("PowerShellThrottlingPolicyUpdater", string.Format("Delete throttling policy '{0}' and clear associates with it.", throttlingPolicy.Name));
							}
							result = true;
						}
					}
				}
				catch (TransientException arg2)
				{
					AuthZLogger.SafeAppendGenericInfo("PowerShellThrottlingPolicyUpdater", string.Format("Occur transient exception on revert throttling policy '{0}': {1}", (throttlingPolicy != null) ? throttlingPolicy.Name : string.Empty, arg2));
					result = false;
				}
				catch (Exception ex)
				{
					AuthZLogger.SafeAppendGenericError("PowerShellThrottlingPolicyUpdater", string.Format("Error on revert throttling policy '{0}': {1}", (throttlingPolicy != null) ? throttlingPolicy.Name : string.Empty, ex), KnownException.IsUnhandledException(ex));
					result = false;
				}
			}
			return result;
		}

		private static bool RevertBackupThrottlingSettings(ThrottlingPolicy policy, out string backupThrottling)
		{
			bool flag = false;
			backupThrottling = (policy[ThrottlingPolicySchema.PowerShellThrottlingBackup] as string);
			if (!string.IsNullOrEmpty(backupThrottling))
			{
				using (StringReader stringReader = new StringReader(backupThrottling))
				{
					while (stringReader.Peek() >= 0)
					{
						string[] array = stringReader.ReadLine().Split(new char[]
						{
							':'
						});
						Unlimited<uint> value;
						if (array.Length == 2 && PowerShellThrottlingPolicyUpdater.PowerShellThrottlingPolicyPropertyDictionary.ContainsKey(array[0]) && Unlimited<uint>.TryParse(array[1], out value))
						{
							policy[PowerShellThrottlingPolicyUpdater.PowerShellThrottlingPolicyPropertyDictionary[array[0]]] = new Unlimited<uint>?(value);
							flag = true;
						}
					}
				}
				if (flag)
				{
					policy[ThrottlingPolicySchema.PowerShellThrottlingBackup] = string.Empty;
				}
			}
			return flag;
		}

		private static ThrottlingPolicy GetWritableThrottlingPolicy(IConfigurationSession session, ThrottlingPolicy policy)
		{
			return ThrottlingPolicyCache.ReadThrottlingPolicyFromAD(session, policy.Id, (IConfigurationSession session1, object id) => session1.Read<ThrottlingPolicy>(policy.Id));
		}

		private static void ClearThrottlingPolicyAssoicate(IRecipientSession session, ThrottlingPolicy policy)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ThrottlingPolicy, policy.Id);
			ADRecipient[] array = session.Find<ADRecipient>(null, QueryScope.SubTree, filter, null, 100);
			if (array != null && array.Length > 0)
			{
				foreach (ADRecipient adrecipient in array)
				{
					adrecipient.ThrottlingPolicy = null;
					session.Save(adrecipient);
				}
			}
		}

		private static IConfigurationSession GetConfiguationSession(OrganizationId organizationId)
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(organizationId), 289, "GetConfiguationSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\PowerShellThrottlingPolicyUpdater.cs");
		}

		private static IRecipientSession GetRecipientSession(OrganizationId organizationId)
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(organizationId), 302, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\rbac\\PowerShellThrottlingPolicyUpdater.cs");
		}

		private const string expiredTPNamePattern = "^\\[(?<expiredtime>[0-9]{4}(-[0-9]{2}){2}T([0-9]{2}:){2}[0-9]{2})\\](?<orginalname>.+)";

		private static readonly ADPropertyDefinition[] PowerShellThrottlingPolicySettings = new ADPropertyDefinition[]
		{
			ThrottlingPolicySchema.ExchangeMaxCmdlets,
			ThrottlingPolicySchema.PowerShellMaxConcurrency,
			ThrottlingPolicySchema.PowerShellMaxTenantConcurrency,
			ThrottlingPolicySchema.PowerShellMaxBurst,
			ThrottlingPolicySchema.PowerShellRechargeRate,
			ThrottlingPolicySchema.PowerShellCutoffBalance,
			ThrottlingPolicySchema.PowerShellMaxOperations,
			ThrottlingPolicySchema.PowerShellMaxCmdlets,
			ThrottlingPolicySchema.PowerShellMaxCmdletsTimePeriod,
			ThrottlingPolicySchema.PowerShellMaxCmdletQueueDepth,
			ThrottlingPolicySchema.PowerShellMaxDestructiveCmdlets,
			ThrottlingPolicySchema.PowerShellMaxDestructiveCmdletsTimePeriod,
			ThrottlingPolicySchema.PowerShellMaxRunspaces,
			ThrottlingPolicySchema.PowerShellMaxTenantRunspaces,
			ThrottlingPolicySchema.PowerShellMaxRunspacesTimePeriod
		};

		private static readonly IDictionary<string, ADPropertyDefinition> PowerShellThrottlingPolicyPropertyDictionary = new Dictionary<string, ADPropertyDefinition>();
	}
}
