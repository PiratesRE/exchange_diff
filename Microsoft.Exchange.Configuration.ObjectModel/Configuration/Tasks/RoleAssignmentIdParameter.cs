using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RoleAssignmentIdParameter : ADIdParameter
	{
		public RoleAssignmentIdParameter()
		{
		}

		public RoleAssignmentIdParameter(string identity) : base(identity)
		{
		}

		public RoleAssignmentIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RoleAssignmentIdParameter(ExchangeRoleAssignment roleAssignment) : base(roleAssignment.Id)
		{
		}

		public RoleAssignmentIdParameter(ExchangeRoleAssignmentPresentation roleAssignmentPresentation) : base(roleAssignmentPresentation.Id)
		{
			this.user = roleAssignmentPresentation.User;
			this.assignmentMethod = roleAssignmentPresentation.AssignmentMethod;
		}

		public RoleAssignmentIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public AssignmentMethod AssignmentMethod
		{
			get
			{
				return this.assignmentMethod;
			}
		}

		public ADObjectId User
		{
			get
			{
				return this.user;
			}
		}

		internal QueryFilter InternalFilter
		{
			get
			{
				return this.internalFilter;
			}
			set
			{
				this.internalFilter = value;
			}
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					this.internalFilter
				});
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		public static RoleAssignmentIdParameter Parse(string identity)
		{
			return new RoleAssignmentIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			IEnumerable<T> objects = base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			EnumerableWrapper<T> wrapper = EnumerableWrapper<T>.GetWrapper(objects);
			if (!wrapper.HasElements())
			{
				LocalizedString value;
				if (((IConfigDataProvider)session).Source != null)
				{
					value = Strings.ErrorManagementObjectNotFoundWithSource(this.ToString(), ((IConfigDataProvider)session).Source);
				}
				else
				{
					value = Strings.ErrorManagementObjectNotFound(this.ToString());
				}
				if (notFoundReason != null)
				{
					string notFound = value;
					LocalizedString? localizedString = notFoundReason;
					value = Strings.ErrorNotFoundWithReason(notFound, (localizedString != null) ? localizedString.GetValueOrDefault() : null);
				}
				notFoundReason = new LocalizedString?(Strings.ErrorRoleAssignmentNotFound(value));
			}
			return objects;
		}

		private AssignmentMethod assignmentMethod = AssignmentMethod.Direct;

		private ADObjectId user;

		private QueryFilter internalFilter;
	}
}
