using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMessageOptionsConfiguration : SetMessagingConfigurationBase
	{
		[DataMember]
		public string AfterMoveOrDeleteBehavior
		{
			get
			{
				return (string)base["AfterMoveOrDeleteBehavior"];
			}
			set
			{
				base["AfterMoveOrDeleteBehavior"] = value;
			}
		}

		[DataMember]
		public int NewItemNotification
		{
			get
			{
				return (int)(base["NewItemNotification"] ?? 15);
			}
			set
			{
				base["NewItemNotification"] = value;
			}
		}

		[DataMember]
		public bool EmptyDeletedItemsOnLogoff
		{
			get
			{
				return (bool)(base["EmptyDeletedItemsOnLogoff"] ?? false);
			}
			set
			{
				base["EmptyDeletedItemsOnLogoff"] = value;
			}
		}

		[DataMember]
		public bool CheckForForgottenAttachments
		{
			get
			{
				return (bool)(base["CheckForForgottenAttachments"] ?? false);
			}
			set
			{
				base["CheckForForgottenAttachments"] = value;
			}
		}
	}
}
