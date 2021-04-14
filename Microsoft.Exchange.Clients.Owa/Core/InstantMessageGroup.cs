using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class InstantMessageGroup : IComparable<InstantMessageGroup>
	{
		private InstantMessageGroup()
		{
		}

		internal string Name { get; private set; }

		internal string Id { get; set; }

		internal InstantMessageGroupType Type { get; private set; }

		internal bool Expanded { get; private set; }

		internal bool VisibleOnClient
		{
			get
			{
				return this.Type != InstantMessageGroupType.Tagged && this.Type != InstantMessageGroupType.Pinned;
			}
		}

		internal static InstantMessageGroup Create(string id, string name)
		{
			return InstantMessageGroup.Create(id, name, InstantMessageGroupType.Standard);
		}

		internal static InstantMessageGroup Create(string id, string name, InstantMessageGroupType type)
		{
			InstantMessageGroup instantMessageGroup = new InstantMessageGroup();
			instantMessageGroup.Id = id;
			instantMessageGroup.Name = name;
			instantMessageGroup.Type = type;
			instantMessageGroup.Expanded = false;
			if (name == "~")
			{
				instantMessageGroup.Name = LocalizedStrings.GetNonEncoded(-1499962683);
				instantMessageGroup.Type = InstantMessageGroupType.OtherContacts;
			}
			return instantMessageGroup;
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
			if (this.Type == InstantMessageGroupType.Favorites)
			{
				return -1;
			}
			if (otherGroup.Type == InstantMessageGroupType.Favorites)
			{
				return 1;
			}
			return this.Name.CompareTo(otherGroup.Name);
		}

		internal void SetExpandedState(HashSet<string> expandedGroupIds)
		{
			this.Expanded = (expandedGroupIds != null && expandedGroupIds.Contains(InstantMessageUtilities.ToGroupFormat(this.Id)));
			if (this.Type == InstantMessageGroupType.OtherContacts && expandedGroupIds == null)
			{
				this.Expanded = true;
			}
		}

		internal string SerializeToJavascript()
		{
			return string.Format("['{0}','{1}',{2},{3}]", new object[]
			{
				Utilities.JavascriptEncode(this.Name ?? string.Empty),
				Utilities.JavascriptEncode(InstantMessageUtilities.ToGroupFormat(this.Id)),
				(int)this.Type,
				this.Expanded ? "1" : "0"
			});
		}

		internal string SerializeIdToJavascript()
		{
			return string.Format("'{0}'", Utilities.JavascriptEncode(InstantMessageUtilities.ToGroupFormat(this.Id)));
		}

		internal string SerializeIdAndNameToJavascript()
		{
			return string.Format("'{0}','{1}'", Utilities.JavascriptEncode(InstantMessageUtilities.ToGroupFormat(this.Id)), Utilities.JavascriptEncode(this.Name ?? string.Empty));
		}

		internal string SerializeIdAndNameToJavascriptArray()
		{
			return string.Format("[{0}]", this.SerializeIdAndNameToJavascript());
		}
	}
}
