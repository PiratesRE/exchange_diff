using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADTreeDeleteNotFinishedException : ADTransientException
	{
		public ADTreeDeleteNotFinishedException(string server) : base(DirectoryStrings.ADTreeDeleteNotFinishedException(server))
		{
			this.server = server;
		}

		public ADTreeDeleteNotFinishedException(string server, Exception innerException) : base(DirectoryStrings.ADTreeDeleteNotFinishedException(server), innerException)
		{
			this.server = server;
		}

		protected ADTreeDeleteNotFinishedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("server", this.server);
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string server;
	}
}
