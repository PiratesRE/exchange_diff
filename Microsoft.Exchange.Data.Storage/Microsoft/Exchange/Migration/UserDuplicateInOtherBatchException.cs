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
	internal class UserDuplicateInOtherBatchException : MigrationPermanentException
	{
		public UserDuplicateInOtherBatchException(string alias, string batchName) : base(Strings.UserDuplicateInOtherBatch(alias, batchName))
		{
			this.alias = alias;
			this.batchName = batchName;
		}

		public UserDuplicateInOtherBatchException(string alias, string batchName, Exception innerException) : base(Strings.UserDuplicateInOtherBatch(alias, batchName), innerException)
		{
			this.alias = alias;
			this.batchName = batchName;
		}

		protected UserDuplicateInOtherBatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.alias = (string)info.GetValue("alias", typeof(string));
			this.batchName = (string)info.GetValue("batchName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("alias", this.alias);
			info.AddValue("batchName", this.batchName);
		}

		public string Alias
		{
			get
			{
				return this.alias;
			}
		}

		public string BatchName
		{
			get
			{
				return this.batchName;
			}
		}

		private readonly string alias;

		private readonly string batchName;
	}
}
