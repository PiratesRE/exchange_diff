using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ObjectsParameter : ObjectArrayParameter
	{
		public ObjectsParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, LocalizedString noSelectionText, string pickerUrl) : base(name, dialogTitle, dialogLabel)
		{
			this.PickerUrl = pickerUrl;
			this.noSelectionText = noSelectionText;
			base.FormletType = typeof(ObjectPicker);
		}

		[DataMember]
		public string PickerUrl { get; private set; }

		[DataMember]
		public string ValueProperty { get; set; }

		[DataMember]
		public int DialogWidth { get; set; }

		[DataMember]
		public int DialogHeight { get; set; }
	}
}
