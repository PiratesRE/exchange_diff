using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class InstantMessageBuddy : IComparable<InstantMessageBuddy>
	{
		private InstantMessageBuddy()
		{
		}

		internal string Guid { get; private set; }

		internal string SipUri { get; private set; }

		internal string DisplayName { get; private set; }

		internal string RequestMessage { get; set; }

		internal int AddressType { get; set; }

		internal bool Tagged { get; set; }

		internal string[] GroupIds
		{
			get
			{
				return (from g in this.groups.Values
				select g.Id).ToArray<string>();
			}
		}

		internal static InstantMessageBuddy Create(string guid, string sipUri, string displayName)
		{
			return new InstantMessageBuddy
			{
				Guid = guid,
				SipUri = InstantMessageUtilities.ToSipFormat(sipUri),
				DisplayName = displayName,
				RequestMessage = string.Empty,
				AddressType = 0,
				groups = new Dictionary<string, InstantMessageGroup>(),
				Tagged = false
			};
		}

		public int CompareTo(InstantMessageBuddy otherBuddy)
		{
			if (this == otherBuddy)
			{
				return 0;
			}
			if (otherBuddy == null)
			{
				return 1;
			}
			return this.DisplayName.CompareTo(otherBuddy.DisplayName);
		}

		internal void AddGroup(InstantMessageGroup group)
		{
			if (group.Type == InstantMessageGroupType.Tagged)
			{
				this.Tagged = true;
			}
			this.groups[group.Id] = group;
		}

		internal void AddGroups(string[] groupIds)
		{
			if (groupIds != null)
			{
				foreach (string id in groupIds)
				{
					this.AddGroup(InstantMessageGroup.Create(id, string.Empty));
				}
			}
		}

		internal string SerializeToJavascript()
		{
			return string.Format("['{0}','{1}','{2}','{3}',{4},{5}]", new object[]
			{
				Utilities.JavascriptEncode(this.SipUri),
				Utilities.JavascriptEncode(this.Guid),
				Utilities.JavascriptEncode(this.DisplayName),
				this.AddressType,
				this.SerializeGroupsToJavascript(),
				this.Tagged ? "1" : "0"
			});
		}

		internal string SerializeSipToJavascript()
		{
			return string.Format("'{0}'", Utilities.JavascriptEncode(this.SipUri));
		}

		private string SerializeGroupsToJavascript()
		{
			return string.Format("[{0}]", string.Join(",", (from g in this.groups.Values
			where g.VisibleOnClient
			select string.Format("'{0}'", Utilities.JavascriptEncode(InstantMessageUtilities.ToGroupFormat(g.Id)))).ToArray<string>()));
		}

		private Dictionary<string, InstantMessageGroup> groups;
	}
}
