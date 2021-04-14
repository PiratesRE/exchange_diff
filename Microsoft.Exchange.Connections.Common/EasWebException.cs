using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EasWebException : ConnectionsTransientException
	{
		public EasWebException(string msg) : base(CXStrings.EasWebExceptionMsg(msg))
		{
			this.msg = msg;
		}

		public EasWebException(string msg, Exception innerException) : base(CXStrings.EasWebExceptionMsg(msg), innerException)
		{
			this.msg = msg;
		}

		protected EasWebException(SerializationInfo info, StreamingContext context) : base(info, context)
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
