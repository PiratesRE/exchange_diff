using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EscalationGetter
	{
		public EscalationGetter(GroupMailboxLocator group, IMailboxSession groupSession)
		{
			ArgumentValidator.ThrowIfNull("group", group);
			ArgumentValidator.ThrowIfNull("groupSession", groupSession);
			this.group = group;
			this.groupSession = groupSession;
		}

		public List<string> Execute()
		{
			IExtensibleLogger logger = GroupEscalationGetterDiagnosticsFrameFactory.Default.CreateLogger(this.groupSession.MailboxGuid, this.groupSession.OrganizationId);
			IMailboxAssociationPerformanceTracker mailboxAssociationPerformanceTracker = GroupEscalationGetterDiagnosticsFrameFactory.Default.CreatePerformanceTracker(null);
			List<string> result;
			using (GroupEscalationGetterDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("XSO", "EscalationGetter", logger, mailboxAssociationPerformanceTracker))
			{
				StoreBuilder storeBuilder = new StoreBuilder(this.groupSession, XSOFactory.Default, logger, this.groupSession.ClientInfoString);
				using (IAssociationStore associationStore = storeBuilder.Create(this.group, mailboxAssociationPerformanceTracker))
				{
					IEnumerable<IPropertyBag> associationsByType = associationStore.GetAssociationsByType("IPM.MailboxAssociation.User", MailboxAssociationBaseSchema.ShouldEscalate, EscalationGetter.EscalateProperties);
					List<string> list = new List<string>();
					foreach (IPropertyBag propertyBag in associationsByType)
					{
						string text = propertyBag[MailboxAssociationBaseSchema.LegacyDN] as string;
						if (text != null)
						{
							list.Add(text);
						}
						else
						{
							EscalationGetter.Tracer.TraceError<string>((long)this.GetHashCode(), "EscalationGetter.Execute: Missing LegacyDn for item with Id {0}.", propertyBag[ItemSchema.Id].ToString());
						}
						bool valueOrDefault = associationStore.GetValueOrDefault<bool>(propertyBag, MailboxAssociationBaseSchema.IsAutoSubscribed, false);
						if (valueOrDefault)
						{
							mailboxAssociationPerformanceTracker.IncrementAutoSubscribedMembers();
						}
					}
					result = list;
				}
			}
			return result;
		}

		private const string OperationContext = "XSO";

		private const string Operation = "EscalationGetter";

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		private readonly GroupMailboxLocator group;

		private readonly IMailboxSession groupSession;

		private static readonly PropertyDefinition[] EscalateProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			MailboxAssociationBaseSchema.ExternalId,
			MailboxAssociationBaseSchema.LegacyDN,
			MailboxAssociationBaseSchema.IsAutoSubscribed
		};
	}
}
