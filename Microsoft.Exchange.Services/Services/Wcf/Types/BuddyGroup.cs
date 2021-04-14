using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class BuddyGroup
	{
		internal BuddyGroup(StoreId id, string displayName)
		{
			this.Id = id.ToString();
			this.DisplayName = displayName;
			this.PersonaIds = new ItemId[0];
		}

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public ItemId[] PersonaIds { get; internal set; }
	}
}
