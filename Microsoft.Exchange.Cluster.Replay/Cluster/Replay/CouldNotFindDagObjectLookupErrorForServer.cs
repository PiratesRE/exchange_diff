using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindDagObjectLookupErrorForServer : TransientException
	{
		public CouldNotFindDagObjectLookupErrorForServer(string serverName, string error) : base(ReplayStrings.CouldNotFindDagObjectLookupErrorForServer(serverName, error))
		{
			this.serverName = serverName;
			this.error = error;
		}

		public CouldNotFindDagObjectLookupErrorForServer(string serverName, string error, Exception innerException) : base(ReplayStrings.CouldNotFindDagObjectLookupErrorForServer(serverName, error), innerException)
		{
			this.serverName = serverName;
			this.error = error;
		}

		protected CouldNotFindDagObjectLookupErrorForServer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serverName = (string)info.GetValue("serverName", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serverName", this.serverName);
			info.AddValue("error", this.error);
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string serverName;

		private readonly string error;
	}
}
