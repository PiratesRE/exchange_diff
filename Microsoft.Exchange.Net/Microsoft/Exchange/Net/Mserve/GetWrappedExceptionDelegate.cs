using System;

namespace Microsoft.Exchange.Net.Mserve
{
	internal delegate Exception GetWrappedExceptionDelegate(Exception wcfException, string targetInfo);
}
