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
	internal class UserDoesNotHaveRBACException : MigrationPermanentException
	{
		public UserDoesNotHaveRBACException(string username) : base(Strings.ErrorUserMissingOrWithoutRBAC(username))
		{
			this.username = username;
		}

		public UserDoesNotHaveRBACException(string username, Exception innerException) : base(Strings.ErrorUserMissingOrWithoutRBAC(username), innerException)
		{
			this.username = username;
		}

		protected UserDoesNotHaveRBACException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.username = (string)info.GetValue("username", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("username", this.username);
		}

		public string Username
		{
			get
			{
				return this.username;
			}
		}

		private readonly string username;
	}
}
