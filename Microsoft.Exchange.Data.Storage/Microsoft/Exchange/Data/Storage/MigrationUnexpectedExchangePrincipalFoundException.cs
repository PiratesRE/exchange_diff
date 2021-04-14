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
	internal class MigrationUnexpectedExchangePrincipalFoundException : MigrationPermanentException
	{
		public MigrationUnexpectedExchangePrincipalFoundException(string smtpAddress) : base(ServerStrings.MigrationUnexpectedExchangePrincipalFound(smtpAddress))
		{
			this.smtpAddress = smtpAddress;
		}

		public MigrationUnexpectedExchangePrincipalFoundException(string smtpAddress, Exception innerException) : base(ServerStrings.MigrationUnexpectedExchangePrincipalFound(smtpAddress), innerException)
		{
			this.smtpAddress = smtpAddress;
		}

		protected MigrationUnexpectedExchangePrincipalFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.smtpAddress = (string)info.GetValue("smtpAddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("smtpAddress", this.smtpAddress);
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		private readonly string smtpAddress;
	}
}
