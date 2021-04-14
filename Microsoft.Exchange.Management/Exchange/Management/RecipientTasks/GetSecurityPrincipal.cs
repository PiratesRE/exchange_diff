using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "SecurityPrincipal", DefaultParameterSetName = "Identity")]
	public sealed class GetSecurityPrincipal : GetMultitenancySystemConfigurationObjectTask<ExtendedSecurityPrincipalIdParameter, ExtendedSecurityPrincipal>
	{
		[Parameter]
		[ValidateNotNullOrEmpty]
		public ExtendedOrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return (ExtendedOrganizationalUnitIdParameter)base.Fields["OrganizationalUnit"];
			}
			set
			{
				base.Fields["OrganizationalUnit"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public SmtpDomain IncludeDomainLocalFrom
		{
			get
			{
				return (SmtpDomain)base.Fields["IncludeDomainLocalFrom"];
			}
			set
			{
				base.Fields["IncludeDomainLocalFrom"] = value;
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

		[Parameter]
		public MultiValuedProperty<SecurityPrincipalType> Types
		{
			get
			{
				return (MultiValuedProperty<SecurityPrincipalType>)base.Fields["Types"];
			}
			set
			{
				base.Fields["Types"] = value;
			}
		}

		[Parameter]
		public SwitchParameter RoleGroupAssignable
		{
			get
			{
				return (SwitchParameter)(base.Fields["RoleGroupAssignable"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["RoleGroupAssignable"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				MonadFilter monadFilter = new MonadFilter(value, this, ADRecipientProperties.Instance);
				this.inputFilter = monadFilter.InnerFilter;
				base.OptionalIdentityData.AdditionalFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				return this.rootId;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					this.inputFilter,
					base.InternalFilter
				});
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.NetCredential, base.SessionSettings, 177, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\GetSecurityPrincipal.cs");
			tenantOrRootOrgRecipientSession.EnforceDefaultScope = true;
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = (this.rootId == null);
			return DirectorySessionFactory.Default.GetReducedRecipientSession(tenantOrRootOrgRecipientSession, 187, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\GetSecurityPrincipal.cs");
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.OrganizationalUnit != null)
			{
				this.organizationalUnit = (ExtendedOrganizationalUnit)base.GetDataObject<ExtendedOrganizationalUnit>(this.OrganizationalUnit, this.ConfigurationSession, null, null, new LocalizedString?(Strings.ErrorManagementObjectAmbiguous(this.OrganizationalUnit.ToString())));
				this.rootId = this.organizationalUnit.Id;
			}
			if (this.IncludeDomainLocalFrom != null)
			{
				this.includeDomainLocalFrom = ADForest.GetLocalForest(this.ConfigurationSession.DomainController).FindDomainByFqdn(this.IncludeDomainLocalFrom.Domain);
				if (this.includeDomainLocalFrom == null)
				{
					base.WriteError(new DomainNotFoundException(this.IncludeDomainLocalFrom.Domain), ErrorCategory.InvalidArgument, this.IncludeDomainLocalFrom);
					TaskLogger.LogExit();
					return;
				}
			}
			if (this.Types == null || this.Types.Count == 0)
			{
				this.types = new MultiValuedProperty<SecurityPrincipalType>();
				this.types.Add(SecurityPrincipalType.WellknownSecurityPrincipal);
				this.types.Add(SecurityPrincipalType.User);
				this.types.Add(SecurityPrincipalType.Computer);
				this.types.Add(SecurityPrincipalType.Group);
			}
			else
			{
				this.types = this.Types;
			}
			if (this.Identity != null)
			{
				this.Identity.IncludeDomainLocalFrom = this.includeDomainLocalFrom;
				this.Identity.Types = this.types;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override IEnumerable<ExtendedSecurityPrincipal> GetPagedData()
		{
			ADObjectId includeDomailLocalFrom = (this.includeDomainLocalFrom != null) ? this.includeDomainLocalFrom.Id : null;
			return ExtendedSecurityPrincipalSearchHelper.PerformSearch(new ExtendedSecurityPrincipalSearcher(this.FindObjects), base.DataSession, this.RootId as ADObjectId, includeDomailLocalFrom, this.types);
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			if (!this.ShouldSkipObject(dataObject))
			{
				this.CompleteDataObject((ExtendedSecurityPrincipal)dataObject);
				base.WriteResult(dataObject);
			}
			TaskLogger.LogExit();
		}

		private void CompleteDataObject(ExtendedSecurityPrincipal dataObject)
		{
			dataObject.UserFriendlyName = SecurityPrincipalIdParameter.GetFriendlyUserName(dataObject.SID, null);
		}

		private bool ShouldSkipObject(IConfigurable dataObject)
		{
			if (this.RoleGroupAssignable)
			{
				ExtendedSecurityPrincipal extendedSecurityPrincipal = (ExtendedSecurityPrincipal)dataObject;
				if (Array.IndexOf<RecipientTypeDetails>(GetSecurityPrincipal.AllowedRecipientTypeDetails, extendedSecurityPrincipal.RecipientTypeDetails) == -1)
				{
					return true;
				}
			}
			return false;
		}

		private IEnumerable<ExtendedSecurityPrincipal> FindObjects(IConfigDataProvider session, ADObjectId rootId, QueryFilter targetFilter)
		{
			return session.FindPaged<ExtendedSecurityPrincipal>(QueryFilter.AndTogether(new QueryFilter[]
			{
				targetFilter,
				this.InternalFilter
			}), rootId, this.DeepSearch, this.InternalSortBy, this.PageSize);
		}

		private QueryFilter inputFilter;

		private ExtendedOrganizationalUnit organizationalUnit;

		private ADDomain includeDomainLocalFrom;

		private ObjectId rootId;

		private MultiValuedProperty<SecurityPrincipalType> types;

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.None,
			RecipientTypeDetails.UserMailbox,
			RecipientTypeDetails.LinkedMailbox,
			RecipientTypeDetails.SharedMailbox,
			RecipientTypeDetails.TeamMailbox,
			RecipientTypeDetails.LegacyMailbox,
			RecipientTypeDetails.MailUser,
			(RecipientTypeDetails)((ulong)int.MinValue),
			RecipientTypeDetails.RemoteSharedMailbox,
			RecipientTypeDetails.RemoteTeamMailbox,
			RecipientTypeDetails.MailUniversalSecurityGroup,
			RecipientTypeDetails.User,
			RecipientTypeDetails.UniversalSecurityGroup,
			RecipientTypeDetails.LinkedUser,
			RecipientTypeDetails.RoleGroup,
			RecipientTypeDetails.AllUniqueRecipientTypes
		};
	}
}
