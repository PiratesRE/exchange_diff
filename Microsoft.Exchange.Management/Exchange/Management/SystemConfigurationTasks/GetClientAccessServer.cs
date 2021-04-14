using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ClientAccessServer", DefaultParameterSetName = "Identity")]
	public sealed class GetClientAccessServer : GetSystemConfigurationObjectTask<ClientAccessServerIdParameter, Server>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeAlternateServiceAccountCredentialStatus
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeAlternateServiceAccountCredentialStatus"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeAlternateServiceAccountCredentialStatus"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IncludeAlternateServiceAccountCredentialPassword
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeAlternateServiceAccountCredentialPassword"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeAlternateServiceAccountCredentialPassword"] = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new BitMaskAndFilter(ServerSchema.CurrentServerRole, 1UL),
					new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.LessThan, ServerSchema.VersionNumber, Server.E15MinVersion),
						new BitMaskAndFilter(ServerSchema.CurrentServerRole, 4UL)
					})
				});
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ClientAccessServer clientAccessServer = new ClientAccessServer((Server)dataObject);
			if (this.IncludeAlternateServiceAccountCredentialPassword.ToBool())
			{
				SetClientAccessServer.EnsureRunningOnTargetServer(this, (Server)dataObject);
				clientAccessServer.AlternateServiceAccountConfiguration = AlternateServiceAccountConfiguration.LoadWithPasswordsFromRegistry();
			}
			else if (this.IncludeAlternateServiceAccountCredentialStatus.ToBool())
			{
				clientAccessServer.AlternateServiceAccountConfiguration = AlternateServiceAccountConfiguration.LoadFromRegistry(clientAccessServer.Fqdn);
			}
			IConfigurable[] array = this.ConfigurationSession.Find<ADRpcHttpVirtualDirectory>((ADObjectId)clientAccessServer.Identity, QueryScope.SubTree, null, null, 1);
			clientAccessServer.OutlookAnywhereEnabled = new bool?(array.Length > 0);
			QueryFilter filter = ExchangeScpObjects.AutodiscoverUrlKeyword.Filter;
			array = this.ConfigurationSession.Find<ADServiceConnectionPoint>((ADObjectId)clientAccessServer.Identity, QueryScope.SubTree, filter, null, 2);
			if (array.Length == 1)
			{
				ADServiceConnectionPoint adserviceConnectionPoint = array[0] as ADServiceConnectionPoint;
				if (adserviceConnectionPoint.ServiceBindingInformation.Count > 0)
				{
					clientAccessServer.AutoDiscoverServiceInternalUri = new Uri(adserviceConnectionPoint.ServiceBindingInformation[0]);
				}
				clientAccessServer.AutoDiscoverServiceGuid = new Guid?(GetClientAccessServer.ScpUrlGuid);
				clientAccessServer.AutoDiscoverServiceCN = Fqdn.Parse(adserviceConnectionPoint.ServiceDnsName);
				clientAccessServer.AutoDiscoverServiceClassName = adserviceConnectionPoint.ServiceClassName;
				if (adserviceConnectionPoint.Keywords != null && adserviceConnectionPoint.Keywords.Count > 1)
				{
					MultiValuedProperty<string> multiValuedProperty = null;
					foreach (string text in adserviceConnectionPoint.Keywords)
					{
						if (text.StartsWith("site=", StringComparison.OrdinalIgnoreCase))
						{
							if (multiValuedProperty == null)
							{
								multiValuedProperty = new MultiValuedProperty<string>();
							}
							multiValuedProperty.Add(text.Substring(5));
						}
					}
					if (multiValuedProperty != null && multiValuedProperty.Count > 0)
					{
						clientAccessServer.AutoDiscoverSiteScope = multiValuedProperty;
					}
				}
			}
			base.WriteResult(clientAccessServer);
			TaskLogger.LogExit();
		}

		private const string IncludeAlternateServiceAccountCredentialStatusTag = "IncludeAlternateServiceAccountCredentialStatus";

		private const string IncludeAlternateServiceAccountCredentialPasswordTag = "IncludeAlternateServiceAccountCredentialPassword";

		private static readonly Guid ScpUrlGuid = new Guid("77378F46-2C66-4aa9-A6A6-3E7A48B19596");
	}
}
