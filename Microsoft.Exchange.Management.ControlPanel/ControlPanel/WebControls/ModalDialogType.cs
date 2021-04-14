using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[DataContract]
	public enum ModalDialogType
	{
		[EnumMember(Value = "Error")]
		Error,
		[EnumMember(Value = "Warning")]
		Warning,
		[EnumMember(Value = "Information")]
		Information
	}
}
