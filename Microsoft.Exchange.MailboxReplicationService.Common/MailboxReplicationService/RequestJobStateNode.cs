using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class RequestJobStateNode
	{
		static RequestJobStateNode()
		{
			RequestJobStateNode.CreateNode(RequestState.None, RequestState.None, null, null);
			RequestJobStateNode.CreateNode(RequestState.OverallMove, RequestState.None, null, null);
			RequestJobStateNode.CreateNode(RequestState.Queued, RequestState.OverallMove, null, null);
			RequestJobStateNode.CreateNode(RequestState.InProgress, RequestState.OverallMove, null, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesTotal);
			RequestJobStateNode.CreateNode(RequestState.InitializingMove, RequestState.InProgress, null, null);
			RequestJobStateNode.CreateNode(RequestState.InitialSeeding, RequestState.InProgress, null, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesInitialSeeding);
			RequestJobStateNode.CreateNode(RequestState.CreatingMailbox, RequestState.InitialSeeding, null, null);
			RequestJobStateNode.CreateNode(RequestState.CreatingFolderHierarchy, RequestState.InitialSeeding, null, null);
			RequestJobStateNode.CreateNode(RequestState.CreatingInitialSyncCheckpoint, RequestState.InitialSeeding, null, null);
			RequestJobStateNode.CreateNode(RequestState.LoadingMessages, RequestState.InitialSeeding, null, null);
			RequestJobStateNode.CreateNode(RequestState.CopyingMessages, RequestState.InitialSeeding, null, null);
			RequestJobStateNode.CreateNode(RequestState.Completion, RequestState.InProgress, null, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesCompletion);
			RequestJobStateNode.CreateNode(RequestState.IncrementalSync, RequestState.Completion, null, null);
			RequestJobStateNode.CreateNode(RequestState.Finalization, RequestState.Completion, null, null);
			RequestJobStateNode.CreateNode(RequestState.DataReplicationWait, RequestState.Finalization, null, null);
			RequestJobStateNode.CreateNode(RequestState.ADUpdate, RequestState.Finalization, null, null);
			RequestJobStateNode.CreateNode(RequestState.Cleanup, RequestState.Completion, null, null);
			RequestJobStateNode.CreateNode(RequestState.Stalled, RequestState.InProgress, (MDBPerfCounterHelper h) => h.StallsTotal, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesStalledTotal);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToHA, RequestState.Stalled, (MDBPerfCounterHelper h) => h.StallsHA, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesStalledHA);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToCI, RequestState.Stalled, (MDBPerfCounterHelper h) => h.StallsCI, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesStalledCI);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToMailboxLock, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToWriteThrottle, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToReadThrottle, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToReadCpu, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToWriteCpu, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToReadUnknown, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.StalledDueToWriteUnknown, RequestState.Stalled, null, null);
			RequestJobStateNode.CreateNode(RequestState.TransientFailure, RequestState.InProgress, (MDBPerfCounterHelper h) => h.TransientTotal, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesTransientFailures);
			RequestJobStateNode.CreateNode(RequestState.NetworkFailure, RequestState.TransientFailure, (MDBPerfCounterHelper h) => h.NetworkFailures, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesNetworkFailures);
			RequestJobStateNode.CreateNode(RequestState.MDBOffline, RequestState.TransientFailure, null, (MailboxReplicationServicePerMdbPerformanceCountersInstance pc) => pc.ActiveMovesMDBOffline);
			RequestJobStateNode.CreateNode(RequestState.ProxyBackoff, RequestState.TransientFailure, (MDBPerfCounterHelper h) => h.ProxyBackoff, null);
			RequestJobStateNode.CreateNode(RequestState.ServerBusyBackoff, RequestState.TransientFailure, null, null);
			RequestJobStateNode.CreateNode(RequestState.Suspended, RequestState.OverallMove, null, null);
			RequestJobStateNode.CreateNode(RequestState.AutoSuspended, RequestState.Suspended, null, null);
			RequestJobStateNode.CreateNode(RequestState.Relinquished, RequestState.OverallMove, null, null);
			RequestJobStateNode.CreateNode(RequestState.RelinquishedMDBFailover, RequestState.Relinquished, null, null);
			RequestJobStateNode.CreateNode(RequestState.RelinquishedDataGuarantee, RequestState.Relinquished, null, null);
			RequestJobStateNode.CreateNode(RequestState.RelinquishedCIStall, RequestState.Relinquished, null, null);
			RequestJobStateNode.CreateNode(RequestState.RelinquishedHAStall, RequestState.Relinquished, null, null);
			RequestJobStateNode.CreateNode(RequestState.RelinquishedWlmStall, RequestState.Relinquished, null, null);
			RequestJobStateNode.CreateNode(RequestState.Failed, RequestState.OverallMove, (MDBPerfCounterHelper h) => h.FailTotal, null);
			RequestJobStateNode.CreateNode(RequestState.FailedBadItemLimit, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailBadItemLimit, null);
			RequestJobStateNode.CreateNode(RequestState.FailedNetwork, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailNetwork, null);
			RequestJobStateNode.CreateNode(RequestState.FailedStallDueToCI, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailStallCI, null);
			RequestJobStateNode.CreateNode(RequestState.FailedStallDueToHA, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailStallHA, null);
			RequestJobStateNode.CreateNode(RequestState.FailedMAPI, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailMAPI, null);
			RequestJobStateNode.CreateNode(RequestState.FailedOther, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailOther, null);
			RequestJobStateNode.CreateNode(RequestState.FailedStuck, RequestState.Failed, (MDBPerfCounterHelper h) => h.FailOther, null);
			RequestJobStateNode.CreateNode(RequestState.Completed, RequestState.None, (MDBPerfCounterHelper h) => h.Completed, null);
			RequestJobStateNode.CreateNode(RequestState.CompletedWithWarnings, RequestState.Completed, (MDBPerfCounterHelper h) => h.CompletedWithWarnings, null);
			RequestJobStateNode.CreateNode(RequestState.Canceled, RequestState.None, (MDBPerfCounterHelper h) => h.Canceled, null);
		}

		private RequestJobStateNode(RequestState mrState, RequestState mrParent, GetCountRatePerfCounterDelegate getCountRatePerfCounter, GetPerfCounterDelegate getActivePerfCounter)
		{
			this.Children = new List<RequestJobStateNode>();
			this.MRState = mrState;
			RequestJobStateNode.states[mrState] = this;
			this.Parent = ((mrParent != RequestState.None) ? RequestJobStateNode.states[mrParent] : null);
			if (this.Parent != null)
			{
				this.Parent.Children.Add(this);
			}
			else
			{
				RequestJobStateNode.RootStates.Add(this);
			}
			this.GetCountRatePerfCounter = getCountRatePerfCounter;
			this.GetActivePerfCounter = getActivePerfCounter;
		}

		public static List<RequestJobStateNode> RootStates { get; private set; } = new List<RequestJobStateNode>();

		public RequestState MRState { get; private set; }

		public RequestJobStateNode Parent { get; private set; }

		public List<RequestJobStateNode> Children { get; private set; }

		internal GetCountRatePerfCounterDelegate GetCountRatePerfCounter { get; private set; }

		internal GetPerfCounterDelegate GetActivePerfCounter { get; private set; }

		public static RequestJobStateNode GetState(RequestState mrState)
		{
			RequestJobStateNode result;
			if (RequestJobStateNode.states.TryGetValue(mrState, out result))
			{
				return result;
			}
			return null;
		}

		public static bool RequestStateIs(RequestState currentState, RequestState stateToCheck)
		{
			for (RequestJobStateNode requestJobStateNode = RequestJobStateNode.GetState(currentState); requestJobStateNode != null; requestJobStateNode = requestJobStateNode.Parent)
			{
				if (requestJobStateNode.MRState == stateToCheck)
				{
					return true;
				}
			}
			return false;
		}

		private static void CreateNode(RequestState mrState, RequestState mrParent, GetCountRatePerfCounterDelegate getCountRatePerfCounter, GetPerfCounterDelegate getActivePerfCounter)
		{
			new RequestJobStateNode(mrState, mrParent, getCountRatePerfCounter, getActivePerfCounter);
		}

		public override string ToString()
		{
			return this.MRState.ToString();
		}

		private static Dictionary<RequestState, RequestJobStateNode> states = new Dictionary<RequestState, RequestJobStateNode>();
	}
}
