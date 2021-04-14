using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.EntitySets.Commands
{
	[DataContract]
	public abstract class EntityCommand<TEntitySet, TResult> : Command<TResult>, IEntityCommand<TEntitySet, TResult>
	{
		public virtual TEntitySet Scope { get; set; }

		protected bool ShouldExpand(string propertyName)
		{
			return this.Context != null && this.Context.Expand != null && this.Context.Expand.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
		}

		protected void RestoreScope(TEntitySet scope)
		{
			this.Scope = scope;
		}

		private void TraceExecution()
		{
			if (this.Trace.IsTraceEnabled(TraceType.DebugTrace))
			{
				this.Trace.TraceDebug((long)this.GetHashCode(), "{0}::Execute({1}{2}){3}", new object[]
				{
					base.GetType().Name,
					this.Scope,
					this.GetCommandTraceDetails(),
					this.Context
				});
			}
		}

		private void OnDeserialized()
		{
			base.RegisterOnBeforeExecute(new Action(this.TraceExecution));
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext streamingContext)
		{
			this.OnDeserialized();
		}
	}
}
