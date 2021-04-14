using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[DataContract]
	public class DropDownItemData : BaseRow
	{
		public DropDownItemData(string text, string value)
		{
			this.Text = text;
			this.Value = value;
		}

		protected DropDownItemData() : base(null, null)
		{
		}

		protected DropDownItemData(ADObject adObject) : base(adObject)
		{
		}

		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public bool Selected { get; set; }
	}
}
