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
	internal class InvalidSmtpAddressException : MigrationPermanentException
	{
		public InvalidSmtpAddressException(string address) : base(ServerStrings.InvalidSmtpAddress(address))
		{
			this.address = address;
		}

		public InvalidSmtpAddressException(string address, Exception innerException) : base(ServerStrings.InvalidSmtpAddress(address), innerException)
		{
			this.address = address;
		}

		protected InvalidSmtpAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.address = (string)info.GetValue("address", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("address", this.address);
		}

		public string Address
		{
			get
			{
				return this.address;
			}
		}

		private readonly string address;
	}
}
