using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InstructResetCredentialsException : LocalizedException
	{
		public InstructResetCredentialsException(string detailedInformation) : base(Strings.InstructResetCredentials(detailedInformation))
		{
			this.detailedInformation = detailedInformation;
		}

		public InstructResetCredentialsException(string detailedInformation, Exception innerException) : base(Strings.InstructResetCredentials(detailedInformation), innerException)
		{
			this.detailedInformation = detailedInformation;
		}

		protected InstructResetCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.detailedInformation = (string)info.GetValue("detailedInformation", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("detailedInformation", this.detailedInformation);
		}

		public string DetailedInformation
		{
			get
			{
				return this.detailedInformation;
			}
		}

		private readonly string detailedInformation;
	}
}
