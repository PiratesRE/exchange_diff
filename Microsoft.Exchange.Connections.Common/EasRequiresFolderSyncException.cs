using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EasRequiresFolderSyncException : ConnectionsPermanentException
	{
		public EasRequiresFolderSyncException(string msg) : base(CXStrings.EasRequiresFolderSyncExceptionMsg(msg))
		{
			this.msg = msg;
		}

		public EasRequiresFolderSyncException(string msg, Exception innerException) : base(CXStrings.EasRequiresFolderSyncExceptionMsg(msg), innerException)
		{
			this.msg = msg;
		}

		protected EasRequiresFolderSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
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
