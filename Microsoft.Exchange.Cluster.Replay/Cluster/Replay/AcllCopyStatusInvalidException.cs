using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllCopyStatusInvalidException : TransientException
	{
		public AcllCopyStatusInvalidException(string dbCopy, string status) : base(ReplayStrings.AcllCopyStatusInvalidException(dbCopy, status))
		{
			this.dbCopy = dbCopy;
			this.status = status;
		}

		public AcllCopyStatusInvalidException(string dbCopy, string status, Exception innerException) : base(ReplayStrings.AcllCopyStatusInvalidException(dbCopy, status), innerException)
		{
			this.dbCopy = dbCopy;
			this.status = status;
		}

		protected AcllCopyStatusInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.status = (string)info.GetValue("status", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("status", this.status);
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

		private readonly string dbCopy;

		private readonly string status;
	}
}
