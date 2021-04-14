using System;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AnchorService
{
	internal abstract class CacheProcessorBase : IDiagnosable
	{
		internal CacheProcessorBase(AnchorContext context, WaitHandle stopEvent)
		{
			this.Context = context;
			this.StopEvent = stopEvent;
		}

		public AnchorContext Context { get; private set; }

		public long Duration { get; set; }

		public ExDateTime? LastRunTime { get; set; }

		public ExDateTime? LastWorkTime { get; set; }

		public ExDateTime? StartWorkTime { get; set; }

		public virtual TimeSpan ActiveRunDelay
		{
			get
			{
				return this.Context.Config.GetConfig<TimeSpan>("ActiveRunDelay");
			}
		}

		internal abstract string Name { get; }

		private protected WaitHandle StopEvent { protected get; private set; }

		private protected int SequenceNumber { protected get; private set; }

		public virtual string GetDiagnosticComponentName()
		{
			return this.Name;
		}

		public virtual XElement GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			return new XElement(this.Name, new object[]
			{
				new XElement("duration", this.Duration),
				new XElement("lastRunTime", (this.LastRunTime != null) ? this.LastRunTime.Value.UniversalTime.ToString() : string.Empty),
				new XElement("lastWorkTime", (this.LastWorkTime != null) ? this.LastWorkTime.Value.UniversalTime.ToString() : string.Empty),
				new XElement("startWorkTime", (this.StartWorkTime != null) ? this.StartWorkTime.Value.UniversalTime.ToString() : string.Empty),
				new XElement("sequenceNumber", this.SequenceNumber)
			});
		}

		internal abstract bool ShouldProcess();

		internal abstract bool Process(JobCache data);

		internal void IncrementSequenceNumber()
		{
			this.SequenceNumber = (this.SequenceNumber + 1) % 100000;
		}
	}
}
