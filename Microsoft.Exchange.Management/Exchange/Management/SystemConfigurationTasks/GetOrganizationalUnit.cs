using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OrganizationalUnit", DefaultParameterSetName = "Identity")]
	public sealed class GetOrganizationalUnit : GetMultitenancySystemConfigurationObjectTask<ExtendedOrganizationalUnitIdParameter, ExtendedOrganizationalUnit>
	{
		[Parameter(ParameterSetName = "Identity")]
		public SwitchParameter SingleNodeOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["SingleNodeOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SingleNodeOnly"] = value;
			}
		}

		[Parameter]
		public SwitchParameter IncludeContainers
		{
			get
			{
				return (SwitchParameter)(base.Fields["IncludeContainers"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IncludeContainers"] = value;
			}
		}

		[Parameter]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[Parameter(ParameterSetName = "SearchSet")]
		public string SearchText { get; set; }

		protected override ObjectId RootId
		{
			get
			{
				if (!this.IsTenant)
				{
					return base.RootId;
				}
				return base.CurrentOrganizationId.OrganizationalUnit;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
			if (this.Identity != null)
			{
				this.Identity.IncludeContainers = this.IncludeContainers;
				if (this.SingleNodeOnly)
				{
					IConfigurable dataObject = base.GetDataObject(this.Identity);
					base.WriteResult(dataObject);
					IEnumerable<ExtendedOrganizationalUnit> dataObjects = ExtendedOrganizationalUnit.FindFirstLevelChildOrganizationalUnit(this.IncludeContainers, configurationSession, dataObject.Identity as ADObjectId, this.InternalFilter, this.InternalSortBy, this.PageSize);
					base.WriteResult<ExtendedOrganizationalUnit>(dataObjects);
				}
				else
				{
					base.InternalProcessRecord();
				}
			}
			else
			{
				if (this.SingleNodeOnly)
				{
					if (this.IsTenant)
					{
						ExtendedOrganizationalUnit dataObject2 = configurationSession.Read<ExtendedOrganizationalUnit>((ADObjectId)this.RootId);
						this.WriteResult(dataObject2);
						goto IL_195;
					}
					ReadOnlyCollection<ADDomain> readOnlyCollection = ADForest.GetLocalForest(configurationSession.DomainController).FindTopLevelDomains();
					using (ReadOnlyCollection<ADDomain>.Enumerator enumerator = readOnlyCollection.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ADDomain addomain = enumerator.Current;
							ExtendedOrganizationalUnit dataObject3 = configurationSession.Read<ExtendedOrganizationalUnit>(addomain.Id);
							this.WriteResult(dataObject3);
						}
						goto IL_195;
					}
				}
				IEnumerable<ExtendedOrganizationalUnit> enumerable = ExtendedOrganizationalUnit.FindSubTreeChildOrganizationalUnit(this.IncludeContainers, configurationSession, this.IsTenant ? ((ADObjectId)this.RootId) : null, this.InternalFilter);
				if (!string.IsNullOrEmpty(this.SearchText))
				{
					string nameToSearch = this.SearchText.ToUpper();
					enumerable = from ou in enumerable
					where ou.CanonicalName.ToUpper().Contains(nameToSearch)
					select ou;
				}
				this.WriteResult<ExtendedOrganizationalUnit>(enumerable);
			}
			IL_195:
			TaskLogger.LogExit();
		}

		private bool IsTenant
		{
			get
			{
				return !base.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession;
			if (this.IsTenant)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 182, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\GetOrganizationalUnit.cs");
			}
			else
			{
				configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 192, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\GetOrganizationalUnit.cs");
			}
			configurationSession.UseConfigNC = false;
			configurationSession.UseGlobalCatalog = true;
			configurationSession.EnforceDefaultScope = false;
			return configurationSession;
		}
	}
}
