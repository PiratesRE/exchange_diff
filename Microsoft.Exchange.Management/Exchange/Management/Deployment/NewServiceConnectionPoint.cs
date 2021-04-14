using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("New", "ServiceConnectionPoint")]
	public sealed class NewServiceConnectionPoint : NewSystemConfigurationObjectTask<ADServiceConnectionPoint>
	{
		[Parameter(Mandatory = false)]
		[Parameter(Mandatory = true, ParameterSetName = "TrustedHoster")]
		public SwitchParameter TrustedHoster
		{
			get
			{
				return (SwitchParameter)(base.Fields["TrustedHoster"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["TrustedHoster"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[Parameter(Mandatory = true, ParameterSetName = "TrustedHoster")]
		public MultiValuedProperty<string> TrustedHostnames
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["TrustedHostnames"];
			}
			set
			{
				base.Fields["TrustedHostnames"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[Parameter(Mandatory = true, ParameterSetName = "DomainToApplicationUri")]
		public SwitchParameter DomainToApplicationUri
		{
			get
			{
				return (SwitchParameter)(base.Fields["DomainToApplicationUri"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DomainToApplicationUri"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[Parameter(Mandatory = true, ParameterSetName = "DomainToApplicationUri")]
		public MultiValuedProperty<string> Domains
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["Domains"];
			}
			set
			{
				base.Fields["Domains"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[Parameter(Mandatory = true, ParameterSetName = "DomainToApplicationUri")]
		public string ApplicationUri
		{
			get
			{
				return (string)base.Fields["ApplicationUri"];
			}
			set
			{
				base.Fields["ApplicationUri"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.DataObject.Keywords = new MultiValuedProperty<string>();
			if (this.TrustedHoster)
			{
				this.DataObject.Keywords.Add("D3614C7C-D214-4F1F-BD4C-00D91C67F93F");
				this.DataObject.ServiceBindingInformation = this.TrustedHostnames;
			}
			else if (this.DomainToApplicationUri)
			{
				this.DataObject.Keywords.Add("E1AA5F5E-2341-4FCB-8560-E3AB6F081468");
				foreach (string str in this.Domains)
				{
					this.DataObject.Keywords.Add("Domain=" + str);
				}
				this.DataObject.ServiceBindingInformation = new MultiValuedProperty<string>(new string[]
				{
					this.ApplicationUri
				});
			}
			else
			{
				base.WriteError(new NewServiceConnectionPointInvalidParametersException(), (ErrorCategory)1001, null);
			}
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			ADObjectId childId = topologyConfigurationSession.GetAutoDiscoverGlobalContainerId().GetChildId(this.DataObject.Name);
			this.DataObject.SetId(childId);
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
		}
	}
}
