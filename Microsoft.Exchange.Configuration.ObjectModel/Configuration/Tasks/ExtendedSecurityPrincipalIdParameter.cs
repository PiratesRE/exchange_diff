using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class ExtendedSecurityPrincipalIdParameter : ADIdParameter
	{
		public ExtendedSecurityPrincipalIdParameter()
		{
		}

		public ExtendedSecurityPrincipalIdParameter(string identity) : base(identity)
		{
		}

		public ExtendedSecurityPrincipalIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ExtendedSecurityPrincipalIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ADDomain IncludeDomainLocalFrom { get; set; }

		public MultiValuedProperty<SecurityPrincipalType> Types { get; set; }

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					this.securityTargetFilter
				});
			}
		}

		public static ExtendedSecurityPrincipalIdParameter Parse(string identity)
		{
			return new ExtendedSecurityPrincipalIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			if (!typeof(T).IsAssignableFrom(typeof(ExtendedSecurityPrincipal)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			IEnumerable<T> result = ExtendedSecurityPrincipalSearchHelper.PerformSearch(new ExtendedSecurityPrincipalSearcher(this.FindObjects), (IConfigDataProvider)session, rootId, (this.IncludeDomainLocalFrom != null) ? this.IncludeDomainLocalFrom.Id : null, this.Types).Cast<T>();
			TaskLogger.LogExit();
			notFoundReason = null;
			return result;
		}

		private IEnumerable<ExtendedSecurityPrincipal> FindObjects(IConfigDataProvider session, ADObjectId rootId, QueryFilter targetFilter)
		{
			this.securityTargetFilter = targetFilter;
			LocalizedString? localizedString;
			return base.GetObjects<ExtendedSecurityPrincipal>(rootId, (IDirectorySession)session, (IDirectorySession)session, null, out localizedString);
		}

		private QueryFilter securityTargetFilter;
	}
}
