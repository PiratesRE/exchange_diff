using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public sealed class ShouldContinueExceptionDetails
	{
		public ShouldContinueExceptionDetails(string cmdlet, string suppressConfirmParameterName)
		{
			this.CurrentCmdlet = cmdlet;
			this.SuppressConfirmParameterName = suppressConfirmParameterName;
		}

		public string CurrentCmdlet { get; set; }

		[DataMember]
		public string SuppressConfirmParameterName { get; set; }
	}
}
