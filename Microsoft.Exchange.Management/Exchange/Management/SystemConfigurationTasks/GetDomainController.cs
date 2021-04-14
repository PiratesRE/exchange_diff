using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "DomainController", DefaultParameterSetName = "DomainController")]
	public sealed class GetDomainController : GetTaskBase<ADServer>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "GlobalCatalog")]
		public SwitchParameter GlobalCatalog
		{
			get
			{
				return (SwitchParameter)(base.Fields["GlobalCatalog"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["GlobalCatalog"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DomainController")]
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

		[Parameter(Mandatory = false, ParameterSetName = "GlobalCatalog")]
		public Fqdn Forest
		{
			get
			{
				return (Fqdn)base.Fields["Forest"];
			}
			set
			{
				base.Fields["Forest"] = value;
			}
		}

		[Parameter]
		public NetworkCredential Credential
		{
			get
			{
				return (NetworkCredential)base.Fields["Credential"];
			}
			set
			{
				base.Fields["Credential"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 84, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\ExchangeServer\\GetDomainController.cs");
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || typeof(NotSupportedException).IsInstanceOfType(exception);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			ADDomainController dataObject2 = new ADDomainController((ADServer)dataObject);
			base.WriteResult(dataObject2);
		}

		protected override void InternalBeginProcessing()
		{
			if (this.Credential != null && this.DomainName == null && this.Forest == null)
			{
				base.WriteError(new ArgumentException(Strings.CannotOnlySpecifyCredential), ErrorCategory.InvalidArgument, this);
			}
			base.InternalBeginProcessing();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			ADForest adforest;
			if (this.Forest == null)
			{
				adforest = ADForest.GetLocalForest();
			}
			else
			{
				adforest = ADForest.GetForest(this.Forest, this.Credential);
			}
			List<ADServer> list = new List<ADServer>();
			if (this.GlobalCatalog)
			{
				list.AddRange(adforest.FindAllGlobalCatalogs(false));
			}
			else
			{
				if (this.DomainName == null)
				{
					using (ReadOnlyCollection<ADDomain>.Enumerator enumerator = adforest.FindDomains().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ADDomain addomain = enumerator.Current;
							list.AddRange(addomain.FindAllDomainControllers(false));
						}
						goto IL_F3;
					}
				}
				ADDomain addomain2;
				if (this.Credential == null)
				{
					addomain2 = adforest.FindDomainByFqdn(this.DomainName.ToString());
				}
				else
				{
					addomain2 = ADForest.FindExternalDomain(this.DomainName.ToString(), this.Credential);
				}
				if (addomain2 != null)
				{
					list.AddRange(addomain2.FindAllDomainControllers(false));
				}
				else
				{
					base.WriteError(new DomainNotFoundException(this.DomainName.ToString()), ErrorCategory.InvalidArgument, null);
				}
			}
			IL_F3:
			this.WriteResult<ADServer>(list);
			TaskLogger.LogExit();
		}
	}
}
