using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationInvalidStatusException : MigrationTransientException
	{
		public MigrationInvalidStatusException(string statusType, string status) : base(ServerStrings.MigrationInvalidStatus(statusType, status))
		{
			this.statusType = statusType;
			this.status = status;
		}

		public MigrationInvalidStatusException(string statusType, string status, Exception innerException) : base(ServerStrings.MigrationInvalidStatus(statusType, status), innerException)
		{
			this.statusType = statusType;
			this.status = status;
		}

		protected MigrationInvalidStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.statusType = (string)info.GetValue("statusType", typeof(string));
			this.status = (string)info.GetValue("status", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("statusType", this.statusType);
			info.AddValue("status", this.status);
		}

		public string StatusType
		{
			get
			{
				return this.statusType;
			}
		}

		public string Status
		{
			get
			{
				return this.status;
			}
		}

		private readonly string statusType;

		private readonly string status;
	}
}
