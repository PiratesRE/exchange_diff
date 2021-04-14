using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederInstanceReseedBlockedException : SeederServerException
	{
		public SeederInstanceReseedBlockedException(string dbCopyName, string errorMsg) : base(ReplayStrings.SeederInstanceReseedBlockedException(dbCopyName, errorMsg))
		{
			this.dbCopyName = dbCopyName;
			this.errorMsg = errorMsg;
		}

		public SeederInstanceReseedBlockedException(string dbCopyName, string errorMsg, Exception innerException) : base(ReplayStrings.SeederInstanceReseedBlockedException(dbCopyName, errorMsg), innerException)
		{
			this.dbCopyName = dbCopyName;
			this.errorMsg = errorMsg;
		}

		protected SeederInstanceReseedBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopyName = (string)info.GetValue("dbCopyName", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopyName", this.dbCopyName);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string DbCopyName
		{
			get
			{
				return this.dbCopyName;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string dbCopyName;

		private readonly string errorMsg;
	}
}
