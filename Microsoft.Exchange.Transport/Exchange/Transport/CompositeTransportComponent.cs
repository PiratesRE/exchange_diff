using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal abstract class CompositeTransportComponent : IStartableTransportComponent, ITransportComponent
	{
		protected CompositeTransportComponent(string description)
		{
			this.description = description;
			for (int i = 0; i < this.timingBuilders.Length; i++)
			{
				this.timingBuilders[i] = new Dictionary<ITransportComponent, Stopwatch>();
			}
		}

		public IList<ITransportComponent> TransportComponents
		{
			get
			{
				return this.children;
			}
		}

		public string LoadTimings
		{
			get
			{
				return new XElement("LoadTimings", this.GetTimings(CompositeTransportComponent.Operation.Load, false)).ToString();
			}
		}

		public string StartTimings
		{
			get
			{
				return new XElement("StartTimings", this.GetTimings(CompositeTransportComponent.Operation.Start, false)).ToString();
			}
		}

		public bool IsUnloading
		{
			get
			{
				return this.currentOperation == CompositeTransportComponent.Operation.Unload;
			}
		}

		public string CurrentState
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(this.LoadTimings);
				stringBuilder.AppendLine(this.StartTimings);
				stringBuilder.AppendLine(new XElement("StopTimings", this.GetTimings(CompositeTransportComponent.Operation.Stop, true)).ToString());
				stringBuilder.AppendLine(new XElement("UnloadTimings", this.GetTimings(CompositeTransportComponent.Operation.Unload, true)).ToString());
				return stringBuilder.ToString();
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		public abstract void Load();

		public void Unload()
		{
			for (int i = this.children.Count - 1; i >= 0; i--)
			{
				ITransportComponent transportComponent = this.children[i];
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Unloading component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
				CompositeTransportComponent.UnRegisterForDiagnostics(transportComponent);
				this.BeginTiming(CompositeTransportComponent.Operation.Unload, transportComponent);
				transportComponent.Unload();
				this.EndTiming(CompositeTransportComponent.Operation.Unload, transportComponent);
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Unloaded component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
			}
		}

		public string OnUnhandledException(Exception e)
		{
			StringBuilder stringBuilder = null;
			try
			{
				stringBuilder = new StringBuilder();
				for (int i = 0; i < this.children.Count; i++)
				{
					ITransportComponent transportComponent = this.children[i];
					string value = transportComponent.OnUnhandledException(e);
					if (!string.IsNullOrEmpty(value))
					{
						stringBuilder.AppendLine(value);
					}
				}
			}
			catch
			{
			}
			if (stringBuilder == null)
			{
				return null;
			}
			return stringBuilder.ToString();
		}

		public void Start(bool initiallyPaused, ServiceState targetRunningState)
		{
			foreach (ITransportComponent transportComponent in this.children)
			{
				IStartableTransportComponent startableTransportComponent = transportComponent as IStartableTransportComponent;
				if (startableTransportComponent != null)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Starting component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
					this.BeginTiming(CompositeTransportComponent.Operation.Start, transportComponent);
					startableTransportComponent.Start(initiallyPaused, targetRunningState);
					this.EndTiming(CompositeTransportComponent.Operation.Start, transportComponent);
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Started component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
				}
			}
		}

		public void Stop()
		{
			for (int i = this.children.Count - 1; i >= 0; i--)
			{
				ITransportComponent transportComponent = this.children[i];
				IStartableTransportComponent startableTransportComponent = transportComponent as IStartableTransportComponent;
				if (startableTransportComponent != null)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Stopping component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
					this.BeginTiming(CompositeTransportComponent.Operation.Stop, transportComponent);
					startableTransportComponent.Stop();
					this.EndTiming(CompositeTransportComponent.Operation.Stop, transportComponent);
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Stopped component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
				}
			}
		}

		public void Pause()
		{
			foreach (ITransportComponent transportComponent in this.children)
			{
				IStartableTransportComponent startableTransportComponent = transportComponent as IStartableTransportComponent;
				if (startableTransportComponent != null)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Pausing component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
					startableTransportComponent.Pause();
				}
			}
		}

		public void Continue()
		{
			foreach (ITransportComponent transportComponent in this.children)
			{
				IStartableTransportComponent startableTransportComponent = transportComponent as IStartableTransportComponent;
				if (startableTransportComponent != null)
				{
					ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Continue component {0}.", CompositeTransportComponent.GetComponentName(transportComponent));
					startableTransportComponent.Continue();
				}
			}
		}

		protected static void RegisterForDiagnostics(ITransportComponent component)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			IDiagnosable diagnosable = component as IDiagnosable;
			if (diagnosable != null)
			{
				string diagnosticComponentName = diagnosable.GetDiagnosticComponentName();
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Registering component '{0}' for Get-ExchangeDiagnosticInfo.", diagnosticComponentName);
				ProcessAccessManager.RegisterComponent(diagnosable);
			}
		}

		protected static void UnRegisterForDiagnostics(ITransportComponent component)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			IDiagnosable diagnosable = component as IDiagnosable;
			if (diagnosable != null)
			{
				string diagnosticComponentName = diagnosable.GetDiagnosticComponentName();
				ExTraceGlobals.GeneralTracer.TraceDebug<string>(0L, "Un-registering component '{0}' for Get-ExchangeDiagnosticInfo.", diagnosticComponentName);
				ProcessAccessManager.UnregisterComponent(diagnosable);
			}
		}

		protected static string GetComponentName(ITransportComponent component)
		{
			IDiagnosable diagnosable = component as IDiagnosable;
			if (diagnosable != null)
			{
				return diagnosable.GetDiagnosticComponentName();
			}
			CompositeTransportComponent compositeTransportComponent = component as CompositeTransportComponent;
			if (compositeTransportComponent != null)
			{
				return compositeTransportComponent.Description;
			}
			return component.GetType().ToString();
		}

		protected void BeginTiming(CompositeTransportComponent.Operation operation, ITransportComponent child)
		{
			this.currentOperation = operation;
			lock (this.syncRoot)
			{
				this.timingBuilders[(int)operation][child] = Stopwatch.StartNew();
			}
		}

		protected void EndTiming(CompositeTransportComponent.Operation operation, ITransportComponent child)
		{
			lock (this.syncRoot)
			{
				if (this.timingBuilders[(int)operation][child] != null)
				{
					this.timingBuilders[(int)operation][child].Stop();
				}
			}
		}

		protected IEnumerable<XElement> GetTimings(CompositeTransportComponent.Operation operation, bool includeCurrentState)
		{
			lock (this.syncRoot)
			{
				foreach (KeyValuePair<ITransportComponent, Stopwatch> keyValuePair in this.timingBuilders[(int)operation])
				{
					XElement child = new XElement("Component");
					XElement xelement = child;
					XName name = "Name";
					KeyValuePair<ITransportComponent, Stopwatch> keyValuePair2 = keyValuePair;
					xelement.SetAttributeValue(name, CompositeTransportComponent.GetComponentName(keyValuePair2.Key));
					XElement xelement2 = child;
					XName name2 = "Elapsed";
					KeyValuePair<ITransportComponent, Stopwatch> keyValuePair3 = keyValuePair;
					xelement2.SetAttributeValue(name2, keyValuePair3.Value.Elapsed.ToString());
					KeyValuePair<ITransportComponent, Stopwatch> keyValuePair4 = keyValuePair;
					if (keyValuePair4.Value.IsRunning)
					{
						child.SetAttributeValue("IsRunning", true);
					}
					KeyValuePair<ITransportComponent, Stopwatch> keyValuePair5 = keyValuePair;
					CompositeTransportComponent component = keyValuePair5.Key as CompositeTransportComponent;
					if (component != null)
					{
						child.Add(component.GetTimings(operation, includeCurrentState));
					}
					else if (includeCurrentState && this.currentOperation == operation)
					{
						KeyValuePair<ITransportComponent, Stopwatch> keyValuePair6 = keyValuePair;
						if (keyValuePair6.Key is IStartableTransportComponent)
						{
							XContainer xcontainer = child;
							KeyValuePair<ITransportComponent, Stopwatch> keyValuePair7 = keyValuePair;
							xcontainer.Add(((IStartableTransportComponent)keyValuePair7.Key).CurrentState);
						}
					}
					yield return child;
				}
			}
			yield break;
		}

		private readonly List<ITransportComponent> children = new List<ITransportComponent>();

		private readonly Dictionary<ITransportComponent, Stopwatch>[] timingBuilders = new Dictionary<ITransportComponent, Stopwatch>[4];

		private readonly string description;

		private readonly object syncRoot = new object();

		private CompositeTransportComponent.Operation currentOperation;

		protected enum Operation
		{
			Load,
			Start,
			Unload,
			Stop
		}
	}
}
