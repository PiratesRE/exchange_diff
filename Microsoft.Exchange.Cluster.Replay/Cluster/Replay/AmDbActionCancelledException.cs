using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionCancelledException : AmDbActionException
	{
		public AmDbActionCancelledException(string dbName, string opr) : base(ReplayStrings.AmDbActionCancelledException(dbName, opr))
		{
			this.dbName = dbName;
			this.opr = opr;
		}

		public AmDbActionCancelledException(string dbName, string opr, Exception innerException) : base(ReplayStrings.AmDbActionCancelledException(dbName, opr), innerException)
		{
			this.dbName = dbName;
			this.opr = opr;
		}

		protected AmDbActionCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.opr = (string)info.GetValue("opr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("opr", this.opr);
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

		private readonly string dbName;

		private readonly string opr;
	}
}
