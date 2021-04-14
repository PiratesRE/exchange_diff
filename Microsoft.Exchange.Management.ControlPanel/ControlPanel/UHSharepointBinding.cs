using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UHSharepointBinding
	{
		public UHSharepointBinding(BindingMetadata sharepointBinding)
		{
			ArgumentValidator.ThrowIfNull("sharepointBinding", sharepointBinding);
			this.siteUrl = sharepointBinding.Name;
		}

		[DataMember]
		public string SiteUrl
		{
			get
			{
				return this.siteUrl;
			}
			set
			{
				this.siteUrl = value;
			}
		}

		private string siteUrl;
	}
}
