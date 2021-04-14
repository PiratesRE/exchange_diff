using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SafeDeleteExistingFilesDataRedundancyNoResultException : SeedPrepareException
	{
		public SafeDeleteExistingFilesDataRedundancyNoResultException(string db) : base(ReplayStrings.SafeDeleteExistingFilesDataRedundancyNoResultException(db))
		{
			this.db = db;
		}

		public SafeDeleteExistingFilesDataRedundancyNoResultException(string db, Exception innerException) : base(ReplayStrings.SafeDeleteExistingFilesDataRedundancyNoResultException(db), innerException)
		{
			this.db = db;
		}

		protected SafeDeleteExistingFilesDataRedundancyNoResultException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.db = (string)info.GetValue("db", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("db", this.db);
		}

		public string Db
		{
			get
			{
				return this.db;
			}
		}

		private readonly string db;
	}
}
