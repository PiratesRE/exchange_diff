using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[DataContract]
	public class GroupedCheckBoxListItem : BaseRow
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string Group { get; set; }

		public GroupedCheckBoxListItem() : base(null, null)
		{
		}

		public GroupedCheckBoxListItem(Identity identity, ADObject configurationObject) : base(identity, configurationObject)
		{
		}

		public GroupedCheckBoxListItem(ADObject configurationObject) : base(configurationObject)
		{
		}
	}
}
