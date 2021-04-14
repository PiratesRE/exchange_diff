using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UpdateAssociationCommand
	{
		protected UpdateAssociationCommand(IExtensibleLogger logger, IAssociationAdaptor masterAdaptor, params IMailboxLocator[] itemLocators)
		{
			ArgumentValidator.ThrowIfNull("logger", logger);
			ArgumentValidator.ThrowIfNull("masterAdaptor", masterAdaptor);
			ArgumentValidator.ThrowIfNull("itemLocators", itemLocators);
			ArgumentValidator.ThrowIfZeroOrNegative("itemLocators.Length", itemLocators.Length);
			this.MasterAdaptor = masterAdaptor;
			this.Logger = logger;
			this.ItemLocators = itemLocators;
		}

		private protected IMailboxLocator[] ItemLocators { protected get; private set; }

		private protected IAssociationAdaptor MasterAdaptor { protected get; private set; }

		public void Execute()
		{
			List<MailboxAssociation> list = new List<MailboxAssociation>(this.ItemLocators.Length);
			foreach (IMailboxLocator mailboxLocator in this.ItemLocators)
			{
				MailboxAssociation mailboxAssociation = this.LoadAssociation(mailboxLocator);
				if (this.UpdateAssociation(mailboxAssociation))
				{
					this.SaveAssociation(mailboxAssociation);
					list.Add(mailboxAssociation);
					UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator>((long)this.GetHashCode(), "Saved association for user {0}", mailboxAssociation.User);
				}
				else if (mailboxAssociation.IsOutOfSync(this.MasterAdaptor.MasterLocator.IdentityHash))
				{
					list.Add(mailboxAssociation);
					UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator>((long)this.GetHashCode(), "Ignored saving association for user {0}, but association is out of sync, so replication will be attempted.", mailboxAssociation.User);
				}
				else
				{
					UpdateAssociationCommand.Tracer.TraceDebug<UserMailboxLocator>((long)this.GetHashCode(), "Ignored saving association for user {0}", mailboxAssociation.User);
				}
			}
			IAssociationReplicator associationReplicator = this.GetAssociationReplicator();
			if (list.Count > 0 && associationReplicator != null)
			{
				associationReplicator.ReplicateAssociation(this.MasterAdaptor, list.ToArray());
			}
			this.OnPostExecute();
		}

		protected virtual void OnPostExecute()
		{
		}

		protected MailboxAssociation LoadAssociation(IMailboxLocator mailboxLocator)
		{
			ArgumentValidator.ThrowIfNull("mailboxLocator", mailboxLocator);
			MailboxAssociation association = this.MasterAdaptor.GetAssociation(mailboxLocator);
			UpdateAssociationCommand.Tracer.TraceDebug<IMailboxLocator, MailboxAssociation>((long)this.GetHashCode(), "LoadAssociation: mailboxLocator={0}, association={1}", mailboxLocator, association);
			return association;
		}

		protected abstract bool UpdateAssociation(MailboxAssociation association);

		protected virtual IAssociationReplicator GetAssociationReplicator()
		{
			return null;
		}

		protected void SaveAssociation(MailboxAssociation association)
		{
			UpdateAssociationCommand.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "SaveAssociation: {0}", association);
			ArgumentValidator.ThrowIfNull("association", association);
			bool markForReplication = this.GetAssociationReplicator() != null;
			this.MasterAdaptor.SaveAssociation(association, markForReplication);
		}

		protected static readonly Trace Tracer = ExTraceGlobals.UpdateAssociationCommandTracer;

		protected readonly IExtensibleLogger Logger;
	}
}
