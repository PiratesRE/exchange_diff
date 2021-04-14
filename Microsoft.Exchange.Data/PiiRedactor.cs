using System;

namespace Microsoft.Exchange.Data
{
	public delegate T PiiRedactor<T>(T value, out string rawPii, out string redactedPii);
}
