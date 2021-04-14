using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum FindPlacesMetadata
	{
		[DisplayName("FPl", "QS")]
		QueryString,
		[DisplayName("FPl", "LT")]
		Latitude,
		[DisplayName("FPl", "LG")]
		Longitude,
		[DisplayName("FPl", "PBLT")]
		PhonebookLatency,
		[DisplayName("FPl", "LCLT")]
		LocationLatency,
		[DisplayName("FPl", "PBSC")]
		PhonebookStatusCode,
		[DisplayName("FPl", "LCSC")]
		LocationStatusCode,
		[DisplayName("FPl", "PBCT")]
		PhonebookResultsCount,
		[DisplayName("FPl", "LCCT")]
		LocationResultsCount,
		[DisplayName("FPl", "PBEM")]
		PhonebookErrorMessage,
		[DisplayName("FPl", "LCEM")]
		LocationErrorMessage,
		[DisplayName("FPl", "PBEC")]
		PhonebookErrorCode,
		[DisplayName("FPl", "LCEC")]
		LocationErrorCode,
		[DisplayName("FPl", "PBFailed")]
		PhonebookFailed,
		[DisplayName("FPl", "LCFailed")]
		LocationFailed
	}
}
