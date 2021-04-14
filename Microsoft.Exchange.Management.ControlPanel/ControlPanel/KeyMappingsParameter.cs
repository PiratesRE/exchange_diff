using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class KeyMappingsParameter : FormletParameter
	{
		public KeyMappingsParameter(string name) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
			this.noSelectionText = Strings.KeyMappingsParameterNoSelectionText;
			base.FormletType = typeof(KeyMappingsEditor);
		}
	}
}
