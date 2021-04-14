using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class DLPParameter : FormletParameter
	{
		public DLPParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel) : this(name, dialogTitle, dialogLabel, LocalizedString.Empty)
		{
		}

		public DLPParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText) : base(name, dialogTitle, dialogLabel)
		{
			base.FormletType = typeof(DLPModalEditor);
			if (string.IsNullOrEmpty(noSelectionText))
			{
				this.noSelectionText = Strings.TransportRuleContainsSensitiveInformationParameterNoSelectionText;
				return;
			}
			this.noSelectionText = noSelectionText;
		}

		[DataMember]
		public string ClassificationNameKey
		{
			get
			{
				return "displayName";
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
