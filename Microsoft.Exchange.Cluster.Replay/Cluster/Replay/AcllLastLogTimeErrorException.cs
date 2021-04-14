using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllLastLogTimeErrorException : TransientException
	{
		public AcllLastLogTimeErrorException(string dbCopy, string logfilePath, string err) : base(ReplayStrings.AcllLastLogTimeErrorException(dbCopy, logfilePath, err))
		{
			this.dbCopy = dbCopy;
			this.logfilePath = logfilePath;
			this.err = err;
		}

		public AcllLastLogTimeErrorException(string dbCopy, string logfilePath, string err, Exception innerException) : base(ReplayStrings.AcllLastLogTimeErrorException(dbCopy, logfilePath, err), innerException)
		{
			this.dbCopy = dbCopy;
			this.logfilePath = logfilePath;
			this.err = err;
		}

		protected AcllLastLogTimeErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.logfilePath = (string)info.GetValue("logfilePath", typeof(string));
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("logfilePath", this.logfilePath);
			info.AddValue("err", this.err);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string LogfilePath
		{
			get
			{
				return this.logfilePath;
			}
		}

		public string Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string dbCopy;

		private readonly string logfilePath;

		private readonly string err;
	}
}
