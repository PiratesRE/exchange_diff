using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabaseNotFoundException : ObjectNotFoundException
	{
		public DatabaseNotFoundException(string databaseId) : base(ServerStrings.DatabaseNotFound(databaseId))
		{
			this.databaseId = databaseId;
		}

		public DatabaseNotFoundException(string databaseId, Exception innerException) : base(ServerStrings.DatabaseNotFound(databaseId), innerException)
		{
			this.databaseId = databaseId;
		}

		protected DatabaseNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.databaseId = (string)info.GetValue("databaseId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("databaseId", this.databaseId);
		}

		public string DatabaseId
		{
			get
			{
				return this.databaseId;
			}
		}

		private readonly string databaseId;
	}
}
