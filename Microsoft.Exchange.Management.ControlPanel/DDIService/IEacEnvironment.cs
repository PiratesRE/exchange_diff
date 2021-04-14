using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public interface IEacEnvironment
	{
		bool IsForefrontForOffice { get; }

		bool IsDataCenter { get; }
	}
}
