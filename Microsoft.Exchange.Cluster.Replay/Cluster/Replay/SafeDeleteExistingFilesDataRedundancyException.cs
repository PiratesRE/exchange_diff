using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SafeDeleteExistingFilesDataRedundancyException : SeedPrepareException
	{
		public SafeDeleteExistingFilesDataRedundancyException(string db, string errMsg2) : base(ReplayStrings.SafeDeleteExistingFilesDataRedundancyException(db, errMsg2))
		{
			this.db = db;
			this.errMsg2 = errMsg2;
		}

		public SafeDeleteExistingFilesDataRedundancyException(string db, string errMsg2, Exception innerException) : base(ReplayStrings.SafeDeleteExistingFilesDataRedundancyException(db, errMsg2), innerException)
		{
			this.db = db;
			this.errMsg2 = errMsg2;
		}

		protected SafeDeleteExistingFilesDataRedundancyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (string)info.GetValue("db", typeof(string));
			this.errMsg2 = (string)info.GetValue("errMsg2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
			info.AddValue("errMsg2", this.errMsg2);
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		public string ErrMsg2
		{
			get
			{
				return this.errMsg2;
			}
		}

		private readonly string db;

		private readonly string errMsg2;
	}
}
