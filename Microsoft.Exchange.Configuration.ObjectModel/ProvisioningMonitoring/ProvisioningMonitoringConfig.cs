using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.ProvisioningMonitoring
{
	internal static class ProvisioningMonitoringConfig
	{
		internal static bool IsCmdletMonitoringEnabled
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.CmdletMonitoring.Enabled;
			}
		}

		internal static string GetInstanceName(string hostName, string orgName)
		{
			string empty = string.Empty;
			ProvisioningMonitoringConfig.hostIds.TryGetValue(hostName, out empty);
			if (string.IsNullOrEmpty(orgName))
			{
				orgName = "First Organization";
			}
			return string.Format("v1-{0}-{1}-{2}", empty, ProvisioningMonitoringConfig.processId, orgName);
		}

		internal static bool IsExceptionWhiteListedForCmdlet(ErrorRecord errorRecord, string cmdletName)
		{
			List<CmdletErrorContext> errorContexts;
			return errorRecord.CategoryInfo.Category == (ErrorCategory)1000 || errorRecord.CategoryInfo.Category == (ErrorCategory)1003 || errorRecord.CategoryInfo.Category == (ErrorCategory)1004 || errorRecord.CategoryInfo.Category == (ErrorCategory)1005 || errorRecord.CategoryInfo.Category == (ErrorCategory)1006 || errorRecord.CategoryInfo.Category == (ErrorCategory)1007 || errorRecord.CategoryInfo.Category == (ErrorCategory)1008 || ProvisioningMonitoringConfig.MatchesErrorContext(ProvisioningMonitoringConfig.commonErrorWhiteList, errorRecord) || (ProvisioningMonitoringConfig.monitoringConfigList.TryGetValue(cmdletName, out errorContexts) && ProvisioningMonitoringConfig.MatchesErrorContext(errorContexts, errorRecord));
		}

		internal static bool IsCmdletMonitored(string cmdletName)
		{
			return ProvisioningMonitoringConfig.monitoringConfigList.ContainsKey(cmdletName);
		}

		internal static bool IsHostMonitored(string hostName)
		{
			return ProvisioningMonitoringConfig.hostIds.ContainsKey(hostName);
		}

		internal static bool IsClientApplicationMonitored(ExchangeRunspaceConfigurationSettings.ExchangeApplication clientApplication)
		{
			return !ProvisioningMonitoringConfig.excludedClientApplications.Contains(clientApplication);
		}

		internal static void AddCmdletToMonitoredList(string cmdletName)
		{
			List<CmdletErrorContext> value = new List<CmdletErrorContext>();
			ProvisioningMonitoringConfig.monitoringConfigList.Add(cmdletName, value);
		}

		internal static void AddToCmdletWhiteList(string cmdletName, CmdletErrorContext errorContext)
		{
			List<CmdletErrorContext> list;
			if (!ProvisioningMonitoringConfig.monitoringConfigList.TryGetValue(cmdletName, out list))
			{
				list = new List<CmdletErrorContext>();
				ProvisioningMonitoringConfig.monitoringConfigList.Add(cmdletName, list);
			}
			list.Add(errorContext);
		}

		internal static void AddToCmdletWhiteList(string cmdletName, List<CmdletErrorContext> errorContextList)
		{
			List<CmdletErrorContext> list;
			if (!ProvisioningMonitoringConfig.monitoringConfigList.TryGetValue(cmdletName, out list))
			{
				list = new List<CmdletErrorContext>();
				ProvisioningMonitoringConfig.monitoringConfigList.Add(cmdletName, list);
			}
			list.AddRange(errorContextList);
		}

		internal static void AddToCommonWhiteList(CmdletErrorContext errorContext)
		{
			ProvisioningMonitoringConfig.commonErrorWhiteList.Add(errorContext);
		}

		internal static void AddToCommonWhiteList(List<CmdletErrorContext> errorContextList)
		{
			ProvisioningMonitoringConfig.commonErrorWhiteList.AddRange(errorContextList);
		}

		internal static bool TryGetPidFromInstanceName(string instanceName, ref int pid)
		{
			string[] array = instanceName.Split(new char[]
			{
				'-'
			});
			return array.Length > 3 && array[0] == "v1" && int.TryParse(array[2], out pid);
		}

		internal static bool TryGetOrganizationNameFromInstanceName(string instanceName, ref string organizationName)
		{
			string pattern = "v\\d*-\\S*-\\d*-(\\S*)";
			Match match = Regex.Match(instanceName, pattern);
			if (match.Success)
			{
				organizationName = match.Groups[1].Value;
				return true;
			}
			return false;
		}

		private static bool MatchesErrorContext(List<CmdletErrorContext> errorContexts, ErrorRecord errorRecord)
		{
			foreach (CmdletErrorContext cmdletErrorContext in errorContexts)
			{
				if (cmdletErrorContext.MatchesErrorContext(errorRecord.Exception, string.Empty))
				{
					return true;
				}
			}
			return false;
		}

		internal const string InstanceNameFormat = "v1-{0}-{1}-{2}";

		internal static CmdletHealthCounters NullCmdletHealthCounters = new CmdletHealthCounters();

		private static Dictionary<string, List<CmdletErrorContext>> monitoringConfigList = new Dictionary<string, List<CmdletErrorContext>>(StringComparer.OrdinalIgnoreCase);

		private static List<CmdletErrorContext> commonErrorWhiteList = new List<CmdletErrorContext>();

		private static Dictionary<string, string> hostIds = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"ConsoleHost",
				"CH"
			},
			{
				"ServerRemoteHost",
				"RH"
			},
			{
				"Exchange Management Console",
				"EMC"
			},
			{
				"SimpleDataMigration",
				"ECPBulk"
			}
		};

		private static List<ExchangeRunspaceConfigurationSettings.ExchangeApplication> excludedClientApplications = new List<ExchangeRunspaceConfigurationSettings.ExchangeApplication>
		{
			ExchangeRunspaceConfigurationSettings.ExchangeApplication.ForwardSync
		};

		private static string processId = Process.GetCurrentProcess().Id.ToString();
	}
}
