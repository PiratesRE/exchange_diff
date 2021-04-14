using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[DataContract]
	public class GroupedCheckBoxTreeItem : GroupedCheckBoxListItem
	{
		[DataMember]
		public string Parent { get; set; }

		public GroupedCheckBoxTreeItem() : base(null, null)
		{
		}

		public GroupedCheckBoxTreeItem(Identity identity, ADObject configurationObject) : base(identity, configurationObject)
		{
		}

		public GroupedCheckBoxTreeItem(ADObject configurationObject) : base(configurationObject)
		{
		}
	}
}
