using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederInstanceNotFoundException : SeederServerException
	{
		public SeederInstanceNotFoundException(string dbGuid) : base(ReplayStrings.SeederInstanceNotFoundException(dbGuid))
		{
			this.dbGuid = dbGuid;
		}

		public SeederInstanceNotFoundException(string dbGuid, Exception innerException) : base(ReplayStrings.SeederInstanceNotFoundException(dbGuid), innerException)
		{
			this.dbGuid = dbGuid;
		}

		protected SeederInstanceNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
