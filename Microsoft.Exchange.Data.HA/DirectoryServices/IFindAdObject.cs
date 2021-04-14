using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Performance;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IFindAdObject<TADWrapperObject> where TADWrapperObject : class, IADObjectCommon
	{
		IADToplogyConfigurationSession AdSession { get; }

		void Clear();

		TADWrapperObject ReadAdObjectByObjectId(ADObjectId objectId);

		TADWrapperObject ReadAdObjectByObjectIdEx(ADObjectId objectId, out Exception ex);

		TADWrapperObject FindAdObjectByGuid(Guid objectGuid);

		TADWrapperObject FindAdObjectByGuidEx(Guid objectGuid, AdObjectLookupFlags flags);

		TADWrapperObject FindAdObjectByGuidEx(Guid objectGuid, AdObjectLookupFlags flags, IPerformanceDataLogger perfLogger);

		TADWrapperObject FindAdObjectByQuery(QueryFilter queryFilter);

		TADWrapperObject FindAdObjectByQueryEx(QueryFilter queryFilter, AdObjectLookupFlags flags);

		TADWrapperObject FindServerByFqdn(string fqdn);

		TADWrapperObject FindServerByFqdnWithException(string fqdn, out Exception ex);
	}
}
