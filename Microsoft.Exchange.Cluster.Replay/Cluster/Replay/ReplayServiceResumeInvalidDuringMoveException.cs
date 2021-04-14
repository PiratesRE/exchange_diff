using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceResumeInvalidDuringMoveException : TaskServerException
	{
		public ReplayServiceResumeInvalidDuringMoveException(string dbCopy) : base(ReplayStrings.ReplayServiceResumeInvalidDuringMoveException(dbCopy))
		{
			this.dbCopy = dbCopy;
		}

		public ReplayServiceResumeInvalidDuringMoveException(string dbCopy, Exception innerException) : base(ReplayStrings.ReplayServiceResumeInvalidDuringMoveException(dbCopy), innerException)
		{
			this.dbCopy = dbCopy;
		}

		protected ReplayServiceResumeInvalidDuringMoveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		private readonly string dbCopy;
	}
}
