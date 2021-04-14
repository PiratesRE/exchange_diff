using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "ClientAccessRole", SupportsShouldProcess = true)]
	public sealed class InstallClientAccessRole : ManageRole
	{
		public InstallClientAccessRole()
		{
			base.Fields["NoSelfSignedCertificates"] = false;
			base.Fields["CustomerFeedbackEnabled"] = null;
			base.Fields["ExternalCASServerDomain"] = null;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallClientAccessRoleDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter NoSelfSignedCertificates
		{
			get
			{
				return new SwitchParameter((bool)base.Fields["NoSelfSignedCertificates"]);
			}
			set
			{
				base.Fields["NoSelfSignedCertificates"] = value.ToBool();
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
		public Fqdn ExternalCASServerDomain
		{
			get
			{
				return (Fqdn)base.Fields["ExternalCASServerDomain"];
			}
			set
			{
				base.Fields["ExternalCASServerDomain"] = value;
			}
		}
	}
}
