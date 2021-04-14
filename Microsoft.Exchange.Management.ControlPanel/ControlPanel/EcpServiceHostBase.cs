using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal abstract class EcpServiceHostBase : ServiceHost
	{
		public EcpServiceHostBase(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
		{
		}

		protected override void ApplyConfiguration()
		{
			base.ApplyConfiguration();
			ServiceDebugBehavior serviceDebugBehavior = base.Description.Behaviors.Find<ServiceDebugBehavior>();
			serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
			ServiceAuthorizationBehavior serviceAuthorizationBehavior = base.Description.Behaviors.Find<ServiceAuthorizationBehavior>();
			serviceAuthorizationBehavior.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			serviceAuthorizationBehavior.ExternalAuthorizationPolicies = EcpServiceHostBase.customAuthorizationPolicies;
			serviceAuthorizationBehavior.ServiceAuthorizationManager = EcpServiceHostBase.rbacAuthorizationManager;
			ContractDescription contract = ContractDescription.GetContract(base.Description.ServiceType);
			foreach (Uri uri in base.BaseAddresses)
			{
				Binding binding = this.CreateBinding(uri);
				ServiceEndpoint serviceEndpoint = new ServiceEndpoint(contract, binding, new EndpointAddress(uri, new AddressHeader[0]));
				serviceEndpoint.Behaviors.Add(EcpServiceHostBase.diagnosticsBehavior);
				this.ApplyServiceEndPointConfiguration(serviceEndpoint);
				base.Description.Endpoints.Add(serviceEndpoint);
			}
		}

		protected abstract Binding CreateBinding(Uri address);

		protected virtual void ApplyServiceEndPointConfiguration(ServiceEndpoint serviceEndpoint)
		{
		}

		private static readonly DiagnosticsBehavior diagnosticsBehavior = new DiagnosticsBehavior();

		private static readonly RbacAuthorizationManager rbacAuthorizationManager = new RbacAuthorizationManager();

		private static readonly ReadOnlyCollection<IAuthorizationPolicy> customAuthorizationPolicies = Array.AsReadOnly<IAuthorizationPolicy>(new IAuthorizationPolicy[]
		{
			new RbacAuthorizationPolicy()
		});
	}
}
