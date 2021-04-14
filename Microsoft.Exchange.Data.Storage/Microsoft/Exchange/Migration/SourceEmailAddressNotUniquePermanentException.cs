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
	internal class SourceEmailAddressNotUniquePermanentException : MigrationPermanentException
	{
		public SourceEmailAddressNotUniquePermanentException(string smtpAddress) : base(Strings.SourceEmailAddressNotUnique(smtpAddress))
		{
			this.smtpAddress = smtpAddress;
		}

		public SourceEmailAddressNotUniquePermanentException(string smtpAddress, Exception innerException) : base(Strings.SourceEmailAddressNotUnique(smtpAddress), innerException)
		{
			this.smtpAddress = smtpAddress;
		}

		protected SourceEmailAddressNotUniquePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
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
