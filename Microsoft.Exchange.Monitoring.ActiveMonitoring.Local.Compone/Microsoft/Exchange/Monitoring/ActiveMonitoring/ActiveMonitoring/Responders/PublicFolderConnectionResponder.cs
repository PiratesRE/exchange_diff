using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders
{
	public class PublicFolderConnectionResponder : ResponderWorkItem
	{
		public PublicFolderConnectionResponder()
		{
			this.isDatacenter = LocalEndpointManager.IsDataCenter;
		}

		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, bool enabled = true)
		{
			return new ResponderDefinition
			{
				AssemblyPath = PublicFolderConnectionResponder.AssemblyPath,
				TypeName = PublicFolderConnectionResponder.TypeName,
				Name = name,
				ServiceName = ExchangeComponent.PublicFolders.Name,
				TargetHealthState = targetHealthState,
				AlertTypeId = alertTypeId,
				AlertMask = alertMask,
				WaitIntervalSeconds = (int)TimeSpan.FromHours(23.0).TotalSeconds,
				TimeoutSeconds = (int)TimeSpan.FromHours(6.0).TotalSeconds,
				MaxRetryAttempts = 3,
				Enabled = enabled
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
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Exception in responder, StackTrace: {0}", ex.StackTrace.ToString(CultureInfo.InvariantCulture), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderConnectionResponder.cs", 131);
				throw;
			}
		}

		public void DoInternalResponderWork(CancellationToken cancellationToken)
		{
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			if (lastFailedProbeResult == null)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Last Probe Failed result is null. Nothing to do.", null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderConnectionResponder.cs", 148);
				return;
			}
			string attributeValueFromProbeResult = PublicFolderMonitoringHelper.GetAttributeValueFromProbeResult(lastFailedProbeResult, "attribute1", base.TraceContext);
			if (string.IsNullOrEmpty(attributeValueFromProbeResult))
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Error: Could not get the name of the PF mailbox.", null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderConnectionResponder.cs", 157);
				throw new InvalidOperationException("PublicFolderConnectionResponder.DoInternalResponderWork.Could not get Valid mailbox name from state attribute1.");
			}
			this.OrganizationName = (this.isDatacenter ? lastFailedProbeResult.StateAttribute3 : string.Empty);
			this.psProvider = new LocalPowerShellProvider();
			if (this.IsThereAMailboxWithActiveSplitJobInProgress())
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "We already have a Split job in progress, Exiting.", null, "DoInternalResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderConnectionResponder.cs", 166);
				return;
			}
			PSObject lastCreatedPFMailbox = this.GetLastCreatedPFMailbox();
			string mailboxNameWithCurrentDateTime = PublicFolderMonitoringHelper.GetMailboxNameWithCurrentDateTime();
			string mailboxToSync = string.Empty;
			if (this.IsThisMailboxServingClients(lastCreatedPFMailbox))
			{
				PublicFolderMonitoringHelper.CreateNewPublicFolderMailbox(this.psProvider, mailboxNameWithCurrentDateTime, this.OrganizationName, true, base.TraceContext);
				mailboxToSync = (this.isDatacenter ? PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, mailboxNameWithCurrentDateTime) : mailboxNameWithCurrentDateTime);
				PublicFolderMonitoringHelper.TriggerHierarchySyncOnMailbox(this.psProvider, mailboxToSync, false, base.TraceContext);
				return;
			}
			string text = lastCreatedPFMailbox.Properties["Name"].Value.ToString();
			mailboxToSync = (this.isDatacenter ? PublicFolderMonitoringHelper.GetFormattedMailboxNameForOrganization(this.OrganizationName, text) : text);
			DateTime dateTime;
			if (PublicFolderMonitoringHelper.GetLastSuccessFullSyncTime(this.psProvider, text, this.OrganizationName, out dateTime, base.TraceContext))
			{
				PublicFolderMonitoringHelper.SetExcludedFromServingHierarchyOnPFMailbox(this.psProvider, text, this.OrganizationName, false, base.TraceContext);
				return;
			}
			PublicFolderMonitoringHelper.TriggerHierarchySyncOnMailbox(this.psProvider, mailboxToSync, false, base.TraceContext);
		}

		private bool IsThereAMailboxWithActiveSplitJobInProgress()
		{
			Collection<PSObject> allPFMoveJobs = PublicFolderMonitoringHelper.GetAllPFMoveJobs(this.psProvider, this.OrganizationName, base.TraceContext);
			return allPFMoveJobs.Any((PSObject psObject) => psObject.Properties["Status"].Value.ToString().ToLowerInvariant().Equals("inprogress"));
		}

		internal PSObject GetLastCreatedPFMailbox()
		{
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
			PSObject psobject = null;
			Collection<PSObject> collection = this.psProvider.RunExchangeCmdlet<string>("Get-Mailbox", dictionary, base.TraceContext, false);
			if (collection.Count > 0)
			{
				psobject = collection[0];
				foreach (PSObject psobject2 in collection)
				{
					if (DateTime.Compare(this.WhenCreatedDateTimeFromPSObject(psobject2), this.WhenCreatedDateTimeFromPSObject(psobject)) > 0)
					{
						psobject = psobject2;
					}
				}
			}
			return psobject;
		}

		internal DateTime WhenCreatedDateTimeFromPSObject(PSObject psObject)
		{
			string s = psObject.Properties["WhenCreated"].Value.ToString();
			DateTime result;
			if (!DateTime.TryParse(s, out result))
			{
				throw new ApplicationException("Could not parse datetime from mailbox Object");
			}
			return result;
		}

		internal bool IsThisMailboxServingClients(PSObject psObject)
		{
			if (psObject == null)
			{
				throw new ArgumentNullException("psObject", "PublicFolderConnectionResponder:IsThisMailboxServingClients: Null PS Object.");
			}
			return !(bool)psObject.Properties["IsExcludedFromServingHierarchy"].Value;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(PublicFolderConnectionResponder).FullName;

		private readonly bool isDatacenter;

		private string OrganizationName;

		private LocalPowerShellProvider psProvider;
	}
}
