using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.Sharing.ConsumerSharing;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingProvider
	{
		private SharingProvider(Guid providerGuid, string providerName, string providerUrl, string providerExternalName, SharingProviderHandler providerHandler)
		{
			this.Guid = providerGuid;
			this.Name = providerName;
			this.Url = providerUrl;
			this.ExternalName = providerExternalName;
			this.sharingProviderHandler = providerHandler;
		}

		internal Guid Guid { get; private set; }

		internal string Name { get; private set; }

		internal string Url { get; private set; }

		internal string ExternalName { get; private set; }

		public override string ToString()
		{
			return this.ExternalName;
		}

		internal static SharingProvider FromExternalName(string externalName)
		{
			if (!string.IsNullOrEmpty(externalName))
			{
				foreach (SharingProvider sharingProvider in SharingProvider.AllSharingProviders)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(sharingProvider.ExternalName, externalName))
					{
						return sharingProvider;
					}
				}
			}
			return null;
		}

		internal static SharingProvider FromGuid(Guid guid)
		{
			foreach (SharingProvider sharingProvider in SharingProvider.AllSharingProviders)
			{
				if (sharingProvider.Guid == guid)
				{
					return sharingProvider;
				}
			}
			return null;
		}

		internal static SharingProvider[] GetCompatibleProviders(Folder folderToShare)
		{
			List<SharingProvider> list = new List<SharingProvider>(SharingProvider.AllSharingProviders.Length);
			if (folderToShare != null)
			{
				foreach (SharingProvider sharingProvider in SharingProvider.AllSharingProviders)
				{
					if (sharingProvider.IsCompatible(folderToShare))
					{
						list.Add(sharingProvider);
					}
				}
			}
			return list.ToArray();
		}

		internal static SharingProvider[] GetCompatibleProviders(SharingProvider provider, Folder folderToShare)
		{
			List<SharingProvider> list = new List<SharingProvider>(SharingProvider.AllSharingProviders.Length);
			if (provider != null)
			{
				list.Add(provider);
				if (provider == SharingProvider.SharingProviderInternal)
				{
					if (SharingProvider.SharingProviderExternal.IsCompatible(folderToShare))
					{
						list.Add(SharingProvider.SharingProviderExternal);
					}
					if (SharingProvider.SharingProviderPublish.IsCompatible(folderToShare))
					{
						list.Add(SharingProvider.SharingProviderPublish);
					}
				}
			}
			return list.ToArray();
		}

		internal SharingMessageProvider CreateSharingMessageProvider()
		{
			return new SharingMessageProvider
			{
				Type = this.ExternalName
			};
		}

		internal SharingMessageProvider CreateSharingMessageProvider(SharingContext context)
		{
			Util.ThrowOnNullArgument(context, "context");
			SharingMessageProvider sharingMessageProvider = this.CreateSharingMessageProvider();
			this.sharingProviderHandler.FillSharingMessageProvider(context, sharingMessageProvider);
			return sharingMessageProvider;
		}

		internal void ParseSharingMessageProvider(SharingContext context, SharingMessageProvider sharingMessageProvider)
		{
			Util.ThrowOnNullArgument(context, "context");
			Util.ThrowOnNullArgument(sharingMessageProvider, "sharingMessageProvider");
			this.sharingProviderHandler.ParseSharingMessageProvider(context, sharingMessageProvider);
		}

		internal bool IsCompatible(Folder folderToShare)
		{
			return this.sharingProviderHandler.ValidateCompatibility(folderToShare);
		}

		internal CheckRecipientsResults CheckRecipients(ADRecipient mailboxOwner, SharingContext context, string[] recipients)
		{
			Util.ThrowOnNullArgument(context, "context");
			CheckRecipientsResults checkRecipientsResults = this.sharingProviderHandler.CheckRecipients(mailboxOwner, recipients);
			context.AvailableSharingProviders[this] = checkRecipientsResults;
			return checkRecipientsResults;
		}

		internal PerformInvitationResults PerformInvitation(MailboxSession mailboxSession, SharingContext context, ValidRecipient[] recipients, IFrontEndLocator frontEndLocator)
		{
			return this.sharingProviderHandler.PerformInvitation(mailboxSession, context, recipients, frontEndLocator);
		}

		internal void PerformRevocation(MailboxSession mailboxSession, SharingContext context)
		{
			this.sharingProviderHandler.PerformRevocation(mailboxSession, context);
		}

		internal SubscribeResults PerformSubscribe(MailboxSession mailboxSession, SharingContext context)
		{
			return this.sharingProviderHandler.PerformSubscribe(mailboxSession, context);
		}

		private const string ExchangeBrand = "Microsoft Exchange";

		private const string ExchangeProviderUrl = "http://www.microsoft.com/exchange/";

		public static readonly SharingProvider SharingProviderInternal = new SharingProvider(new Guid("{0006F0AE-0000-0000-C000-000000000046}"), "Microsoft Exchange", "http://www.microsoft.com/exchange/", "ms-exchange-internal", new SharingProviderHandlerInternal());

		public static readonly SharingProvider SharingProviderExternal = new SharingProvider(new Guid("{0006F0C0-0000-0000-C000-000000000046}"), "Microsoft Exchange", "http://www.microsoft.com/exchange/", "ms-exchange-external", new SharingProviderHandlerExternal());

		public static readonly SharingProvider SharingProviderPublish = new SharingProvider(new Guid("{0006F0AC-0000-0000-C000-000000000046}"), "Microsoft Exchange", "http://www.microsoft.com/exchange/", "ms-exchange-publish", new SharingProviderHandlerPublish());

		public static readonly SharingProvider SharingProviderPublishReach = new SharingProvider(new Guid("{0006F0AC-0000-0000-C000-000000000046}"), "Microsoft Exchange", "http://www.microsoft.com/exchange/", "ms-exchange-publish", new SharingProviderHandlerPublishReach());

		public static readonly SharingProvider SharingProviderConsumer = new SharingProvider(new Guid("{45BA1A35-B7D3-48E2-8466-B0E22509629A}"), "Microsoft Exchange", "http://www.microsoft.com/exchange/", "ms-exchange-consumer", new SharingProviderHandlerConsumer());

		internal static readonly SharingProvider[] AllSharingProviders = new SharingProvider[]
		{
			SharingProvider.SharingProviderConsumer,
			SharingProvider.SharingProviderInternal,
			SharingProvider.SharingProviderExternal,
			SharingProvider.SharingProviderPublish
		};

		private readonly SharingProviderHandler sharingProviderHandler;
	}
}
