using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RepairStateFailedPendingPagePatchException : RepairStateException
	{
		public RepairStateFailedPendingPagePatchException(string dbName, string errorMsg) : base(ReplayStrings.RepairStateFailedPendingPagePatchException(dbName, errorMsg))
		{
			this.dbName = dbName;
			this.errorMsg = errorMsg;
		}

		public RepairStateFailedPendingPagePatchException(string dbName, string errorMsg, Exception innerException) : base(ReplayStrings.RepairStateFailedPendingPagePatchException(dbName, errorMsg), innerException)
		{
			this.dbName = dbName;
			this.errorMsg = errorMsg;
		}

		protected RepairStateFailedPendingPagePatchException(SerializationInfo info, StreamingContext context) : base(info, context)
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
