using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidObjectGuidException : LocalizedException
	{
		public InvalidObjectGuidException(string smtpAddress) : base(Strings.InvalidObjectGuidException(smtpAddress))
		{
			this.smtpAddress = smtpAddress;
		}

		public InvalidObjectGuidException(string smtpAddress, Exception innerException) : base(Strings.InvalidObjectGuidException(smtpAddress), innerException)
		{
			this.smtpAddress = smtpAddress;
		}

		protected InvalidObjectGuidException(SerializationInfo info, StreamingContext context) : base(info, context)
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
