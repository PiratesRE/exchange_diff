using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SenderNotifyParameter : FormletParameter
	{
		public SenderNotifyParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText, string[] taskParameterNames) : base(name, dialogTitle, dialogLabel, taskParameterNames)
		{
			base.FormletType = typeof(SenderNotifyEditor);
			if (string.IsNullOrEmpty(noSelectionText))
			{
				this.noSelectionText = Strings.TransportRuleContainsSensitiveInformationParameterNoSelectionText;
			}
			else
			{
				this.noSelectionText = noSelectionText;
			}
			Array values = Enum.GetValues(typeof(NotifySenderType));
			EnumValue[] array = new EnumValue[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = new EnumValue((Enum)values.GetValue(i));
			}
			this.Values = array;
		}

		[DataMember]
		public EnumValue[] Values { get; internal set; }
	}
}
