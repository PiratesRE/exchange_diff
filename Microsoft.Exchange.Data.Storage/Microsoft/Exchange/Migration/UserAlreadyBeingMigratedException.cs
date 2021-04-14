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
	internal class UserAlreadyBeingMigratedException : MigrationPermanentException
	{
		public UserAlreadyBeingMigratedException(string identity) : base(Strings.ErrorUserAlreadyBeingMigrated(identity))
		{
			this.identity = identity;
		}

		public UserAlreadyBeingMigratedException(string identity, Exception innerException) : base(Strings.ErrorUserAlreadyBeingMigrated(identity), innerException)
		{
			this.identity = identity;
		}

		protected UserAlreadyBeingMigratedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
