using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	public delegate LocalizedString ValidationErrorStringProvider(string objectName, string cmdLet, string parameters, string capabilities);
}
