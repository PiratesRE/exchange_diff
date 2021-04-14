using System;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
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
	[Cmdlet("Get", "ExchangeCertificate", DefaultParameterSetName = "Thumbprint")]
	public class GetExchangeCertificate : DataAccessTask<Server>, IIdentityExchangeCertificateCmdlet
	{
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

		[Parameter(Mandatory = false, ParameterSetName = "Instance", ValueFromPipeline = true)]
		public X509Certificate2 Instance
		{
			get
			{
				return (X509Certificate2)base.Fields["Certificate"];
			}
			set
			{
				base.Fields["Certificate"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Thumbprint")]
		[Parameter(Mandatory = false, ParameterSetName = "Instance")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Thumbprint", ValueFromPipeline = true, Position = 0)]
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

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpDomain> DomainName
		{
			internal get
			{
				return (MultiValuedProperty<SmtpDomain>)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 121, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageSecurity\\ExchangeCertificate\\GetExchangeCertificate.cs");
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
			if (this.Instance != null)
			{
				exchangeCertificateRpc.GetByCertificate = this.Instance.Export(X509ContentType.SerializedCert);
			}
			if (this.DomainName != null && this.DomainName.Count > 0)
			{
				exchangeCertificateRpc.GetByDomains = this.DomainName;
			}
			if (this.Thumbprint != null)
			{
				exchangeCertificateRpc.GetByThumbprint = this.Thumbprint;
			}
			ExchangeCertificateRpcVersion exchangeCertificateRpcVersion = ExchangeCertificateRpcVersion.Version1;
			byte[] outputBlob = null;
			try
			{
				byte[] inBlob = exchangeCertificateRpc.SerializeInputParameters(ExchangeCertificateRpcVersion.Version2);
				ExchangeCertificateRpcClient2 exchangeCertificateRpcClient = new ExchangeCertificateRpcClient2(this.serverObject.Name);
				outputBlob = exchangeCertificateRpcClient.GetCertificate2(0, inBlob);
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
					outputBlob = exchangeCertificateRpcClient2.GetCertificate(0, inBlob2);
				}
				catch (RpcException e)
				{
					ManageExchangeCertificate.WriteRpcError(e, this.serverObject.Name, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
			}
			ExchangeCertificateRpc exchangeCertificateRpc2 = new ExchangeCertificateRpc(exchangeCertificateRpcVersion, null, outputBlob);
			ExchangeCertificateRpc.OutputTaskMessages(this.serverObject, exchangeCertificateRpc2, new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError));
			foreach (ExchangeCertificate exchangeCertificate in exchangeCertificateRpc2.ReturnCertList)
			{
				exchangeCertificate.Identity = this.serverObject.Fqdn + "\\" + exchangeCertificate.Thumbprint;
				if (string.IsNullOrEmpty(exchangeCertificate.FriendlyName))
				{
					exchangeCertificate.FriendlyName = exchangeCertificate.Issuer;
				}
				base.WriteObject(exchangeCertificate);
			}
		}

		internal static void PrepareParameters(IIdentityExchangeCertificateCmdlet cmdlet)
		{
			if ((cmdlet.Server == null && cmdlet.Identity == null) || (cmdlet.Server == null && cmdlet.Identity.ServerIdParameter == null))
			{
				cmdlet.Server = new ServerIdParameter();
			}
			else if (cmdlet.Identity != null && cmdlet.Identity.ServerIdParameter != null)
			{
				cmdlet.Server = cmdlet.Identity.ServerIdParameter;
			}
			if (cmdlet.Identity != null && cmdlet.Identity.Thumbprint != null)
			{
				cmdlet.Thumbprint = cmdlet.Identity.Thumbprint;
			}
		}

		private Server serverObject;
	}
}
