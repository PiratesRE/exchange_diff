using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllTempLogCreationFailedException : TransientException
	{
		public AcllTempLogCreationFailedException(string dbCopy, string err) : base(ReplayStrings.AcllTempLogCreationFailedException(dbCopy, err))
		{
			this.dbCopy = dbCopy;
			this.err = err;
		}

		public AcllTempLogCreationFailedException(string dbCopy, string err, Exception innerException) : base(ReplayStrings.AcllTempLogCreationFailedException(dbCopy, err), innerException)
		{
			this.dbCopy = dbCopy;
			this.err = err;
		}

		protected AcllTempLogCreationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("err", this.err);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
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

		private readonly string err;
	}
}
