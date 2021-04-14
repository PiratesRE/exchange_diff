using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal abstract class MonitoringContextBase
	{
		public MonitoringContextBase(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			this.broker = broker;
			this.traceContext = traceContext;
			this.ContextType = base.GetType().ToString();
			this.ExceptionCaught = new List<Exception>();
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

		public List<Exception> ExceptionCaught { get; private set; }

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

		private string ContextType { get; set; }

		public abstract void CreateContext();

		protected void InvokeCatchAndLog(Action action)
		{
			try
			{
				if (action != null)
				{
					action();
				}
			}
			catch (Exception ex)
			{
				if (this.ExceptionCaught == null)
				{
					this.ExceptionCaught = new List<Exception>();
				}
				this.ExceptionCaught.Add(ex);
				this.AddMessage(string.Format("Exception caught in {0} - Exception: {1}", this.ContextType, ex.ToString()));
			}
		}

		protected void EnrollWorkItem<TDefinition>(TDefinition workDefinition) where TDefinition : WorkDefinition
		{
			TDefinition tdefinition = workDefinition;
			MonitoringContextBase.EnrollmentType workItemType = MonitoringContextBase.EnrollmentType.Unknown;
			if (tdefinition is ProbeDefinition)
			{
				workItemType = MonitoringContextBase.EnrollmentType.Probe;
			}
			else if (tdefinition is MonitorDefinition)
			{
				workItemType = MonitoringContextBase.EnrollmentType.Monitor;
				MonitorDefinition monitorDefinition = (MonitorDefinition)((object)tdefinition);
				if (monitorDefinition.MonitoringIntervalSeconds <= 0)
				{
					this.message.Add(string.Format("Monitor '{0}' MonitoringInterval was '{1}'. Rectified to default value '{2}'", monitorDefinition.Name, monitorDefinition.MonitoringIntervalSeconds, 300));
					monitorDefinition.MonitoringIntervalSeconds = 300;
					tdefinition = (TDefinition)((object)monitorDefinition);
				}
			}
			else if (tdefinition is ResponderDefinition)
			{
				workItemType = MonitoringContextBase.EnrollmentType.Responder;
			}
			this.enrolledWorkItems.Add(new MonitoringContextBase.EnrollmentResult
			{
				WorkItemResultName = tdefinition.ConstructWorkItemResultName(),
				WorkItemClass = tdefinition.TypeName,
				WorkItemType = workItemType
			});
			this.broker.AddWorkDefinition<TDefinition>(tdefinition, this.TraceContext);
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
					if (string.IsNullOrEmpty(responder.ServiceName) || !ExchangeComponent.WellKnownComponents.ContainsKey(responder.ServiceName))
					{
						responder.ServiceName = HighAvailabilityConstants.ServiceName;
					}
					list2.Add(responder);
				}
			}
			monitor.MonitorStateTransitions = list.ToArray();
			if (string.IsNullOrEmpty(monitor.ServiceName))
			{
				monitor.ServiceName = HighAvailabilityConstants.ServiceName;
			}
			this.EnrollWorkItem<MonitorDefinition>(monitor);
			bool disableResponders = HighAvailabilityConstants.DisableResponders;
			if (disableResponders)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HighAvailabilityTracer, this.TraceContext, "MonitoringContextBase:: AddChainedResponders(): Responders Disabled, master switch is SET.", null, "AddChainedResponders", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\MonitoringContext\\MonitoringContextBase.cs", 276);
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
