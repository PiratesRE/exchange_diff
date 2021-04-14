using System;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetEscalate : UpdateAssociationCommand
	{
		public SetEscalate(IExtensibleLogger logger, bool shouldEscalate, SmtpAddress userSmtpAddress, IUserAssociationAdaptor masterAdaptor, UserMailboxLocator itemLocator, int maxEscalatedMembers = 400) : base(logger, masterAdaptor, new IMailboxLocator[]
		{
			itemLocator
		})
		{
			this.shouldEscalate = shouldEscalate;
			this.userSmtpAddress = userSmtpAddress;
			this.maxEscalatedMembers = maxEscalatedMembers;
		}

		protected override bool UpdateAssociation(MailboxAssociation association)
		{
			if (this.shouldEscalate == association.ShouldEscalate)
			{
				UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator, bool>((long)this.GetHashCode(), "User {0} escalate state is already same {1}", association.User, this.shouldEscalate);
				return false;
			}
			if (this.shouldEscalate && !association.IsMember)
			{
				UpdateAssociationCommand.Tracer.TraceError<UserMailboxLocator, IMailboxLocator>((long)this.GetHashCode(), "Only Members can Subscribe to group, throwing NotAMemberException. User={0}, Group={1}", association.User, base.MasterAdaptor.MasterLocator);
				throw new NotAMemberException(Strings.CannotEscalateForNonMember);
			}
			if (this.shouldEscalate && GetEscalatedAssociations.GetEscalatedAssociationsCount(base.MasterAdaptor) >= this.maxEscalatedMembers)
			{
				throw new ExceededMaxSubscribersException(Strings.MaxSubscriptionsForGroupReached);
			}
			association.UserSmtpAddress = this.userSmtpAddress;
			association.ShouldEscalate = this.shouldEscalate;
			return true;
		}

		public const int DefaultMaxEscalatedMembers = 400;

		private readonly int maxEscalatedMembers;

		private readonly bool shouldEscalate;

		private readonly SmtpAddress userSmtpAddress;
	}
}
