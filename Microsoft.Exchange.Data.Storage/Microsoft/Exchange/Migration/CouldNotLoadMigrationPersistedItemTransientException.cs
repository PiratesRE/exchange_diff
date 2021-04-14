using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CouldNotLoadMigrationPersistedItemTransientException : MigrationTransientException
	{
		public CouldNotLoadMigrationPersistedItemTransientException(string itemId) : base(Strings.CouldNotLoadMigrationPersistedItem(itemId))
		{
			this.itemId = itemId;
		}

		public CouldNotLoadMigrationPersistedItemTransientException(string itemId, Exception innerException) : base(Strings.CouldNotLoadMigrationPersistedItem(itemId), innerException)
		{
			this.itemId = itemId;
		}

		protected CouldNotLoadMigrationPersistedItemTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.itemId = (string)info.GetValue("itemId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("itemId", this.itemId);
		}

		public string ItemId
		{
			get
			{
				return this.itemId;
			}
		}

		private readonly string itemId;
	}
}
