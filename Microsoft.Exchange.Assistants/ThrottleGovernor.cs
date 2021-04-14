using System;

namespace Microsoft.Exchange.Assistants
{
	internal abstract class ThrottleGovernor : Governor
	{
		public ThrottleGovernor(Governor parentGovernor, Throttle throttle) : base(parentGovernor)
		{
			this.throttle = throttle;
		}

		public Throttle Throttle
		{
			get
			{
				return this.throttle;
			}
		}

		public override void ExportToQueryableObject(QueryableObject queryableObject)
		{
			base.ExportToQueryableObject(queryableObject);
			QueryableThrottleGovernor queryableThrottleGovernor = queryableObject as QueryableThrottleGovernor;
			if (queryableThrottleGovernor != null)
			{
				QueryableThrottle queryableObject2 = new QueryableThrottle();
				this.throttle.ExportToQueryableObject(queryableObject2);
				queryableThrottleGovernor.Throttle = queryableObject2;
			}
		}

		protected override void Run()
		{
			this.throttle.OpenThrottle();
		}

		protected override void Retry()
		{
			this.throttle.SetThrottle(1);
		}

		protected override void OnFailure()
		{
			this.throttle.CloseThrottle();
		}

		private Throttle throttle;
	}
}
