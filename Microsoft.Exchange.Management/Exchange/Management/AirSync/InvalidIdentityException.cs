using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.AirSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidIdentityException : LocalizedException
	{
		public InvalidIdentityException(string smtpAddress) : base(Strings.InvalidIdentityException(smtpAddress))
		{
			this.smtpAddress = smtpAddress;
		}

		public InvalidIdentityException(string smtpAddress, Exception innerException) : base(Strings.InvalidIdentityException(smtpAddress), innerException)
		{
			this.smtpAddress = smtpAddress;
		}

		protected InvalidIdentityException(SerializationInfo info, StreamingContext context) : base(info, context)
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
