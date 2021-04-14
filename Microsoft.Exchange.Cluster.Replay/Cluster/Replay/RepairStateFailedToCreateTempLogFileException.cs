using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RepairStateFailedToCreateTempLogFileException : RepairStateException
	{
		public RepairStateFailedToCreateTempLogFileException(string dbName, string errorMsg) : base(ReplayStrings.RepairStateFailedToCreateTempLogFile(dbName, errorMsg))
		{
			this.dbName = dbName;
			this.errorMsg = errorMsg;
		}

		public RepairStateFailedToCreateTempLogFileException(string dbName, string errorMsg, Exception innerException) : base(ReplayStrings.RepairStateFailedToCreateTempLogFile(dbName, errorMsg), innerException)
		{
			this.dbName = dbName;
			this.errorMsg = errorMsg;
		}

		protected RepairStateFailedToCreateTempLogFileException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string dbName;

		private readonly string errorMsg;
	}
}
