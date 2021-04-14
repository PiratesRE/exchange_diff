using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[DataContract]
	public sealed class InfoCore
	{
		[DataMember(EmitDefaultValue = false)]
		public string JsonTitle { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Message { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Details { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ModalDialogType MessageBoxType { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Help { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string HelpUrl { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string StackTrace { get; set; }
	}
}
