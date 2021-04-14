using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Management.RbacTasks
{
	internal sealed class RoleAssignmentExpansion
	{
		internal RoleAssignmentExpansion(IRecipientSession recSession, OrganizationId organizationId)
		{
			RoleAssignmentExpansion.groupProperties.Add(ADRecipientSchema.MemberOfGroup);
			this.recipientSession = recSession;
			this.adRecipientExpansion = new ADRecipientExpansion(this.recipientSession, true);
		}

		internal MultiValuedProperty<FormattedADObjectIdCollection> GetAssignmentChains(ADObjectId root, ADObjectId user)
		{
			if (this.assignmentChainsDictionary.ContainsKey(this.GetKey(root, user)))
			{
				return new MultiValuedProperty<FormattedADObjectIdCollection>(this.assignmentChainsDictionary[this.GetKey(root, user)]);
			}
			return null;
		}

		internal List<ADObjectId> GetEffectiveUsersForRoleAssignment(ExchangeRoleAssignment roleAssignment)
		{
			if (this.usersListLookup.ContainsKey(roleAssignment.User))
			{
				return this.usersListLookup[roleAssignment.User];
			}
			this.usersList = new List<ADObjectId>();
			ADRawEntry adrawEntry = this.recipientSession.ReadADRawEntry(roleAssignment.User, RoleAssignmentExpansion.groupProperties);
			if (adrawEntry != null)
			{
				this.adRecipientExpansion.Expand(adrawEntry, new ADRecipientExpansion.HandleRecipientDelegate(this.OnRecipient), new ADRecipientExpansion.HandleFailureDelegate(this.OnFailure));
				foreach (RoleAssignmentExpansion.UserNode userNode in this.userNodeLookupTable.Values)
				{
					userNode.ResetTraverseIndex();
				}
				this.CalculateAssignmentChains(roleAssignment);
				this.usersListLookup[roleAssignment.User] = this.usersList;
			}
			else
			{
				ExTraceGlobals.ADConfigTracer.TraceError<ADObjectId>(0L, "Error while getting ADRawEntry for User '{0}'", roleAssignment.User);
			}
			return this.usersList;
		}

		private void CalculateAssignmentChains(ExchangeRoleAssignment roleAssignment)
		{
			RoleAssignmentExpansion.UserNode node = this.AddORGetNode(roleAssignment.User);
			this.CalculateAssignmentChainsNonRecursive(node);
		}

		private void CalculateAssignmentChainsNonRecursive(RoleAssignmentExpansion.UserNode node)
		{
			RoleAssignmentExpansion.UserNode userNode = node;
			List<RoleAssignmentExpansion.UserNode> list = new List<RoleAssignmentExpansion.UserNode>();
			list.Add(node);
			while (list.Count > 0)
			{
				while (node.Children.Count > 0)
				{
					RoleAssignmentExpansion.UserNode nextChild = node.GetNextChild();
					if (nextChild == null)
					{
						break;
					}
					node = nextChild;
					list.Add(node);
					if (node.Children.Count == 0)
					{
						List<ADObjectId> list2 = new List<ADObjectId>();
						foreach (RoleAssignmentExpansion.UserNode userNode2 in list)
						{
							list2.Add(userNode2.User);
						}
						list2.Remove(node.User);
						if (this.assignmentChainsDictionary.ContainsKey(this.GetKey(userNode.User, node.User)))
						{
							List<FormattedADObjectIdCollection> list3 = this.assignmentChainsDictionary[this.GetKey(userNode.User, node.User)];
							list3.Add(new FormattedADObjectIdCollection(list2));
						}
						else
						{
							List<FormattedADObjectIdCollection> list4 = new List<FormattedADObjectIdCollection>();
							list4.Add(new FormattedADObjectIdCollection(list2));
							this.assignmentChainsDictionary.Add(this.GetKey(userNode.User, node.User), list4);
						}
					}
				}
				list.Remove(node);
				if (list.Count > 0)
				{
					node = list[list.Count - 1];
				}
			}
		}

		private string GetKey(ADObjectId parent, ADObjectId user)
		{
			return parent.ObjectGuid.ToString() + RoleAssignmentExpansion.CommaSeparator + user.ObjectGuid.ToString();
		}

		private RoleAssignmentExpansion.UserNode AddORGetNode(ADObjectId parentId)
		{
			if (this.userNodeLookupTable.ContainsKey(parentId))
			{
				return this.userNodeLookupTable[parentId];
			}
			RoleAssignmentExpansion.UserNode userNode = new RoleAssignmentExpansion.UserNode(parentId);
			this.userNodeLookupTable[parentId] = userNode;
			return userNode;
		}

		private ExpansionControl OnRecipient(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
		{
			RoleAssignmentExpansion.UserNode item = this.AddORGetNode(recipient.Id);
			if (parent != null)
			{
				RoleAssignmentExpansion.UserNode userNode = this.AddORGetNode(parent.Id);
				if (!userNode.Children.Contains(item))
				{
					userNode.Children.Add(item);
				}
			}
			if (recipientExpansionType != ExpansionType.GroupMembership && !this.usersList.Contains(recipient.Id))
			{
				this.usersList.Add(recipient.Id);
			}
			if (recipientExpansionType == ExpansionType.None || recipientExpansionType == ExpansionType.GroupMembership)
			{
				return ExpansionControl.Continue;
			}
			return ExpansionControl.Skip;
		}

		private ExpansionControl OnFailure(ExpansionFailure failure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
		{
			if (failure == ExpansionFailure.LoopDetected)
			{
				return ExpansionControl.Skip;
			}
			return ExpansionControl.Continue;
		}

		private ADRecipientExpansion adRecipientExpansion;

		private static List<PropertyDefinition> groupProperties = new List<PropertyDefinition>(ADRecipientExpansion.RequiredProperties);

		private IRecipientSession recipientSession;

		private static string CommaSeparator = ":";

		private List<ADObjectId> usersList;

		private Dictionary<ADObjectId, RoleAssignmentExpansion.UserNode> userNodeLookupTable = new Dictionary<ADObjectId, RoleAssignmentExpansion.UserNode>();

		private Dictionary<ADObjectId, List<ADObjectId>> usersListLookup = new Dictionary<ADObjectId, List<ADObjectId>>();

		private Dictionary<string, List<FormattedADObjectIdCollection>> assignmentChainsDictionary = new Dictionary<string, List<FormattedADObjectIdCollection>>();

		private sealed class UserNode
		{
			internal UserNode(ADObjectId user)
			{
				this.userId = user;
				this.children = new List<RoleAssignmentExpansion.UserNode>();
			}

			internal ADObjectId User
			{
				get
				{
					return this.userId;
				}
			}

			internal List<RoleAssignmentExpansion.UserNode> Children
			{
				get
				{
					return this.children;
				}
			}

			internal RoleAssignmentExpansion.UserNode GetNextChild()
			{
				if (this.index < this.Children.Count)
				{
					this.index++;
					return this.Children[this.index - 1];
				}
				this.index = 0;
				return null;
			}

			internal void ResetTraverseIndex()
			{
				this.index = 0;
			}

			private ADObjectId userId;

			private List<RoleAssignmentExpansion.UserNode> children;

			private int index;
		}
	}
}
