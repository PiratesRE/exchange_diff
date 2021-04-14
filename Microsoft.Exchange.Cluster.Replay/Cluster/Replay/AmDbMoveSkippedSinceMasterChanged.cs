using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMoveSkippedSinceMasterChanged : AmDbActionException
	{
		public AmDbMoveSkippedSinceMasterChanged(string dbName) : base(ReplayStrings.AmDbMoveSkippedSinceMasterChanged(dbName))
		{
			this.dbName = dbName;
		}

		public AmDbMoveSkippedSinceMasterChanged(string dbName, Exception innerException) : base(ReplayStrings.AmDbMoveSkippedSinceMasterChanged(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected AmDbMoveSkippedSinceMasterChanged(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		private readonly string dbName;
	}
}
