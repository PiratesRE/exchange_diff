using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public class TransitionCount : IComparable<TransitionCount>
	{
		public TenantRelocationTransition Transition { get; internal set; }

		public ushort Count { get; internal set; }

		internal TransitionCount(TenantRelocationTransition transition, ushort count)
		{
			this.Transition = transition;
			this.Count = count;
		}

		int IComparable<TransitionCount>.CompareTo(TransitionCount other)
		{
			if (other == null)
			{
				return 1;
			}
			return ((byte)this.Transition).CompareTo((byte)other.Transition);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.Transition, this.Count);
		}
	}
}
