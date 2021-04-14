using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbMoveOperationNotSupportedStandaloneException : AmDbActionException
	{
		public AmDbMoveOperationNotSupportedStandaloneException(string dbName) : base(ReplayStrings.AmDbMoveOperationNotSupportedStandaloneException(dbName))
		{
			this.dbName = dbName;
		}

		public AmDbMoveOperationNotSupportedStandaloneException(string dbName, Exception innerException) : base(ReplayStrings.AmDbMoveOperationNotSupportedStandaloneException(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected AmDbMoveOperationNotSupportedStandaloneException(SerializationInfo info, StreamingContext context) : base(info, context)
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
