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
	internal class MigrationUserMovedToAnotherBatchException : MigrationPermanentException
	{
		public MigrationUserMovedToAnotherBatchException(string batchName) : base(Strings.MigrationUserMovedToAnotherBatch(batchName))
		{
			this.batchName = batchName;
		}

		public MigrationUserMovedToAnotherBatchException(string batchName, Exception innerException) : base(Strings.MigrationUserMovedToAnotherBatch(batchName), innerException)
		{
			this.batchName = batchName;
		}

		protected MigrationUserMovedToAnotherBatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.batchName = (string)info.GetValue("batchName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("batchName", this.batchName);
		}

		public string BatchName
		{
			get
			{
				return this.batchName;
			}
		}

		private readonly string batchName;
	}
}
