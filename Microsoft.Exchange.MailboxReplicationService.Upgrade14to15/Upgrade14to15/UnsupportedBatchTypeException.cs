using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnsupportedBatchTypeException : MigrationTransientException
	{
		public UnsupportedBatchTypeException(string batchType) : base(UpgradeHandlerStrings.UnsupportedBatchType(batchType))
		{
			this.batchType = batchType;
		}

		public UnsupportedBatchTypeException(string batchType, Exception innerException) : base(UpgradeHandlerStrings.UnsupportedBatchType(batchType), innerException)
		{
			this.batchType = batchType;
		}

		protected UnsupportedBatchTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.batchType = (string)info.GetValue("batchType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("batchType", this.batchType);
		}

		public string BatchType
		{
			get
			{
				return this.batchType;
			}
		}

		private readonly string batchType;
	}
}
