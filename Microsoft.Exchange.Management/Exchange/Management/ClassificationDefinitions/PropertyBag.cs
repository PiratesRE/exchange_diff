using System;
using Microsoft.Mce.Interop.Api;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class PropertyBag : IPropertyBag
	{
		public int Read(string propertyName, ref object propertyValue, IErrorLog errorLog)
		{
			if (string.Equals(propertyName, "UseLazyRegexCompilation", StringComparison.OrdinalIgnoreCase))
			{
				propertyValue = false;
				return 0;
			}
			return -2147467259;
		}

		public int Write(string propertyName, ref object propertyValue)
		{
			return -2147467263;
		}
	}
}
