using System;

namespace Microsoft.Exchange.BITS
{
	internal enum BG_CERT_STORE_LOCATION
	{
		BG_CERT_STORE_LOCATION_CURRENT_USER,
		BG_CERT_STORE_LOCATION_LOCAL_MACHINE,
		BG_CERT_STORE_LOCATION_CURRENT_SERVICE,
		BG_CERT_STORE_LOCATION_SERVICES,
		BG_CERT_STORE_LOCATION_USERS,
		BG_CERT_STORE_LOCATION_CURRENT_USER_GROUP_POLICY,
		BG_CERT_STORE_LOCATION_LOCAL_MACHINE_GROUP_POLICY,
		BG_CERT_STORE_LOCATION_LOCAL_MACHINE_ENTERPRISE
	}
}
