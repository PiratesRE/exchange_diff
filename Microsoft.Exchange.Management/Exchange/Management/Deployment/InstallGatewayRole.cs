using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "GatewayRole", SupportsShouldProcess = true)]
	public sealed class InstallGatewayRole : ManageGatewayRole
	{
		public InstallGatewayRole()
		{
			this.AdamLdapPort = 50389;
			this.AdamSslPort = 50636;
			this.StartTransportService = true;
			base.Fields["CustomerFeedbackEnabled"] = null;
			base.Fields["Industry"] = IndustryType.NotSpecified;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallGatewayRoleDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public ushort AdamLdapPort
		{
			get
			{
				return (ushort)base.Fields["AdamLdapPort"];
			}
			set
			{
				base.Fields["AdamLdapPort"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ushort AdamSslPort
		{
			get
			{
				return (ushort)base.Fields["AdamSslPort"];
			}
			set
			{
				base.Fields["AdamSslPort"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool StartTransportService
		{
			get
			{
				return (bool)base.Fields["StartTransportService"];
			}
			set
			{
				base.Fields["StartTransportService"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool? CustomerFeedbackEnabled
		{
			get
			{
				return (bool?)base.Fields["CustomerFeedbackEnabled"];
			}
			set
			{
				base.Fields["CustomerFeedbackEnabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public IndustryType Industry
		{
			get
			{
				return (IndustryType)base.Fields["Industry"];
			}
			set
			{
				base.Fields["Industry"] = value;
			}
		}
	}
}
