using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.Common
{
	internal class ConversationClutterInformation
	{
		public ConversationClutterInformation()
		{
			this.ClutterMessageCount = 0;
			this.UnClutterMessageCount = 0;
			this.ClutterMessageIds = null;
			this.InheritedState = null;
			this.State = ConversationClutterState.Mixed;
		}

		public void Initialize(int clutterCount, int unClutterCount, IReadOnlyList<IIdentity> messageIds, ConversationClutterState? inheritedState)
		{
			if (clutterCount == 0 && unClutterCount == 0 && inheritedState == null)
			{
				throw new ArgumentException("Both clutter and un-clutter count cannot be zero unless there is an inheritable state");
			}
			this.ClutterMessageCount = clutterCount;
			this.UnClutterMessageCount = unClutterCount;
			this.ClutterMessageIds = messageIds;
			this.InheritedState = inheritedState;
			this.State = this.GetConversationState();
		}

		public int ClutterMessageCount { get; private set; }

		public int UnClutterMessageCount { get; private set; }

		public ConversationClutterState State { get; private set; }

		public IReadOnlyList<IIdentity> ClutterMessageIds { get; private set; }

		public ConversationClutterState? InheritedState { get; private set; }

		public bool IsNewMessageClutter(bool computedClutter, out bool ShouldMarkConversationAsNotClutter)
		{
			bool result = computedClutter;
			ShouldMarkConversationAsNotClutter = false;
			if (computedClutter && this.State != ConversationClutterState.Clutter)
			{
				result = !computedClutter;
			}
			else if (!computedClutter && this.State == ConversationClutterState.Clutter)
			{
				ShouldMarkConversationAsNotClutter = (this.ClutterMessageIds != null && this.ClutterMessageIds.Any<IIdentity>());
			}
			return result;
		}

		public virtual void MarkItemsAsNotClutter(bool userOverride)
		{
		}

		public void MarkItemsAsNotClutter()
		{
			this.MarkItemsAsNotClutter(false);
		}

		public override string ToString()
		{
			return string.Format("ClutterMessageCount={0} UnClutterMessageCount={1} State={2} InheritedState={3}", new object[]
			{
				this.ClutterMessageCount,
				this.UnClutterMessageCount,
				this.State,
				this.InheritedState
			});
		}

		private ConversationClutterState GetConversationState()
		{
			if (this.ClutterMessageCount > 0 && this.UnClutterMessageCount == 0)
			{
				return ConversationClutterState.Clutter;
			}
			if (this.ClutterMessageCount == 0 && this.UnClutterMessageCount > 0)
			{
				return ConversationClutterState.UnClutter;
			}
			if (this.ClutterMessageCount == 0 && this.UnClutterMessageCount == 0 && this.InheritedState != null)
			{
				return this.InheritedState.Value;
			}
			return ConversationClutterState.Mixed;
		}
	}
}
