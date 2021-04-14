using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnExpectedPageSizeException : TransientException
	{
		public UnExpectedPageSizeException(string db, long pageSize) : base(ReplayStrings.UnExpectedPageSize(db, pageSize))
		{
			this.db = db;
			this.pageSize = pageSize;
		}

		public UnExpectedPageSizeException(string db, long pageSize, Exception innerException) : base(ReplayStrings.UnExpectedPageSize(db, pageSize), innerException)
		{
			this.db = db;
			this.pageSize = pageSize;
		}

		protected UnExpectedPageSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (string)info.GetValue("db", typeof(string));
			this.pageSize = (long)info.GetValue("pageSize", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
			info.AddValue("pageSize", this.pageSize);
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		public long PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		private readonly string db;

		private readonly long pageSize;
	}
}
