using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindExchangeServerDirectoryEntryException : LocalizedException
	{
		public CouldNotFindExchangeServerDirectoryEntryException(string server) : base(Strings.CouldNotFindExchangeServerDirectoryEntryException(server))
		{
			this.server = server;
		}

		public CouldNotFindExchangeServerDirectoryEntryException(string server, Exception innerException) : base(Strings.CouldNotFindExchangeServerDirectoryEntryException(server), innerException)
		{
			this.server = server;
		}

		protected CouldNotFindExchangeServerDirectoryEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
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
