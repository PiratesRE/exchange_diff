using System;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public abstract class ProbeWorkItem : WorkItem
	{
		public new ProbeDefinition Definition
		{
			get
			{
				return (ProbeDefinition)base.Definition;
			}
		}

		public new ProbeResult Result
		{
			get
			{
				return (ProbeResult)base.Result;
			}
		}

		protected new IProbeWorkBroker Broker
		{
			get
			{
				return (IProbeWorkBroker)base.Broker;
			}
		}
	}
}
