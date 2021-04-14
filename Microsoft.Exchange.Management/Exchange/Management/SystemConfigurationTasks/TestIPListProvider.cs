using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class TestIPListProvider<TIdentity, TProvider> : SystemConfigurationObjectActionTask<TIdentity, TProvider> where TIdentity : IIdentityParameter, new() where TProvider : IPListProvider, new()
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true)]
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
			if (this.Server == null)
			{
				try
				{
					this.serverObject = ((ITopologyConfigurationSession)base.DataSession).ReadLocalServer();
					goto IL_A9;
				}
				catch (TransientException exception)
				{
					this.WriteError(exception, ErrorCategory.ResourceUnavailable, this.DataObject, false);
					return;
				}
			}
			this.serverObject = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, this.RootId, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			if (this.serverObject != null)
			{
				goto IL_A9;
			}
			return;
			IL_A9:
			if (this.serverObject == null || (!this.serverObject.IsHubTransportServer && !this.serverObject.IsEdgeServer))
			{
				this.WriteError(new LocalizedException(Strings.ErrorInvalidServerRole((this.serverObject != null) ? this.serverObject.Name : Environment.MachineName)), ErrorCategory.InvalidOperation, this.serverObject, false);
				return;
			}
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				base.GetType().FullName
			});
			TestProviderResult<TProvider> testProviderResult = new TestProviderResult<TProvider>();
			testProviderResult.Provider = this.DataObject;
			testProviderResult.Matched = Provider.Query(this.serverObject, testProviderResult.Provider, this.ipAddress, out testProviderResult.ProviderResult);
			base.WriteObject(testProviderResult);
			TaskLogger.LogExit();
		}

		private IPAddress ipAddress;

		private Server serverObject;
	}
}
