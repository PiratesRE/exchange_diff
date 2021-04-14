using System;

namespace Microsoft.Exchange.Data.Mapi
{
	[Serializable]
	public sealed class DatabaseId : MapiObjectId
	{
		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.databaseName))
			{
				return this.Guid.ToString();
			}
			return this.databaseName;
		}

		public DatabaseId()
		{
		}

		public DatabaseId(byte[] bytes) : base(bytes)
		{
			this.guid = new Guid(bytes);
		}

		public DatabaseId(Guid guid) : base(new MapiEntryId(guid.ToByteArray()))
		{
			this.guid = guid;
		}

		public DatabaseId(string serverName, string dbName)
		{
			this.serverName = serverName;
			this.databaseName = dbName;
		}

		internal DatabaseId(MapiEntryId entryId, string serverName, string dbName, Guid guid) : base(entryId)
		{
			this.serverName = serverName;
			this.databaseName = dbName;
			this.guid = guid;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.guid;
			}
		}

		private readonly Guid guid;

		private readonly string serverName;

		private readonly string databaseName;
	}
}
