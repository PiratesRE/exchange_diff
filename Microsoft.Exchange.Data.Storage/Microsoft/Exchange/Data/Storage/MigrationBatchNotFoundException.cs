using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationBatchNotFoundException : ObjectNotFoundException
	{
		public MigrationBatchNotFoundException(string batchName) : base(ServerStrings.MigrationBatchNotFoundError(batchName))
		{
			this.batchName = batchName;
		}

		public MigrationBatchNotFoundException(string batchName, Exception innerException) : base(ServerStrings.MigrationBatchNotFoundError(batchName), innerException)
		{
			this.batchName = batchName;
		}

		protected MigrationBatchNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
