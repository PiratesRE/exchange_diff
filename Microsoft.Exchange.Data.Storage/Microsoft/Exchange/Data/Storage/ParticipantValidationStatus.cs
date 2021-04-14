using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum ParticipantValidationStatus
	{
		NoError,
		AddressAndRoutingTypeMismatch = 16,
		AddressRequiredForRoutingType = 32,
		DisplayNameRequiredForRoutingType = 37,
		RoutingTypeRequired = 48,
		InvalidAddressFormat = 64,
		InvalidRoutingTypeFormat = 80,
		AddressAndOriginMismatch = 96,
		OperationNotSupportedForRoutingType = 112,
		DisplayNameTooBig = 128,
		EmailAddressTooBig = 144,
		RoutingTypeTooBig = 160
	}
}
