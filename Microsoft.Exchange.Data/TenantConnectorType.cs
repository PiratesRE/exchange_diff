using System;

namespace Microsoft.Exchange.Data
{
	public enum TenantConnectorType
	{
		[LocDescription(DataStrings.IDs.ConnectorTypeOnPremises)]
		OnPremises = 1,
		[LocDescription(DataStrings.IDs.ConnectorTypePartner)]
		Partner
	}
}
