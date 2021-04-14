using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "Trust", DefaultParameterSetName = "Trust")]
	public sealed class GetTrust : GetTaskBase<ADDomainTrustInfo>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter]
		public Fqdn DomainName
		{
			get
			{
				return (Fqdn)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 58, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ExchangeServer\\GetTrust.cs");
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADForest localForest = ADForest.GetLocalForest();
			if (this.DomainName != null)
			{
				this.WriteResult<ADDomainTrustInfo>(localForest.FindAllTrustedForests());
				this.WriteResult<ADDomainTrustInfo>(localForest.FindTrustedDomains(this.DomainName));
			}
			else
			{
				HashSet<string> hashSet = new HashSet<string>();
				foreach (ADDomainTrustInfo addomainTrustInfo in localForest.FindAllTrustedForests())
				{
					hashSet.Add(addomainTrustInfo.Name);
					this.WriteResult(addomainTrustInfo);
				}
				foreach (ADDomain addomain in localForest.FindDomains())
				{
					foreach (ADDomainTrustInfo addomainTrustInfo2 in localForest.FindTrustedDomains(addomain.Fqdn))
					{
						if (!hashSet.Contains(addomainTrustInfo2.Name))
						{
							hashSet.Add(addomainTrustInfo2.Name);
							this.WriteResult(addomainTrustInfo2);
						}
					}
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteResult(new ADTrust(dataObject as ADDomainTrustInfo));
		}
	}
}
