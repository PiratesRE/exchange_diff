using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class AcceptedDomain : DropDownItemData
	{
		public AuthenticationType? AuthenticationType { get; set; }

		public AcceptedDomainType DomainType { get; set; }

		public AcceptedDomain(AcceptedDomain domain) : base(domain)
		{
			string text = domain.DomainName.IsStar ? domain.DomainName.ToString() : domain.DomainName.SmtpDomain.ToString();
			base.Text = text;
			base.Value = text;
			base.Selected = domain.Default;
			this.AuthenticationType = domain.AuthenticationType;
			this.DomainType = domain.DomainType;
		}
	}
}
