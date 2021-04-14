using System;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class XSOSyncContentConversion
	{
		internal static string DefaultDomain
		{
			private get
			{
				if (XSOSyncContentConversion.testDomain != null)
				{
					return XSOSyncContentConversion.testDomain;
				}
				return Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain;
			}
			set
			{
				XSOSyncContentConversion.testDomain = value;
			}
		}

		internal static InboundConversionOptions GetScopedInboundConversionOptions(IRecipientSession recipientSession)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			return new InboundConversionOptions(recipientSession, XSOSyncContentConversion.DefaultDomain)
			{
				IsSenderTrusted = true,
				ServerSubmittedSecurely = true,
				RecipientCache = null,
				ClearCategories = true,
				Limits = 
				{
					MimeLimits = MimeLimits.Unlimited
				},
				ApplyHeaderFirewall = true
			};
		}

		internal static OutboundConversionOptions GetScopedOutboundConversionOptions(IRecipientSession recipientSession)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			return new OutboundConversionOptions(recipientSession, XSOSyncContentConversion.DefaultDomain)
			{
				IsSenderTrusted = true,
				RecipientCache = null,
				ClearCategories = true,
				Limits = 
				{
					MimeLimits = MimeLimits.Unlimited
				},
				ClearCategories = false,
				AllowPartialStnefConversion = true
			};
		}

		internal static Stream ConvertItemToMime(Item item, OutboundConversionOptions scopedOutboundConversionOptions)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (scopedOutboundConversionOptions == null)
			{
				throw new ArgumentNullException("scopedOutboundConversionOptions");
			}
			Stream stream = TemporaryStorage.Create();
			bool flag = false;
			try
			{
				ItemConversion.ConvertItemToMime(item, stream, scopedOutboundConversionOptions);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					stream.Dispose();
				}
			}
			stream.Position = 0L;
			return stream;
		}

		internal static void ConvertAnyMimeToItem(Stream mimeStream, Item item, InboundConversionOptions scopedInboundConversionOptions)
		{
			if (mimeStream == null)
			{
				throw new ArgumentNullException("mimeStream");
			}
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (scopedInboundConversionOptions == null)
			{
				throw new ArgumentNullException("scopedInboundConversionOptions");
			}
			ItemConversion.ConvertAnyMimeToItem(item, mimeStream, scopedInboundConversionOptions);
		}

		private static string testDomain;
	}
}
