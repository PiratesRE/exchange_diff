using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActivityMessageIterator : DisposableObject, IMessageIterator, IDisposable
	{
		public ActivityMessageIterator(IEnumerable<Activity> activities)
		{
			Util.ThrowOnNullArgument(activities, "activities");
			this.activities = activities;
		}

		public IEnumerator<IMessage> GetMessages()
		{
			foreach (Activity activity in this.activities)
			{
				yield return activity.CreateMessageAdapter();
			}
			yield break;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityMessageIterator>(this);
		}

		private readonly IEnumerable<Activity> activities;
	}
}
