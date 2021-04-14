using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public sealed class MailboxDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "MailboxDiscovery.DoWork", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 80);
			if (!DiscoveryUtils.IsMailboxRoleInstalled())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "MailboxDiscovery.DoWork(): Mailbox role not installed. Skip.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 84);
				base.Result.StateAttribute1 = "MailboxDiscovery: Mailbox role not installed. Skip.";
				return;
			}
			GenericWorkItemHelper.CreateAllDefinitions(new List<string>
			{
				"Delivery_Mailbox.xml",
				"Submission_Mailbox.xml",
				"SmtpProbesDelivery_Mailbox.xml",
				"Core_Mailbox.xml"
			}, base.Broker, base.TraceContext, base.Result);
			this.BuildOverrideList();
			this.InstallMailboxDeliveryAvailabilityMonitor();
		}

		private void InstallMailboxDeliveryAvailabilityMonitor()
		{
			ICollection<MailboxDatabaseInfo> monitoringMailboxes = this.GetMonitoringMailboxes();
			if (monitoringMailboxes == null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "MailboxDeliveryAvailability Discovery: No mailboxes found, proceeding as success", null, "InstallMailboxDeliveryAvailabilityMonitor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 115);
				return;
			}
			this.InstallMailboxInstanceMonitors(monitoringMailboxes, new MailboxDiscovery.InstanceProbeDefintionDelegate(MailboxDeliveryInstanceAvailabilityProbe.CreateMailboxDeliveryInstanceAvailabilityProbe), new MailboxDiscovery.InstanceMonitorDefinitionDelegate(MailboxDeliveryInstanceAvailabilityProbe.CreateMailboxDeliveryInstanceAvailabilityMonitor), new MailboxDiscovery.InstanceResponderDefinitionDelegate[]
			{
				new MailboxDiscovery.InstanceResponderDefinitionDelegate(MailboxDeliveryInstanceAvailabilityProbe.CreateMailboxDeliveryInstanceAvailabilityEscalateResponder)
			});
			ProbeDefinition probeDefinition = MailboxDeliveryAvailabilityProbe.CreateMailboxDeliveryAvailabilityProbe(monitoringMailboxes.Count);
			MonitorDefinition monitorDefinition = MailboxDeliveryAvailabilityProbe.CreateMailboxDeliveryAvailabilityMonitor(probeDefinition);
			ResponderDefinition definition = MailboxDeliveryAvailabilityProbe.CreateMailboxDeliveryAvailabilityRestartResponder(probeDefinition, monitorDefinition);
			ResponderDefinition definition2 = MailboxDeliveryAvailabilityProbe.CreateMailboxDeliveryAvailabilityEscalateResponder(probeDefinition, monitorDefinition);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
		}

		private void InstallMailboxInstanceMonitors(ICollection<MailboxDatabaseInfo> mailboxDatabases, MailboxDiscovery.InstanceProbeDefintionDelegate probeDelegate, MailboxDiscovery.InstanceMonitorDefinitionDelegate monitorDelegate, params MailboxDiscovery.InstanceResponderDefinitionDelegate[] responderDelegate)
		{
			foreach (MailboxDatabaseInfo mailboxDatabase in mailboxDatabases)
			{
				if (probeDelegate != null)
				{
					ProbeDefinition probeDefinition = probeDelegate(mailboxDatabase);
					this.ApplyOverrides(probeDefinition);
					base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
					if (monitorDelegate != null)
					{
						MonitorDefinition monitorDefinition = monitorDelegate(mailboxDatabase, probeDefinition);
						List<MonitorStateTransition> list = new List<MonitorStateTransition>();
						foreach (MailboxDiscovery.InstanceResponderDefinitionDelegate instanceResponderDefinitionDelegate in responderDelegate)
						{
							MonitorStateTransition item;
							ResponderDefinition responderDefinition = instanceResponderDefinitionDelegate(mailboxDatabase, monitorDefinition, out item);
							list.Add(item);
							this.ApplyOverrides(responderDefinition);
							base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
						}
						monitorDefinition.MonitorStateTransitions = list.ToArray();
						this.ApplyOverrides(monitorDefinition);
						base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
					}
				}
			}
		}

		private ICollection<MailboxDatabaseInfo> GetMonitoringMailboxes()
		{
			if (!base.Broker.IsLocal())
			{
				throw new SmtpConnectionProbeException("MailboxDiscovery is a local-only discovery and should not be used outside in");
			}
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance == null || instance.ExchangeServerRoleEndpoint == null)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "No instance endpoint found on this server", null, "GetMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 204);
				return null;
			}
			ICollection<MailboxDatabaseInfo> result;
			try
			{
				if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "No mailboxes found, proceeding as success", null, "GetMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 212);
					result = null;
				}
				else
				{
					result = instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend;
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, "Discovery ended due to EndpointManagerEndpointUninitializedException, ignoring exception and treating as transient.", null, "GetMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 221);
				result = null;
			}
			catch
			{
				throw;
			}
			return result;
		}

		private List<MailboxDiscovery.Override> GetOverrideList(string workitemType)
		{
			return new List<MailboxDiscovery.Override>(from o in this.overrideList
			where o.TargetWorkItem.Equals(workitemType, StringComparison.OrdinalIgnoreCase)
			select o);
		}

		private void BuildOverrideList()
		{
			this.overrideList = new List<MailboxDiscovery.Override>();
			using (List<string>.Enumerator enumerator = MailboxDiscovery.OverrideEnabledWorkitemTypes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string type = enumerator.Current;
					IEnumerable<KeyValuePair<string, string>> enumerable = from a in base.Definition.Attributes
					where a.Key.StartsWith(type, StringComparison.OrdinalIgnoreCase)
					select a;
					foreach (KeyValuePair<string, string> keyValuePair in enumerable)
					{
						string[] array = keyValuePair.Key.Split(new char[]
						{
							'_'
						});
						if (array.Length != 2 || string.IsNullOrEmpty(array[1]))
						{
							WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, string.Format("Bad Override: {0}={1}", keyValuePair.Key, keyValuePair.Value), null, "BuildOverrideList", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 263);
						}
						else
						{
							MailboxDiscovery.Override item = new MailboxDiscovery.Override
							{
								TargetWorkItem = type,
								Property = array[1],
								Value = keyValuePair.Value
							};
							this.overrideList.Add(item);
							WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, string.Format("Override found: {0}", item.ToString()), null, "BuildOverrideList", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 274);
						}
					}
				}
			}
		}

		private void ApplyOverrides(WorkDefinition workDefinition)
		{
			List<MailboxDiscovery.Override> list = this.GetOverrideList(workDefinition.Name);
			Type type = workDefinition.GetType();
			foreach (MailboxDiscovery.Override @override in list)
			{
				try
				{
					PropertyInfo property = type.GetProperty(@override.Property, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
					if (property == null || !property.CanWrite)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, string.Format("Override cannot be applied because Property doesn't exist or is read only: {0}", @override.ToString()), null, "ApplyOverrides", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 299);
					}
					else
					{
						property.SetValue(workDefinition, Convert.ChangeType(@override.Value, property.PropertyType), null);
					}
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.TransportTracer, base.TraceContext, string.Format("Exception caught while attempting to set override value: {0}", ex.ToString()), null, "ApplyOverrides", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\Transport\\Discovery\\MailboxDiscovery.cs", 311);
				}
			}
		}

		internal const char ExtensionAttributeSeperator = '_';

		internal static readonly List<string> OverrideEnabledWorkitemTypes = new List<string>
		{
			"MailboxDeliveryInstanceAvailabilityProbe",
			"MailboxDeliveryInstanceAvailabilityMonitor",
			"MailboxDeliveryInstanceAvailabilityEscalateResponder"
		};

		private List<MailboxDiscovery.Override> overrideList;

		public delegate ProbeDefinition InstanceProbeDefintionDelegate(MailboxDatabaseInfo mailboxDatabase);

		public delegate MonitorDefinition InstanceMonitorDefinitionDelegate(MailboxDatabaseInfo mailboxDatabase, ProbeDefinition instanceProbeDefinition);

		public delegate ResponderDefinition InstanceResponderDefinitionDelegate(MailboxDatabaseInfo mailboxDatabase, MonitorDefinition instanceMonitorDefinition, out MonitorStateTransition transition);

		private struct Override
		{
			public override string ToString()
			{
				return string.Format("Target WorkItem: {0}, Property: {1}, Value: {2}", this.TargetWorkItem, this.Property, this.Value);
			}

			public string TargetWorkItem;

			public string Property;

			public string Value;
		}
	}
}
