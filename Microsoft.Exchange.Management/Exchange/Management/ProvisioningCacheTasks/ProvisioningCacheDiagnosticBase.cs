using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ProvisioningCacheTasks
{
	public abstract class ProvisioningCacheDiagnosticBase : Task
	{
		[Parameter(Mandatory = true, ParameterSetName = "GlobalCache", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[Parameter(Mandatory = true, ParameterSetName = "OrganizationCache", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		[ValidateNotNullOrEmpty]
		public Fqdn Server
		{
			get
			{
				return (Fqdn)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "GlobalCache")]
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, ParameterSetName = "OrganizationCache")]
		public string Application
		{
			get
			{
				return (string)base.Fields["Application"];
			}
			set
			{
				base.Fields["Application"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "GlobalCache")]
		public SwitchParameter GlobalCache
		{
			get
			{
				return (SwitchParameter)(base.Fields["GlobalCache"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["GlobalCache"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "OrganizationCache")]
		public MultiValuedProperty<OrganizationIdParameter> Organizations
		{
			get
			{
				return (MultiValuedProperty<OrganizationIdParameter>)base.Fields["Organizations"];
			}
			set
			{
				base.Fields["Organizations"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "OrganizationCache")]
		public SwitchParameter CurrentOrganization
		{
			get
			{
				return (SwitchParameter)(base.Fields["CurrentOrganization"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CurrentOrganization"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Guid> CacheKeys
		{
			get
			{
				return (MultiValuedProperty<Guid>)base.Fields["CacheKeys"];
			}
			set
			{
				base.Fields["CacheKeys"] = value;
			}
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			if (!TaskHelper.GetLocalServerFqdn(new Task.TaskWarningLoggingDelegate(this.WriteWarning)).StartsWith(this.Server, StringComparison.OrdinalIgnoreCase))
			{
				string remoteServerFqdn = this.Server.ToString();
				int e15MinVersion = Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.E15MinVersion;
				CmdletProxy.ThrowExceptionIfProxyIsNeeded(base.CurrentTaskContext, remoteServerFqdn, e15MinVersion, false, this.ConfirmationMessage, null);
			}
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			base.CheckExclusiveParameters(new object[]
			{
				"Organizations",
				"CurrentOrganization"
			});
			if (!CacheApplicationManager.IsApplicationDefined(this.Application))
			{
				base.WriteError(new ArgumentException(Strings.ErrorApplicationNotDefined(this.Application)), (ErrorCategory)1000, null);
			}
			ICollection<Guid> collection = this.ResolveOrganizations();
			DiagnosticType diagnosticType = this.GetDiagnosticType();
			DiagnosticType command = diagnosticType;
			ICollection<Guid> orgIds = collection;
			ICollection<Guid> entries;
			if (this.CacheKeys != null)
			{
				ICollection<Guid> cacheKeys = this.CacheKeys;
				entries = cacheKeys;
			}
			else
			{
				entries = new List<Guid>();
			}
			DiagnosticCommand diagnosticCommand = new DiagnosticCommand(command, orgIds, entries);
			using (NamedPipeClientStream namedPipeClientStream = this.PrepareClientStream())
			{
				byte[] array = diagnosticCommand.ToSendMessage();
				byte[] array2 = new byte[5000];
				try
				{
					namedPipeClientStream.Write(array, 0, array.Length);
					namedPipeClientStream.Flush();
					int num;
					do
					{
						Array.Clear(array2, 0, array2.Length);
						num = namedPipeClientStream.Read(array2, 0, array2.Length);
						if (num > 0)
						{
							this.ProcessReceivedData(array2, num);
						}
					}
					while (num > 0 && namedPipeClientStream.IsConnected);
				}
				catch (IOException exception)
				{
					base.WriteError(exception, (ErrorCategory)1001, null);
				}
				catch (ObjectDisposedException exception2)
				{
					base.WriteError(exception2, (ErrorCategory)1001, null);
				}
				catch (NotSupportedException exception3)
				{
					base.WriteError(exception3, (ErrorCategory)1001, null);
				}
				catch (InvalidOperationException exception4)
				{
					base.WriteError(exception4, (ErrorCategory)1001, null);
				}
			}
		}

		protected abstract void ProcessReceivedData(byte[] buffer, int bufLen);

		internal abstract DiagnosticType GetDiagnosticType();

		private ICollection<Guid> ResolveOrganizations()
		{
			if (this.GlobalCache)
			{
				return null;
			}
			List<Guid> list = new List<Guid>();
			if (this.CurrentOrganization)
			{
				if (OrganizationId.ForestWideOrgId.Equals(base.ExecutingUserOrganizationId))
				{
					list.Add(Guid.Empty);
				}
				else
				{
					list.Add(base.ExecutingUserOrganizationId.ConfigurationUnit.ObjectGuid);
				}
				return list;
			}
			if (this.GlobalCache || this.Organizations == null)
			{
				return list;
			}
			ITopologyConfigurationSession session = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 258, "ResolveOrganizations", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ProvisioningCache\\ProvisioningCacheDiagnosticBase.cs");
			foreach (OrganizationIdParameter organizationIdParameter in this.Organizations)
			{
				IEnumerable<ExchangeConfigurationUnit> objects = organizationIdParameter.GetObjects<ExchangeConfigurationUnit>(null, session);
				bool flag = true;
				foreach (ExchangeConfigurationUnit exchangeConfigurationUnit in objects)
				{
					flag = false;
					if (!list.Contains(exchangeConfigurationUnit.Guid))
					{
						list.Add(exchangeConfigurationUnit.Guid);
					}
				}
				if (flag)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(organizationIdParameter.ToString())), ExchangeErrorCategory.ServerOperation, null);
				}
			}
			return list;
		}

		private NamedPipeClientStream PrepareClientStream()
		{
			NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", CacheApplicationManager.GetAppPipeName(this.Application), PipeDirection.InOut);
			try
			{
				namedPipeClientStream.Connect(2000);
			}
			catch (TimeoutException)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorConnectToApplicationTimeout(this.Application, this.Server)), (ErrorCategory)1002, null);
			}
			return namedPipeClientStream;
		}

		private const int recvBufSize = 5000;
	}
}
