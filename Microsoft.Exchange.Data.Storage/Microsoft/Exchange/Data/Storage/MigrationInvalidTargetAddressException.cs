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
	internal class MigrationInvalidTargetAddressException : MigrationPermanentException
	{
		public MigrationInvalidTargetAddressException(string email) : base(ServerStrings.MigrationInvalidTargetAddress(email))
		{
			this.email = email;
		}

		public MigrationInvalidTargetAddressException(string email, Exception innerException) : base(ServerStrings.MigrationInvalidTargetAddress(email), innerException)
		{
			this.email = email;
		}

		protected MigrationInvalidTargetAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.email = (string)info.GetValue("email", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("email", this.email);
		}

		public string Email
		{
			get
			{
				return this.email;
			}
		}

		private readonly string email;
	}
}
