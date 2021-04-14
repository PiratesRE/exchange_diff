using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class ThrottlingContext
	{
		public ThrottlingContext(DateTime startTime)
		{
			this.startTime = startTime;
		}

		public ThrottlingContext(DateTime startTime, Cost cost)
		{
			this.startTime = startTime;
			this.cost = cost;
		}

		public ThrottlingContext(Cost cost)
		{
			this.cost = cost;
		}

		public DateTime CreationTime
		{
			get
			{
				return this.startTime;
			}
		}

		public Cost Cost
		{
			get
			{
				return this.cost;
			}
		}

		public void AddMemoryCost(ByteQuantifiedSize bytesUsed)
		{
			if (this.cost != null)
			{
				this.cost.AddMemoryCost(bytesUsed);
			}
		}

		public void AddBreadcrumb(CategorizerBreadcrumb breadcrumb)
		{
			this.breadcrumbs.Drop(breadcrumb);
		}

		private Breadcrumbs<CategorizerBreadcrumb> breadcrumbs = new Breadcrumbs<CategorizerBreadcrumb>(256);

		private readonly DateTime startTime;

		private readonly Cost cost;
	}
}
