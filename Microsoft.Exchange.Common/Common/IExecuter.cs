using System;

namespace Microsoft.Exchange.Common
{
	internal interface IExecuter
	{
		void Execute(Action wrappedCall);
	}
}
