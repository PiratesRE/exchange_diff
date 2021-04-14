using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindServerObject : TransientException
	{
		public CouldNotFindServerObject(string serverName) : base(Strings.CouldNotFindServerObject(serverName))
		{
			this.serverName = serverName;
		}

		public CouldNotFindServerObject(string serverName, Exception innerException) : base(Strings.CouldNotFindServerObject(serverName), innerException)
		{
			this.serverName = serverName;
		}

		protected CouldNotFindServerObject(SerializationInfo info, StreamingContext context) : base(info, context)
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
