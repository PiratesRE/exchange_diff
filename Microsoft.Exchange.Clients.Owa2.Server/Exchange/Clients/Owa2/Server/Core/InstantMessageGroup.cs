using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InstantMessageGroup : IComparable<InstantMessageGroup>
	{
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false, Order = 2)]
		public string Id { get; set; }

		[IgnoreDataMember]
		public InstantMessageGroupType GroupType { get; set; }

		[DataMember(Name = "GroupType", Order = 3)]
		public string GroupTypeString
		{
			get
			{
				return this.GroupType.ToString();
			}
			set
			{
				this.GroupType = InstantMessageUtilities.ParseEnumValue<InstantMessageGroupType>(value, InstantMessageGroupType.Standard);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 4)]
		public bool Expanded { get; private set; }

		[IgnoreDataMember]
		internal bool VisibleOnClient
		{
			get
			{
				return this.GroupType != InstantMessageGroupType.Tagged && this.GroupType != InstantMessageGroupType.Pinned;
			}
		}

		public int CompareTo(InstantMessageGroup otherGroup)
		{
			if (this == otherGroup)
			{
				return 0;
			}
			if (otherGroup == null)
			{
				return 1;
			}
			if (this.GroupType == InstantMessageGroupType.Favorites)
			{
				return -1;
			}
			if (otherGroup.GroupType == InstantMessageGroupType.Favorites)
			{
				return 1;
			}
			return this.Name.CompareTo(otherGroup.Name);
		}

		internal static InstantMessageGroup Create(string groupId, string groupName)
		{
			return InstantMessageGroup.Create(groupId, groupName, InstantMessageGroupType.Standard);
		}

		internal static InstantMessageGroup Create(string groupId, string groupName, InstantMessageGroupType groupType)
		{
			InstantMessageGroup instantMessageGroup = new InstantMessageGroup
			{
				Id = groupId,
				Name = groupName,
				GroupType = groupType,
				Expanded = false
			};
			if (groupName.Equals("~"))
			{
				instantMessageGroup.Name = Strings.GetLocalizedString(-1499962683);
				instantMessageGroup.GroupType = InstantMessageGroupType.OtherContacts;
			}
			return instantMessageGroup;
		}

		internal void SetExpandedState(HashSet<string> expandedGroupIds)
		{
			this.Expanded = (expandedGroupIds != null && expandedGroupIds.Contains(InstantMessageUtilities.ToGroupFormat(this.Id)));
			if (this.GroupType == InstantMessageGroupType.OtherContacts && expandedGroupIds == null)
			{
				this.Expanded = true;
			}
		}

		public const string OtherContactsGroupName = "~";
	}
}
