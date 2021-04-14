using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class MeteringComponent : IMeteringComponent, ITransportComponent, IDiagnosable
	{
		public MeteringComponent() : this(() => DateTime.UtcNow)
		{
		}

		public MeteringComponent(Func<DateTime> currentTimeProvider)
		{
			this.currentTimeProvider = currentTimeProvider;
		}

		public ICountTracker<MeteredEntity, MeteredCount> Metering { get; private set; }

		public ICountTrackerDiagnostics<MeteredEntity, MeteredCount> MeteringDiagnostics { get; private set; }

		public void SetLoadtimeDependencies(ICountTrackerConfig config, Trace tracer)
		{
			this.config = config;
			this.tracer = tracer;
		}

		public void Load()
		{
			this.MeteringDiagnostics = new CountTrackerDiagnostics<MeteredEntity, MeteredCount>();
			this.Metering = new CountTracker<MeteredEntity, MeteredCount>(this.config, this.MeteringDiagnostics, this.tracer, this.currentTimeProvider);
		}

		public void Unload()
		{
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public string GetDiagnosticComponentName()
		{
			return "MeteringComponent";
		}

		public XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			bool flag = parameters.Argument.Equals("config", StringComparison.InvariantCultureIgnoreCase);
			bool flag2 = !flag || parameters.Argument.IndexOf("help", StringComparison.OrdinalIgnoreCase) != -1;
			if (parameters.Argument.StartsWith("Tenant-", StringComparison.InvariantCultureIgnoreCase))
			{
				xelement.Add(this.Metering.GetDiagnosticInfo(new SimpleEntityName<MeteredEntity>(MeteredEntity.Tenant, parameters.Argument.Substring("Tenant-".Length))));
			}
			else if (parameters.Argument.StartsWith("Sender-", StringComparison.InvariantCultureIgnoreCase))
			{
				xelement.Add(this.Metering.GetDiagnosticInfo(new SimpleEntityName<MeteredEntity>(MeteredEntity.Sender, parameters.Argument.Substring("Sender-".Length))));
			}
			else
			{
				this.Metering.GetDiagnosticInfo(parameters.Argument, xelement);
			}
			if (flag)
			{
				xelement.Add(TransportAppConfig.GetDiagnosticInfoForType(this.config));
			}
			if (flag2)
			{
				xelement.Add(new XElement("help", string.Format("Supported arguments: verbose, help, config, {0}'tenantguid', {1}'sender'", "Tenant-", "Sender-")));
			}
			return xelement;
		}

		private ICountTrackerConfig config;

		private Trace tracer;

		private Func<DateTime> currentTimeProvider;
	}
}
