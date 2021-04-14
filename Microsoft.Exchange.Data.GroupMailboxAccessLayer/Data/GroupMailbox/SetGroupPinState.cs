using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net.AAD;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetGroupPinState : UpdateAssociationCommand
	{
		public SetGroupPinState(IExtensibleLogger logger, IAssociationReplicator associationReplicator, bool pin, IGroupAssociationAdaptor masterAdaptor, GroupMailboxLocator itemLocator, IMailboxAssociationPerformanceTracker performanceTracker = null, bool isModernGroupsNewArchitecture = false) : base(logger, masterAdaptor, new IMailboxLocator[]
		{
			itemLocator
		})
		{
			this.associationReplicator = associationReplicator;
			this.pin = pin;
			this.performanceTracker = performanceTracker;
			this.isModernGroupsNewArchitecture = isModernGroupsNewArchitecture;
		}

		protected override IAssociationReplicator GetAssociationReplicator()
		{
			return this.associationReplicator;
		}

		protected override bool UpdateAssociation(MailboxAssociation association)
		{
			if (this.pin == association.IsPin)
			{
				UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, bool>((long)this.GetHashCode(), "User {0} pin state is already same {1}", association.User, this.pin);
				return false;
			}
			association.IsPin = this.pin;
			association.PinDate = (this.pin ? ExDateTime.UtcNow : default(ExDateTime));
			return this.VerifyMembershipState(association);
		}

		private bool VerifyMembershipState(MailboxAssociation association)
		{
			if (association.IsPin && !association.IsMember && (ExEnvironment.IsTest || this.isModernGroupsNewArchitecture))
			{
				UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, GroupMailboxLocator>((long)this.GetHashCode(), "User {0} is trying to pin, but membership for {1} is not updated. Trying to fix it", association.User, association.Group);
				if (!this.IsUserMemberOfGroup(association))
				{
					UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, string>((long)this.GetHashCode(), "User {0} is not a member for {1} in AAD, ignoring it", association.User, association.Group.ExternalId);
					throw new NotAMemberException(Strings.CannotPinGroupForNonMember);
				}
				UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, string>((long)this.GetHashCode(), "User {0} is a member for {1} but not marked in EXO. Fixing it", association.User, association.Group.ExternalId);
				association.IsMember = true;
				association.JoinDate = ExDateTime.UtcNow;
				this.Logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Warning>
				{
					{
						MailboxAssociationLogSchema.Warning.Context,
						"SetGroupPinState"
					},
					{
						MailboxAssociationLogSchema.Warning.Message,
						string.Format("User {0} is a member for {1} but not marked in EXO. Fixing it", association.User.ExternalId, association.Group.ExternalId)
					}
				});
			}
			return true;
		}

		private bool IsUserMemberOfGroup(MailboxAssociation association)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool result;
			try
			{
				IAadClient aadclient = this.GetAADClient(association);
				if (aadclient != null)
				{
					result = aadclient.IsUserMemberOfGroup(association.User.ExternalId, association.Group.ExternalId);
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				stopwatch.Stop();
				if (this.performanceTracker != null)
				{
					this.performanceTracker.SetAADQueryLatency(stopwatch.ElapsedMilliseconds);
				}
			}
			return result;
		}

		private IAadClient GetAADClient(MailboxAssociation association)
		{
			if (AADClientTestHooks.GraphApi_GetAadClient != null)
			{
				return AADClientTestHooks.GraphApi_GetAadClient();
			}
			ADUser user = association.User.FindAdUser();
			return AADClientFactory.Create(user);
		}

		private readonly IAssociationReplicator associationReplicator;

		private readonly bool pin;

		private readonly IMailboxAssociationPerformanceTracker performanceTracker;

		private readonly bool isModernGroupsNewArchitecture;
	}
}
