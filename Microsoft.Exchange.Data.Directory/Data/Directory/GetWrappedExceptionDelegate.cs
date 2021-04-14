using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal delegate Exception GetWrappedExceptionDelegate(Exception wcfException, string targetInfo);
}
