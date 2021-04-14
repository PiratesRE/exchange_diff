using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.AirSync
{
	internal class PerThreadTimeTracker
	{
		internal PerThreadTimeTracker()
		{
			this.IsValid = true;
			this.Root = this.Start(TimeId.Root);
		}

		public bool IsValid { get; private set; }

		public string InvalidReason { get; private set; }

		public TimeEntry Root { get; private set; }

		public TimeEntry Start(TimeId timeId)
		{
			if (this.Root != null)
			{
				this.Root.VerifyThread();
			}
			TimeEntry timeEntry = new TimeEntry(timeId, new Action<TimeEntry>(this.End));
			this.callStack.Push(timeEntry);
			return timeEntry;
		}

		private void Invalidate(string reason)
		{
			this.IsValid = false;
			if (this.InvalidReason == null)
			{
				this.InvalidReason = reason;
			}
		}

		private void End(TimeEntry timeEntry)
		{
			this.Root.VerifyThread();
			if (this.callStack.Count == 0)
			{
				this.Invalidate("EmptyStack-" + timeEntry.TimeId);
				return;
			}
			TimeEntry timeEntry2 = this.callStack.Pop();
			if (object.ReferenceEquals(timeEntry, timeEntry2))
			{
				if (this.callStack.Count > 0)
				{
					TimeEntry timeEntry3 = this.callStack.Peek();
					if (timeEntry3 != null)
					{
						timeEntry3.AddChild(timeEntry2);
					}
				}
				return;
			}
			if (timeEntry.TimeId == timeEntry2.TimeId)
			{
				this.Invalidate(string.Format("MismatchStartEnd-RefMismatch:'{0}'", timeEntry.TimeId));
				return;
			}
			this.Invalidate(string.Format("MismatchStartEnd-Exp:'{0}'-Act:'{1}'", timeEntry2.TimeId, timeEntry.TimeId));
		}

		public override string ToString()
		{
			if (!this.IsValid)
			{
				return this.InvalidReason;
			}
			return this.Root.ToString();
		}

		private Stack<TimeEntry> callStack = new Stack<TimeEntry>();
	}
}
