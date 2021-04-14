using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class FilteredViewHierarchyNotificationPayload : HierarchyNotificationPayload
	{
		[DataMember(Name = "Filter", IsRequired = false)]
		public string FilterString
		{
			get
			{
				return this.Filter.ToString();
			}
			set
			{
				this.Filter = (ViewFilter)Enum.Parse(typeof(ViewFilter), value);
			}
		}

		[IgnoreDataMember]
		internal ViewFilter Filter { get; set; }
	}
}
