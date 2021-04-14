using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToFindServerException : TransientException
	{
		public FailedToFindServerException(string serverName) : base(ReplayStrings.FailedToFindLocalServerException(serverName))
		{
			this.serverName = serverName;
		}

		public FailedToFindServerException(string serverName, Exception innerException) : base(ReplayStrings.FailedToFindLocalServerException(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected FailedToFindServerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
