using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementRollbackFailedException : LastLogReplacementException
	{
		public LastLogReplacementRollbackFailedException(string dbCopy, string error) : base(ReplayStrings.LastLogReplacementRollbackFailedException(dbCopy, error))
		{
			this.dbCopy = dbCopy;
			this.error = error;
		}

		public LastLogReplacementRollbackFailedException(string dbCopy, string error, Exception innerException) : base(ReplayStrings.LastLogReplacementRollbackFailedException(dbCopy, error), innerException)
		{
			this.dbCopy = dbCopy;
			this.error = error;
		}

		protected LastLogReplacementRollbackFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("error", this.error);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string dbCopy;

		private readonly string error;
	}
}
