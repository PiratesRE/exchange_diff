using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RoleGroupIdParameter : GroupIdParameter
	{
		public RoleGroupIdParameter(string identity) : base(identity)
		{
			GuidHelper.TryParseGuid(identity, out this.guid);
		}

		public RoleGroupIdParameter()
		{
		}

		public RoleGroupIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RoleGroupIdParameter(RoleGroup group) : base(group.Id)
		{
		}

		public RoleGroupIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
			GuidHelper.TryParseGuid(namedIdentity.Identity, out this.guid);
		}

		public RoleGroupIdParameter(Guid guid) : base(guid.ToString())
		{
			this.guid = guid;
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return RoleGroupIdParameter.AllowedRecipientTypes;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return RoleGroupIdParameter.GetRoleGroupFilter(base.AdditionalQueryFilter);
			}
		}

		public new static RoleGroupIdParameter Parse(string identity)
		{
			return new RoleGroupIdParameter(identity);
		}

		private bool HasEmptyGuid
		{
			get
			{
				return Guid.Empty.Equals(this.guid);
			}
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			notFoundReason = null;
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (!wrapper.HasElements())
			{
				wrapper = EnumerableWrapper<T>.GetWrapper(this.TryGetObjectsFromDC<T>(rootId, subTreeSession, optionalData));
			}
			if (!wrapper.HasElements() && !Guid.Empty.Equals(this.guid) && typeof(T).IsAssignableFrom(typeof(ADGroup)))
			{
				ADObjectId containerId;
				if (session.SessionSettings.CurrentOrganizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					containerId = session.GetConfigurationNamingContext();
				}
				else
				{
					containerId = session.SessionSettings.CurrentOrganizationId.ConfigurationUnit;
				}
				bool useGlobalCatalog = session.UseGlobalCatalog;
				bool useConfigNC = session.UseConfigNC;
				ADGroup adgroup = null;
				try
				{
					session.UseGlobalCatalog = true;
					session.UseConfigNC = false;
					adgroup = session.ResolveWellKnownGuid<ADGroup>(this.guid, containerId);
				}
				finally
				{
					session.UseGlobalCatalog = useGlobalCatalog;
					session.UseConfigNC = useConfigNC;
				}
				if (adgroup != null)
				{
					wrapper = EnumerableWrapper<T>.GetWrapper(new List<ADGroup>(1)
					{
						adgroup
					}.Cast<T>());
				}
			}
			return wrapper;
		}

		private IEnumerable<T> TryGetObjectsFromDC<T>(ADObjectId rootId, IDirectorySession subTreeSession, OptionalIdentityData optionalData) where T : IConfigurable, new()
		{
			if (rootId != null || this.HasEmptyGuid || Datacenter.GetExchangeSku() != Datacenter.ExchangeSku.Enterprise)
			{
				return EnumerableWrapper<T>.Empty;
			}
			rootId = subTreeSession.GetRootDomainNamingContext();
			if (rootId != null && optionalData != null && optionalData.RootOrgDomainContainerId != null)
			{
				optionalData.RootOrgDomainContainerId = null;
			}
			bool useGlobalCatalog = subTreeSession.UseGlobalCatalog;
			IEnumerable<T> exactMatchObjects;
			try
			{
				subTreeSession.UseGlobalCatalog = false;
				exactMatchObjects = base.GetExactMatchObjects<T>(rootId, subTreeSession, optionalData);
			}
			finally
			{
				subTreeSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return exactMatchObjects;
		}

		internal static QueryFilter GetRoleGroupFilter(QueryFilter additionalFilter)
		{
			return QueryFilter.AndTogether(new QueryFilter[]
			{
				additionalFilter,
				Filters.GetRecipientTypeDetailsFilterOptimization(RecipientTypeDetails.RoleGroup)
			});
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeRoleGroup(id);
		}

		private Guid guid = Guid.Empty;

		private new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.Group
		};
	}
}
