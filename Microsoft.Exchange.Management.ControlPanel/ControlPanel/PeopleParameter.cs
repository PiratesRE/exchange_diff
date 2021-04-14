using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class PeopleParameter : ObjectArrayParameter
	{
		public PeopleParameter(string name, PickerType pickerType, LocalizedString noSelectionText) : base(name, LocalizedString.Empty, LocalizedString.Empty)
		{
			this.PickerType = pickerType.ToString();
			if (string.IsNullOrEmpty(noSelectionText))
			{
				this.noSelectionText = Strings.TransportRulePeopleParameterNoSelectionText;
			}
			else
			{
				this.noSelectionText = noSelectionText;
			}
			base.FormletType = typeof(PeoplePicker);
		}

		public PeopleParameter(string name, PickerType pickerType) : this(name, pickerType, LocalizedString.Empty)
		{
		}

		public PeopleParameter(string name) : this(name, Microsoft.Exchange.Management.ControlPanel.PickerType.PickTo)
		{
		}

		[DataMember]
		public string PickerType { get; private set; }
	}
}
