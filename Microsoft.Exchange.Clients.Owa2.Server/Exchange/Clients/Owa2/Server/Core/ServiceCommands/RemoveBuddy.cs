using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class RemoveBuddy : InstantMessageCommandBase<bool>
	{
		static RemoveBuddy()
		{
			OwsLogRegistry.Register(OwaApplication.GetRequestDetailsLogger.Get(ExtensibleLoggerMetadata.EventId), typeof(InstantMessagingBuddyMetadata), new Type[0]);
		}

		public RemoveBuddy(CallContext callContext, InstantMessageBuddy instantMessageBuddy, ItemId personId) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(instantMessageBuddy, "instantMessageBuddy", "RemoveBuddy");
			WcfServiceCommandBase.ThrowIfNull(personId, "personId", "RemoveBuddy");
			this.instantMessageBuddy = instantMessageBuddy;
			this.personId = IdConverter.EwsIdToPersonId(personId.Id);
		}

		protected override bool InternalExecute()
		{
			InstantMessageProvider provider = base.Provider;
			StoreId storeId = null;
			if (provider == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "instant message provider is null");
				return false;
			}
			using (Folder folder = Folder.Bind(base.MailboxIdentityMailboxSession, DefaultFolderType.QuickContacts))
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Removing instant message buddy:{0},{1}", this.instantMessageBuddy.DisplayName, this.instantMessageBuddy.SipUri);
				ComparisonFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ContactSchema.PersonId, this.personId);
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, queryFilter, null, RemoveBuddy.contactProperties))
				{
					IStorePropertyBag[] propertyBags = queryResult.GetPropertyBags(1);
					if (propertyBags == null || propertyBags.Length == 0)
					{
						ExTraceGlobals.InstantMessagingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Lync contact cannot be found in store: {0},{1}", this.instantMessageBuddy.DisplayName, this.instantMessageBuddy.SipUri);
						return false;
					}
					storeId = propertyBags[0].GetValueOrDefault<VersionedId>(ItemSchema.Id, null).ObjectId;
				}
			}
			ExTraceGlobals.InstantMessagingTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Removing instant message buddy after retrieving store id of the contact:{0},{1}", this.instantMessageBuddy.DisplayName, storeId.ToBase64String());
			provider.RemoveBuddy(base.MailboxIdentityMailboxSession, this.instantMessageBuddy, storeId);
			return true;
		}

		private static readonly PropertyDefinition[] contactProperties = new PropertyDefinition[]
		{
			ContactSchema.PersonId,
			ItemSchema.Id
		};

		private InstantMessageBuddy instantMessageBuddy;

		private PersonId personId;
	}
}
