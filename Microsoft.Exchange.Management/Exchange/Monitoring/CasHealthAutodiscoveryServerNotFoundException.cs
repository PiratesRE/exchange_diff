using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthAutodiscoveryServerNotFoundException : LocalizedException
	{
		public CasHealthAutodiscoveryServerNotFoundException(string smtpAddress, string additionalInformation) : base(Strings.CasHealthAutodiscoveryServerNotFound(smtpAddress, additionalInformation))
		{
			this.smtpAddress = smtpAddress;
			this.additionalInformation = additionalInformation;
		}

		public CasHealthAutodiscoveryServerNotFoundException(string smtpAddress, string additionalInformation, Exception innerException) : base(Strings.CasHealthAutodiscoveryServerNotFound(smtpAddress, additionalInformation), innerException)
		{
			this.smtpAddress = smtpAddress;
			this.additionalInformation = additionalInformation;
		}

		protected CasHealthAutodiscoveryServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.smtpAddress = (string)info.GetValue("smtpAddress", typeof(string));
			this.additionalInformation = (string)info.GetValue("additionalInformation", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("smtpAddress", this.smtpAddress);
			info.AddValue("additionalInformation", this.additionalInformation);
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		public string AdditionalInformation
		{
			get
			{
				return this.additionalInformation;
			}
		}

		private readonly string smtpAddress;

		private readonly string additionalInformation;
	}
}
