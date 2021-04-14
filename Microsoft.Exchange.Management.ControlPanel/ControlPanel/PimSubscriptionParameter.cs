using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class PimSubscriptionParameter : SelfMailboxParameters
	{
		public PimSubscriptionParameter()
		{
		}

		[DataMember]
		public string EmailAddress
		{
			get
			{
				return (string)base["EmailAddress"];
			}
			set
			{
				base["EmailAddress"] = value.Trim();
			}
		}

		[DataMember]
		public string IncomingPassword
		{
			private get
			{
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					base[this.PasswordParameterName] = value.ToSecureString();
				}
			}
		}

		protected virtual string PasswordParameterName
		{
			get
			{
				return "Password";
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return (string)base["DisplayName"];
			}
			set
			{
				base["DisplayName"] = value;
			}
		}
	}
}
