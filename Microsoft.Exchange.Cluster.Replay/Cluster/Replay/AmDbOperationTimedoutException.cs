using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbOperationTimedoutException : AmDbOperationException
	{
		public AmDbOperationTimedoutException(string dbName, string opr, TimeSpan timeout) : base(ReplayStrings.AmDbOperationTimedoutException(dbName, opr, timeout))
		{
			this.dbName = dbName;
			this.opr = opr;
			this.timeout = timeout;
		}

		public AmDbOperationTimedoutException(string dbName, string opr, TimeSpan timeout, Exception innerException) : base(ReplayStrings.AmDbOperationTimedoutException(dbName, opr, timeout), innerException)
		{
			this.dbName = dbName;
			this.opr = opr;
			this.timeout = timeout;
		}

		protected AmDbOperationTimedoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.opr = (string)info.GetValue("opr", typeof(string));
			this.timeout = (TimeSpan)info.GetValue("timeout", typeof(TimeSpan));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("opr", this.opr);
			info.AddValue("timeout", this.timeout);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string Opr
		{
			get
			{
				return this.opr;
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		private readonly string dbName;

		private readonly string opr;

		private readonly TimeSpan timeout;
	}
}
