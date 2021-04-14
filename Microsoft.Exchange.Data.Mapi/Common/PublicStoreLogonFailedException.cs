using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PublicStoreLogonFailedException : MapiLogonFailedException
	{
		public PublicStoreLogonFailedException(string server) : base(Strings.PublicStoreLogonFailedExceptionError(server))
		{
			this.server = server;
		}

		public PublicStoreLogonFailedException(string server, Exception innerException) : base(Strings.PublicStoreLogonFailedExceptionError(server), innerException)
		{
			this.server = server;
		}

		protected PublicStoreLogonFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
