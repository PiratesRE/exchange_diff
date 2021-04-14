using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.FastTransfer;

namespace Microsoft.Exchange.Data.Storage.ActivityLog
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ActivityMessageIteratorClient : DisposableObject, IMessageIteratorClient, IDisposable
	{
		public ActivityMessageIteratorClient(Action<Activity> deserializationAction)
		{
			Util.ThrowOnNullArgument(deserializationAction, "deserializationAction");
			this.deserializationAction = deserializationAction;
		}

		public IMessage UploadMessage(bool isAssociatedMessage)
		{
			if (isAssociatedMessage)
			{
				throw new NotSupportedException("Activity cannot have isAssociatedMessage flag set.");
			}
			return Activity.CreateMessageAdapter(this.deserializationAction);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityMessageIteratorClient>(this);
		}

		private readonly Action<Activity> deserializationAction;
	}
}
