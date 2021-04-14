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
	internal class MigrationJobCannotBeDeletedWithPendingItemsException : MigrationPermanentException
	{
		public MigrationJobCannotBeDeletedWithPendingItemsException(int count) : base(Strings.MigrationJobCannotBeDeletedWithPendingItems(count))
		{
			this.count = count;
		}

		public MigrationJobCannotBeDeletedWithPendingItemsException(int count, Exception innerException) : base(Strings.MigrationJobCannotBeDeletedWithPendingItems(count), innerException)
		{
			this.count = count;
		}

		protected MigrationJobCannotBeDeletedWithPendingItemsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.count = (int)info.GetValue("count", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("count", this.count);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private readonly int count;
	}
}
