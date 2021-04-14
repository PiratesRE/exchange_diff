using System;

namespace Microsoft.Exchange.Data
{
	public interface IErrorContextException
	{
		void SetContext(IErrorExecutionContext context);
	}
}
