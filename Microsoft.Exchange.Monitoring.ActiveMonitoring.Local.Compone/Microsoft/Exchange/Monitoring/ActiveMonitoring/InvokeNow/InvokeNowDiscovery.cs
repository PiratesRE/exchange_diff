using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.InvokeNow
{
	public sealed class InvokeNowDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			DateTime starttime = ExDateTime.Now.LocalTime.AddSeconds((double)(-(double)base.Definition.RecurrenceIntervalSeconds * 2));
			List<InvokeNowEntry> invokeNowRequests = InvokeNowDiscovery.GetInvokeNowRequests(starttime);
			foreach (InvokeNowEntry invokeNowEntry in invokeNowRequests)
			{
				CrimsonHelper.WriteBookmark(invokeNowEntry.GetType().Name, "InvokeNow", "InvokeNowBookmark", invokeNowEntry.LocalDataAccessMetaData.Bookmark);
				ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadStarted.Log<string, string, string, string, string, string, string, string, InvokeNowResult, string, string>(invokeNowEntry.Id.ToString("N"), invokeNowEntry.TypeName, invokeNowEntry.AssemblyPath, invokeNowEntry.MonitorIdentity, invokeNowEntry.PropertyBag, invokeNowEntry.ExtensionAttributes, invokeNowEntry.RequestTime.ToString(), InvokeNowState.DefinitionUploadStarted.ToString(), InvokeNowResult.None, invokeNowEntry.RequestTime.ToString(), string.Empty);
				base.Result.StateAttribute1 = invokeNowEntry.LocalDataAccessMetaData.TimeStamp.ToString();
				if (string.IsNullOrEmpty(invokeNowEntry.AssemblyPath) || string.IsNullOrEmpty(invokeNowEntry.TypeName))
				{
					WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.GenericRpcTracer, base.TraceContext, "RpcGetMonitoringItemHelpImpl.GetPropertyInformation() returning null. (monitorIdentity: {0}, assemblyPath:{1}, typeName:{2})", invokeNowEntry.MonitorIdentity, invokeNowEntry.AssemblyPath, invokeNowEntry.TypeName, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\InvokeNow\\InvokeNowDiscovery.cs", 86);
				}
				else
				{
					try
					{
						Dictionary<string, string> dictionary = CrimsonHelper.ConvertXmlToDictionary(invokeNowEntry.PropertyBag);
						if (!string.IsNullOrWhiteSpace(invokeNowEntry.ExtensionAttributes))
						{
							dictionary.Add("ExtensionAttributes", invokeNowEntry.ExtensionAttributes);
						}
						string text = string.Empty;
						WorkDefinition workDefinition = null;
						try
						{
							workDefinition = InvokeNowDiscovery.CreateInvokeNowDefinition(invokeNowEntry.AssemblyPath, invokeNowEntry.TypeName, dictionary, invokeNowEntry.MonitorIdentity);
						}
						catch (LocalizedException ex)
						{
							text = ex.Message;
						}
						if (workDefinition != null)
						{
							ProbeDefinition probeDefinition = (ProbeDefinition)workDefinition;
							InvokeNowDiscovery.UpdateDefinitionForInvokeNow(probeDefinition, invokeNowEntry.Id, invokeNowEntry.MonitorIdentity, invokeNowEntry.AssemblyPath, invokeNowEntry.TypeName);
							base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
							base.Result.StateAttribute2 = probeDefinition.Name;
							ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadSucceeded.Log<string, string, string, string, string, string, string, string, InvokeNowResult, string, string>(invokeNowEntry.Id.ToString("N"), invokeNowEntry.TypeName, invokeNowEntry.AssemblyPath, invokeNowEntry.MonitorIdentity, invokeNowEntry.PropertyBag, invokeNowEntry.ExtensionAttributes, invokeNowEntry.RequestTime.ToString(), InvokeNowState.DefinitionUploadFinished.ToString(), InvokeNowResult.Succeeded, string.Empty, probeDefinition.Id.ToString());
						}
						else
						{
							ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(invokeNowEntry.Id.ToString("N"), invokeNowEntry.TypeName, invokeNowEntry.AssemblyPath, invokeNowEntry.MonitorIdentity, invokeNowEntry.PropertyBag, invokeNowEntry.ExtensionAttributes, invokeNowEntry.RequestTime.ToString(), InvokeNowState.DefinitionUploadFinished, InvokeNowResult.Failed, string.IsNullOrWhiteSpace(text) ? Strings.InvokeNowInvalidWorkDefinition(invokeNowEntry.Id.ToString("N")) : text, string.Empty);
						}
					}
					catch (Exception ex2)
					{
						ManagedAvailabilityCrimsonEvents.InvokeNowDefinitionUploadFailed.Log<string, string, string, string, string, string, string, InvokeNowState, InvokeNowResult, string, string>(invokeNowEntry.Id.ToString("N"), invokeNowEntry.TypeName, invokeNowEntry.AssemblyPath, invokeNowEntry.MonitorIdentity, invokeNowEntry.PropertyBag, invokeNowEntry.ExtensionAttributes, invokeNowEntry.RequestTime.ToString(), InvokeNowState.DefinitionUploadFinished, InvokeNowResult.Failed, ex2.Message, string.Empty);
					}
				}
			}
		}

		private static void UpdateDefinitionForInvokeNow(WorkDefinition wd, Guid requestId, string monitorIdentity, string assemblyPath, string typeName)
		{
			wd.StartTime = DateTime.MinValue;
			wd.RecurrenceIntervalSeconds = 0;
			wd.Name = InvokeNowDiscovery.GetInvokeNowDefinitionName(requestId, wd.Name);
			string text = string.IsNullOrWhiteSpace(monitorIdentity) ? string.Empty : MonitoringItemIdentity.MonitorIdentityId.GetTargetResource(monitorIdentity);
			if (!string.IsNullOrWhiteSpace(text))
			{
				wd.TargetResource = text;
			}
			wd.AssemblyPath = assemblyPath;
			wd.TypeName = typeName;
			wd.ServiceName = "InvokeNow";
			if (wd.TimeoutSeconds <= 0)
			{
				wd.TimeoutSeconds = 30;
			}
		}

		private static string GetInvokeNowDefinitionName(Guid requestId, string defName)
		{
			return requestId.ToString("N") + "-" + defName;
		}

		private static List<InvokeNowEntry> GetInvokeNowRequests(DateTime starttime)
		{
			List<InvokeNowEntry> list = new List<InvokeNowEntry>();
			EventBookmark bookmark = CrimsonHelper.ReadBookmark(typeof(InvokeNowEntry).Name, "InvokeNow", "InvokeNowBookmark");
			using (CrimsonReader<InvokeNowEntry> crimsonReader = new CrimsonReader<InvokeNowEntry>(null, bookmark, "Microsoft-Exchange-ManagedAvailability/InvokeNowRequest"))
			{
				crimsonReader.QueryEndTime = new DateTime?(ExDateTime.Now.LocalTime.AddDays(1.0));
				crimsonReader.QueryStartTime = new DateTime?(starttime);
				while (!crimsonReader.EndOfEventsReached)
				{
					InvokeNowEntry invokeNowEntry = crimsonReader.ReadNext();
					if (invokeNowEntry != null)
					{
						list.Add(invokeNowEntry);
					}
				}
			}
			return list;
		}

		private static WorkDefinition CreateInvokeNowDefinition(string assemblyPath, string monitorType, Dictionary<string, string> propertyBag, string monitorIdentity)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyPath);
			Type type = assembly.GetType(monitorType, false, true);
			WorkItem workItem = (WorkItem)Activator.CreateInstance(type, new object[0]);
			WorkDefinition workDefinition = InvokeNowDiscovery.CreateDefinition(workItem);
			workDefinition.Name = MonitoringItemIdentity.MonitorIdentityId.GetMonitor(monitorIdentity);
			workDefinition.TargetResource = MonitoringItemIdentity.MonitorIdentityId.GetTargetResource(monitorIdentity);
			workDefinition.ExtensionAttributes = InvokeNowCommon.GetWorkDefinition(monitorIdentity).ExtensionAttributes;
			workItem.PopulateDefinition<WorkDefinition>(workDefinition, propertyBag);
			if (!workDefinition.Enabled)
			{
				throw new ArgumentException("InvokeNow work items must be Enabled.");
			}
			return workDefinition;
		}

		private static WorkDefinition CreateDefinition(WorkItem workitem)
		{
			if (workitem is ProbeWorkItem)
			{
				return new ProbeDefinition();
			}
			if (workitem is MonitorWorkItem)
			{
				return new MonitorDefinition();
			}
			if (workitem is ResponderWorkItem)
			{
				return new ResponderDefinition();
			}
			throw new ApplicationException("Unknown workitem type");
		}

		private const string InvokeNowHealthSetName = "InvokeNow";

		private const string InvokeNowBookmarkName = "InvokeNowBookmark";

		private const int DefaultTimeOutInSeconds = 30;
	}
}
