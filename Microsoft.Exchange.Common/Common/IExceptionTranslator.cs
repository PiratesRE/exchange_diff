using System;

namespace Microsoft.Exchange.Common
{
	internal interface IExceptionTranslator
	{
		bool TryTranslate(Exception exception, out Exception translatedException);
	}
}
