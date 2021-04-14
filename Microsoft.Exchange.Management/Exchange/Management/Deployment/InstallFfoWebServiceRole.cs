using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "FfoWebServiceRole", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallFfoWebServiceRole : ManageFfoWebServiceRole
	{
		public InstallFfoWebServiceRole()
		{
			base.Fields["NoSelfSignedCertificates"] = false;
			base.Fields["CustomerFeedbackEnabled"] = null;
			base.Fields["ExternalCASServerDomain"] = null;
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.InstallFfoWebServiceRoleDescription;
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

		private const string NoSelfSignedCertificatesProperty = "NoSelfSignedCertificates";

		private const string CustomerFeedbackEnabledProperty = "CustomerFeedbackEnabled";

		private const string ExternalCASServerDomainProperty = "ExternalCASServerDomain";
	}
}
