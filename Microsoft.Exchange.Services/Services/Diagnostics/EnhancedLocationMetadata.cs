using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum EnhancedLocationMetadata
	{
		[DisplayName("ELC", "HLC")]
		HasLocation,
		[DisplayName("ELC", "SRC")]
		LocationSource,
		[DisplayName("ELC", "HAN")]
		HasAnnotation,
		[DisplayName("ELC", "HCD")]
		HasCoordinates
	}
}
