using System;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Ehf;
using Microsoft.Exchange.HostedServices.AdminCenter.UI.Services;
using Microsoft.Exchange.Management.EdgeSync;

namespace Microsoft.Exchange.Management.PerimeterConfig
{
	public abstract class DCPerimeterConfigTask : Task
	{
		[Parameter]
		public Fqdn DomainController
		{
			get
			{
				return (Fqdn)base.Fields["DomainController"];
			}
			set
			{
				base.Fields["DomainController"] = value;
			}
		}

		[Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public EdgeSyncEhfConnectorIdParameter ConnectorId
		{
			get
			{
				return this.connectorId;
			}
			set
			{
				this.connectorId = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			ITopologyConfigurationSession session = this.CreateSession();
			EhfTargetServerConfig config = Utils.CreateEhfTargetConfig(session, this.ConnectorId, this);
			using (EhfProvisioningService ehfProvisioningService = new EhfProvisioningService(config))
			{
				Exception ex = null;
				try
				{
					this.InvokeWebService(session, config, ehfProvisioningService);
				}
				catch (FaultException<ServiceFault> faultException)
				{
					ServiceFault detail = faultException.Detail;
					if (detail.Id == FaultId.UnableToConnectToDatabase)
					{
						ex = new InvalidOperationException("ServiceFault: EHF is unable to connect to its database", faultException);
					}
					else
					{
						ex = faultException;
					}
				}
				catch (MessageSecurityException ex2)
				{
					switch (EhfProvisioningService.DecodeMessageSecurityException(ex2))
					{
					case EhfProvisioningService.MessageSecurityExceptionReason.DatabaseFailure:
						ex = new InvalidOperationException("MessageSecurityException: EHF is unable to connect to its database", ex2.InnerException);
						goto IL_A4;
					case EhfProvisioningService.MessageSecurityExceptionReason.InvalidCredentials:
						ex = new InvalidOperationException("MessageSecurityException: EHF connector contains invalid credentials", ex2.InnerException);
						goto IL_A4;
					}
					ex = ex2;
					IL_A4:;
				}
				catch (CommunicationException ex3)
				{
					ex = ex3;
				}
				catch (TimeoutException ex4)
				{
					ex = ex4;
				}
				catch (EhfProvisioningService.ContractViolationException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					base.WriteError(ex, ErrorCategory.InvalidOperation, null);
				}
			}
		}

		internal abstract void InvokeWebService(IConfigurationSession session, EhfTargetServerConfig config, EhfProvisioningService provisioningService);

		private ITopologyConfigurationSession CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 155, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\PerimeterConfig\\DCPerimeterConfigTask.cs");
		}

		private EdgeSyncEhfConnectorIdParameter connectorId;
	}
}
