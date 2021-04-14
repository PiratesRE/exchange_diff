using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InstantMessageBuddy : IComparable<InstantMessageBuddy>
	{
		[DataMember(Order = 1)]
		public string SipUri { get; set; }

		[DataMember(Order = 2)]
		public string Guid { get; set; }

		[DataMember(Order = 3)]
		public string DisplayName { get; set; }

		[DataMember(Order = 5)]
		public int AddressType { get; set; }

		[DataMember(Order = 6)]
		public bool Tagged { get; set; }

		[DataMember(Order = 7)]
		public string[] GroupIds
		{
			get
			{
				return (from g in this.groups.Values
				select g.Id).ToArray<string>();
			}
			set
			{
			}
		}

		[DataMember(Order = 8)]
		public string FirstName { get; set; }

		[DataMember(Order = 9)]
		public string LastName { get; set; }

		[DataMember(Order = 10)]
		public EmailAddressWrapper EmailAddress { get; set; }

		[DataMember(Order = 11)]
		public ItemId PersonaId { get; set; }

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

		internal static InstantMessageBuddy Create(string guid, string sipUri, string displayName, EmailAddressWrapper emailAddress)
		{
			return new InstantMessageBuddy
			{
				Guid = guid,
				SipUri = InstantMessageUtilities.ToSipFormat(sipUri),
				DisplayName = displayName,
				AddressType = 0,
				groups = new Dictionary<string, InstantMessageGroup>(),
				Tagged = false,
				FirstName = null,
				LastName = null,
				EmailAddress = emailAddress
			};
		}

		internal void AddGroup(InstantMessageGroup group)
		{
			if (group.GroupType == InstantMessageGroupType.Tagged)
			{
				this.Tagged = true;
			}
			this.groups[group.Id] = group;
		}

		internal void AddGroups(string[] groupIds)
		{
			if (groupIds != null)
			{
				foreach (string groupId in groupIds)
				{
					this.AddGroup(InstantMessageGroup.Create(groupId, string.Empty));
				}
			}
		}

		private Dictionary<string, InstantMessageGroup> groups;
	}
}
