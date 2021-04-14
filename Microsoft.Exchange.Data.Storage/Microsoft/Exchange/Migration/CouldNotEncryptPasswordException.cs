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
	internal class CouldNotEncryptPasswordException : MigrationPermanentException
	{
		public CouldNotEncryptPasswordException(string username) : base(Strings.ErrorCouldNotEncryptPassword(username))
		{
			this.username = username;
		}

		public CouldNotEncryptPasswordException(string username, Exception innerException) : base(Strings.ErrorCouldNotEncryptPassword(username), innerException)
		{
			this.username = username;
		}

		protected CouldNotEncryptPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
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
