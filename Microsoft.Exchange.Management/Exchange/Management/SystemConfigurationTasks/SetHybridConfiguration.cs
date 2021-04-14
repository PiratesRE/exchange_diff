using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "HybridConfiguration", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium, DefaultParameterSetName = "Identity")]
	public sealed class SetHybridConfiguration : SetSingletonSystemConfigurationObjectTask<HybridConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return HybridStrings.ConfirmationMessageSetHybridConfiguration;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return HybridConfiguration.GetWellKnownParentLocation(base.CurrentOrgContainerId);
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<ServerIdParameter> ClientAccessServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["ClientAccessServers"];
			}
			set
			{
				base.Fields["ClientAccessServers"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<ServerIdParameter> ReceivingTransportServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["ReceivingTransportServers"];
			}
			set
			{
				base.Fields["ReceivingTransportServers"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<ServerIdParameter> SendingTransportServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["SendingTransportServers"];
			}
			set
			{
				base.Fields["SendingTransportServers"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<ServerIdParameter> EdgeTransportServers
		{
			get
			{
				return (MultiValuedProperty<ServerIdParameter>)base.Fields["EdgeTransportServers"];
			}
			set
			{
				base.Fields["EdgeTransportServers"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SmtpX509Identifier TlsCertificateName
		{
			get
			{
				return SmtpX509Identifier.Parse((string)base.Fields["TlsCertificateName"]);
			}
			set
			{
				base.Fields["TlsCertificateName"] = ((value == null) ? null : value.ToString());
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public SmtpDomain OnPremisesSmartHost
		{
			get
			{
				return (SmtpDomain)base.Fields["OnPremisesSmartHost"];
			}
			set
			{
				base.Fields["OnPremisesSmartHost"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<AutoDiscoverSmtpDomain> Domains
		{
			get
			{
				return (MultiValuedProperty<AutoDiscoverSmtpDomain>)base.Fields["Domains"];
			}
			set
			{
				base.Fields["Domains"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<HybridFeature> Features
		{
			get
			{
				return (MultiValuedProperty<HybridFeature>)base.Fields["Features"];
			}
			set
			{
				base.Fields["Features"] = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public MultiValuedProperty<IPRange> ExternalIPAddresses
		{
			get
			{
				return (MultiValuedProperty<IPRange>)base.Fields["ExternalIPAddresses"];
			}
			set
			{
				base.Fields["ExternalIPAddresses"] = ((value == null) ? new MultiValuedProperty<IPRange>() : value);
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
		public int ServiceInstance
		{
			get
			{
				return (int)base.Fields["ServiceInstance"];
			}
			set
			{
				base.Fields["ServiceInstance"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			this.DataObject = (HybridConfiguration)base.PrepareDataObject();
			return SetHybridConfigurationLogic.PrepareDataObject(base.Fields, this.DataObject, base.DataSession, new HybridConfigurationTaskUtility.GetUniqueObject(base.GetDataObject<Server>), new Task.TaskErrorLoggingDelegate(base.WriteError));
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			int count = HybridConfigurationTaskUtility.GetCount<ADObjectId>(this.DataObject.EdgeTransportServers);
			int count2 = HybridConfigurationTaskUtility.GetCount<ADObjectId>(this.DataObject.SendingTransportServers);
			int count3 = HybridConfigurationTaskUtility.GetCount<ADObjectId>(this.DataObject.ReceivingTransportServers);
			if (count > 0 && count2 + count3 > 0)
			{
				base.WriteError(new LocalizedException(HybridStrings.HybridErrorMixedTransportServersSet), ErrorCategory.InvalidArgument, this.DataObject.Name);
			}
			if (count2 + count3 > 0 && (count2 == 0 || count3 == 0))
			{
				base.WriteError(new LocalizedException(HybridStrings.HybridErrorBothTransportServersNotSet), ErrorCategory.InvalidArgument, this.DataObject.Name);
			}
			if (this.Domains != null)
			{
				if ((from d in this.Domains
				where d.AutoDiscover
				select d).Count<AutoDiscoverSmtpDomain>() > 1)
				{
					base.WriteError(new LocalizedException(HybridStrings.HybridErrorOnlyOneAutoDiscoverDomainMayBeSet), ErrorCategory.InvalidArgument, this.DataObject.Name);
				}
			}
			SetHybridConfigurationLogic.Validate(this.DataObject, base.HasErrors, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}
	}
}
