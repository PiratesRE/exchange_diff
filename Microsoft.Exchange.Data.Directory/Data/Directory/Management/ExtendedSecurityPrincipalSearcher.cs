using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal delegate IEnumerable<ExtendedSecurityPrincipal> ExtendedSecurityPrincipalSearcher(IConfigDataProvider session, ADObjectId rootId, QueryFilter targetFilter);
}
