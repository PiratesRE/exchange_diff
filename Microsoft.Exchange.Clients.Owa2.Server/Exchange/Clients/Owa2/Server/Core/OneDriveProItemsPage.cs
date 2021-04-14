using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class OneDriveProItemsPage
	{
		public OneDriveProItemsPage()
		{
		}

		internal OneDriveProItemsPage(int pageIndex, IListItem item)
		{
			this.PageIndex = pageIndex;
			this.ID = item["ID"].ToString();
			this.Name = (string)item["FileLeafRef"];
			this.ObjectType = (string)item["FSObjType"];
			this.SortBehavior = ((FieldLookupValue)item["SortBehavior"]).LookupValue;
		}

		[DataMember]
		public int PageIndex { get; set; }

		[DataMember]
		public string ID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ObjectType { get; set; }

		[DataMember]
		public string SortBehavior { get; set; }
	}
}
