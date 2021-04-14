using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class PublicFolderMailboxResponder : ResponderWorkItem
	{
		public PublicFolderMailboxResponder()
		{
			this.isDatacenter = LocalEndpointManager.IsDataCenter;
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent)
		{
			return new ResponderDefinition
			{
				AssemblyPath = PublicFolderMailboxResponder.AssemblyPath,
				TypeName = PublicFolderMailboxResponder.TypeName,
				Name = name,
				ServiceName = ExchangeComponent.PublicFolders.Name,
				TargetHealthState = targetHealthState,
				AlertTypeId = alertTypeId,
				AlertMask = alertMask,
				WaitIntervalSeconds = (int)TimeSpan.FromHours(23.0).TotalSeconds,
				TimeoutSeconds = (int)TimeSpan.FromHours(6.0).TotalSeconds,
				MaxRetryAttempts = 3,
				Enabled = true
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			try
			{
				this.DoInternalResponderWork(cancellationToken);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Exception in responder, StackTrace: {0}", ex.StackTrace.ToString(CultureInfo.InvariantCulture), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 148);
				throw;
			}
		}

		internal void DoInternalResponderWork(CancellationToken cancellationToken)
		{
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			string message = string.Empty;
			this.OrganizationName = string.Empty;
			if (lastFailedProbeResult == null)
			{
				base.Result.ResponseAction = "None";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Last Probe Failed result is null. Nothing to do.", null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 166);
				return;
			}
			string attributeValueFromProbeResult = PublicFolderMonitoringHelper.GetAttributeValueFromProbeResult(lastFailedProbeResult, "attribute1", base.TraceContext);
			if (string.IsNullOrEmpty(attributeValueFromProbeResult))
			{
				message = "PublicFolderMailboxResponder.DoInternalResponderWork: Could not get Valid mailbox name. State attribute1 in last probe result is null or empty.";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 175);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				throw new Exception(message);
			}
			this.psProvider = new LocalPowerShellProvider();
			if (this.isDatacenter)
			{
				string attributeValueFromProbeResult2 = PublicFolderMonitoringHelper.GetAttributeValueFromProbeResult(lastFailedProbeResult, "attribute2", base.TraceContext);
				if (ExEnvironment.IsTest)
				{
					this.OrganizationName = attributeValueFromProbeResult2;
				}
				else
				{
					this.OrganizationName = this.GetOrgIdFromTenantHint(attributeValueFromProbeResult2);
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>
			{
				{
					"PublicFolder",
					"SwitchValue"
				}
			};
			if (this.isDatacenter)
			{
				dictionary.Add("Organization", this.OrganizationName);
			}
			Collection<PSObject> collection = this.psProvider.RunExchangeCmdlet<string>("Get-Mailbox", dictionary, base.TraceContext, true);
			bool flag = false;
			if (collection.Count > 0)
			{
				foreach (PSObject psobject in collection)
				{
					string text = psobject.Properties["Name"].Value.ToString();
					ulong num;
					if ((bool)psobject.Properties["UseDatabaseQuotaDefaults"].Value)
					{
						string mailboxDatabaseName = psobject.Properties["Database"].Value.ToString();
						num = this.GetProhibitSendRecvQuotaFromMailboxDatabase(mailboxDatabaseName);
					}
					else
					{
						num = PublicFolderMonitoringHelper.GetSafeQuotaSizePropertyValue(psobject, "ProhibitSendReceiveQuota", base.TraceContext);
					}
					ulong mailboxSizeInUse;
					if (num < 1UL)
					{
						WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Could not get Quota value for the mailbox.", null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 237);
						PublicFolderMonitoringHelper.LogMessage("Could not get Quota value for the mailbox {0}. Ignore and move on to next.", new object[]
						{
							text
						});
					}
					else if (!this.GetCurrentPFMailboxSize(text, out mailboxSizeInUse))
					{
						WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Could not get current mailbox size.", null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 245);
						PublicFolderMonitoringHelper.LogMessage("Could not get Size of the mailbox {0}. Ignore and move on to next.", new object[]
						{
							text
						});
					}
					else
					{
						bool flag2 = PublicFolderMonitoringHelper.CheckIfOKToUseThisMailbox(num, mailboxSizeInUse, 10, base.TraceContext);
						PublicFolderMonitoringHelper.LogMessage("CheckIfOKToUseThisMailbox {0} as a target of split, returned {1}", new object[]
						{
							text,
							flag2.ToString()
						});
						if (flag2 && !(bool)psobject.Properties["IsRootPublicFolderMailbox"].Value)
						{
							flag = true;
							string mailboxToSync = this.isDatacenter ? PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, text) : text;
							DateTime t;
							if (!PublicFolderMonitoringHelper.GetLastSuccessFullSyncTime(this.psProvider, text, this.OrganizationName, out t, base.TraceContext))
							{
								base.Result.ResponseAction = "TriggerSync";
								PublicFolderMonitoringHelper.LogMessage("Selected target mailbox {0} was never synced. Invoking sync and Exiting", new object[]
								{
									text
								});
								PublicFolderMonitoringHelper.TriggerHierarchySyncOnMailbox(this.psProvider, mailboxToSync, false, base.TraceContext);
								break;
							}
							base.Result.ResponseAction = "TriggerSync";
							PublicFolderMonitoringHelper.LogMessage("Invoking incremental sync on target mailbox {0}", new object[]
							{
								text
							});
							PublicFolderMonitoringHelper.TriggerHierarchySyncOnMailbox(this.psProvider, mailboxToSync, true, base.TraceContext);
							DateTime t2;
							if (!PublicFolderMonitoringHelper.GetLastSuccessFullSyncTime(this.psProvider, text, this.OrganizationName, out t2, base.TraceContext))
							{
								message = string.Format("PublicFolderMailboxResponder.DoInternalResponderWork: GetLastSuccessFullSyncTime, the second time around, failed", new object[0]);
								PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
								throw new Exception(message);
							}
							if (DateTime.Compare(t2, t) <= 0)
							{
								PublicFolderMonitoringHelper.LogMessage("Incremental sync on target mailbox {0} did not complete successfully even after a min. Exiting this round", new object[]
								{
									text
								});
								break;
							}
							PublicFolderMonitoringHelper.LogMessage("Incremental sync completed successfully. Managing current move jobs, if any", new object[0]);
							bool flag3 = this.ManageCurrentMoveJobs();
							if (flag3)
							{
								base.Result.ResponseAction = "InvokeSplit";
								this.InvokeSplit(attributeValueFromProbeResult, text);
								break;
							}
							break;
						}
					}
				}
			}
			if (!flag)
			{
				base.Result.ResponseAction = "CreateMailbox";
				string mailboxNameWithCurrentDateTime = PublicFolderMonitoringHelper.GetMailboxNameWithCurrentDateTime();
				string text2 = this.isDatacenter ? PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, mailboxNameWithCurrentDateTime) : mailboxNameWithCurrentDateTime;
				PublicFolderMonitoringHelper.LogMessage("Did not find an appropriate mailbox to use as split target. Creating a new mailbox: {0}", new object[]
				{
					mailboxNameWithCurrentDateTime
				});
				PublicFolderMonitoringHelper.CreateNewPublicFolderMailbox(this.psProvider, mailboxNameWithCurrentDateTime, this.OrganizationName, true, base.TraceContext);
				try
				{
					PublicFolderMonitoringHelper.LogMessage("Triggering Hierarchy Sync on the new mailbox {0}", new object[]
					{
						text2
					});
					PublicFolderMonitoringHelper.TriggerHierarchySyncOnMailbox(this.psProvider, text2, false, base.TraceContext);
				}
				catch (Exception ex)
				{
					PublicFolderMonitoringHelper.LogMessage("TriggerHierarchySyncOnMailbox soon after Create failed with exception: {0}. Ignoring as this could be due to replication.", new object[]
					{
						ex.ToString()
					});
				}
			}
		}

		internal bool ManageCurrentMoveJobs()
		{
			bool result = true;
			Collection<PSObject> allPFMoveJobs = PublicFolderMonitoringHelper.GetAllPFMoveJobs(this.psProvider, this.OrganizationName, base.TraceContext);
			if (allPFMoveJobs == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "No existing PF Move job to process.", null, "ManageCurrentMoveJobs", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 371);
				return result;
			}
			foreach (PSObject psobject in allPFMoveJobs)
			{
				string text = psobject.Properties["Status"].Value.ToString();
				string text2 = psobject.Properties["Name"].Value.ToString();
				string a;
				if ((a = text.ToLowerInvariant()) != null)
				{
					if (a == "completed")
					{
						this.RemovePublicFolderMoveJob(text2);
						result = true;
						continue;
					}
					if (a == "inprogress" || a == "cleanup")
					{
						this.ManageInProgressMoveJobRequest(text2);
						result = false;
						continue;
					}
					if (a == "failed")
					{
						this.RaiseAlertForAPFMoveJob(text2);
						result = false;
						continue;
					}
				}
				string message = string.Format("Found a move job with unexpected status: {0}", text);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "ManageCurrentMoveJobs", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 399);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				result = false;
			}
			return result;
		}

		internal void ManageInProgressMoveJobRequest(string pfMoveJobName)
		{
			string message = string.Empty;
			if (string.IsNullOrEmpty(pfMoveJobName))
			{
				message = "ERROR !, Move job name is null/empty. Ignore and continue.";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "ManageInProgressMoveJobRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 419);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				return;
			}
			if (this.IsThereAnUpdateInLast24Hours(pfMoveJobName))
			{
				message = string.Format("PF Move job {0} is already in progress, we will not take any action at this time.", pfMoveJobName);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "ManageInProgressMoveJobRequest", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 428);
				return;
			}
			this.RaiseAlertForAPFMoveJob(pfMoveJobName);
		}

		internal void RaiseAlertForAPFMoveJob(string moveJobName)
		{
			PublicFolderMonitoringHelper.LogMessage("Raising alert for failed/stuck move job {0}", new object[]
			{
				moveJobName
			});
			this.PublishMessageInCrimsonToTriggerEscalation("PFMoveJobStuck", moveJobName);
		}

		internal bool IsThereAnUpdateInLast24Hours(string moveJobName)
		{
			string message = string.Empty;
			if (string.IsNullOrEmpty(moveJobName))
			{
				throw new ApplicationException("IsThereAnUpdateInLast24Hours: ERROR !, Move job name is null/empty. Cannot continue.");
			}
			moveJobName = PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, moveJobName);
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					"Identity",
					moveJobName
				}
			};
			Collection<PSObject> collection = this.psProvider.RunExchangeCmdlet<string>("Get-PublicFolderMoveRequestStatistics", parameters, base.TraceContext, true);
			if (collection == null || collection.Count < 1)
			{
				message = string.Format("IsThereAnUpdateInLast24Hours: Get-PublicFolderMoveRequestStatistics {0} returned null or zero results", moveJobName);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "IsThereAnUpdateInLast24Hours", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 471);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				throw new ApplicationException(message);
			}
			if (collection[0] != null)
			{
				DateTime d = (DateTime)collection[0].Properties["LastUpdateTimestamp"].Value;
				TimeSpan t = (DateTime)ExDateTime.Now - d;
				if (t < TimeSpan.FromHours(24.0))
				{
					message = string.Format("There is an update in the last 24 hours for move job {0}. Let it proceed.", moveJobName);
					WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "IsThereAnUpdateInLast24Hours", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 485);
					return true;
				}
			}
			message = string.Format("No update for move job {0} in the last 24 hours. Need to Alert !", moveJobName);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "IsThereAnUpdateInLast24Hours", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 491);
			PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
			return false;
		}

		public void PublishMessageInCrimsonToTriggerEscalation(string eventToNotify, string moveJobName)
		{
			EventNotificationItem eventNotificationItem = new EventNotificationItem(ExchangeComponent.PublicFolders.Name, eventToNotify, string.Empty, ResultSeverityLevel.Error);
			eventNotificationItem.CustomProperties["TenantHint"] = this.OrganizationName;
			eventNotificationItem.StateAttribute1 = moveJobName;
			eventNotificationItem.StateAttribute2 = this.OrganizationName;
			eventNotificationItem.Publish(false);
		}

		internal void RemovePublicFolderMoveJob(string jobIdentity)
		{
			jobIdentity = PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, jobIdentity);
			string message = string.Format("Attempting to remove job {0}.", jobIdentity);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "RemovePublicFolderMoveJob", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 527);
			PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("Identity", jobIdentity);
			dictionary.Add("Confirm", false);
			this.psProvider.RunExchangeCmdlet<object>("Remove-PublicFolderMoveRequest", dictionary, base.TraceContext, true);
		}

		internal string GetOrgIdFromTenantHint(string tenantHintAsHexString)
		{
			string message = string.Empty;
			if (string.IsNullOrEmpty(tenantHintAsHexString))
			{
				message = "PublicFolderMailboxResponder.ParseTenantHint: Invalid TenantHint - The value is null or emtpy.";
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "GetOrgIdFromTenantHint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 546);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				throw new Exception(message);
			}
			string text = null;
			string arg = null;
			try
			{
				arg = "HexConverter.HexStringToByteArray";
				PublicFolderMonitoringHelper.LogMessage("Convert tenantHint hex string [{0}] to byte array.", new object[]
				{
					tenantHintAsHexString
				});
				byte[] buffer = HexConverter.HexStringToByteArray(tenantHintAsHexString);
				arg = "TenantPartitionHint.FromPersistablePartitionHint";
				PublicFolderMonitoringHelper.LogMessage("Get the tenantPartitionHint from the byte array.", new object[0]);
				TenantPartitionHint tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(buffer);
				arg = "GetExternalDirectoryOrganizationId";
				PublicFolderMonitoringHelper.LogMessage("Get Organization Id from the tenant partition hint", new object[0]);
				text = tenantPartitionHint.GetExternalDirectoryOrganizationId().ToString();
				PublicFolderMonitoringHelper.LogMessage("OrgId = {0}", new object[]
				{
					text
				});
			}
			catch (Exception ex)
			{
				message = string.Format("PublicFolderMailboxResponder.ParseTenantHint: Exception @ [{0}] when trying to resolve tenantHint [{1}]: {2}", arg, tenantHintAsHexString, ex.Message);
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "GetOrgIdFromTenantHint", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 574);
				PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
				throw new Exception(message, ex);
			}
			return text;
		}

		internal void InvokeSplit(string sourceMailboxName, string targetMailboxName)
		{
			string text = Path.Combine(Utils.GetExchangeScriptsPath(), "Split-PublicFolderMailbox.ps1");
			string path = Path.Combine(Utils.GetExchangeLogPath(), "PublicFolder");
			string path2 = "Microsoft.Exchange.SplitPublicFolderMailboxLog_" + DateTime.UtcNow.ToString("yyyy_MM_dd") + ".txt";
			string logFile = Path.Combine(path, path2);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("SourcePublicFolderMailbox", sourceMailboxName);
			dictionary.Add("TargetPublicFolderMailbox", targetMailboxName);
			dictionary.Add("AllowLargeItems", "SwitchValue");
			if (this.isDatacenter)
			{
				dictionary.Add("Organization", this.OrganizationName);
			}
			string message = string.Format("Invoking split script {0} ", text);
			WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "InvokeSplit", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 600);
			PublicFolderMonitoringHelper.LogMessage(message, new object[0]);
			this.psProvider.RunPowershellScript(text, dictionary, base.TraceContext, true, logFile);
			PublicFolderMonitoringHelper.LogMessage("Split script trigerred successfully on source mailbox {0} with target as {1}", new object[]
			{
				sourceMailboxName,
				targetMailboxName
			});
		}

		internal bool GetCurrentPFMailboxSize(string mailboxName, out ulong sizeValue)
		{
			sizeValue = 0UL;
			if (string.IsNullOrEmpty(mailboxName))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "GetCurrentPFMailboxSize:Mailbox name should not be null here.", null, "GetCurrentPFMailboxSize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 618);
				return false;
			}
			if (this.isDatacenter)
			{
				mailboxName = PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, mailboxName);
			}
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					"Identity",
					mailboxName
				}
			};
			Collection<PSObject> collection = this.psProvider.RunExchangeCmdlet<string>("Get-MailboxStatistics", parameters, base.TraceContext, false);
			if (collection.Count < 1)
			{
				sizeValue = 1UL;
				return true;
			}
			sizeValue = PublicFolderMonitoringHelper.GetSafeQuotaSizePropertyValue(collection[0], "TotalItemSize", base.TraceContext);
			if (sizeValue < 0UL)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "GetCurrentPFMailboxSize:Error reading size from Mailbox name {0}.", mailboxName, null, "GetCurrentPFMailboxSize", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 648);
				return false;
			}
			return true;
		}

		internal ulong GetProhibitSendRecvQuotaFromMailboxDatabase(string mailboxDatabaseName)
		{
			ulong result = 0UL;
			if (string.IsNullOrEmpty(mailboxDatabaseName))
			{
				return result;
			}
			Dictionary<string, string> parameters = new Dictionary<string, string>
			{
				{
					"Identity",
					mailboxDatabaseName
				}
			};
			Collection<PSObject> collection = null;
			try
			{
				collection = this.psProvider.RunExchangeCmdlet<string>("Get-MailboxDatabase", parameters, base.TraceContext, false);
			}
			catch (Exception)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Error running Get-MailboxDatabase, rbac issue?", null, "GetProhibitSendRecvQuotaFromMailboxDatabase", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 684);
				return result;
			}
			if (collection.Count != 1)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Did not get back anything from Get-MailboxDatabase!!", null, "GetProhibitSendRecvQuotaFromMailboxDatabase", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderMailboxResponder.cs", 690);
				return result;
			}
			result = PublicFolderMonitoringHelper.GetSafeQuotaSizePropertyValue(collection[0], "ProhibitSendReceiveQuota", base.TraceContext);
			return result;
		}

		private const string SPLIT_SCRIPT_NAME = "Split-PublicFolderMailbox.ps1";

		private const string SPLIT_LOG_PREFIX = "Microsoft.Exchange.SplitPublicFolderMailboxLog_";

		private const int MimimumMailboxQuotaPercentage = 10;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(PublicFolderMailboxResponder).FullName;

		private readonly bool isDatacenter;

		private string OrganizationName;

		private LocalPowerShellProvider psProvider;
	}
}
