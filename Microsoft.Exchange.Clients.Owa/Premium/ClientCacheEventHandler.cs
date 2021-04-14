using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	[OwaEventNamespace("ClientCache")]
	internal sealed class ClientCacheEventHandler : OwaEventHandlerBase
	{
		public static void Register()
		{
			OwaEventRegistry.RegisterHandler(typeof(ClientCacheEventHandler));
		}

		[OwaEvent("Get")]
		public void Get()
		{
			ExTraceGlobals.MailCallTracer.TraceDebug((long)this.GetHashCode(), "ClientCacheEventHandler.Get");
			base.ResponseContentType = OwaEventContentType.Javascript;
			this.Writer.WriteLine("{");
			this.Writer.Write("sAcc : \"");
			using (RecipientWellEventHandler recipientWellObject = new RecipientWellEventHandler())
			{
				RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
				{
					using (TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
					{
						recipientWellObject.GetCache(textWriter, this.OwaContext, this.UserContext);
						Utilities.JavascriptEncode(textWriter.ToString(), this.Writer);
					}
				}, this.GetHashCode());
			}
			this.Writer.WriteLine("\",");
			this.Writer.Write("sPr : \"");
			RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
			{
				RecipientInfoCacheEntry entry = AutoCompleteCacheEntry.ParseLogonExchangePrincipal(base.UserContext.ExchangePrincipal, base.UserContext.SipUri, base.UserContext.MobilePhoneNumber);
				using (TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					AutoCompleteCacheEntry.RenderEntryJavascript(textWriter, entry);
					Utilities.JavascriptEncode(textWriter.ToString(), this.Writer);
				}
			}, this.GetHashCode());
			this.Writer.WriteLine("\",");
			RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
			{
				SubscriptionCache cache = SubscriptionCache.GetCache(base.UserContext);
				SendAddressDefaultSetting sendAddressDefaultSetting = new SendAddressDefaultSetting(base.UserContext);
				SubscriptionCacheEntry subscriptionCacheEntry;
				if (cache.TryGetSendAsDefaultEntry(sendAddressDefaultSetting, out subscriptionCacheEntry))
				{
					this.Writer.Write("sDfltFr : \"");
					using (TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
					{
						subscriptionCacheEntry.RenderToJavascript(textWriter);
						Utilities.JavascriptEncode(textWriter.ToString(), this.Writer);
					}
					this.Writer.WriteLine("\",");
				}
			}, this.GetHashCode());
			this.Writer.Write("sSc : \"");
			RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
			{
				SubscriptionCache cache = SubscriptionCache.GetCache(base.UserContext);
				using (TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					cache.RenderToJavascript(textWriter);
					Utilities.JavascriptEncode(textWriter.ToString(), this.Writer);
				}
			}, this.GetHashCode());
			this.Writer.WriteLine("\",");
			this.Writer.Write("sMruc : \"");
			RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
			{
				SendFromCache sendFromCache = SendFromCache.TryGetCache(base.UserContext);
				if (sendFromCache != null)
				{
					using (TextWriter textWriter = new StringWriter())
					{
						sendFromCache.RenderToJavascript(textWriter);
						Utilities.JavascriptEncode(textWriter.ToString(), this.Writer);
					}
				}
			}, this.GetHashCode());
			this.Writer.WriteLine("\",");
			this.Writer.Write("sLng : '");
			Utilities.JavascriptEncode(base.UserContext.UserCulture.Name, this.Writer);
			this.Writer.WriteLine("',");
			this.Writer.Write("iThmId : ");
			this.Writer.Write(ThemeManager.GetIdFromStorageId(base.UserContext.UserOptions.ThemeStorageId));
			this.Writer.WriteLine(",");
			this.Writer.Write("sSigHtml : '");
			RenderingUtilities.RenderSignature(this.Writer, base.UserContext, true);
			this.Writer.WriteLine("',");
			this.Writer.Write("sSigTxt : '");
			RenderingUtilities.RenderSignature(this.Writer, base.UserContext, false);
			this.Writer.WriteLine("',");
			this.Writer.Write("fSig : ");
			this.Writer.Write((base.UserContext.IsFeatureEnabled(Feature.Signature) && base.UserContext.UserOptions.AutoAddSignature) ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fSp : '");
			this.Writer.Write(base.UserContext.IsFeatureEnabled(Feature.SpellChecker));
			this.Writer.WriteLine("',");
			this.Writer.Write("iSp : '");
			this.Writer.Write(base.UserContext.UserOptions.SpellingDictionaryLanguage);
			this.Writer.WriteLine("',");
			this.Writer.Write("fSpSn : ");
			this.Writer.Write(base.UserContext.UserOptions.SpellingCheckBeforeSend ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("iFmtBrSt : ");
			this.Writer.Write((int)base.UserContext.UserOptions.FormatBarState);
			this.Writer.WriteLine(",");
			this.Writer.Write("sMruFnts : '");
			Utilities.JavascriptEncode(base.UserContext.UserOptions.MruFonts, this.Writer);
			this.Writer.WriteLine("',");
			this.Writer.Write("sInitFntNm : '");
			Utilities.JavascriptEncode(base.UserContext.UserOptions.ComposeFontName, this.Writer);
			this.Writer.WriteLine("',");
			this.Writer.Write("sInitFntSz : '");
			this.Writer.Write(base.UserContext.UserOptions.ComposeFontSize);
			this.Writer.WriteLine("',");
			this.Writer.Write("sDefFntStyl : '");
			RenderingUtilities.RenderDefaultFontStyle(this.Writer, base.UserContext);
			this.Writer.WriteLine("',");
			this.Writer.Write("fDefFntBold : ");
			this.Writer.Write((int)(base.UserContext.UserOptions.ComposeFontFlags & FontFlags.Bold));
			this.Writer.WriteLine(",");
			this.Writer.Write("fDefFntItalc : ");
			this.Writer.Write((int)(base.UserContext.UserOptions.ComposeFontFlags & FontFlags.Italic));
			this.Writer.WriteLine(",");
			this.Writer.Write("fDefFntUndl : ");
			this.Writer.Write((int)(base.UserContext.UserOptions.ComposeFontFlags & FontFlags.Underline));
			this.Writer.WriteLine(",");
			this.Writer.Write("fTxt : ");
			this.Writer.Write((int)base.UserContext.UserOptions.ComposeMarkup);
			this.Writer.WriteLine(",");
			this.Writer.Write("fShwBcc : ");
			this.Writer.Write(base.UserContext.UserOptions.AlwaysShowBcc ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fShwFrm : ");
			this.Writer.Write(base.UserContext.UserOptions.AlwaysShowFrom ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fAEnc : ");
			this.Writer.Write(OwaRegistryKeys.AlwaysEncrypt ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fASgn : ");
			this.Writer.Write(OwaRegistryKeys.AlwaysSign ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fSgn : ");
			this.Writer.Write(base.UserContext.UserOptions.SmimeSign ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fEnc : ");
			this.Writer.Write(base.UserContext.UserOptions.SmimeEncrypt ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("fMstUpd : ");
			this.Writer.WriteLine(OwaRegistryKeys.ForceSMimeClientUpgrade ? 1 : 0);
			this.Writer.WriteLine(",");
			this.Writer.Write("oMailTipsConfig : {");
			this.Writer.Write("'fHideByDefault' : ");
			this.Writer.Write(base.UserContext.UserOptions.HideMailTipsByDefault ? 1 : 0);
			RecipientCache.RunGetCacheOperationUnderDefaultExceptionHandler(delegate
			{
				CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(base.UserContext.ExchangePrincipal.MailboxInfo.OrganizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
				this.Writer.Write(",");
				this.Writer.Write("'fEnabled' : ");
				this.Writer.Write(instance.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled ? 1 : 0);
				this.Writer.Write(",");
				this.Writer.Write("'fMailboxEnabled' : ");
				this.Writer.Write(instance.OrganizationConfiguration.Configuration.MailTipsMailboxSourcedTipsEnabled ? 1 : 0);
				this.Writer.Write(",");
				this.Writer.Write("'fGroupMetricsEnabled' : ");
				this.Writer.Write(instance.OrganizationConfiguration.Configuration.MailTipsGroupMetricsEnabled ? 1 : 0);
				this.Writer.Write(",");
				this.Writer.Write("'fExternalEnabled' : ");
				this.Writer.Write(instance.OrganizationConfiguration.Configuration.MailTipsExternalRecipientsTipsEnabled ? 1 : 0);
				this.Writer.Write(",");
				this.Writer.Write("'iLargeAudienceThreshold' : ");
				this.Writer.Write(instance.OrganizationConfiguration.Configuration.MailTipsLargeAudienceThreshold);
			}, this.GetHashCode());
			this.Writer.Write("}");
			this.Writer.Write("}");
		}

		public const string EventNamespace = "ClientCache";

		public const string MethodGet = "Get";
	}
}
