using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UHExchangeBinding
	{
		public UHExchangeBinding(BindingMetadata exchangeBinding)
		{
			ArgumentValidator.ThrowIfNull("exchangeBinding", exchangeBinding);
			this.PrimarySmtpAddress = exchangeBinding.Name;
			this.displayName = exchangeBinding.DisplayName;
		}

		[DataMember]
		public string PrimarySmtpAddress
		{
			get
			{
				return this.primarySmtpAddress;
			}
			set
			{
				this.primarySmtpAddress = value;
			}
		}

		[DataMember]
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		private string primarySmtpAddress;

		private string displayName;
	}
}
