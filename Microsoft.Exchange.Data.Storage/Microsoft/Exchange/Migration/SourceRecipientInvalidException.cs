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
	internal class SourceRecipientInvalidException : MigrationPermanentException
	{
		public SourceRecipientInvalidException(string email) : base(Strings.RecipientInfoInvalidAtSource(email))
		{
			this.email = email;
		}

		public SourceRecipientInvalidException(string email, Exception innerException) : base(Strings.RecipientInfoInvalidAtSource(email), innerException)
		{
			this.email = email;
		}

		protected SourceRecipientInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
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
