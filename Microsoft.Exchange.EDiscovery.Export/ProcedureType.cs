using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal enum ProcedureType : uint
	{
		Prepare = 1U,
		Export,
		Stop,
		Rollback
	}
}
