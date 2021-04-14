using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CiSeederSearchCatalogException : SeedInProgressException
	{
		public CiSeederSearchCatalogException(string sourceServer, Guid database, string specificError) : base(ReplayStrings.CiSeederSearchCatalogException(sourceServer, database, specificError))
		{
			this.sourceServer = sourceServer;
			this.database = database;
			this.specificError = specificError;
		}

		public CiSeederSearchCatalogException(string sourceServer, Guid database, string specificError, Exception innerException) : base(ReplayStrings.CiSeederSearchCatalogException(sourceServer, database, specificError), innerException)
		{
			this.sourceServer = sourceServer;
			this.database = database;
			this.specificError = specificError;
		}

		protected CiSeederSearchCatalogException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sourceServer = (string)info.GetValue("sourceServer", typeof(string));
			this.database = (Guid)info.GetValue("database", typeof(Guid));
			this.specificError = (string)info.GetValue("specificError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sourceServer", this.sourceServer);
			info.AddValue("database", this.database);
			info.AddValue("specificError", this.specificError);
		}

		public string SourceServer
		{
			get
			{
				return this.sourceServer;
			}
		}

		public Guid Database
		{
			get
			{
				return this.database;
			}
		}

		public string SpecificError
		{
			get
			{
				return this.specificError;
			}
		}

		private readonly string sourceServer;

		private readonly Guid database;

		private readonly string specificError;
	}
}
