using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AlreadyConnectedToSMTPServerException : LocalizedException
	{
		public AlreadyConnectedToSMTPServerException(string server) : base(NetException.AlreadyConnectedToSMTPServerException(server))
		{
			this.server = server;
		}

		public AlreadyConnectedToSMTPServerException(string server, Exception innerException) : base(NetException.AlreadyConnectedToSMTPServerException(server), innerException)
		{
			this.server = server;
		}

		protected AlreadyConnectedToSMTPServerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
