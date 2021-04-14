using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ReferralTree : TreeManager
	{
		public ReferralTree(ReferralTree.PathInsertedHandler pathInsertedHandler)
		{
			this.pathInsertedHandler = pathInsertedHandler;
		}

		internal void InsertPaths(Node referralNode, TrackingAuthorityKind authorityKind, IEnumerable<List<RecipientTrackingEvent>> paths)
		{
			Dictionary<string, List<Node>> dictionary = new Dictionary<string, List<Node>>();
			foreach (IList<RecipientTrackingEvent> list in paths)
			{
				Node[] array = new Node[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					string text = list[i].RecipientAddress.ToString();
					string parentKey = (i == 0) ? list[i].RootAddress : text;
					array[i] = new Node(text, parentKey, list[i].Clone());
					if (i != 0)
					{
						array[i - 1].AddChild(array[i]);
					}
				}
				List<Node> list2;
				if (!dictionary.TryGetValue(array[0].ParentKey, out list2))
				{
					list2 = new List<Node>();
					dictionary.Add(array[0].ParentKey, list2);
				}
				list2.Add(array[0]);
			}
			foreach (string text2 in dictionary.Keys)
			{
				base.InsertAllChildrenForOneNode(text2, dictionary[text2], (Node parentNode, Node childNode) => this.Root == parentNode || parentNode == referralNode, (Node parent, Node child) => this.DoPostInsertionProcessing(parent, child, authorityKind));
			}
		}

		protected override Node DisambiguateParentRecord(LinkedList<Node> possibleParents, Node node)
		{
			if (possibleParents == null || possibleParents.Count == 0)
			{
				return null;
			}
			return possibleParents.First.Value;
		}

		protected override int GetNodePriorities(Node node, out int secondaryPriority)
		{
			RecipientTrackingEvent recipientTrackingEvent = (RecipientTrackingEvent)node.Value;
			EventDescriptionInformation eventDescriptionInformation;
			if (!EnumAttributeInfo<EventDescription, EventDescriptionInformation>.TryGetValue((int)recipientTrackingEvent.EventDescription, out eventDescriptionInformation))
			{
				throw new InvalidOperationException(string.Format("Value {0} was not annotated", Names<EventDescription>.Map[(int)recipientTrackingEvent.EventDescription]));
			}
			secondaryPriority = EventTree.GetSecondaryPriority(recipientTrackingEvent.BccRecipient, recipientTrackingEvent.HiddenRecipient, string.Equals(recipientTrackingEvent.RootAddress, (string)recipientTrackingEvent.RecipientAddress, StringComparison.OrdinalIgnoreCase));
			return eventDescriptionInformation.EventPriority;
		}

		protected override bool IsNodeRootChildCandidate(Node node)
		{
			return true;
		}

		private Node DoPostInsertionProcessing(Node parent, Node child, TrackingAuthorityKind authorityKind)
		{
			this.ApplyInheritance(parent, child);
			Node node = child;
			while (node.HasChildren)
			{
				if (node.Children.Count != 1)
				{
					throw new InvalidOperationException(string.Format("Unexpected number of child nodes ({0}) in referral tree", node.Children.Count));
				}
				this.ApplyInheritance(node, node.Children[0]);
				node = node.Children[0];
			}
			this.pathInsertedHandler(node, authorityKind);
			return node;
		}

		private void ApplyInheritance(Node parent, Node child)
		{
			RecipientTrackingEvent recipientTrackingEvent = (RecipientTrackingEvent)child.Value;
			if (parent == base.Root)
			{
				return;
			}
			RecipientTrackingEvent recipientTrackingEvent2 = (RecipientTrackingEvent)parent.Value;
			if (recipientTrackingEvent2.HiddenRecipient)
			{
				recipientTrackingEvent.HiddenRecipient = true;
			}
			recipientTrackingEvent.BccRecipient = recipientTrackingEvent2.BccRecipient;
			recipientTrackingEvent.RootAddress = recipientTrackingEvent2.RootAddress;
		}

		private ReferralTree.PathInsertedHandler pathInsertedHandler;

		public delegate void PathInsertedHandler(Node lastNodeInPath, TrackingAuthorityKind authorityKind);

		public delegate bool IsNodePendingReferral(Node parent);
	}
}
