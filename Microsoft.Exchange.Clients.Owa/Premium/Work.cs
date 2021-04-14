using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class Work
	{
		public float WorkAmount
		{
			get
			{
				return this.workAmount;
			}
			set
			{
				this.workAmount = value;
			}
		}

		public DurationUnit WorkUnit
		{
			get
			{
				return this.workUnit;
			}
			set
			{
				this.workUnit = value;
			}
		}

		internal Work(float workAmount, DurationUnit workUnit)
		{
			this.workAmount = workAmount;
			this.workUnit = workUnit;
		}

		private float workAmount;

		private DurationUnit workUnit;
	}
}
