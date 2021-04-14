using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class ADAttributeParameter : FormletParameter
	{
		public ADAttributeParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel) : this(name, dialogTitle, dialogLabel, LocalizedString.Empty)
		{
		}

		public ADAttributeParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText) : base(name, dialogTitle, dialogLabel)
		{
			base.FormletType = typeof(ADAttributeModalEditor);
			if (string.IsNullOrEmpty(noSelectionText))
			{
				this.noSelectionText = Strings.TransportRuleADAttributeParameterNoSelectionText;
				return;
			}
			this.noSelectionText = noSelectionText;
		}
	}
}
