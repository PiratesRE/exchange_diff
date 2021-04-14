using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RedundantAccountSubscriptionException : LocalizedException
	{
		public RedundantAccountSubscriptionException(string username, string server) : base(Strings.RedundantAccountSubscription(username, server))
		{
			this.username = username;
			this.server = server;
		}

		public RedundantAccountSubscriptionException(string username, string server, Exception innerException) : base(Strings.RedundantAccountSubscription(username, server), innerException)
		{
			this.username = username;
			this.server = server;
		}

		protected RedundantAccountSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.username = (string)info.GetValue("username", typeof(string));
			this.server = (string)info.GetValue("server", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("username", this.username);
			info.AddValue("server", this.server);
		}

		public string Username
		{
			get
			{
				return this.username;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		private readonly string username;

		private readonly string server;
	}
}
