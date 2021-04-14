using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncSeedDivergenceCheckFailedException : LocalizedException
	{
		public IncSeedDivergenceCheckFailedException(string dbName, string sourceServer, string error) : base(ReplayStrings.IncSeedDivergenceCheckFailedException(dbName, sourceServer, error))
		{
			this.dbName = dbName;
			this.sourceServer = sourceServer;
			this.error = error;
		}

		public IncSeedDivergenceCheckFailedException(string dbName, string sourceServer, string error, Exception innerException) : base(ReplayStrings.IncSeedDivergenceCheckFailedException(dbName, sourceServer, error), innerException)
		{
			this.dbName = dbName;
			this.sourceServer = sourceServer;
			this.error = error;
		}

		protected IncSeedDivergenceCheckFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.sourceServer = (string)info.GetValue("sourceServer", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("sourceServer", this.sourceServer);
			info.AddValue("error", this.error);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string SourceServer
		{
			get
			{
				return this.sourceServer;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string dbName;

		private readonly string sourceServer;

		private readonly string error;
	}
}
