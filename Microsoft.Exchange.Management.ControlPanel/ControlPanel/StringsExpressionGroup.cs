using System;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class StringsExpressionGroup
	{
		public StringsExpressionGroup(Type stringsType, Type idsType)
		{
			this.StringsType = stringsType;
			this.IdsType = idsType;
		}

		public Type StringsType { get; set; }

		public Type IdsType { get; set; }
	}
}
