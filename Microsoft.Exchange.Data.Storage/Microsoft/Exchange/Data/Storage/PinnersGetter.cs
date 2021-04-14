using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PinnersGetter
	{
		public PinnersGetter(GroupMailboxLocator group, MailboxSession groupSession)
		{
			ArgumentValidator.ThrowIfNull("group", group);
			ArgumentValidator.ThrowIfNull("groupSession", groupSession);
			this.group = group;
			this.groupSession = groupSession;
		}

		public List<string> Execute()
		{
			IExtensibleLogger logger = MailboxAssociationDiagnosticsFrameFactory.Default.CreateLogger(this.groupSession.MailboxGuid, this.groupSession.OrganizationId);
			IMailboxAssociationPerformanceTracker performanceTracker = MailboxAssociationDiagnosticsFrameFactory.Default.CreatePerformanceTracker(null);
			List<string> result;
			using (MailboxAssociationDiagnosticsFrameFactory.Default.CreateDiagnosticsFrame("XSO", "PinnersGetter", logger, performanceTracker))
			{
				StoreBuilder storeBuilder = new StoreBuilder(this.groupSession, XSOFactory.Default, logger, this.groupSession.ClientInfoString);
				using (IAssociationStore associationStore = storeBuilder.Create(this.group, performanceTracker))
				{
					IEnumerable<IPropertyBag> associationsByType = associationStore.GetAssociationsByType("IPM.MailboxAssociation.User", MailboxAssociationBaseSchema.IsPin, PinnersGetter.PinnerProperties);
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
							PinnersGetter.Tracer.TraceError<string>((long)this.GetHashCode(), "PinnersGetter.Execute: Missing LegacyDn for item with Id {0}.", propertyBag[ItemSchema.Id].ToString());
						}
					}
					result = list;
				}
			}
			return result;
		}

		private const string OperationContext = "XSO";

		private const string Operation = "PinnersGetter";

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;

		private readonly GroupMailboxLocator group;

		private readonly MailboxSession groupSession;

		private static readonly PropertyDefinition[] PinnerProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			MailboxAssociationBaseSchema.ExternalId,
			MailboxAssociationBaseSchema.LegacyDN
		};
	}
}
