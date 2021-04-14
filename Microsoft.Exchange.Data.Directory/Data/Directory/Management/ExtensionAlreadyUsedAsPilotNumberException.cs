using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExtensionAlreadyUsedAsPilotNumberException : LocalizedException
	{
		public ExtensionAlreadyUsedAsPilotNumberException(string phoneNumber, string dialPlan) : base(DirectoryStrings.ExtensionAlreadyUsedAsPilotNumber(phoneNumber, dialPlan))
		{
			this.phoneNumber = phoneNumber;
			this.dialPlan = dialPlan;
		}

		public ExtensionAlreadyUsedAsPilotNumberException(string phoneNumber, string dialPlan, Exception innerException) : base(DirectoryStrings.ExtensionAlreadyUsedAsPilotNumber(phoneNumber, dialPlan), innerException)
		{
			this.phoneNumber = phoneNumber;
			this.dialPlan = dialPlan;
		}

		protected ExtensionAlreadyUsedAsPilotNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.phoneNumber = (string)info.GetValue("phoneNumber", typeof(string));
			this.dialPlan = (string)info.GetValue("dialPlan", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("phoneNumber", this.phoneNumber);
			info.AddValue("dialPlan", this.dialPlan);
		}

		public string PhoneNumber
		{
			get
			{
				return this.phoneNumber;
			}
		}

		public string DialPlan
		{
			get
			{
				return this.dialPlan;
			}
		}

		private readonly string phoneNumber;

		private readonly string dialPlan;
	}
}
