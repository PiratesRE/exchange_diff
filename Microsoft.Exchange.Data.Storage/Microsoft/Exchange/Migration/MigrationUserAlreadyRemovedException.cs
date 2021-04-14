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
	internal class MigrationUserAlreadyRemovedException : MigrationPermanentException
	{
		public MigrationUserAlreadyRemovedException(string user) : base(Strings.MigrationUserAlreadyRemoved(user))
		{
			this.user = user;
		}

		public MigrationUserAlreadyRemovedException(string user, Exception innerException) : base(Strings.MigrationUserAlreadyRemoved(user), innerException)
		{
			this.user = user;
		}

		protected MigrationUserAlreadyRemovedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string user;
	}
}
