using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum DSNConversionOption
	{
		[LocDescription(DataStrings.IDs.UseExchangeDSNs)]
		UseExchangeDSNs = 0,
		[LocDescription(DataStrings.IDs.PreserveDSNBody)]
		PreserveDSNBody = 1,
		[LocDescription(DataStrings.IDs.DoNotConvert)]
		DoNotConvert = 2
	}
}
