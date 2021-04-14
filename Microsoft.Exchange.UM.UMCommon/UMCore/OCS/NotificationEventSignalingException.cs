using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotificationEventSignalingException : NotificationEventException
	{
		public NotificationEventSignalingException(string msg) : base(Strings.NotificationEventSignalingException(msg))
		{
			this.msg = msg;
		}

		public NotificationEventSignalingException(string msg, Exception innerException) : base(Strings.NotificationEventSignalingException(msg), innerException)
		{
			this.msg = msg;
		}

		protected NotificationEventSignalingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}
