using System;
using System.Management.Automation;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.SenderId;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Test", "SenderId", SupportsShouldProcess = true)]
	public sealed class TestSenderId : Task
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestSenderId(this.IPAddress.ToString(), this.PurportedResponsibleDomain.ToString());
			}
		}

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

		[Parameter(Mandatory = true)]
		public IPAddress IPAddress
		{
			get
			{
				return this.ipAddress;
			}
			set
			{
				this.ipAddress = value;
			}
		}

		[Parameter(Mandatory = true)]
		public SmtpDomain PurportedResponsibleDomain
		{
			get
			{
				return this.purportedResponsibleDomain;
			}
			set
			{
				this.purportedResponsibleDomain = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string HelloDomain
		{
			get
			{
				return this.helloDomain;
			}
			set
			{
				this.helloDomain = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true)]
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

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				base.GetType().FullName
			});
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(this.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 118, "InternalValidate", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\MessageHygiene\\Diagnostics\\TestSenderId.cs");
			if (this.Server == null)
			{
				try
				{
					this.serverObject = topologyConfigurationSession.ReadLocalServer();
					goto IL_A6;
				}
				catch (TransientException exception)
				{
					this.WriteError(exception, ErrorCategory.ResourceUnavailable, null, false);
					return;
				}
			}
			this.serverObject = topologyConfigurationSession.FindServerByName(this.Server.ToString());
			if (this.serverObject != null)
			{
				goto IL_A6;
			}
			this.WriteError(new LocalizedException(Strings.ErrorServerNotFound(this.Server.ToString())), ErrorCategory.InvalidOperation, null, false);
			return;
			IL_A6:
			if (this.serverObject == null || (!this.serverObject.IsHubTransportServer && !this.serverObject.IsEdgeServer))
			{
				this.WriteError(new LocalizedException(Strings.ErrorInvalidServerRole((this.serverObject != null) ? this.serverObject.Name : Environment.MachineName)), ErrorCategory.InvalidOperation, this.serverObject, false);
				return;
			}
			Dns andInitializeDns = Provider.GetAndInitializeDns(this.serverObject);
			Util.AsyncDns.SetDns(andInitializeDns);
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			RoutingAddress purportedResponsibleAddress = new RoutingAddress("postmaster", this.purportedResponsibleDomain.Domain);
			this.validator = new SenderIdValidator(new TestSenderId.TestServer());
			this.resetEvent = new AutoResetEvent(false);
			this.validator.BeginCheckHost(this.ipAddress, purportedResponsibleAddress, (this.helloDomain != null) ? this.helloDomain : string.Empty, true, new AsyncCallback(this.CheckHostCallback), null);
			this.resetEvent.WaitOne();
			base.WriteObject(this.result);
		}

		private void CheckHostCallback(IAsyncResult ar)
		{
			this.result = this.validator.EndCheckHost(ar);
			this.resetEvent.Set();
		}

		private IPAddress ipAddress;

		private SmtpDomain purportedResponsibleDomain;

		private string helloDomain;

		private SenderIdValidator validator;

		private AutoResetEvent resetEvent;

		private SenderIdResult result;

		private Server serverObject;

		private class TestServer : SmtpServer
		{
			public override string Name
			{
				get
				{
					return "TestServer";
				}
			}

			public override Version Version
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public override IPPermission IPPermission
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public override AcceptedDomainCollection AcceptedDomains
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public override RemoteDomainCollection RemoteDomains
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public override AddressBook AddressBook
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public override void SubmitMessage(EmailMessage message)
			{
				throw new NotImplementedException();
			}
		}
	}
}
