using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRCROperationOnNonRcrDB : LocalizedException
	{
		public InvalidRCROperationOnNonRcrDB(string dbName) : base(ReplayStrings.InvalidRCROperationOnNonRcrDB(dbName))
		{
			this.dbName = dbName;
		}

		public InvalidRCROperationOnNonRcrDB(string dbName, Exception innerException) : base(ReplayStrings.InvalidRCROperationOnNonRcrDB(dbName), innerException)
		{
			this.dbName = dbName;
		}

		protected InvalidRCROperationOnNonRcrDB(SerializationInfo info, StreamingContext context) : base(info, context)
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
