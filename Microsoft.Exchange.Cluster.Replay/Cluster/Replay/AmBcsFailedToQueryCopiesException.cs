using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmBcsFailedToQueryCopiesException : AmBcsSelectionException
	{
		public AmBcsFailedToQueryCopiesException(string dbName, string queryError) : base(ReplayStrings.AmBcsFailedToQueryCopiesException(dbName, queryError))
		{
			this.dbName = dbName;
			this.queryError = queryError;
		}

		public AmBcsFailedToQueryCopiesException(string dbName, string queryError, Exception innerException) : base(ReplayStrings.AmBcsFailedToQueryCopiesException(dbName, queryError), innerException)
		{
			this.dbName = dbName;
			this.queryError = queryError;
		}

		protected AmBcsFailedToQueryCopiesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.queryError = (string)info.GetValue("queryError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("queryError", this.queryError);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string QueryError
		{
			get
			{
				return this.queryError;
			}
		}

		private readonly string dbName;

		private readonly string queryError;
	}
}
