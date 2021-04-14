using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthDataValidationErrorException : LocalizedException
	{
		public CasHealthDataValidationErrorException(string additionalInformation) : base(Strings.CasHealthDataValidationError(additionalInformation))
		{
			this.additionalInformation = additionalInformation;
		}

		public CasHealthDataValidationErrorException(string additionalInformation, Exception innerException) : base(Strings.CasHealthDataValidationError(additionalInformation), innerException)
		{
			this.additionalInformation = additionalInformation;
		}

		protected CasHealthDataValidationErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.additionalInformation = (string)info.GetValue("additionalInformation", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("additionalInformation", this.additionalInformation);
		}

		public string AdditionalInformation
		{
			get
			{
				return this.additionalInformation;
			}
		}

		private readonly string additionalInformation;
	}
}
