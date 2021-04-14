using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeCertificate;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Enable", "ExchangeCertificate", SupportsShouldProcess = true, DefaultParameterSetName = "Thumbprint")]
	public class EnableExchangeCertificate : DataAccessTask<Server>, IIdentityExchangeCertificateCmdlet
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0)]
		public ExchangeCertificateIdParameter Identity
		{
			get
			{
				return (ExchangeCertificateIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "Thumbprint", Position = 0)]
		public string Thumbprint
		{
			get
			{
				return (string)base.Fields["Thumbprint"];
			}
			set
			{
				base.Fields["Thumbprint"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public AllowedServices Services
		{
			get
			{
				return (AllowedServices)base.Fields["Services"];
			}
			set
			{
				base.Fields["Services"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter NetworkServiceAllowed
		{
			get
			{
				return (SwitchParameter)(base.Fields["NetworkServiceAllowed"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["NetworkServiceAllowed"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter DoNotRequireSsl
		{
			get
			{
				return (SwitchParameter)(base.Fields["DoNotRequireSsl"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DoNotRequireSsl"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Thumbprint")]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public new Fqdn DomainController
		{
			get
			{
				return base.DomainController;
			}
			set
			{
				base.DomainController = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 144, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\EnableExchangeCertificate.cs");
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmEnableExchangeCertificate(this.Thumbprint);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			GetExchangeCertificate.PrepareParameters(this);
			this.serverObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound((string)this.Server)), new LocalizedString?(Strings.ErrorServerNotUnique((string)this.Server)));
			if (!this.serverObject.IsE14OrLater)
			{
				base.WriteError(new ArgumentException(Strings.RemoteCertificateExchangeVersionNotSupported(this.serverObject.Name)), ErrorCategory.InvalidArgument, null);
			}
			base.VerifyIsWithinScopes(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromCustomScopeSet(base.ScopeSet, ADSystemConfigurationSession.GetRootOrgContainerId(this.DomainController, null), base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true), 186, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\EnableExchangeCertificate.cs"), this.serverObject, true, new DataAccessTask<Server>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			if (!string.IsNullOrEmpty(this.Thumbprint))
			{
				this.Thumbprint = ManageExchangeCertificate.UnifyThumbprintFormat(this.Thumbprint);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (base.HasErrors)
			{
				return;
			}
			ExchangeCertificateRpc exchangeCertificateRpc = new ExchangeCertificateRpc();
			exchangeCertificateRpc.EnableByThumbprint = this.Thumbprint;
			exchangeCertificateRpc.EnableServices = this.Services;
			exchangeCertificateRpc.RequireSsl = !this.DoNotRequireSsl;
			exchangeCertificateRpc.EnableAllowConfirmation = !this.Force;
			exchangeCertificateRpc.EnableNetworkService = this.NetworkServiceAllowed;
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(this.serverObject.Name);
				outputBlob = exchangeCertificateRpcClient.EnableCertificate2(0, inBlob);
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version2;
			}
			catch (RpcException)
			{
				exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			}
			if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
			{
				try
				{
					byte[] inBlob2 = exchangeCertificateRpc.SerializeInputParameters(exchangeCertificateRpcVersion);
					ExchangeCertificateRpcClient exchangeCertificateRpcClient2 = new ExchangeCertificateRpcClient(this.serverObject.Name);
					outputBlob = exchangeCertificateRpcClient2.EnableCertificate(0, inBlob2);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.WriteRpcError(e, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, exchangeCertificateRpc2, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (exchangeCertificateRpc2.ReturnConfirmationList != null)
			{
				foreach (KeyValuePair<AllowedServices, LocalizedString> keyValuePair in exchangeCertificateRpc2.ReturnConfirmationList)
				{
					if (base.ShouldContinue(keyValuePair.Value))
					{
						ExchangeCertificateRpc exchangeCertificateRpc3 = new ExchangeCertificateRpc();
						exchangeCertificateRpc3.EnableAllowConfirmation = false;
						exchangeCertificateRpc3.EnableByThumbprint = this.Thumbprint;
						exchangeCertificateRpc3.RequireSsl = !this.DoNotRequireSsl;
						exchangeCertificateRpc3.EnableNetworkService = this.NetworkServiceAllowed;
						exchangeCertificateRpc3.EnableServices = keyValuePair.Key;
						AllowedServices key = keyValuePair.Key;
						if (key == AllowedServices.SMTP)
						{
							exchangeCertificateRpc3.EnableUpdateAD = true;
						}
						try
						{
							byte[] inBlob3 = exchangeCertificateRpc3.SerializeInputParameters(exchangeCertificateRpcVersion);
							if (exchangeCertificateRpcVersion == ExchangeCertificateRpcVersion.Version1)
							{
								ExchangeCertificateRpcClient exchangeCertificateRpcClient3 = new ExchangeCertificateRpcClient(this.serverObject.Name);
								outputBlob = exchangeCertificateRpcClient3.EnableCertificate(0, inBlob3);
							}
							else
							{
								ExchangeCertificateRpcClient2 exchangeCertificateRpcClient4 = new ExchangeCertificateRpcClient2(this.serverObject.Name);
								outputBlob = exchangeCertificateRpcClient4.EnableCertificate2(0, inBlob3);
							}
						}
						catch (RpcException e2)
						{
							ManageExchangeCertificate.WriteRpcError(e2, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
						}
						exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
						ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, exchangeCertificateRpc2, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
					}
				}
			}
		}

		private Server serverObject;
	}
}
