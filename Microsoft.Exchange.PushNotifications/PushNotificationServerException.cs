using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications
{
	[Serializable]
	public sealed class PushNotificationServerException<T> : LocalizedException where T : PushNotificationFault
	{
		public PushNotificationServerException(T faultContract, Exception error = null) : base(Strings.PushNotificationServerExceptionMessage(faultContract.Message))
		{
			this.FaultContract = faultContract;
			this.Exception = error;
		}

		public PushNotificationServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.FaultContract = (T)((object)info.GetValue("FaultContract", typeof(T)));
			this.Exception = (Exception)info.GetValue("Exception", typeof(Exception));
		}

		public T FaultContract { get; private set; }

		public Exception Exception { get; private set; }

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("FaultContract", this.FaultContract);
			info.AddValue("Exception", this.Exception);
		}
	}
}
