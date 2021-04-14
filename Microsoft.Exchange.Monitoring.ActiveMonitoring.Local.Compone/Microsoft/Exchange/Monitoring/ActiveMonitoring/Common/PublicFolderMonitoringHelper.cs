using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class PublicFolderMonitoringHelper
	{
		static PublicFolderMonitoringHelper()
		{
			MonitoringLogConfiguration configuration = new MonitoringLogConfiguration("PublicFolders");
			PublicFolderMonitoringHelper.monitoringLogger = new MonitoringLogger(configuration);
		}

		public static bool CheckIfOKToUseThisMailbox(ulong totalMailboxSize, ulong mailboxSizeInUse, int desiredQuota, TracingContext traceContext)
		{
			bool result = false;
			if (totalMailboxSize < 1UL || mailboxSizeInUse >= totalMailboxSize || desiredQuota > 99)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, "This mailbox is already full/Invalid quota check requested", null, "CheckIfOKToUseThisMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 84);
				PublicFolderMonitoringHelper.LogMessage("This mailbox is already full/Invalid quota check requested", new object[0]);
				return result;
			}
			if (mailboxSizeInUse / totalMailboxSize * 100.0 < (double)desiredQuota)
			{
				result = true;
			}
			return result;
		}

		public static ulong GetSafeQuotaSizePropertyValue(PSObject psObject, string propName, TracingContext traceContext)
		{
			ulong result = 0UL;
			try
			{
				result = ((Unlimited<ByteQuantifiedSize>)psObject.Properties[propName].Value).Value.ToBytes();
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, "Error occurred while getting the value for property using GetSafeQuotaSizePropertyValue.", null, "GetSafeQuotaSizePropertyValue", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 113);
				PublicFolderMonitoringHelper.LogMessage("Error occurred while getting the value for property using GetSafeQuotaSizePropertyValue: {0}", new object[]
				{
					ex.ToString()
				});
			}
			return result;
		}

		public static string GetFormattedMailboxNameForOrganization(string orgName, string entityName)
		{
			if (!string.IsNullOrEmpty(orgName))
			{
				return string.Format("{0}\\{1}", orgName, entityName);
			}
			return entityName;
		}

		public static string GetAttributeValueFromProbeResult(ProbeResult probeResult, string attributeName, TracingContext traceContext)
		{
			string empty = string.Empty;
			if (probeResult == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, "GetAttributeValueFromProbeResult: NULL Probe result while trying to get attribute value.", null, "GetAttributeValueFromProbeResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 147);
				PublicFolderMonitoringHelper.LogMessage("GetAttributeValueFromProbeResult: NULL Probe result while trying to get attribute value.", new object[0]);
				return empty;
			}
			string a;
			if ((a = attributeName.ToLower()) != null)
			{
				if (!(a == "attribute1"))
				{
					if (!(a == "attribute2"))
					{
						goto IL_F7;
					}
					if (!string.IsNullOrWhiteSpace(probeResult.StateAttribute2))
					{
						return probeResult.StateAttribute2;
					}
					WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, "NULL/Empty StateAttribute2 while trying to get Org name.", null, "GetAttributeValueFromProbeResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 176);
					PublicFolderMonitoringHelper.LogMessage("GetAttributeValueFromProbeResult: NULL/Empty StateAttribute2 while trying to get tenant hint.", new object[0]);
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(probeResult.StateAttribute1))
					{
						return probeResult.StateAttribute1;
					}
					WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, "NULL/Empty StateAttribute1 while trying to get mailbox name.", null, "GetAttributeValueFromProbeResult", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 160);
					PublicFolderMonitoringHelper.LogMessage("GetAttributeValueFromProbeResult: NULL/Empty StateAttribute1 while trying to get mailbox name.", new object[0]);
				}
				return empty;
			}
			IL_F7:
			throw new ArgumentException("Unknown state attribute name: " + attributeName);
		}

		public static bool GetLastSuccessFullSyncTime(LocalPowerShellProvider psProvider, string pfMailboxName, string orgName, out DateTime lastSuccessFullSyncTime, TracingContext traceContext)
		{
			lastSuccessFullSyncTime = DateTime.MinValue;
			if (string.IsNullOrEmpty(pfMailboxName))
			{
				throw new ArgumentException("PublicFolderMonitoringHelper.GetLastSuccessFullSyncTime: Null or Empty PF mailbox Name");
			}
			string formattedMailboxNameForOrganization = PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(orgName, pfMailboxName);
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					"Identity",
					formattedMailboxNameForOrganization
				}
			};
			Collection<PSObject> collection = psProvider.RunExchangeCmdlet<string>("Get-PublicFolderMailboxDiagnostics", parameters, traceContext, true);
			if (collection[0] != null)
			{
				PublicFolderMailboxSynchronizerInfo publicFolderMailboxSynchronizerInfo = (PublicFolderMailboxSynchronizerInfo)collection[0].Properties["SyncInfo"].Value;
				ExDateTime? lastSuccessfulSyncTime = publicFolderMailboxSynchronizerInfo.LastSuccessfulSyncTime;
				if (lastSuccessfulSyncTime != null)
				{
					lastSuccessFullSyncTime = PublicFolderMonitoringHelper.ExDateTimeToDateTime(lastSuccessfulSyncTime.Value);
					PublicFolderMonitoringHelper.LogMessage("PF mailbox {0} was last synced on {1}.", new object[]
					{
						formattedMailboxNameForOrganization,
						lastSuccessFullSyncTime
					});
					return true;
				}
			}
			string message = string.Format("Could not get LastSuccessFullSyncTime for mailbox {0}. Never synced before ?", formattedMailboxNameForOrganization);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, message, null, "GetLastSuccessFullSyncTime", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 232);
			PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
			return false;
		}

		public static void TriggerHierarchySyncOnMailbox(LocalPowerShellProvider psProvider, string mailboxToSync, bool attemptWaitForSyncComplete, TracingContext traceContext)
		{
			if (string.IsNullOrEmpty(mailboxToSync))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, "TriggerHierarchySyncOnMailbox: Target Mailbox Identity is Empty.", null, "TriggerHierarchySyncOnMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 253);
				PublicFolderMonitoringHelper.LogMessage("TriggerHierarchySyncOnMailbox: Target Mailbox Identity is Empty.", new object[0]);
				throw new ArgumentNullException(mailboxToSync, "TriggerHierarchySyncOnMailbox: Target Mailbox Identity is Empty.");
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"Identity",
					mailboxToSync
				},
				{
					"InvokeSynchronizer",
					"SwitchValue"
				}
			};
			if (!attemptWaitForSyncComplete)
			{
				dictionary.Add("SuppressStatus", "SwitchValue");
			}
			psProvider.RunExchangeCmdlet<string>("Update-PublicFolderMailbox", dictionary, traceContext, true);
		}

		public static Collection<PSObject> GetAllPFMoveJobs(LocalPowerShellProvider psProvider, string orgName, TracingContext traceContext)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(orgName))
			{
				dictionary.Add("Organization", orgName);
			}
			return psProvider.RunExchangeCmdlet<string>("Get-PublicFolderMoveRequest", dictionary, traceContext, true);
		}

		public static void LogMessage(string message, params object[] messageArgs)
		{
			PublicFolderMonitoringHelper.monitoringLogger.LogEvent(PublicFolderMonitoringHelper.ExDateTimeToDateTime(ExDateTime.Now), message, messageArgs);
		}

		public static DateTime ExDateTimeToDateTime(ExDateTime time)
		{
			return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond);
		}

		public static string GetMailboxNameWithCurrentDateTime()
		{
			return "PublicFolderMailbox_" + DateTime.UtcNow.ToString("yyyy_MM_dd");
		}

		internal static Collection<PSObject> CreateNewPublicFolderMailbox(LocalPowerShellProvider psProvider, string mailboxName, string organizationName, bool excludeFromServingHierarchy, TracingContext traceContext)
		{
			if (string.IsNullOrEmpty(mailboxName))
			{
				mailboxName = "PublicFolderMailbox_" + DateTime.UtcNow.ToString("yyyy_MM_dd");
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					"Name",
					mailboxName
				},
				{
					"PublicFolder",
					"SwitchValue"
				}
			};
			if (!string.IsNullOrEmpty(organizationName))
			{
				dictionary.Add("Organization", organizationName);
			}
			if (excludeFromServingHierarchy)
			{
				dictionary.Add("IsExcludedFromServingHierarchy", true);
			}
			return psProvider.RunExchangeCmdlet<object>("New-Mailbox", dictionary, traceContext, true);
		}

		public static bool SetExcludedFromServingHierarchyOnPFMailbox(LocalPowerShellProvider psProvider, string pfMailboxName, string orgName, bool isExCludedFromServingHierarchy, TracingContext traceContext)
		{
			if (string.IsNullOrEmpty(pfMailboxName))
			{
				return false;
			}
			pfMailboxName = PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(orgName, pfMailboxName);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, string.Format("Updating mailbox {0}.", pfMailboxName), null, "SetExcludedFromServingHierarchyOnPFMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 388);
			Dictionary<string, object> parameters = new Dictionary<string, object>
			{
				{
					"Identity",
					pfMailboxName
				},
				{
					"PublicFolder",
					"SwitchValue"
				},
				{
					"IsExcludedFromServingHierarchy",
					isExCludedFromServingHierarchy
				}
			};
			if (psProvider.RunExchangeCmdlet<object>("Set-Mailbox", parameters, traceContext, false) == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, traceContext, string.Format("Could not update mailbox {0} dot IsExcludedFromServingHierarchy to {1}.", pfMailboxName, isExCludedFromServingHierarchy.ToString(CultureInfo.InvariantCulture)), null, "SetExcludedFromServingHierarchyOnPFMailbox", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMonitoringHelper.cs", 400);
				return false;
			}
			return true;
		}

		public const string SWITCH_VALUE = "SwitchValue";

		public const string AutoPFMailboxPrefix = "PublicFolderMailbox_";

		public const string PFMoveJobStuckProbeResultCompName = "PFMoveJobStuck";

		private static MonitoringLogger monitoringLogger;
	}
}
