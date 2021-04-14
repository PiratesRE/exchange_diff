using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetConversationsConfiguration : SetMessagingConfigurationBase
	{
		[DataMember]
		public string ConversationSortOrder
		{
			get
			{
				return (string)base["ConversationSortOrder"];
			}
			set
			{
				base["ConversationSortOrder"] = value;
			}
		}

		[DataMember]
		public bool ShowConversationAsTree
		{
			get
			{
				return (bool)(base["ShowConversationAsTree"] ?? false);
			}
			set
			{
				base["ShowConversationAsTree"] = value;
			}
		}

		[DataMember]
		public bool HideDeletedItems
		{
			get
			{
				return (bool)(base["HideDeletedItems"] ?? false);
			}
			set
			{
				base["HideDeletedItems"] = value;
			}
		}
	}
}
