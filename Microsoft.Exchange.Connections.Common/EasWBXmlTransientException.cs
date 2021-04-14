using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EasWBXmlTransientException : ConnectionsTransientException
	{
		public EasWBXmlTransientException(string msg) : base(CXStrings.EasWBXmlExceptionMsg(msg))
		{
			this.msg = msg;
		}

		public EasWBXmlTransientException(string msg, Exception innerException) : base(CXStrings.EasWBXmlExceptionMsg(msg), innerException)
		{
			this.msg = msg;
		}

		protected EasWBXmlTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
