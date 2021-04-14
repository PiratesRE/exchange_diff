using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	internal class EnhancedEnumParameter : EnumParameter
	{
		public EnhancedEnumParameter(string name, LocalizedString dialogTitle, LocalizedString dialogLabel, string servicePath, LocalizedString noSelectionWarningText, string defaultValue) : base(name, dialogTitle, dialogLabel, defaultValue)
		{
			this.ServicePath = servicePath;
			this.locNoSelectionWarningText = noSelectionWarningText;
			base.FormletType = typeof(EnhancedEnumComboBoxModalEditor);
		}

		[DataMember]
		public string ServicePath { get; private set; }

		[DataMember]
		public string NoSelectionWarningText
		{
			get
			{
				return this.locNoSelectionWarningText.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private LocalizedString locNoSelectionWarningText;
	}
}
