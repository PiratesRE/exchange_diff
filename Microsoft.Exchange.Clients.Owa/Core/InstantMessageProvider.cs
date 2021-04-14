using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class InstantMessageProvider : DisposeTrackableBase
	{
		internal virtual bool IsActivityBasedPresenceSet { get; set; }

		internal abstract bool IsSessionStarted { get; }

		internal abstract void EstablishSession();

		internal abstract void ResetPresence();

		internal abstract int SendMessage(InstantMessageProvider.ProviderMessage message);

		internal abstract int SendNewChatMessage(InstantMessageProvider.ProviderMessage message);

		internal abstract void AddBuddy(InstantMessageBuddy buddy, InstantMessageGroup group);

		internal abstract void RemoveBuddy(InstantMessageBuddy buddy);

		internal abstract void EndChatSession(int chatSessionId, bool disconnectSession);

		internal abstract void NotifyTyping(int chatSessionId, bool typingCanceled);

		internal abstract void PublishSelfPresence(int presence);

		internal abstract void MakeEndpointMostActive();

		internal abstract void RemoveFromGroup(InstantMessageGroup group, InstantMessageBuddy buddy);

		internal abstract void MoveBuddy(InstantMessageGroup oldGroup, InstantMessageGroup newGroup, InstantMessageBuddy buddy);

		internal abstract void CopyBuddy(InstantMessageGroup group, InstantMessageBuddy buddy);

		internal abstract void CreateGroup(string groupName);

		internal abstract void RemoveGroup(InstantMessageGroup group);

		internal abstract void RenameGroup(InstantMessageGroup group, string newGroupName);

		internal abstract void AcceptBuddy(InstantMessageBuddy buddy, InstantMessageGroup group);

		internal abstract void DeclineBuddy(InstantMessageBuddy buddy);

		internal abstract void GetBuddyList();

		internal virtual void BlockBuddy(InstantMessageBuddy buddy)
		{
			throw new NotImplementedException();
		}

		internal virtual void UnblockBuddy(InstantMessageBuddy buddy)
		{
			throw new NotImplementedException();
		}

		internal virtual void AddSubscription(string[] sipUris)
		{
			throw new NotImplementedException();
		}

		internal virtual void RemoveSubscription(string sipUri)
		{
			throw new NotImplementedException();
		}

		internal virtual void QueryPresence(string[] sipUris)
		{
			throw new NotImplementedException();
		}

		internal virtual void PublishResetStatus()
		{
			throw new NotImplementedException();
		}

		internal virtual void ParticipateInConversation(int conversationId)
		{
			throw new NotImplementedException();
		}

		internal InstantMessagePayload Payload;

		protected UserContext userContext;

		internal HashSet<string> ExpandedGroupIds = new HashSet<string>();

		protected bool isEarlierSignInSuccessful = true;

		internal struct ProviderMessage
		{
			public string Body;

			public string Format;

			public int ChatSessionId;

			public string[] Recipients;

			public int[] AddressTypes;
		}
	}
}
