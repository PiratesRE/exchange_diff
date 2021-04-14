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
	[Cmdlet("New", "HybridConfiguration", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class NewHybridConfiguration : NewFixedNameSystemConfigurationObjectTask<HybridConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return HybridStrings.ConfirmationMessageNewHybridConfiguration;
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
				base.Fields["ExternalIPAddresses"] = value;
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
			HybridConfiguration hybridConfiguration = (HybridConfiguration)base.PrepareDataObject();
			IConfigurationSession session = base.DataSession as IConfigurationSession;
			hybridConfiguration.SetId(session, "Hybrid Configuration");
			if (base.Fields.IsModified("ClientAccessServers"))
			{
				HybridConfiguration hybridConfiguration2 = hybridConfiguration;
				ADPropertyDefinition clientAccessServers = HybridConfigurationSchema.ClientAccessServers;
				IConfigDataProvider dataSession = base.DataSession;
				MultiValuedProperty<ServerIdParameter> clientAccessServers2 = this.ClientAccessServers;
				HybridConfigurationTaskUtility.GetUniqueObject getServer = new HybridConfigurationTaskUtility.GetUniqueObject(base.GetDataObject<Server>);
				Task.TaskErrorLoggingDelegate writeError = new Task.TaskErrorLoggingDelegate(base.WriteError);
				HybridConfigurationTaskUtility.ServerCriterion[] array = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsClientAccessServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotCAS));
				array[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE14OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotE14CAS));
				hybridConfiguration2.ClientAccessServers = HybridConfigurationTaskUtility.ValidateServers(clientAccessServers, dataSession, clientAccessServers2, getServer, writeError, array);
			}
			if (base.Fields.IsModified("SendingTransportServers"))
			{
				HybridConfiguration hybridConfiguration3 = hybridConfiguration;
				ADPropertyDefinition sendingTransportServers = HybridConfigurationSchema.SendingTransportServers;
				IConfigDataProvider dataSession2 = base.DataSession;
				MultiValuedProperty<ServerIdParameter> sendingTransportServers2 = this.SendingTransportServers;
				HybridConfigurationTaskUtility.GetUniqueObject getServer2 = new HybridConfigurationTaskUtility.GetUniqueObject(base.GetDataObject<Server>);
				Task.TaskErrorLoggingDelegate writeError2 = new Task.TaskErrorLoggingDelegate(base.WriteError);
				HybridConfigurationTaskUtility.ServerCriterion[] array2 = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array2[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsHubTransportServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorSendingTransportServerNotHub));
				array2[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE15OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorSendingTransportServerNotE15Hub));
				hybridConfiguration3.SendingTransportServers = HybridConfigurationTaskUtility.ValidateServers(sendingTransportServers, dataSession2, sendingTransportServers2, getServer2, writeError2, array2);
			}
			if (base.Fields.IsModified("ReceivingTransportServers"))
			{
				HybridConfiguration hybridConfiguration4 = hybridConfiguration;
				ADPropertyDefinition receivingTransportServers = HybridConfigurationSchema.ReceivingTransportServers;
				IConfigDataProvider dataSession3 = base.DataSession;
				MultiValuedProperty<ServerIdParameter> receivingTransportServers2 = this.ReceivingTransportServers;
				HybridConfigurationTaskUtility.GetUniqueObject getServer3 = new HybridConfigurationTaskUtility.GetUniqueObject(base.GetDataObject<Server>);
				Task.TaskErrorLoggingDelegate writeError3 = new Task.TaskErrorLoggingDelegate(base.WriteError);
				HybridConfigurationTaskUtility.ServerCriterion[] array3 = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array3[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsFrontendTransportServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorReceivingTransportServerNotFrontEnd));
				array3[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE15OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorReceivingTransportServerNotE15FrontEnd));
				hybridConfiguration4.ReceivingTransportServers = HybridConfigurationTaskUtility.ValidateServers(receivingTransportServers, dataSession3, receivingTransportServers2, getServer3, writeError3, array3);
			}
			if (base.Fields.IsModified("EdgeTransportServers"))
			{
				HybridConfiguration hybridConfiguration5 = hybridConfiguration;
				ADPropertyDefinition edgeTransportServers = HybridConfigurationSchema.EdgeTransportServers;
				IConfigDataProvider dataSession4 = base.DataSession;
				MultiValuedProperty<ServerIdParameter> edgeTransportServers2 = this.EdgeTransportServers;
				HybridConfigurationTaskUtility.GetUniqueObject getServer4 = new HybridConfigurationTaskUtility.GetUniqueObject(base.GetDataObject<Server>);
				Task.TaskErrorLoggingDelegate writeError4 = new Task.TaskErrorLoggingDelegate(base.WriteError);
				HybridConfigurationTaskUtility.ServerCriterion[] array4 = new HybridConfigurationTaskUtility.ServerCriterion[2];
				array4[0] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsEdgeServer, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotEdge));
				array4[1] = new HybridConfigurationTaskUtility.ServerCriterion((Server s) => s.IsE14Sp1OrLater, new Func<string, LocalizedString>(HybridStrings.HybridErrorServerNotE14Edge));
				hybridConfiguration5.EdgeTransportServers = HybridConfigurationTaskUtility.ValidateServers(edgeTransportServers, dataSession4, edgeTransportServers2, getServer4, writeError4, array4);
			}
			if (base.Fields.IsModified("TlsCertificateName"))
			{
				this.DataObject.TlsCertificateName = this.TlsCertificateName;
			}
			if (base.Fields.IsModified("OnPremisesSmartHost"))
			{
				this.DataObject.OnPremisesSmartHost = this.OnPremisesSmartHost;
			}
			if (base.Fields.IsModified("Domains"))
			{
				this.DataObject.Domains = this.Domains;
			}
			if (base.Fields.IsModified("Features"))
			{
				this.DataObject.Features = this.Features;
			}
			if (base.Fields.IsModified("ExternalIPAddresses"))
			{
				this.DataObject.ExternalIPAddresses = HybridConfigurationTaskUtility.ValidateExternalIPAddresses(this.ExternalIPAddresses, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			if (base.Fields.IsModified("ServiceInstance"))
			{
				this.DataObject.ServiceInstance = this.ServiceInstance;
			}
			return hybridConfiguration;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			HybridConfiguration[] array = ((IConfigurationSession)base.DataSession).Find<HybridConfiguration>(null, QueryScope.SubTree, null, null, 0);
			if (array == null || array.Length == 0)
			{
				base.InternalProcessRecord();
			}
			else
			{
				base.WriteError(new HybridConfigurationAlreadyDefinedException(), ErrorCategory.InvalidArgument, this.DataObject.Name);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			int count = HybridConfigurationTaskUtility.GetCount<ServerIdParameter>(this.EdgeTransportServers);
			int count2 = HybridConfigurationTaskUtility.GetCount<ServerIdParameter>(this.SendingTransportServers);
			int count3 = HybridConfigurationTaskUtility.GetCount<ServerIdParameter>(this.ReceivingTransportServers);
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
			TaskLogger.LogExit();
		}
	}
}
