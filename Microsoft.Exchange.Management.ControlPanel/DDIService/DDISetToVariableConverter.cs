using System;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDISetToVariableConverter : IDDIValidationObjectConverter
	{
		public object ConvertTo(object obj)
		{
			return (obj as Set).Variable;
		}
	}
}
