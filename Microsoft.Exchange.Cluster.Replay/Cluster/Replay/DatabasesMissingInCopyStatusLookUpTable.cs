using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabasesMissingInCopyStatusLookUpTable : DatabaseVolumeInfoException
	{
		public DatabasesMissingInCopyStatusLookUpTable(string databaseNames) : base(ReplayStrings.DatabasesMissingInCopyStatusLookUpTable(databaseNames))
		{
			this.databaseNames = databaseNames;
		}

		public DatabasesMissingInCopyStatusLookUpTable(string databaseNames, Exception innerException) : base(ReplayStrings.DatabasesMissingInCopyStatusLookUpTable(databaseNames), innerException)
		{
			this.databaseNames = databaseNames;
		}

		protected DatabasesMissingInCopyStatusLookUpTable(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseNames = (string)info.GetValue("databaseNames", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseNames", this.databaseNames);
		}

		public string DatabaseNames
		{
			get
			{
				return this.databaseNames;
			}
		}

		private readonly string databaseNames;
	}
}
