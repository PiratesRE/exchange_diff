using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class AssistantCollectionEntry : Base
	{
		public AssistantCollectionEntry(AssistantType assistantType, DatabaseInfo databaseInfo)
		{
			this.assistantType = assistantType;
			this.Instance = assistantType.CreateInstance(databaseInfo);
			this.State = AssistantCollectionEntry.AssistantState.Constructed;
		}

		public AssistantType Type
		{
			get
			{
				return this.assistantType;
			}
		}

		public IEventBasedAssistant Instance { get; private set; }

		public Guid Identity
		{
			get
			{
				return this.assistantType.Identity;
			}
		}

		public string Name
		{
			get
			{
				return this.Instance.Name;
			}
		}

		public PerformanceCountersPerAssistantInstance PerformanceCounters
		{
			get
			{
				return this.assistantType.PerformanceCounters;
			}
		}

		private AssistantCollectionEntry.AssistantState State { get; set; }

		public void Start(EventBasedStartInfo startInfo)
		{
			AIBreadcrumbs.StartupTrail.Drop("Starting event assistant: " + this.Instance.Name);
			this.State = AssistantCollectionEntry.AssistantState.Starting;
			this.Instance.OnStart(startInfo);
			this.State = AssistantCollectionEntry.AssistantState.Started;
			AIBreadcrumbs.StartupTrail.Drop("Finished starting " + this.Instance.Name);
			base.TracePfd("PFD AIS {0} Start request for {1} ", new object[]
			{
				24983,
				this.Instance
			});
		}

		public void Shutdown()
		{
			if (this.State == AssistantCollectionEntry.AssistantState.Started)
			{
				base.TracePfd("PFD AIS {0} Shutdown requested for assistant {1}", new object[]
				{
					16791,
					this.Instance
				});
				this.State = AssistantCollectionEntry.AssistantState.Stopping;
				AIBreadcrumbs.ShutdownTrail.Drop("Shutting down event assistant: " + this.Instance.Name);
				this.Instance.OnShutdown();
				this.State = AssistantCollectionEntry.AssistantState.Stopped;
				AIBreadcrumbs.ShutdownTrail.Drop("Finished shutting down " + this.Instance.Name);
				return;
			}
			ExTraceGlobals.EventBasedAssistantCollectionTracer.TraceDebug<LocalizedString, AssistantCollectionEntry.AssistantState>((long)this.GetHashCode(), "Entry for {0}: Shutdown requested for this assistant while current state was {1}.", this.Instance.Name, this.State);
		}

		private AssistantType assistantType;

		private enum AssistantState
		{
			Constructed,
			Starting,
			Started,
			Stopping,
			Stopped
		}
	}
}
