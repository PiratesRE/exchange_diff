using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllCopyStatusResumeBlockedException : TransientException
	{
		public AcllCopyStatusResumeBlockedException(string dbCopy, string errorMsg) : base(ReplayStrings.AcllCopyStatusResumeBlockedException(dbCopy, errorMsg))
		{
			this.dbCopy = dbCopy;
			this.errorMsg = errorMsg;
		}

		public AcllCopyStatusResumeBlockedException(string dbCopy, string errorMsg, Exception innerException) : base(ReplayStrings.AcllCopyStatusResumeBlockedException(dbCopy, errorMsg), innerException)
		{
			this.dbCopy = dbCopy;
			this.errorMsg = errorMsg;
		}

		protected AcllCopyStatusResumeBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string ErrorMsg
		{
			get
			{
				return this.errorMsg;
			}
		}

		private readonly string dbCopy;

		private readonly string errorMsg;
	}
}
