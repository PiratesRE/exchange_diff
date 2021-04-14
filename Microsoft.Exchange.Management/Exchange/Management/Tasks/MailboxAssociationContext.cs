using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MailboxAssociationContext
	{
		public MailboxAssociationContext(IRecipientSession adSession, ADUser mailbox, string cmdletName, MailboxAssociationIdParameter associationId, bool includeNotPromotedProperties = false)
		{
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("mailbox", mailbox);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("cmdletName", cmdletName);
			this.adSession = adSession;
			this.mailbox = mailbox;
			this.associationId = associationId;
			this.clientInfoString = string.Format("Client=Management;Action={0}", cmdletName);
			this.cmdletName = cmdletName;
			this.includeNotPromotedProperties = includeNotPromotedProperties;
			this.groupMailboxAccessLayerFactory = GroupMailboxAccessLayerEntityFactory.Instantiate(adSession, mailbox);
		}

		public void Execute(Action<MailboxAssociationFromStore, IAssociationAdaptor, ADUser, IExtensibleLogger> task)
		{
			IExtensibleLogger logger = MailboxAssociationDiagnosticsFrameFactory.Default.CreateLogger(this.mailbox.ExchangeGuid, this.mailbox.OrganizationId);
			IMailboxAssociationPerformanceTracker performanceTracker = MailboxAssociationDiagnosticsFrameFactory.Default.CreatePerformanceTracker(null);
			using (MailboxAssociationDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("MailboxAssociationContext.Execute", this.clientInfoString, logger, performanceTracker))
			{
				StoreBuilder storeBuilder = new StoreBuilder(null, XSOFactory.Default, logger, this.clientInfoString);
				GroupMailboxAccessLayer groupMailboxAccessLayer = new GroupMailboxAccessLayer(this.adSession, storeBuilder, performanceTracker, logger, this.clientInfoString);
				MailboxLocator mailboxLocator = this.groupMailboxAccessLayerFactory.CreateMasterLocator();
				using (IAssociationStore associationStore = storeBuilder.Create(mailboxLocator, groupMailboxAccessLayer.PerformanceTracker))
				{
					BaseAssociationAdaptor associationAdaptor = this.groupMailboxAccessLayerFactory.CreateAssociationAdaptor(mailboxLocator, associationStore);
					if (this.associationId.AssociationIdType == null)
					{
						this.ExecuteForAllAssociations(task, associationAdaptor, logger);
					}
					else
					{
						this.ExecuteForSingleAssociation(task, associationAdaptor, logger);
					}
				}
			}
		}

		private void ExecuteForSingleAssociation(Action<MailboxAssociationFromStore, IAssociationAdaptor, ADUser, IExtensibleLogger> task, BaseAssociationAdaptor associationAdaptor, IExtensibleLogger logger)
		{
			MailboxAssociation mailboxAssociation;
			if (this.associationId.AssociationIdType == MailboxAssociationIdParameter.IdTypeItemId)
			{
				mailboxAssociation = this.GetAssociationByItemId(associationAdaptor, this.associationId.AssociationIdValue);
			}
			else
			{
				mailboxAssociation = this.GetAssociationByLocator(associationAdaptor);
			}
			MailboxAssociationContext.Tracer.TraceDebug<string, MailboxAssociation>((long)this.GetHashCode(), "MailboxAssociationContext.ExecuteForSingleAssociation [{0}]: Found association {1}", this.cmdletName, mailboxAssociation);
			task(mailboxAssociation as MailboxAssociationFromStore, associationAdaptor, this.mailbox, logger);
		}

		private MailboxAssociation GetAssociationByLocator(BaseAssociationAdaptor associationAdaptor)
		{
			IMailboxLocator mailboxLocator = this.groupMailboxAccessLayerFactory.CreateSlaveLocator(this.associationId);
			MailboxAssociationContext.Tracer.TraceDebug<string, IMailboxLocator>((long)this.GetHashCode(), "MailboxAssociationContext.GetAssociationByLocator [{0}]: Querying association with locator {1}", this.cmdletName, mailboxLocator);
			return associationAdaptor.GetAssociation(mailboxLocator);
		}

		private MailboxAssociation GetAssociationByItemId(BaseAssociationAdaptor associationAdaptor, string base64ItemId)
		{
			MailboxAssociationContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxAssociationContext.GetAssociationByItemId [{0}]: Querying association by item id parameter", this.cmdletName);
			StoreObjectId itemId = StoreObjectId.Deserialize(base64ItemId);
			VersionedId itemId2 = new VersionedId(itemId, new byte[0]);
			return associationAdaptor.GetAssociation(itemId2);
		}

		private MailboxAssociation GetAssociationByItemId(BaseAssociationAdaptor associationAdaptor, MailboxAssociation association)
		{
			MailboxAssociationContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxAssociationContext.GetAssociationByItemId [{0}]: Querying association by its item id", this.cmdletName);
			MailboxAssociationFromStore mailboxAssociationFromStore = association as MailboxAssociationFromStore;
			if (mailboxAssociationFromStore != null)
			{
				association = associationAdaptor.GetAssociation(mailboxAssociationFromStore.ItemId);
			}
			return association;
		}

		private void ExecuteForAllAssociations(Action<MailboxAssociationFromStore, IAssociationAdaptor, ADUser, IExtensibleLogger> task, BaseAssociationAdaptor associationAdaptor, IExtensibleLogger logger)
		{
			MailboxAssociationContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxAssociationContext.ExecuteForAllAssociations [{0}]: Querying all associations for the mailbox.", this.cmdletName);
			IEnumerable<MailboxAssociation> allAssociations = associationAdaptor.GetAllAssociations();
			foreach (MailboxAssociation mailboxAssociation in allAssociations)
			{
				MailboxAssociation mailboxAssociation2 = mailboxAssociation;
				if (this.includeNotPromotedProperties)
				{
					MailboxAssociationContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MailboxAssociationContext.ExecuteForAllAssociations [{0}]: Querying association by ItemId to retrieve not promoted properties.", this.cmdletName);
					mailboxAssociation2 = this.GetAssociationByItemId(associationAdaptor, mailboxAssociation);
				}
				MailboxAssociationContext.Tracer.TraceDebug<string, MailboxAssociation>((long)this.GetHashCode(), "MailboxAssociationContext.ExecuteForAllAssociations [{0}]: Found association {1}", this.cmdletName, mailboxAssociation);
				task(mailboxAssociation2 as MailboxAssociationFromStore, associationAdaptor, this.mailbox, logger);
			}
		}

		private static readonly Trace Tracer = ExTraceGlobals.CmdletsTracer;

		private readonly IRecipientSession adSession;

		private readonly ADUser mailbox;

		private readonly MailboxAssociationIdParameter associationId;

		private readonly GroupMailboxAccessLayerEntityFactory groupMailboxAccessLayerFactory;

		private readonly string clientInfoString;

		private readonly string cmdletName;

		private readonly bool includeNotPromotedProperties;
	}
}
