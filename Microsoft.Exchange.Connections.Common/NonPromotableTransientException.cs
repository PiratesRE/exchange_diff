using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NonPromotableTransientException : ConnectionsTransientException
	{
		public NonPromotableTransientException(string msg) : base(CXStrings.NonPromotableTransientExceptionMsg(msg))
		{
			this.msg = msg;
		}

		public NonPromotableTransientException(string msg, Exception innerException) : base(CXStrings.NonPromotableTransientExceptionMsg(msg), innerException)
		{
			this.msg = msg;
		}

		protected NonPromotableTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
