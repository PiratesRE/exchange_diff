using System;
using System.Threading;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackScheduler
	{
		public virtual void ScheduleOnce(Action action, int dueTime)
		{
			Timer timer = null;
			timer = new Timer(delegate(object state)
			{
				action();
				timer.Dispose();
			});
			timer.Change(dueTime, -1);
		}

		public static readonly ApnsFeedbackScheduler DefaultScheduler = new ApnsFeedbackScheduler();
	}
}
