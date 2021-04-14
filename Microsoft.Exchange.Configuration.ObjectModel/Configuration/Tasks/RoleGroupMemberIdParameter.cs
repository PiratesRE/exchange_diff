using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RoleGroupMemberIdParameter : RecipientIdParameter
	{
		public RoleGroupMemberIdParameter(string identity) : base(identity)
		{
		}

		public RoleGroupMemberIdParameter()
		{
		}

		public RoleGroupMemberIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RoleGroupMemberIdParameter(RoleGroup group) : base(group.Id)
		{
		}

		public RoleGroupMemberIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return GeneralRecipientIdParameter.AllowedRecipientTypes;
			}
		}

		public new static RoleGroupMemberIdParameter Parse(string identity)
		{
			return new RoleGroupMemberIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (typeof(T) != typeof(ReducedRecipient))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			OptionalIdentityData optionalIdentityData;
			if (optionalData == null)
			{
				optionalIdentityData = new OptionalIdentityData();
			}
			else
			{
				optionalIdentityData = optionalData.Clone();
			}
			RoleGroupIdParameter @object = new RoleGroupIdParameter(base.RawIdentity);
			ADGroup object2 = base.GetObject<ADGroup>(rootId, session, subTreeSession, optionalIdentityData, new RecipientIdParameter.GetObjectsDelegate<ADGroup>(@object.GetObjects<ADGroup>), out notFoundReason);
			if (object2 == null)
			{
				return EnumerableWrapper<T>.Empty;
			}
			optionalIdentityData.RootOrgDomainContainerId = null;
			IDirectorySession directorySession = TaskHelper.UnderscopeSessionToOrganization(session, object2.OrganizationId, true);
			IDirectorySession reducedRecipientSession = DirectorySessionFactory.Default.GetReducedRecipientSession((IRecipientSession)directorySession, 160, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\RecipientParameters\\RoleGroupMemberIdParameter.cs");
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MemberOfGroup, object2.Id);
			return base.PerformPrimarySearch<T>(filter, null, reducedRecipientSession, true, optionalIdentityData);
		}
	}
}
