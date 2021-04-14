using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Configuration.TenantMonitoring;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal sealed class NewMailboxHealthCountersModule : CmdletHealthCountersModule
	{
		public NewMailboxHealthCountersModule(TaskContext context) : base(context)
		{
		}

		protected override CounterType CounterTypeForAttempts
		{
			get
			{
				return CounterType.NewMailboxAttempts;
			}
		}

		protected override CounterType CounterTypeForSuccesses
		{
			get
			{
				return CounterType.NewMailboxSuccesses;
			}
		}

		protected override CounterType CounterTypeForIterationAttempts
		{
			get
			{
				return CounterType.NewMailboxIterationAttempts;
			}
		}

		protected override CounterType CounterTypeForIterationSuccesses
		{
			get
			{
				return CounterType.NewMailboxIterationSuccesses;
			}
		}

		protected override string TenantNameForMonitoringCounters
		{
			get
			{
				string text = string.Empty;
				base.CurrentTaskContext.TryGetItem<string>("TenantNameForMonitoring", ref text);
				if (string.IsNullOrEmpty(text))
				{
					object obj = base.CurrentTaskContext.InvocationInfo.Fields["Organization"];
					text = ((obj != null) ? obj.ToString() : base.TenantNameForMonitoringCounters);
				}
				return text;
			}
		}
	}
}
