using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	public enum OperationBit
	{
		Find = 1,
		Save,
		Read = 4,
		FindPaged = 8,
		Delete = 16
	}
}
