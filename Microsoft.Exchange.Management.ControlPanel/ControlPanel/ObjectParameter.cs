using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ObjectParameter : FormletParameter
	{
		public ObjectParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType) : this(name, dialogTitle, dialogLabel, objectType, "~/Pickers/PeoplePicker.aspx", "Identity")
		{
		}

		public ObjectParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, string pickerUrl) : this(name, dialogTitle, dialogLabel, objectType, pickerUrl, "Identity")
		{
		}

		public ObjectParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, Type objectType, string pickerUrl, string valueProperty) : base(name, dialogTitle, dialogLabel)
		{
			this.PickerUrl = pickerUrl;
			base.FormletType = typeof(ObjectPicker);
			this.ValueProperty = valueProperty;
		}

		[DataMember]
		public string ValueProperty { get; private set; }

		[DataMember]
		public string PickerUrl { get; private set; }
	}
}
