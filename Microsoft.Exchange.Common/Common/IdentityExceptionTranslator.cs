using System;

namespace Microsoft.Exchange.Common
{
	internal sealed class IdentityExceptionTranslator : IExceptionTranslator
	{
		public bool TryTranslate(Exception exception, out Exception translatedException)
		{
			translatedException = null;
			return false;
		}
	}
}
