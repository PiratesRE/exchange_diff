using System;
using System.Collections;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Cmdlet("New", "DlpPolicy", SupportsShouldProcess = true)]
	public sealed class NewDlpPolicy : NewMultitenancyFixedNameSystemConfigurationObjectTask<ADComplianceProgram>
	{
		[Parameter(Mandatory = false)]
		public string Template
		{
			get
			{
				return (string)base.Fields["Template"];
			}
			set
			{
				base.Fields["Template"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RuleState State
		{
			get
			{
				return (RuleState)base.Fields["State"];
			}
			set
			{
				base.Fields["State"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RuleMode Mode
		{
			get
			{
				return (RuleMode)base.Fields["Mode"];
			}
			set
			{
				base.Fields["Mode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public byte[] TemplateData
		{
			get
			{
				return (byte[])base.Fields["TemplateData"];
			}
			set
			{
				base.Fields["TemplateData"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Hashtable Parameters
		{
			get
			{
				return (Hashtable)base.Fields["Parameters"];
			}
			set
			{
				base.Fields["Parameters"] = value;
			}
		}

		[Parameter(Mandatory = false, Position = 0)]
		public string Name
		{
			get
			{
				return (string)base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Description
		{
			get
			{
				return (string)base.Fields["Description"];
			}
			set
			{
				base.Fields["Description"] = value;
			}
		}

		public NewDlpPolicy()
		{
			this.impl = new NewDlpPolicyImpl(this);
		}

		public OrganizationId ResolveOrganization()
		{
			return this.ResolveCurrentOrganization();
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageInstallDlpPolicy(this.Name);
			}
		}

		protected override void InternalProcessRecord()
		{
			this.SetupImpl();
			this.impl.ProcessRecord();
		}

		protected override void InternalValidate()
		{
			this.DataObject = (ADComplianceProgram)this.PrepareDataObject();
			if (this.Name != null)
			{
				this.DataObject.SetId(base.DataSession as IConfigurationSession, this.Name);
			}
			this.SetupImpl();
			this.impl.Validate();
		}

		private void SetupImpl()
		{
			this.impl.DataSession = new MessagingPoliciesSyncLogDataSession(base.DataSession, null, null);
			this.impl.ShouldContinue = new CmdletImplementation.ShouldContinueMethod(base.ShouldContinue);
		}

		private NewDlpPolicyImpl impl;

		internal static readonly string DefaultVersion = "15.00.0002.000";

		public delegate void WarningWriterDelegate(string text);
	}
}
