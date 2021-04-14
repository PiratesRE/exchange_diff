using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetUserMembershipState : UpdateAssociationCommand
	{
		public SetUserMembershipState(IExtensibleLogger logger, IAssociationReplicator associationReplicator, bool isMember, string joinedBy, IUserAssociationAdaptor masterAdaptor, IRecipientSession adSession, params UserMailboxLocator[] itemLocators) : base(logger, masterAdaptor, itemLocators)
		{
			this.associationReplicator = associationReplicator;
			this.isMember = isMember;
			this.joinedBy = joinedBy;
			this.adSession = adSession;
			this.groupAdUser = base.MasterAdaptor.MasterLocator.FindAdUser();
		}

		protected override IAssociationReplicator GetAssociationReplicator()
		{
			return this.associationReplicator;
		}

		protected override bool UpdateAssociation(MailboxAssociation association)
		{
			if (association.IsMember == this.isMember)
			{
				UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, bool>((long)this.GetHashCode(), "User {0} membership state is already same {1}", association.User, this.isMember);
				return false;
			}
			association.IsMember = this.isMember;
			association.JoinedBy = this.joinedBy;
			association.JoinDate = ExDateTime.UtcNow;
			if (this.isMember)
			{
				bool flag = association.User.IsValidReplicationTarget();
				bool flag2 = false;
				if (base.MasterAdaptor != null && base.MasterAdaptor.MasterLocator != null && base.MasterAdaptor.MasterLocator.FindAdUser() != null)
				{
					flag2 = base.MasterAdaptor.MasterLocator.FindAdUser().AutoSubscribeNewGroupMembers;
				}
				if (flag2 || !flag)
				{
					UpdateAssociationCommand.Tracer.TraceDebug((long)this.GetHashCode(), "Escalating association : User={0}, Group={1}, Reason : IsValidReplicationTarget={2}, AutoSubscribe={3}", new object[]
					{
						association.User,
						base.MasterAdaptor.MasterLocator,
						flag,
						flag2
					});
					association.ShouldEscalate = true;
					association.IsAutoSubscribed = true;
				}
			}
			else
			{
				UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, IMailboxLocator>((long)this.GetHashCode(), "User Leaving group, updating ShouldEscalate and IsPin to False. User={0}, Group={1}", association.User, base.MasterAdaptor.MasterLocator);
				association.ShouldEscalate = false;
				association.IsAutoSubscribed = false;
				association.IsPin = false;
			}
			return true;
		}

		protected override void OnPostExecute()
		{
			if (this.groupAdUser.AutoSubscribeNewGroupMembers && GetEscalatedAssociations.GetEscalatedAssociationsCount(base.MasterAdaptor) > 100)
			{
				LocalizedException ex = null;
				try
				{
					this.groupAdUser.AutoSubscribeNewGroupMembers = false;
					this.adSession.Save(this.groupAdUser);
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
				catch (ADExternalException ex3)
				{
					ex = ex3;
				}
				catch (ADOperationException ex4)
				{
					ex = ex4;
				}
				finally
				{
					if (ex != null)
					{
						this.Logger.LogEvent(new SchemaBasedLogEvent<MailboxAssociationLogSchema.Error>
						{
							{
								MailboxAssociationLogSchema.Error.Context,
								"SetUserMembershipState: failed to save ADUserObject after unsetting auto-subscribe bit."
							},
							{
								MailboxAssociationLogSchema.Error.Exception,
								ex
							}
						});
					}
				}
			}
		}

		public const int DefaultMaxEscalatedMembers = 100;

		private readonly IAssociationReplicator associationReplicator;

		private readonly IRecipientSession adSession;

		private readonly bool isMember;

		private readonly string joinedBy;

		private readonly ADUser groupAdUser;
	}
}
