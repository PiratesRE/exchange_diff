using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class ExtensionsDialedParameter : FormletParameter
	{
		public ExtensionsDialedParameter(string name) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
			this.noSelectionText = Strings.ExtensionsDialedParameterNoSelectionText;
			base.FormletType = typeof(ExtensionsDialedEditor);
		}
	}
}
