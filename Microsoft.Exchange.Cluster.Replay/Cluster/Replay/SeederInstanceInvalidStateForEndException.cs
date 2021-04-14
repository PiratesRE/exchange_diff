using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederInstanceInvalidStateForEndException : SeederServerException
	{
		public SeederInstanceInvalidStateForEndException(string dbGuid) : base(ReplayStrings.SeederInstanceInvalidStateForEndException(dbGuid))
		{
			this.dbGuid = dbGuid;
		}

		public SeederInstanceInvalidStateForEndException(string dbGuid, Exception innerException) : base(ReplayStrings.SeederInstanceInvalidStateForEndException(dbGuid), innerException)
		{
			this.dbGuid = dbGuid;
		}

		protected SeederInstanceInvalidStateForEndException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbGuid = (string)info.GetValue("dbGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbGuid", this.dbGuid);
		}

		public string DbGuid
		{
			get
			{
				return this.dbGuid;
			}
		}

		private readonly string dbGuid;
	}
}
