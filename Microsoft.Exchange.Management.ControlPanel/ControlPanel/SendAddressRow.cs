using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SendAddressRow : DropDownItemData
	{
		public SendAddressRow(SendAddress sendAddress)
		{
			base.Text = sendAddress.DisplayName;
			base.Value = sendAddress.AddressId;
		}
	}
}
