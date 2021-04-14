using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment
{
	internal abstract class MonitoringContextBase
	{
		public MonitoringContextBase(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			this.broker = broker;
			this.traceContext = traceContext;
			this.enrolledWorkItems = new List<MonitoringContextBase.EnrollmentResult>();
		}

		public MonitoringContextBase(IMaintenanceWorkBroker broker, LocalEndpointManager endpointManager, TracingContext traceContext) : this(broker, traceContext)
		{
			this.endpointManager = endpointManager;
		}

		public List<MonitoringContextBase.EnrollmentResult> WorkItemsEnrollmentResult
		{
			get
			{
				return this.enrolledWorkItems;
			}
		}

		public string LoggedMessages
		{
			get
			{
				if (this.message != null && this.message.Count > 0)
				{
					return string.Join(Environment.NewLine, this.message);
				}
				return string.Empty;
			}
		}

		protected LocalEndpointManager EndpointManager
		{
			get
			{
				return this.endpointManager;
			}
		}

		protected TracingContext TraceContext
		{
			get
			{
				return this.traceContext;
			}
		}

		public abstract void CreateContext();

		protected void EnrollWorkItem<TDefinition>(TDefinition workDefinition) where TDefinition : WorkDefinition
		{
			MonitoringContextBase.EnrollmentType workItemType = MonitoringContextBase.EnrollmentType.Unknown;
			if (workDefinition is ProbeDefinition)
			{
				workItemType = MonitoringContextBase.EnrollmentType.Probe;
			}
			else if (workDefinition is MonitorDefinition)
			{
				workItemType = MonitoringContextBase.EnrollmentType.Monitor;
			}
			else if (workDefinition is ResponderDefinition)
			{
				workItemType = MonitoringContextBase.EnrollmentType.Responder;
			}
			this.enrolledWorkItems.Add(new MonitoringContextBase.EnrollmentResult
			{
				WorkItemResultName = workDefinition.ConstructWorkItemResultName(),
				WorkItemClass = workDefinition.TypeName,
				WorkItemType = workItemType
			});
			this.broker.AddWorkDefinition<TDefinition>(workDefinition, this.TraceContext);
		}

		protected void AddChainedResponders(ref MonitorDefinition monitor, params MonitorStateResponderTuple[] tuples)
		{
			List<MonitorStateTransition> list = new List<MonitorStateTransition>();
			List<ResponderDefinition> list2 = new List<ResponderDefinition>();
			for (int i = 0; i < tuples.Length; i++)
			{
				list.Add(tuples[i].MonitorState);
				ResponderDefinition responder = tuples[i].Responder;
				if (responder != null)
				{
					responder.TargetHealthState = tuples[i].MonitorState.ToState;
					if (string.IsNullOrEmpty(responder.ServiceName) || responder.ServiceName != "BitlockerDeployment")
					{
						responder.ServiceName = "BitlockerDeployment";
					}
					list2.Add(responder);
				}
			}
			monitor.MonitorStateTransitions = list.ToArray();
			if (string.IsNullOrEmpty(monitor.ServiceName) || monitor.ServiceName != "BitlockerDeployment")
			{
				monitor.ServiceName = "BitlockerDeployment";
			}
			this.EnrollWorkItem<MonitorDefinition>(monitor);
			bool flag = false;
			if (flag)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HighAvailabilityTracer, this.TraceContext, "MonitoringContextBase:: AddChainedResponders(): Responders Disabled, master switch is SET.", null, "AddChainedResponders", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\BitlockerDeployment\\MonitoringContext\\MonitoringContextBase.cs", 215);
				return;
			}
			foreach (ResponderDefinition responderDefinition in list2)
			{
				if (responderDefinition != null)
				{
					this.EnrollWorkItem<ResponderDefinition>(responderDefinition);
				}
			}
		}

		protected void AddMessage(string messageToBeLogged)
		{
			this.message.Add(messageToBeLogged);
		}

		private IMaintenanceWorkBroker broker;

		private LocalEndpointManager endpointManager;

		private TracingContext traceContext;

		private List<MonitoringContextBase.EnrollmentResult> enrolledWorkItems;

		private List<string> message = new List<string>();

		public enum EnrollmentType
		{
			Probe,
			Monitor,
			Responder,
			Unknown
		}

		public struct EnrollmentResult
		{
			public string WorkItemResultName;

			public string WorkItemClass;

			public MonitoringContextBase.EnrollmentType WorkItemType;
		}
	}
}
