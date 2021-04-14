using System;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal interface IAcceptedDomain
	{
		string DomainNameDomain { get; }

		bool IsCoexistenceDomain { get; }
	}
}
