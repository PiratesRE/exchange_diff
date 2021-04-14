using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class ExtendedOrganizationalUnitIdParameter : OrganizationalUnitIdParameterBase
	{
		public ExtendedOrganizationalUnitIdParameter()
		{
		}

		public ExtendedOrganizationalUnitIdParameter(string identity) : base(identity)
		{
		}

		public ExtendedOrganizationalUnitIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public ExtendedOrganizationalUnitIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public ExtendedOrganizationalUnitIdParameter(ExtendedOrganizationalUnit organizationalUnit) : base(organizationalUnit.Id)
		{
		}

		internal bool IncludeContainers
		{
			get
			{
				return this.includeContainers;
			}
			set
			{
				this.includeContainers = value;
			}
		}

		public static ExtendedOrganizationalUnitIdParameter Parse(string identity)
		{
			return new ExtendedOrganizationalUnitIdParameter(identity);
		}

		internal override IEnumerable<T> PerformSearch<T>(QueryFilter filter, ADObjectId rootId, IDirectorySession session, bool deepSearch)
		{
			TaskLogger.LogEnter();
			if (!typeof(ExtendedOrganizationalUnit).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			bool useGlobalCatalog = session.UseGlobalCatalog;
			bool useConfigNC = session.UseConfigNC;
			IEnumerable<T> result = null;
			try
			{
				session.UseGlobalCatalog = true;
				session.UseConfigNC = false;
				result = (deepSearch ? ((IEnumerable<T>)ExtendedOrganizationalUnit.FindSubTreeChildOrganizationalUnit(this.IncludeContainers, (IConfigurationSession)session, rootId, filter)) : ((IEnumerable<T>)ExtendedOrganizationalUnit.FindFirstLevelChildOrganizationalUnit(this.IncludeContainers, (IConfigurationSession)session, rootId, filter, null, 0)));
			}
			finally
			{
				session.UseGlobalCatalog = useGlobalCatalog;
				session.UseConfigNC = useConfigNC;
			}
			TaskLogger.LogExit();
			return result;
		}

		private bool includeContainers = true;
	}
}
