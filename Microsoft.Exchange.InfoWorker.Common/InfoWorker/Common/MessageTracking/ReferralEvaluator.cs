using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Logging.Search;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ReferralEvaluator
	{
		public ReferralEvaluator(DirectoryContext directoryContext, ReferralEvaluator.TryProcessReferralMethod tryProcessReferral, ReferralEvaluator.GetAuthorityAndRemapReferralMethod getAuthorityAndRemapReferral, string recipientPathFilter, SearchScope scope)
		{
			this.directoryContext = directoryContext;
			this.referralQueue = new ReferralQueue(directoryContext);
			this.tryProcessReferral = tryProcessReferral;
			this.getAuthorityAndRemapReferral = getAuthorityAndRemapReferral;
			this.recipientPathFilter = recipientPathFilter;
			this.scope = scope;
			this.referralTree = new ReferralTree(new ReferralTree.PathInsertedHandler(this.PathInsertedHandler));
		}

		public void Evaluate(IEnumerable<List<RecipientTrackingEvent>> initialEvents, TrackingAuthorityKind authorityKind)
		{
			this.InsertPaths(null, initialEvents, authorityKind);
			ReferralQueue.ReferralData referralData;
			while (this.referralQueue.DeQueue(out referralData))
			{
				if (!this.directoryContext.TrackingBudget.IsUnderBudget())
				{
					TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Over budget following referrals, stopping", new object[0]);
					return;
				}
				Node node = referralData.Node;
				if (node.HasChildren)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "Skipping referral, node already has child.", new object[0]);
				}
				else
				{
					this.referralsFollowed++;
					if (this.referralsFollowed > 5000)
					{
						TraceWrapper.SearchLibraryTracer.TraceError<int>(this.GetHashCode(), "Processed {0} referrals.  Giving up.", this.referralsFollowed);
						return;
					}
					ReferralQueue referralQueue = this.referralQueue;
					ReferralQueue.State state2 = new ReferralQueue.State();
					state2.AuthorityKey = referralData.Authority.ToString();
					state2.WorkerState = referralData;
					state2.WorkerMethod = delegate(object state)
					{
						ReferralQueue.ReferralData referralData2 = (ReferralQueue.ReferralData)state;
						Node node2 = referralData2.Node;
						TrackingAuthority authority = referralData2.Authority;
						RecipientTrackingEvent recipientTrackingEvent = (RecipientTrackingEvent)referralData2.Node.Value;
						IEnumerable<List<RecipientTrackingEvent>> paths;
						if (this.tryProcessReferral(recipientTrackingEvent, authority, out paths))
						{
							TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Inserting paths after following referral for recipient {0} from {1}.", recipientTrackingEvent.RecipientAddress, Names<TrackingAuthorityKind>.Map[(int)authority.TrackingAuthorityKind]);
							this.InsertPaths(node2, paths, authority.TrackingAuthorityKind);
							return;
						}
						TraceWrapper.SearchLibraryTracer.TraceError<SmtpAddress>(this.GetHashCode(), "Did not get any results for referral to recipient {0}.", recipientTrackingEvent.RecipientAddress);
					};
					referralQueue.BeginWorker(state2);
				}
			}
		}

		public List<RecipientTrackingEvent> GetLeaves()
		{
			ICollection<Node> leafNodes = this.referralTree.GetLeafNodes();
			return this.ConvertNodesToEvents(leafNodes);
		}

		public List<RecipientTrackingEvent> GetPathToRecipient(string recipient)
		{
			ICollection<Node> pathToLeaf = this.referralTree.GetPathToLeaf(recipient);
			return this.ConvertNodesToEvents(pathToLeaf);
		}

		public RecipientEventData GetEventDataForRecipient(string recipient)
		{
			List<RecipientTrackingEvent> pathToRecipient = this.GetPathToRecipient(recipient);
			if (pathToRecipient.Count > 0)
			{
				return new RecipientEventData(pathToRecipient);
			}
			List<RecipientTrackingEvent> leaves = this.GetLeaves();
			List<List<RecipientTrackingEvent>> list = null;
			foreach (RecipientTrackingEvent recipientTrackingEvent in leaves)
			{
				if (this.IsOutOfScopeTransfer(recipientTrackingEvent))
				{
					if (list == null)
					{
						list = new List<List<RecipientTrackingEvent>>();
					}
					list.Add(this.GetPathToRecipient(recipientTrackingEvent.RecipientAddress.ToString()));
				}
			}
			if (list != null)
			{
				return new RecipientEventData(null, list);
			}
			return null;
		}

		private void InsertPaths(Node referralNode, IEnumerable<List<RecipientTrackingEvent>> paths, TrackingAuthorityKind authorityKind)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Inserting paths from referral for {0}", (referralNode != null) ? referralNode.Key : "<null>");
			this.referralTree.InsertPaths(referralNode, authorityKind, this.FilterForPathsToInsert(referralNode, paths));
		}

		private IEnumerable<List<RecipientTrackingEvent>> FilterForPathsToInsert(Node referralNode, IEnumerable<List<RecipientTrackingEvent>> paths)
		{
			foreach (List<RecipientTrackingEvent> path in paths)
			{
				if (path.Count == 0)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug(this.GetHashCode(), "No events in path, ignoring.", new object[0]);
				}
				else
				{
					string rootAddressOfPath = path[0].RootAddress;
					TraceWrapper.SearchLibraryTracer.TraceDebug<int, SmtpAddress, string>(this.GetHashCode(), "Path has {0} events, first event has recip {1}, root address: {2}", path.Count, path[0].RecipientAddress, path[0].RootAddress);
					if (referralNode == null || referralNode.Key.Equals(rootAddressOfPath, StringComparison.OrdinalIgnoreCase) || !this.referralsProcessed.Contains(rootAddressOfPath))
					{
						this.referralsProcessed.Add(rootAddressOfPath);
						yield return path;
					}
					else
					{
						TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Path with recip {0}, is not inserted for following {1}", path[0].RecipientAddress, referralNode.Key);
					}
				}
			}
			yield break;
		}

		private void PathInsertedHandler(Node lastNodeInPath, TrackingAuthorityKind authorityKind)
		{
			RecipientTrackingEvent recipientTrackingEvent = (RecipientTrackingEvent)lastNodeInPath.Value;
			if (!string.IsNullOrEmpty(this.recipientPathFilter) && this.recipientPathFilter.Equals(recipientTrackingEvent.RecipientAddress.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				if (!this.hasRecipPathMatch)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress>(this.GetHashCode(), "Clearing queue because last event in path with recipient {0} matches recipient path fitler.", recipientTrackingEvent.RecipientAddress);
					this.referralQueue.Clear();
				}
				this.hasRecipPathMatch = true;
			}
			else if (this.hasRecipPathMatch)
			{
				return;
			}
			if (this.IsRemoteEventPotentialReferral(recipientTrackingEvent, authorityKind))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Getting authority for possible referral {0}, Original recip event={1}.", recipientTrackingEvent.RecipientAddress, Names<EventDescription>.Map[(int)recipientTrackingEvent.EventDescription]);
				TrackingAuthority trackingAuthority = this.getAuthorityAndRemapReferral(ref recipientTrackingEvent);
				TraceWrapper.SearchLibraryTracer.TraceDebug<string, string>(this.GetHashCode(), "Got authority {0}, Recip event is now {1}.", (trackingAuthority == null) ? "null" : Names<TrackingAuthorityKind>.Map[(int)trackingAuthority.TrackingAuthorityKind], Names<EventDescription>.Map[(int)recipientTrackingEvent.EventDescription]);
				if (trackingAuthority != null && !this.IsOutOfScopeTransfer(recipientTrackingEvent))
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Adding last event in path to referral queue address is {0}, the authority to follow is {1}.", recipientTrackingEvent.RecipientAddress, Names<TrackingAuthorityKind>.Map[(int)trackingAuthority.TrackingAuthorityKind]);
					this.referralQueue.Enqueue(new ReferralQueue.ReferralData
					{
						Node = lastNodeInPath,
						Authority = trackingAuthority
					});
				}
			}
		}

		private List<RecipientTrackingEvent> ConvertNodesToEvents(ICollection<Node> nodes)
		{
			List<RecipientTrackingEvent> list = new List<RecipientTrackingEvent>(nodes.Count);
			foreach (Node node in nodes)
			{
				RecipientTrackingEvent item = (RecipientTrackingEvent)node.Value;
				list.Add(item);
			}
			return list;
		}

		private bool IsRemoteEventPotentialReferral(RecipientTrackingEvent recipientTrackingEvent, TrackingAuthorityKind authorityKindFollowed)
		{
			TraceWrapper.SearchLibraryTracer.TraceDebug<SmtpAddress, string>(this.GetHashCode(), "Evaluating referral for recipient: {0}, authority kind followed is: {1}", recipientTrackingEvent.RecipientAddress, Names<TrackingAuthorityKind>.Map[(int)authorityKindFollowed]);
			if (recipientTrackingEvent.EventDescription != EventDescription.SmtpSendCrossSite && recipientTrackingEvent.EventDescription != EventDescription.SmtpSendCrossForest && recipientTrackingEvent.EventDescription != EventDescription.TransferredToForeignOrg && recipientTrackingEvent.EventDescription != EventDescription.TransferredToPartnerOrg && recipientTrackingEvent.EventDescription != EventDescription.PendingModeration && recipientTrackingEvent.EventDescription != EventDescription.SubmittedCrossSite)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Event: {0} does not indicate a referral", Names<EventDescription>.Map[(int)recipientTrackingEvent.EventDescription]);
				return false;
			}
			if (recipientTrackingEvent.EventDescription == EventDescription.PendingModeration && (authorityKindFollowed == TrackingAuthorityKind.RemoteForest || authorityKindFollowed == TrackingAuthorityKind.RemoteTrustedOrg))
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Pending moderation is not a referral for authority {0}.", Names<TrackingAuthorityKind>.Map[(int)authorityKindFollowed]);
				return false;
			}
			if ((authorityKindFollowed == TrackingAuthorityKind.RemoteForest || authorityKindFollowed == TrackingAuthorityKind.RemoteTrustedOrg) && recipientTrackingEvent.EventDescription != EventDescription.SmtpSendCrossForest && recipientTrackingEvent.EventDescription != EventDescription.TransferredToForeignOrg && recipientTrackingEvent.EventDescription != EventDescription.TransferredToPartnerOrg)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Following {0} should result only in referrals to other orgs or forests.", Names<TrackingAuthorityKind>.Map[(int)authorityKindFollowed]);
				return false;
			}
			return true;
		}

		private bool IsOutOfScopeTransfer(RecipientTrackingEvent recipEvent)
		{
			return (recipEvent.EventDescription == EventDescription.SmtpSendCrossSite && this.scope == SearchScope.Site) || (recipEvent.EventDescription == EventDescription.PendingModeration && this.scope == SearchScope.Site) || (recipEvent.EventDescription == EventDescription.SmtpSendCrossForest && (this.scope == SearchScope.Site || this.scope == SearchScope.Forest || this.scope == SearchScope.Organization)) || (recipEvent.EventDescription == EventDescription.TransferredToPartnerOrg && (this.scope == SearchScope.Site || this.scope == SearchScope.Forest || this.scope == SearchScope.Organization)) || recipEvent.EventDescription == EventDescription.TransferredToForeignOrg;
		}

		private const int MaxReferralsToAllowed = 5000;

		private ReferralQueue referralQueue;

		private ReferralTree referralTree;

		private ReferralEvaluator.TryProcessReferralMethod tryProcessReferral;

		private ReferralEvaluator.GetAuthorityAndRemapReferralMethod getAuthorityAndRemapReferral;

		private string recipientPathFilter;

		private SearchScope scope;

		private int referralsFollowed;

		private bool hasRecipPathMatch;

		private HashSet<string> referralsProcessed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private DirectoryContext directoryContext;

		public delegate bool TryProcessReferralMethod(RecipientTrackingEvent referralEvent, TrackingAuthority authority, out IEnumerable<List<RecipientTrackingEvent>> paths);

		public delegate TrackingAuthority GetAuthorityAndRemapReferralMethod(ref RecipientTrackingEvent referralEvent);
	}
}
