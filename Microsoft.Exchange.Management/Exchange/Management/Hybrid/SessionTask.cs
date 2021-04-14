using System;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal abstract class SessionTask : TaskBase
	{
		public SessionTask(string taskName, int weight) : base(taskName, weight)
		{
		}

		protected ITenantSession TenantSession
		{
			get
			{
				return base.TaskContext.TenantSession;
			}
		}

		protected IOnPremisesSession OnPremisesSession
		{
			get
			{
				return base.TaskContext.OnPremisesSession;
			}
		}

		protected ILogger Logger
		{
			get
			{
				return base.TaskContext.Logger;
			}
		}
	}
}
