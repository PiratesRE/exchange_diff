using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class TransitionInfo
	{
		private TransitionInfo(ConditionMethod condition, ActionMethod action, uint targetState)
		{
			this.condition = condition;
			this.action = action;
			this.targetState = targetState;
		}

		internal uint TargetState
		{
			get
			{
				return this.targetState;
			}
		}

		internal ConditionMethod Condition
		{
			get
			{
				return this.condition;
			}
		}

		internal ActionMethod Action
		{
			get
			{
				return this.action;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}::{1}, target:{1}", (this.Action == null) ? null : this.Action.Method.DeclaringType, this.TargetState);
		}

		internal static TransitionInfo Create(ConditionMethod condition, ActionMethod action, uint targetState)
		{
			return new TransitionInfo(condition, action, targetState);
		}

		private uint targetState;

		private ConditionMethod condition;

		private ActionMethod action;
	}
}
