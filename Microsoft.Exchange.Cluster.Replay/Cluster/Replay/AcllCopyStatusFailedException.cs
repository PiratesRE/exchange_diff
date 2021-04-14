using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllCopyStatusFailedException : TransientException
	{
		public AcllCopyStatusFailedException(string dbCopy, string status, string errorMsg) : base(ReplayStrings.AcllCopyStatusFailedException(dbCopy, status, errorMsg))
		{
			this.dbCopy = dbCopy;
			this.status = status;
			this.errorMsg = errorMsg;
		}

		public AcllCopyStatusFailedException(string dbCopy, string status, string errorMsg, Exception innerException) : base(ReplayStrings.AcllCopyStatusFailedException(dbCopy, status, errorMsg), innerException)
		{
			this.dbCopy = dbCopy;
			this.status = status;
			this.errorMsg = errorMsg;
		}

		protected AcllCopyStatusFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.status = (string)info.GetValue("status", typeof(string));
			this.errorMsg = (string)info.GetValue("errorMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("status", this.status);
			info.AddValue("errorMsg", this.errorMsg);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string Status
		{
			get
			{
				return this.status;
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

		private readonly string status;

		private readonly string errorMsg;
	}
}
