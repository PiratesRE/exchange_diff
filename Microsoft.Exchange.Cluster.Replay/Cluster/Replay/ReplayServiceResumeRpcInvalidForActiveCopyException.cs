using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceResumeRpcInvalidForActiveCopyException : TaskServerException
	{
		public ReplayServiceResumeRpcInvalidForActiveCopyException(string dbCopy) : base(ReplayStrings.ReplayServiceResumeRpcInvalidForActiveCopyException(dbCopy))
		{
			this.dbCopy = dbCopy;
		}

		public ReplayServiceResumeRpcInvalidForActiveCopyException(string dbCopy, Exception innerException) : base(ReplayStrings.ReplayServiceResumeRpcInvalidForActiveCopyException(dbCopy), innerException)
		{
			this.dbCopy = dbCopy;
		}

		protected ReplayServiceResumeRpcInvalidForActiveCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
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
