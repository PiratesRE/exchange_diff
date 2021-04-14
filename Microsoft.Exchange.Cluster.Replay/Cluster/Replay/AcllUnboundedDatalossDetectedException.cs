using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllUnboundedDatalossDetectedException : TransientException
	{
		public AcllUnboundedDatalossDetectedException(string dbName, string lastUpdatedTimeStr, string allowedDurationStr, string actualDurationStr) : base(ReplayStrings.AcllUnboundedDatalossDetectedException(dbName, lastUpdatedTimeStr, allowedDurationStr, actualDurationStr))
		{
			this.dbName = dbName;
			this.lastUpdatedTimeStr = lastUpdatedTimeStr;
			this.allowedDurationStr = allowedDurationStr;
			this.actualDurationStr = actualDurationStr;
		}

		public AcllUnboundedDatalossDetectedException(string dbName, string lastUpdatedTimeStr, string allowedDurationStr, string actualDurationStr, Exception innerException) : base(ReplayStrings.AcllUnboundedDatalossDetectedException(dbName, lastUpdatedTimeStr, allowedDurationStr, actualDurationStr), innerException)
		{
			this.dbName = dbName;
			this.lastUpdatedTimeStr = lastUpdatedTimeStr;
			this.allowedDurationStr = allowedDurationStr;
			this.actualDurationStr = actualDurationStr;
		}

		protected AcllUnboundedDatalossDetectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.lastUpdatedTimeStr = (string)info.GetValue("lastUpdatedTimeStr", typeof(string));
			this.allowedDurationStr = (string)info.GetValue("allowedDurationStr", typeof(string));
			this.actualDurationStr = (string)info.GetValue("actualDurationStr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("lastUpdatedTimeStr", this.lastUpdatedTimeStr);
			info.AddValue("allowedDurationStr", this.allowedDurationStr);
			info.AddValue("actualDurationStr", this.actualDurationStr);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string LastUpdatedTimeStr
		{
			get
			{
				return this.lastUpdatedTimeStr;
			}
		}

		public string AllowedDurationStr
		{
			get
			{
				return this.allowedDurationStr;
			}
		}

		public string ActualDurationStr
		{
			get
			{
				return this.actualDurationStr;
			}
		}

		private readonly string dbName;

		private readonly string lastUpdatedTimeStr;

		private readonly string allowedDurationStr;

		private readonly string actualDurationStr;
	}
}
