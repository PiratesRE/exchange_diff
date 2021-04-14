using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetSmsOptions : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-TextMessagingAccount";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string CountryRegionId
		{
			private get
			{
				return (string)base[TextMessagingAccountSchema.CountryRegionId];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					base[TextMessagingAccountSchema.CountryRegionId] = value;
				}
			}
		}

		[DataMember]
		public string CountryCode { get; set; }

		[DataMember]
		public string VerificationCode { get; set; }

		[DataMember]
		public string MobileOperatorId
		{
			get
			{
				return (string)base[TextMessagingAccountSchema.MobileOperatorId];
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					base[TextMessagingAccountSchema.MobileOperatorId] = value;
				}
			}
		}

		[DataMember]
		public string NotificationPhoneNumber
		{
			get
			{
				return (string)base[TextMessagingAccountSchema.NotificationPhoneNumber];
			}
			set
			{
				base[TextMessagingAccountSchema.NotificationPhoneNumber] = value;
			}
		}
	}
}
