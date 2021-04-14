using System;

namespace Microsoft.Exchange.Diagnostics
{
	[CLSCompliant(true)]
	public interface IOrganizationIdForEventLog
	{
		string IdForEventLog { get; }
	}
}
