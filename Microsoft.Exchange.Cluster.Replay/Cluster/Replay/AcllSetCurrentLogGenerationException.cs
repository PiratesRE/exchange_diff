using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllSetCurrentLogGenerationException : TransientException
	{
		public AcllSetCurrentLogGenerationException(string dbCopy, string e00logPath, string err) : base(ReplayStrings.AcllSetCurrentLogGenerationException(dbCopy, e00logPath, err))
		{
			this.dbCopy = dbCopy;
			this.e00logPath = e00logPath;
			this.err = err;
		}

		public AcllSetCurrentLogGenerationException(string dbCopy, string e00logPath, string err, Exception innerException) : base(ReplayStrings.AcllSetCurrentLogGenerationException(dbCopy, e00logPath, err), innerException)
		{
			this.dbCopy = dbCopy;
			this.e00logPath = e00logPath;
			this.err = err;
		}

		protected AcllSetCurrentLogGenerationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.e00logPath = (string)info.GetValue("e00logPath", typeof(string));
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("e00logPath", this.e00logPath);
			info.AddValue("err", this.err);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string E00logPath
		{
			get
			{
				return this.e00logPath;
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

		private readonly string e00logPath;

		private readonly string err;
	}
}
