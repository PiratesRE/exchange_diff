using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Search.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IndexStatusNotFoundException : IndexStatusException
	{
		public IndexStatusNotFoundException(string database) : base(Strings.IndexStatusNotFound(database))
		{
			this.database = database;
		}

		public IndexStatusNotFoundException(string database, Exception innerException) : base(Strings.IndexStatusNotFound(database), innerException)
		{
			this.database = database;
		}

		protected IndexStatusNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		private readonly string database;
	}
}
