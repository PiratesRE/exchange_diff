using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	internal class ServerWideOfflineProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, double thresholdInHours, int recurrenceInterval, int timeout, int maxRetry)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = typeof(ServerWideOfflineProbe).FullName;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = Environment.MachineName;
			probeDefinition.Attributes[ServerWideOfflineProbe.ThresholddAttrName] = thresholdInHours.ToString();
			return probeDefinition;
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, double thresholdInHours, int recurrenceInterval)
		{
			return ServerWideOfflineProbe.CreateDefinition(name, serviceName, thresholdInHours, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		public override void PopulateDefinition<ProbeDefinition>(ProbeDefinition pDef, Dictionary<string, string> propertyBag)
		{
			if (pDef == null)
			{
				throw new ArgumentException("Please specify a value for probeDefinition");
			}
			if (propertyBag.ContainsKey(ServerWideOfflineProbe.ThresholddAttrName))
			{
				pDef.Attributes[ServerWideOfflineProbe.ThresholddAttrName] = propertyBag[ServerWideOfflineProbe.ThresholddAttrName].ToString().Trim();
				return;
			}
			throw new ArgumentException("Please specify value for" + ServerWideOfflineProbe.ThresholddAttrName);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			double num = 0.0;
			if (string.IsNullOrWhiteSpace(base.Definition.Attributes["threshold"]) || !double.TryParse(base.Definition.Attributes["threshold"], out num))
			{
				throw new HighAvailabilityMAProbeException("Required attribute 'threshold' is either in wrong format or is empty");
			}
			DateTime utcNow = DateTime.UtcNow;
			base.Result.StateAttribute1 = string.Format("TargetResource='{0}'", base.Definition.TargetResource);
			base.Result.StateAttribute4 = string.Format("Threshold={0:0.00} hours. CurrentTime={1}", num, utcNow.ToString());
			ServerWideOfflineProbe.ComponentState componentState = ServerWideOfflineProbe.ComponentState.ConstructFromLocalRegistry(base.TraceContext);
			base.Result.StateAttribute2 = string.Format("ServerWideStatus={0}", componentState.IsOnline ? "Online" : "Offline");
			base.Result.StateAttribute3 = string.Format("StateTimestamp={0}", componentState.Timestamp.ToString());
			if (!componentState.IsOnline && utcNow - componentState.Timestamp > TimeSpan.FromHours(num))
			{
				base.Result.StateAttribute5 = "Fail.";
				throw new HighAvailabilityMAProbeRedResultException(string.Format("Server '{0}' is offline since {1} which is longer than {2:0.00} hours. Current time is {3}", new object[]
				{
					base.Definition.TargetResource,
					componentState.Timestamp,
					num,
					utcNow.ToString()
				}));
			}
			base.Result.StateAttribute5 = string.Format("Pass", new object[0]);
		}

		public static readonly string ThresholddAttrName = "threshold";

		private class ComponentState
		{
			private ComponentState(ServerComponentEnum component, TracingContext traceContext)
			{
				this.traceContext = traceContext;
				this.component = component;
				this.RetrieveAndUpdateIfNecessary();
			}

			public DateTime Timestamp
			{
				get
				{
					return this.timestamp;
				}
			}

			public bool IsOnline
			{
				get
				{
					return this.isOnline;
				}
			}

			public ServerComponentEnum Component
			{
				get
				{
					return this.component;
				}
			}

			public static ServerWideOfflineProbe.ComponentState ConstructFromLocalRegistry(TracingContext traceContext)
			{
				return ServerWideOfflineProbe.ComponentState.ConstructFromLocalRegistry(ServerComponentEnum.ServerWideOffline, traceContext);
			}

			public static ServerWideOfflineProbe.ComponentState ConstructFromLocalRegistry(ServerComponentEnum component, TracingContext traceContext)
			{
				return new ServerWideOfflineProbe.ComponentState(component, traceContext);
			}

			private void RetrieveAndUpdateIfNecessary()
			{
				bool flag = ServerComponentStateManager.IsOnline(this.component);
				DateTime utcNow = DateTime.UtcNow;
				string value = HighAvailabilityUtility.NonCachedRegReader.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States\\ServerComponentStates", this.component.ToString(), string.Empty);
				if (!this.TryDeserializeFromRegistry(value, out this.isOnline, out this.timestamp) || flag != this.isOnline)
				{
					this.isOnline = flag;
					this.timestamp = utcNow;
					HighAvailabilityUtility.RegWriter.CreateSubKey(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States\\ServerComponentStates");
					HighAvailabilityUtility.RegWriter.SetValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States\\ServerComponentStates", this.component.ToString(), this.SerializeToRegistry(this.isOnline, this.timestamp), RegistryValueKind.String);
				}
			}

			private bool TryDeserializeFromRegistry(string s, out bool isOnline, out DateTime timestamp)
			{
				isOnline = false;
				timestamp = DateTime.MinValue;
				if (string.IsNullOrWhiteSpace(s))
				{
					WTFDiagnostics.TraceError(ExTraceGlobals.HighAvailabilityTracer, this.traceContext, "Unable to deserialize state from registry - either NULL or Empty", null, "TryDeserializeFromRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ServerWideOfflineProbe.cs", 296);
					return false;
				}
				string[] array = s.Split(new char[]
				{
					';'
				});
				if (array.Length != 2)
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.HighAvailabilityTracer, this.traceContext, "Unable to deserialize state from registry - value '{0}' is not in correct format", s, null, "TryDeserializeFromRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ServerWideOfflineProbe.cs", 308);
					return false;
				}
				if (!bool.TryParse(array[0], out isOnline) || !DateTime.TryParse(array[1], out timestamp))
				{
					WTFDiagnostics.TraceError<string>(ExTraceGlobals.HighAvailabilityTracer, this.traceContext, "Unable to deserialize state from registry - value '{0}' cannot be deserialized to respective data types", s, null, "TryDeserializeFromRegistry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HighAvailability\\Probes\\ServerWideOfflineProbe.cs", 319);
					return false;
				}
				return true;
			}

			private string SerializeToRegistry(bool isOnline, DateTime timestamp)
			{
				return string.Join(';'.ToString(), new string[]
				{
					isOnline.ToString(),
					timestamp.ToString()
				});
			}

			private const char RegValueSeperator = ';';

			private DateTime timestamp;

			private ServerComponentEnum component;

			private bool isOnline;

			private TracingContext traceContext;
		}
	}
}
