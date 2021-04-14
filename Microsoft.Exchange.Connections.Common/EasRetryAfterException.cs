using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EasRetryAfterException : ConnectionsTransientException
	{
		public EasRetryAfterException(TimeSpan delay, string msg) : base(CXStrings.EasRetryAfterExceptionMsg(delay, msg))
		{
			this.delay = delay;
			this.msg = msg;
		}

		public EasRetryAfterException(TimeSpan delay, string msg, Exception innerException) : base(CXStrings.EasRetryAfterExceptionMsg(delay, msg), innerException)
		{
			this.delay = delay;
			this.msg = msg;
		}

		protected EasRetryAfterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.delay = (TimeSpan)info.GetValue("delay", typeof(TimeSpan));
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("delay", this.delay);
			info.AddValue("msg", this.msg);
		}

		public TimeSpan Delay
		{
			get
			{
				return this.delay;
			}
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly TimeSpan delay;

		private readonly string msg;
	}
}
