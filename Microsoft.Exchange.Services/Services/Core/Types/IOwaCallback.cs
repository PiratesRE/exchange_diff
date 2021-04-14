using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IOwaCallback
	{
		void ProcessCallback(object owaContext);
	}
}
