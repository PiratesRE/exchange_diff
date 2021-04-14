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
	internal class CouldNotCreateCredentialsPermanentException : MigrationPermanentException
	{
		public CouldNotCreateCredentialsPermanentException(string username) : base(Strings.ErrorCouldNotCreateCredentials(username))
		{
			this.username = username;
		}

		public CouldNotCreateCredentialsPermanentException(string username, Exception innerException) : base(Strings.ErrorCouldNotCreateCredentials(username), innerException)
		{
			this.username = username;
		}

		protected CouldNotCreateCredentialsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
