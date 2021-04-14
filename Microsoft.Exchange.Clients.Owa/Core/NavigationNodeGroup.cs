using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class NavigationNodeGroup : NavigationNode, ICloneable
	{
		public NavigationNodeGroup(string subject, NavigationNodeGroupSection navigationNodeGroupSection, Guid navigationNodeGroupClassId) : base(NavigationNodeType.Header, subject, navigationNodeGroupSection)
		{
			this.NavigationNodeGroupClassId = navigationNodeGroupClassId;
			this.children = new NavigationNodeGroup.NavigationNodeFolderList(this);
			base.ClearDirty();
		}

		internal NavigationNodeGroup(object[] values, Dictionary<PropertyDefinition, int> propertyMap) : base(NavigationNodeGroup.groupProperties, values, propertyMap)
		{
			this.children = new NavigationNodeGroup.NavigationNodeFolderList(this);
		}

		private NavigationNodeGroup(MemoryPropertyBag propertyBag) : base(propertyBag)
		{
			this.children = new NavigationNodeGroup.NavigationNodeFolderList(this);
		}

		internal Guid NavigationNodeGroupClassId
		{
			get
			{
				return base.GuidGetter(NavigationNodeSchema.GroupClassId);
			}
			private set
			{
				base.GuidSetter(NavigationNodeSchema.GroupClassId, value);
			}
		}

		internal NavigationNodeList<NavigationNodeFolder> Children
		{
			get
			{
				return this.children;
			}
		}

		public object Clone()
		{
			NavigationNodeGroup navigationNodeGroup = new NavigationNodeGroup(this.propertyBag);
			this.children.CopyToList(navigationNodeGroup.children);
			if (base.IsNew)
			{
				navigationNodeGroup.IsNew = true;
			}
			return navigationNodeGroup;
		}

		public override int GetHashCode()
		{
			return this.NavigationNodeGroupClassId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			NavigationNodeGroup navigationNodeGroup = obj as NavigationNodeGroup;
			return navigationNodeGroup != null && this.NavigationNodeGroupClassId.Equals(navigationNodeGroup.NavigationNodeGroupClassId);
		}

		public int RemoveFolderByLegacyDNandId(string mailboxLegacyDN, StoreObjectId folderId)
		{
			int num = 0;
			for (int i = this.children.Count - 1; i >= 0; i--)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(mailboxLegacyDN, this.children[i].MailboxLegacyDN) && folderId.CompareTo(this.children[i].FolderId) == 0)
				{
					this.children.RemoveAt(i);
					num++;
				}
			}
			return num;
		}

		public int FindEquivalentNode(NavigationNodeFolder nodeFolder)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (nodeFolder.Equals(this.children[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public NavigationNodeFolder[] FindFoldersById(StoreObjectId folderId)
		{
			List<NavigationNodeFolder> list = new List<NavigationNodeFolder>();
			foreach (NavigationNodeFolder navigationNodeFolder in this.children)
			{
				if (navigationNodeFolder.FolderId != null && folderId.CompareTo(navigationNodeFolder.FolderId) == 0)
				{
					list.Add(navigationNodeFolder);
				}
			}
			return list.ToArray();
		}

		public NavigationNodeFolder[] FindGSCalendarsByLegacyDN(string mailboxLegacyDN)
		{
			List<NavigationNodeFolder> list = new List<NavigationNodeFolder>();
			foreach (NavigationNodeFolder navigationNodeFolder in this.children)
			{
				if (string.Equals(mailboxLegacyDN, navigationNodeFolder.MailboxLegacyDN, StringComparison.OrdinalIgnoreCase) && navigationNodeFolder.IsGSCalendar)
				{
					list.Add(navigationNodeFolder);
				}
			}
			return list.ToArray();
		}

		public bool IsExpanded
		{
			get
			{
				return !Utilities.IsFlagSet(this.propertyBag.GetValueOrDefault<int>(ViewStateProperties.TreeNodeCollapseStatus), 1);
			}
			set
			{
				this.propertyBag.SetProperty(ViewStateProperties.TreeNodeCollapseStatus, value ? StatusPersistTreeNodeType.None : StatusPersistTreeNodeType.CurrentNode);
			}
		}

		public override string Subject
		{
			get
			{
				if (base.IsFlagSet(NavigationNodeFlags.OneOffName))
				{
					return base.Subject;
				}
				NavigationNodeGroupType navigationNodeGroupType = NavigationNodeGroupType.UserCreatedGroup;
				if (NavigationNodeCollection.MyFoldersClassId.Equals(this.NavigationNodeGroupClassId))
				{
					navigationNodeGroupType = NavigationNodeGroupType.MyFoldersGroup;
				}
				else if (NavigationNodeCollection.OtherFoldersClassId.Equals(this.NavigationNodeGroupClassId))
				{
					navigationNodeGroupType = NavigationNodeGroupType.OtherFoldersGroup;
				}
				else if (NavigationNodeCollection.PeoplesFoldersClassId.Equals(this.NavigationNodeGroupClassId))
				{
					navigationNodeGroupType = NavigationNodeGroupType.SharedFoldersGroup;
				}
				if (navigationNodeGroupType == NavigationNodeGroupType.UserCreatedGroup)
				{
					return base.Subject;
				}
				return NavigationNodeCollection.GetDefaultGroupSubject(navigationNodeGroupType, base.NavigationNodeGroupSection);
			}
			set
			{
				base.NavigationNodeFlags |= NavigationNodeFlags.OneOffName;
				base.Subject = value;
			}
		}

		protected override void UpdateMessage(MessageItem message)
		{
			base.UpdateMessage(message);
			message[NavigationNodeSchema.GroupClassId] = this.NavigationNodeGroupClassId.ToByteArray();
			message[ViewStateProperties.TreeNodeCollapseStatus] = (this.IsExpanded ? 0 : 1);
		}

		private readonly NavigationNodeGroup.NavigationNodeFolderList children;

		private static PropertyDefinition[] groupProperties = new PropertyDefinition[]
		{
			ViewStateProperties.TreeNodeCollapseStatus,
			NavigationNodeSchema.GroupClassId
		};

		private class NavigationNodeFolderList : NavigationNodeList<NavigationNodeFolder>
		{
			public NavigationNodeFolderList(NavigationNodeGroup parentGroup)
			{
				this.parentGroup = parentGroup;
			}

			protected override void OnBeforeNodeAdd(NavigationNodeFolder node)
			{
				this.MakeNavigationNodeFolderAsChild(node);
				base.OnBeforeNodeAdd(node);
			}

			private void MakeNavigationNodeFolderAsChild(NavigationNodeFolder node)
			{
				if (this.parentGroup.NavigationNodeGroupSection != node.NavigationNodeGroupSection)
				{
					throw new ArgumentException(string.Format("Current node list is for group section {0}, but the new node is in group section {1}, which does not matched.", this.parentGroup.NavigationNodeGroupSection.ToString(), node.NavigationNodeGroupSection.ToString()));
				}
				if (node.NavigationNodeGroupSection != NavigationNodeGroupSection.First && !node.NavigationNodeParentGroupClassId.Equals(this.parentGroup.NavigationNodeGroupClassId))
				{
					node.NavigationNodeParentGroupClassId = this.parentGroup.NavigationNodeGroupClassId;
					node.NavigationNodeGroupName = this.parentGroup.Subject;
				}
			}

			private readonly NavigationNodeGroup parentGroup;
		}
	}
}
