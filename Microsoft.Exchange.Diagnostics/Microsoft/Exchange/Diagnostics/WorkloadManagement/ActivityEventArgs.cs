using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class ActivityEventArgs : EventArgs
	{
		public ActivityEventArgs(ActivityEventType activityEventType, string message = null)
		{
			this.activityEventType = activityEventType;
			this.Message = message;
		}

		public ActivityEventType ActivityEventType
		{
			get
			{
				return this.activityEventType;
			}
		}

		public string Message { get; private set; }

		private readonly ActivityEventType activityEventType;
	}
}
