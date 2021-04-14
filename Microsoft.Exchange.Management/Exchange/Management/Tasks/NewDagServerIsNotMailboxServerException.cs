using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NewDagServerIsNotMailboxServerException : LocalizedException
	{
		public NewDagServerIsNotMailboxServerException(string serverName) : base(Strings.NewDagServerIsNotMailboxServerException(serverName))
		{
			this.serverName = serverName;
		}

		public NewDagServerIsNotMailboxServerException(string serverName, Exception innerException) : base(Strings.NewDagServerIsNotMailboxServerException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected NewDagServerIsNotMailboxServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		private readonly string serverName;
	}
}
