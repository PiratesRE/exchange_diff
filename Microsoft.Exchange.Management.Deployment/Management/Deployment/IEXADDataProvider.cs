using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IEXADDataProvider
	{
		RawSecurityDescriptor GetSystemConfigurationSecurityDescriptor(string distinguishedName);

		ADRawEntry SystemConfigurationRunQuery(bool useGC, PropertyDefinition[] propertyBags);

		ADRawEntry[] SystemConfigurationRunQuery(bool useGC, PropertyDefinition[] propertyBags, QueryScope queryScope, SortBy sortBy, QueryFilter queryFilter);

		RawSecurityDescriptor GetTopologyConfigurationSecurityDescriptor(string distinguishedName);

		ADRawEntry TopologyConfigurationRunQuery(bool useGC, PropertyDefinition[] propertyBags);

		ADRawEntry[] TopologyConfigurationRunQuery(bool useGC, PropertyDefinition[] propertyBags, QueryScope queryScope, SortBy sortBy, QueryFilter queryFilter);
	}
}
