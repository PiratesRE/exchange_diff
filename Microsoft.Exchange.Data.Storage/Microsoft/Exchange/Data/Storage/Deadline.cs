using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Deadline
	{
		public TimeSpan TimeLeft
		{
			get
			{
				return this.FinalTime - DateTime.UtcNow;
			}
		}

		private Deadline()
		{
		}

		public Deadline(TimeSpan maxTime)
		{
			this.FinalTime = DateTime.UtcNow.Add(maxTime);
		}

		public virtual bool IsOver
		{
			get
			{
				return DateTime.UtcNow >= this.FinalTime;
			}
		}

		public override string ToString()
		{
			return this.FinalTime.ToString();
		}

		public static readonly Deadline NoDeadline = new Deadline();

		private readonly DateTime FinalTime = DateTime.MaxValue;
	}
}
