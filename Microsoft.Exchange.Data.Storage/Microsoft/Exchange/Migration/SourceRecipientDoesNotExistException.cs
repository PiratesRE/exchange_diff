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
	internal class SourceRecipientDoesNotExistException : MigrationPermanentException
	{
		public SourceRecipientDoesNotExistException(string email) : base(Strings.RecipientDoesNotExistAtSource(email))
		{
			this.email = email;
		}

		public SourceRecipientDoesNotExistException(string email, Exception innerException) : base(Strings.RecipientDoesNotExistAtSource(email), innerException)
		{
			this.email = email;
		}

		protected SourceRecipientDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
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
