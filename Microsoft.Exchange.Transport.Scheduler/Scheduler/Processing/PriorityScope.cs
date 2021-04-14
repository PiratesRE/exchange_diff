using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Scheduler.Contracts;

namespace Microsoft.Exchange.Transport.Scheduler.Processing
{
	internal sealed class PriorityScope : IMessageScope, IEquatable<IMessageScope>, IEquatable<PriorityScope>
	{
		public PriorityScope(DeliveryPriority priority)
		{
			ArgumentValidator.ThrowIfNull("priority", priority);
			this.priority = priority;
		}

		public string Display
		{
			get
			{
				return "Priority:" + this.Value;
			}
		}

		public MessageScopeType Type
		{
			get
			{
				return MessageScopeType.Priority;
			}
		}

		public object Value
		{
			get
			{
				return this.priority;
			}
		}

		public static bool operator ==(PriorityScope left, PriorityScope right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(PriorityScope left, PriorityScope right)
		{
			return !object.Equals(left, right);
		}

		public bool Equals(PriorityScope other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || this.priority == other.priority);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && (object.ReferenceEquals(this, obj) || (!(obj.GetType() != base.GetType()) && this.Equals((PriorityScope)obj)));
		}

		public override int GetHashCode()
		{
			return (int)this.priority;
		}

		public bool Equals(IMessageScope other)
		{
			return this.Equals(other as PriorityScope);
		}

		public override string ToString()
		{
			return this.Display;
		}

		private readonly DeliveryPriority priority;
	}
}
