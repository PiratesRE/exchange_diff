using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PartitionExtractor
	{
		public virtual IEnumerable<LoadPartition> GetPartitions(LoadContainer topology)
		{
			HashSet<string> constraints = this.GetConstraints(topology);
			this.SetCommittedLoads(topology);
			foreach (string constraint in constraints)
			{
				LoadContainer newRoot = this.CopyForConstraint(topology, constraint);
				if (newRoot != null)
				{
					this.MergeConstraintSetsIntoDatabases(newRoot);
					yield return new LoadPartition(newRoot, constraint);
				}
			}
			yield break;
		}

		private LoadContainer CopyForConstraint(LoadContainer root, string constraint)
		{
			List<LoadContainer> list = new List<LoadContainer>();
			foreach (LoadEntity loadEntity in root.Children)
			{
				LoadContainer root2 = (LoadContainer)loadEntity;
				LoadContainer loadContainer = this.CopyForConstraint(root2, constraint);
				if (loadContainer != null)
				{
					list.Add(loadContainer);
				}
			}
			bool flag = this.ShouldCullNode(root, list, constraint);
			if (flag)
			{
				return null;
			}
			LoadContainer shallowCopy = root.GetShallowCopy();
			foreach (LoadContainer child in list)
			{
				shallowCopy.AddChild(child);
			}
			return shallowCopy;
		}

		private HashSet<string> GetConstraints(LoadContainer root)
		{
			Queue<LoadContainer> queue = new Queue<LoadContainer>(new LoadContainer[]
			{
				root
			});
			HashSet<string> hashSet = new HashSet<string>();
			hashSet.Add(string.Empty);
			while (queue.Any<LoadContainer>())
			{
				LoadContainer loadContainer = queue.Dequeue();
				if (loadContainer.ContainerType == ContainerType.ConstraintSet)
				{
					hashSet.Add(loadContainer.DirectoryObjectIdentity.Name);
				}
				foreach (LoadEntity loadEntity in loadContainer.Children)
				{
					queue.Enqueue((LoadContainer)loadEntity);
				}
			}
			return hashSet;
		}

		private void MergeConstraintSetsIntoDatabases(LoadContainer root)
		{
			LoadContainer loadContainer = null;
			foreach (LoadEntity loadEntity in root.Children)
			{
				LoadContainer loadContainer2 = loadEntity as LoadContainer;
				if (loadContainer2 != null && loadContainer2.ContainerType == ContainerType.ConstraintSet)
				{
					loadContainer = loadContainer2;
				}
				this.MergeConstraintSetsIntoDatabases(loadContainer2);
			}
			if (root.Constraint != null)
			{
				root.Constraint = root.Constraint.CloneForContainer(root);
			}
			if (loadContainer != null)
			{
				root.ConsumedLoad = new LoadMetricStorage(loadContainer.ConsumedLoad);
				root.CommittedLoad = new LoadMetricStorage(loadContainer.CommittedLoad);
				root.RemoveChild(loadContainer.Guid);
			}
		}

		private void SetCommittedLoads(LoadContainer root)
		{
			LoadMetricStorage left = new LoadMetricStorage();
			if (root.ContainerType == ContainerType.Database)
			{
				foreach (LoadEntity loadEntity in root.Children)
				{
					LoadContainer loadContainer = (LoadContainer)loadEntity;
					left += loadContainer.ConsumedLoad;
				}
				using (List<LoadEntity>.Enumerator enumerator2 = root.Children.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						LoadEntity loadEntity2 = enumerator2.Current;
						LoadContainer loadContainer2 = (LoadContainer)loadEntity2;
						loadContainer2.CommittedLoad = left - loadContainer2.ConsumedLoad;
					}
					return;
				}
			}
			foreach (LoadEntity loadEntity3 in root.Children)
			{
				LoadContainer committedLoads = (LoadContainer)loadEntity3;
				this.SetCommittedLoads(committedLoads);
			}
		}

		private bool ShouldCullNode(LoadContainer node, List<LoadContainer> copiedChildren, string constraint)
		{
			switch (node.ContainerType)
			{
			case ContainerType.Database:
			{
				if (node.DirectoryObject == null)
				{
					return true;
				}
				IMailboxProvisioningConstraint mailboxProvisioningConstraint = new MailboxProvisioningConstraint(constraint);
				DirectoryDatabase directoryDatabase = (DirectoryDatabase)node.DirectoryObject;
				return (!string.IsNullOrEmpty(constraint) && directoryDatabase.MailboxProvisioningAttributes == null) || (directoryDatabase.MailboxProvisioningAttributes != null && !mailboxProvisioningConstraint.IsMatch(directoryDatabase.MailboxProvisioningAttributes));
			}
			case ContainerType.ConstraintSet:
				return node.DirectoryObjectIdentity.Name != constraint;
			default:
				return !copiedChildren.Any<LoadContainer>();
			}
		}
	}
}
