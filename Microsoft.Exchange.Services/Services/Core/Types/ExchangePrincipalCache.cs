using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ExchangePrincipalCache
	{
		private static string BuildSmtpKey(string smtpAddress, bool isArchive)
		{
			return "_EPSMTP_" + (isArchive ? "_A_" : string.Empty) + smtpAddress.ToLowerInvariant();
		}

		private static string BuildMailboxGuidKey(Guid mailboxGuid)
		{
			return "_EPMBXGUID_" + mailboxGuid.ToString().ToLowerInvariant();
		}

		private static string BuildSidKey(SecurityIdentifier sid, bool isArchive)
		{
			return "_EPSID_" + (isArchive ? "_A_" : string.Empty) + sid.ToString();
		}

		private static void RequireUniquePrimarySmtpAddress(string primarySmtpAddress, SecurityIdentifier sid, ADRecipientSessionContext adRecipientSessionContext, string badPrincipalCacheKey)
		{
			if (!string.IsNullOrEmpty(primarySmtpAddress))
			{
				IRecipientSession adRecipientSession = adRecipientSessionContext.GetADRecipientSession();
				try
				{
					ADRecipient adRecipient = null;
					RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.RecipientLookupLatency, delegate()
					{
						Directory.TryFindRecipient(primarySmtpAddress, adRecipientSession, out adRecipient);
					});
				}
				catch (ADConfigurationException)
				{
					ExchangePrincipalCache.HandleBadPrincipal(badPrincipalCacheKey, sid, "ExchangePrincipalCache.RequireUniquePrimarySmtpAddress  Found an account with a duplicate primary smtp address:  User Sid: {0}", new CoreResources.IDs[0]);
					throw;
				}
			}
		}

		private static void AddToCache(ExchangePrincipal exchangePrincipal)
		{
			string primarySmtpAddress = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			ExchangePrincipalCache.AddToCache(exchangePrincipal, primarySmtpAddress);
		}

		private static void AddToCache(ExchangePrincipal exchangePrincipal, string primarySmtpAddress)
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			bool isArchive = exchangePrincipal.MailboxInfo != null && exchangePrincipal.MailboxInfo.IsArchive;
			string text = string.IsNullOrEmpty(primarySmtpAddress) ? null : ExchangePrincipalCache.BuildSmtpKey(primarySmtpAddress, isArchive);
			string text2 = (exchangePrincipal.MailboxInfo == null || exchangePrincipal.MailboxInfo.MailboxGuid == Guid.Empty) ? null : ExchangePrincipalCache.BuildMailboxGuidKey(exchangePrincipal.MailboxInfo.MailboxGuid);
			string key = ExchangePrincipalCache.BuildSidKey(exchangePrincipal.Sid, isArchive);
			bool flag = exchangePrincipal.MailboxInfo != null && exchangePrincipal.MailboxInfo.IsAggregated;
			bool flag2 = exchangePrincipal.MailboxInfo != null && exchangePrincipal.MailboxInfo.MailboxType == MailboxLocationType.AuxArchive;
			if (text != null && !flag && !flag2)
			{
				ExchangePrincipalCache.AddOrInsertToCache(text, exchangePrincipal);
			}
			if (text2 != null)
			{
				ExchangePrincipalCache.AddOrInsertToCache(text2, exchangePrincipal);
			}
			if (!flag && !flag2)
			{
				ExchangePrincipalCache.AddOrInsertToCache(key, exchangePrincipal);
			}
		}

		private static void AddOrInsertToCache(string key, ExchangePrincipal exchangePrincipal)
		{
			Cache cache = HttpRuntime.Cache;
			int num = (Global.ExchangePrincipalCacheTimeoutInMinutes <= 0) ? 5 : Global.ExchangePrincipalCacheTimeoutInMinutes;
			ExchangePrincipalWrapper value = new ExchangePrincipalWrapper(exchangePrincipal);
			if (cache[key] == null)
			{
				cache.Add(key, value, null, ExDateTime.Now.AddMinutes((double)num).UniversalTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
				return;
			}
			cache.Insert(key, value, null, ExDateTime.Now.AddMinutes((double)num).UniversalTime, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
		}

		internal static ExchangePrincipal GetFromCache(string smtpAddress, ADRecipientSessionContext adRecipientSessionContext)
		{
			return ExchangePrincipalCache.GetFromCache(smtpAddress, adRecipientSessionContext, false);
		}

		internal static ExchangePrincipal GetFromCache(string smtpAddress, ADRecipientSessionContext adRecipientSessionContext, bool isArchive)
		{
			if (string.IsNullOrEmpty(smtpAddress))
			{
				throw new ArgumentNullException("smtpAddress");
			}
			string key = ExchangePrincipalCache.BuildSmtpKey(smtpAddress, isArchive);
			string text = BadExchangePrincipalCache.BuildKey(key, adRecipientSessionContext.OrganizationPrefix);
			if (BadExchangePrincipalCache.Contains(text))
			{
				ExchangePrincipalCache.HandleBadPrincipal(text, smtpAddress, "ExchangePrincipalCache.GetADUser.  Smtp address was found in bad principal cache.  SMTP Address: {0}", new CoreResources.IDs[]
				{
					(CoreResources.IDs)4088802584U
				});
			}
			ADUser aduser = ExchangePrincipalCache.GetADUser(smtpAddress, adRecipientSessionContext, text);
			ExchangePrincipal exchangePrincipal = null;
			if (!ExchangePrincipalCache.TryGetFromCache(key, out exchangePrincipal))
			{
				if (ExchangePrincipalCache.TryCreateExchangePrincipal(aduser, Guid.Empty, out exchangePrincipal, false))
				{
					ExchangePrincipalCache.AddToCache(exchangePrincipal);
				}
				else
				{
					ExchangePrincipalCache.HandleBadPrincipal(text, smtpAddress, "ExchangePrincipalCache.GetFromCache.  Cannot create ExchangePrincipal for SMTP Address: {0}", new CoreResources.IDs[]
					{
						(CoreResources.IDs)4088802584U
					});
				}
			}
			return exchangePrincipal;
		}

		private static ADUser GetADUser(string smtpAddress, ADRecipientSessionContext adRecipientSessionContext, string badPrincipalCacheKey)
		{
			ADUser result = null;
			try
			{
				if (!ADIdentityInformationCache.Singleton.TryGetADUser(smtpAddress, adRecipientSessionContext, out result))
				{
					ExchangePrincipalCache.HandleBadPrincipal(badPrincipalCacheKey, smtpAddress, "ExchangePrincipalCache.GetADUser.  ExchangePrincipal was not found in AD.  SMTP Address: {0}", new CoreResources.IDs[]
					{
						(CoreResources.IDs)4088802584U
					});
				}
			}
			catch (InvalidSmtpAddressException arg)
			{
				ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<string, InvalidSmtpAddressException>(0L, "ExchangePrincipalCache.GetADUser.  Invalid SMTP Address: {0}  Exception: {1}", smtpAddress, arg);
				ExchangePrincipalCache.HandleBadPrincipal(badPrincipalCacheKey, smtpAddress, "ExchangePrincipalCache.GetADUser.  Invalid SMTP Address: {0}", new CoreResources.IDs[]
				{
					(CoreResources.IDs)4088802584U
				});
			}
			catch (ADConfigurationException arg2)
			{
				ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<ADConfigurationException>(0L, "ExchangePrincipalCache.GetADUser.  ExchangePrincipal may not have a non-unique address.  Exception {0}", arg2);
				ExchangePrincipalCache.HandleBadPrincipal(badPrincipalCacheKey, smtpAddress, "ExchangePrincipalCache.GetADUser.  Can't get ADUser for smtp address. {0}", new CoreResources.IDs[0]);
				throw;
			}
			return result;
		}

		public static ExchangePrincipal GetFromCacheByGuid(Guid mailboxGuid, ADRecipientSessionContext adRecipientSessionContext)
		{
			string key = ExchangePrincipalCache.BuildMailboxGuidKey(mailboxGuid);
			string text = BadExchangePrincipalCache.BuildKey(key, adRecipientSessionContext.OrganizationPrefix);
			ExchangePrincipal exchangePrincipal = null;
			if (BadExchangePrincipalCache.Contains(text) || mailboxGuid == Guid.Empty)
			{
				ExchangePrincipalCache.HandleBadPrincipal(text, mailboxGuid, "ExchangePrincipalCache.GetADUser.  MailboxGuid was found in the bad principal cache.  MailboxGuid: {0}", new CoreResources.IDs[]
				{
					(CoreResources.IDs)3279543955U
				});
			}
			ADUser aduser = ExchangePrincipalCache.GetADUser(mailboxGuid, adRecipientSessionContext, text);
			if (!ExchangePrincipalCache.TryGetFromCache(key, out exchangePrincipal))
			{
				if (ExchangePrincipalCache.TryCreateExchangePrincipal(aduser, mailboxGuid, out exchangePrincipal, false))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ExchangePrincipalCache", "Miss");
					ExchangePrincipalCache.AddToCache(exchangePrincipal);
				}
				else
				{
					ExchangePrincipalCache.HandleBadPrincipal(text, mailboxGuid, "ExchangePrincipalCache.GetFromCacheByGuid. Cannot create ExchangePrincipal for mailboxGuid: {0}.", new CoreResources.IDs[]
					{
						(CoreResources.IDs)3279543955U
					});
				}
			}
			return exchangePrincipal;
		}

		private static ADUser GetADUser(Guid mailboxGuid, ADRecipientSessionContext adRecipientSessionContext, string badPrincipalCacheKey)
		{
			ADUser result = null;
			if (!ADIdentityInformationCache.Singleton.TryGetADUser(mailboxGuid, adRecipientSessionContext, out result))
			{
				ADRecipientSessionContext adrecipientSessionContext = ADRecipientSessionContext.CreateForOrganization(adRecipientSessionContext.OrganizationId);
				adrecipientSessionContext.GetADRecipientSession().SessionSettings.IncludeInactiveMailbox = true;
				if (!ADIdentityInformationCache.Singleton.TryGetADUser(mailboxGuid, adrecipientSessionContext, out result))
				{
					ExchangePrincipalCache.HandleBadPrincipal(badPrincipalCacheKey, mailboxGuid, "ExchangePrincipalCache.GetADUser. ADUser not found for mailboxGuid: {0}.", new CoreResources.IDs[]
					{
						(CoreResources.IDs)3279543955U
					});
				}
			}
			return result;
		}

		private static void HandleBadPrincipal(string key, object invalidObject, string traceMessage, params CoreResources.IDs[] exceptionMessage)
		{
			ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug(0L, traceMessage, new object[]
			{
				invalidObject
			});
			BadExchangePrincipalCache.Add(key);
			if (exceptionMessage.Length > 0)
			{
				throw new NonExistentMailboxException(exceptionMessage[0], invalidObject.ToString());
			}
		}

		private static bool TryGetFromCache(string key, out ExchangePrincipal exchangePrincipal)
		{
			exchangePrincipal = null;
			Cache cache = HttpRuntime.Cache;
			ExchangePrincipalWrapper exchangePrincipalWrapper = (ExchangePrincipalWrapper)cache[key];
			exchangePrincipal = ((exchangePrincipalWrapper != null) ? exchangePrincipalWrapper.ExchangePrincipal : null);
			return exchangePrincipalWrapper != null;
		}

		public static ExchangePrincipal GetFromCache(MailboxId mailboxId, ADRecipientSessionContext adRecipientSessionContext)
		{
			if (mailboxId == null)
			{
				throw new ArgumentNullException("mailboxId");
			}
			if (string.IsNullOrEmpty(mailboxId.SmtpAddress) && string.IsNullOrEmpty(mailboxId.MailboxGuid))
			{
				throw new ArgumentException("MailboxId.smtpAddress and mailbox guid are both null or empty.", "mailboxId");
			}
			ExchangePrincipal result;
			if (!string.IsNullOrEmpty(mailboxId.MailboxGuid))
			{
				result = ExchangePrincipalCache.GetFromCacheByGuid(new Guid(mailboxId.MailboxGuid), adRecipientSessionContext);
			}
			else
			{
				result = ExchangePrincipalCache.GetFromCache(mailboxId.SmtpAddress, adRecipientSessionContext);
			}
			return result;
		}

		public static bool TryGetFromCache(SecurityIdentifier sid, ADRecipientSessionContext adRecipientSessionContext, out ExchangePrincipal exchangePrincipal)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			exchangePrincipal = null;
			string key = ExchangePrincipalCache.BuildSidKey(sid, false);
			string text = BadExchangePrincipalCache.BuildKey(key, adRecipientSessionContext.OrganizationPrefix);
			if (BadExchangePrincipalCache.Contains(text))
			{
				ExchangePrincipalCache.HandleBadPrincipal(text, sid, "ExchangePrincipalCache.GetADUser.  Sid was found in the bad principal cache.  UserSid: {0}", new CoreResources.IDs[0]);
				return false;
			}
			ADUser adUser = null;
			if (ExchangePrincipalCache.TryGetADUser(sid, adRecipientSessionContext, text, out adUser) && !ExchangePrincipalCache.TryGetFromCache(key, out exchangePrincipal))
			{
				if (ExchangePrincipalCache.TryCreateExchangePrincipal(adUser, Guid.Empty, out exchangePrincipal, false))
				{
					string primarySmtpAddress;
					if (exchangePrincipal is RemoteUserMailboxPrincipal)
					{
						primarySmtpAddress = ((RemoteUserMailboxPrincipal)exchangePrincipal).PrimarySmtpAddress.ToString();
					}
					else
					{
						primarySmtpAddress = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
					}
					ExchangePrincipalCache.RequireUniquePrimarySmtpAddress(primarySmtpAddress, sid, adRecipientSessionContext, text);
					ExchangePrincipalCache.AddToCache(exchangePrincipal, primarySmtpAddress);
				}
				else
				{
					ExchangePrincipalCache.HandleBadPrincipal(text, sid, "ExchangePrincipalCache.GetFromCache.  Cannot create ExchangePrincipal for sid: {0}", new CoreResources.IDs[0]);
				}
			}
			return exchangePrincipal != null;
		}

		private static bool TryGetADUser(SecurityIdentifier sid, ADRecipientSessionContext adRecipientSessionContext, string badPrincipalCacheKey, out ADUser adUser)
		{
			adUser = null;
			if (!ADIdentityInformationCache.Singleton.TryGetADUser(sid, adRecipientSessionContext, out adUser))
			{
				ExchangePrincipalCache.HandleBadPrincipal(badPrincipalCacheKey, sid, "ExchangePrincipalCache.GetADUser.  ExchangePrincipal was not found in AD.  UserSid: {0}", new CoreResources.IDs[0]);
			}
			return adUser != null;
		}

		private static bool TryCreateExchangePrincipal(ADUser adUser, Guid mailboxGuid, out ExchangePrincipal exchangePrincipal, bool includeInactiveMailboxes = false)
		{
			ExchangePrincipal localExchangePrincipal = null;
			RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.ExchangePrincipalLatency, delegate()
			{
				try
				{
					bool flag = mailboxGuid != Guid.Empty && adUser.ArchiveGuid == mailboxGuid;
					ADSessionSettings adsessionSettings = adUser.OrganizationId.ToADSessionSettings();
					if (includeInactiveMailboxes)
					{
						adsessionSettings.IncludeInactiveMailbox = true;
					}
					if (mailboxGuid == Guid.Empty || flag)
					{
						localExchangePrincipal = ExchangePrincipal.FromADUser(adsessionSettings, adUser, RemotingOptions.AllowCrossSite);
					}
					else
					{
						localExchangePrincipal = ExchangePrincipal.FromMailboxGuid(adsessionSettings, mailboxGuid, RemotingOptions.AllowCrossSite, null);
					}
					if (localExchangePrincipal.MailboxInfo != null && !localExchangePrincipal.MailboxInfo.IsArchive && flag)
					{
						localExchangePrincipal = localExchangePrincipal.GetArchiveExchangePrincipal(RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise);
					}
				}
				catch (ObjectNotFoundException arg)
				{
					ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<ObjectNotFoundException, ADUser>(0L, "ExchangePrincipalCache.TryCreateExchangePrincipal.  ExchangePrincipal creation failed with exception: {0} user: {1}", arg, adUser);
					if (!includeInactiveMailboxes)
					{
						ExchangePrincipalCache.TryCreateExchangePrincipal(adUser, mailboxGuid, out localExchangePrincipal, true);
					}
				}
			});
			exchangePrincipal = localExchangePrincipal;
			return exchangePrincipal != null;
		}

		internal static bool TryGetExchangePrincipalForHybridPublicFolderAccess(SecurityIdentifier sid, ADRecipientSessionContext adRecipientSessionContext, out ExchangePrincipal exchangePrincipal, bool includeInactiveMailboxes = false)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			exchangePrincipal = null;
			string key = ExchangePrincipalCache.BuildSidKey(sid, false);
			string text = BadExchangePrincipalCache.BuildKey(key, adRecipientSessionContext.OrganizationPrefix);
			if (BadExchangePrincipalCache.Contains(text))
			{
				ExchangePrincipalCache.HandleBadPrincipal(text, sid, "ExchangePrincipalCache.GetADUser.  Sid was found in the bad principal cache.  UserSid: {0}", new CoreResources.IDs[0]);
				return false;
			}
			ADUser adUser = null;
			bool flag = ExchangePrincipalCache.TryGetADUser(sid, adRecipientSessionContext, text, out adUser);
			if (flag && adUser.RecipientType == RecipientType.MailUser && adUser.RecipientTypeDetails == (RecipientTypeDetails)((ulong)-2147483648))
			{
				ExchangePrincipal localExchangePrincipal = null;
				RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.ExchangePrincipalLatency, delegate()
				{
					try
					{
						ADSessionSettings adsessionSettings = adUser.OrganizationId.ToADSessionSettings();
						if (includeInactiveMailboxes)
						{
							adsessionSettings.IncludeInactiveMailbox = true;
						}
						localExchangePrincipal = ExchangePrincipal.FromADUser(adsessionSettings, adUser, RemotingOptions.AllowHybridAccess);
					}
					catch (ObjectNotFoundException arg)
					{
						ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<ObjectNotFoundException, ADUser>(0L, "ExchangePrincipalCache.TryCreateExchangePrincipal.  ExchangePrincipal creation failed with exception: {0} user: {1}", arg, adUser);
						if (!includeInactiveMailboxes)
						{
							ExchangePrincipalCache.TryGetExchangePrincipalForHybridPublicFolderAccess(sid, adRecipientSessionContext, out localExchangePrincipal, true);
						}
					}
				});
				exchangePrincipal = localExchangePrincipal;
			}
			return exchangePrincipal != null;
		}

		public static ADSessionSettings GetAdSessionSettingsForOrg(OrganizationId orgId)
		{
			ADSessionSettings localSettings = null;
			RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.EPCacheGetAdSessionSettingsForOrg, delegate()
			{
				localSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ExchangePrincipalCache.servicesRootOrgId.Value, orgId, null, false);
			});
			return localSettings;
		}

		public static void Remove(IExchangePrincipal exchangePrincipal)
		{
			ExchangePrincipalCache.TryRemoveInternal(exchangePrincipal, true);
		}

		public static bool TryRemoveStale(IExchangePrincipal exchangePrincipal)
		{
			return ExchangePrincipalCache.TryRemoveInternal(exchangePrincipal, false);
		}

		private static bool TryRemoveInternal(IExchangePrincipal exchangePrincipal, bool forceRemove)
		{
			if (exchangePrincipal == null)
			{
				throw new ArgumentNullException("exchangePrincipal");
			}
			string text = exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
			string text2 = string.IsNullOrEmpty(text) ? null : ExchangePrincipalCache.BuildSmtpKey(text, exchangePrincipal.MailboxInfo.IsArchive);
			string text3 = (exchangePrincipal.MailboxInfo.MailboxGuid == Guid.Empty) ? null : ExchangePrincipalCache.BuildMailboxGuidKey(exchangePrincipal.MailboxInfo.MailboxGuid);
			string text4 = ExchangePrincipalCache.BuildSidKey(exchangePrincipal.Sid, exchangePrincipal.MailboxInfo.IsArchive);
			bool result = false;
			if (text2 != null)
			{
				ExchangePrincipalWrapper exchangePrincipalWrapper = (ExchangePrincipalWrapper)HttpRuntime.Cache[text2];
				if (exchangePrincipalWrapper != null && (forceRemove || DateTime.UtcNow.Subtract(exchangePrincipalWrapper.CreatedOn).TotalSeconds > (double)Global.ExchangePrincipalCacheTimeoutInSecondsOnError))
				{
					object obj = HttpRuntime.Cache.Remove(text2);
					ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<string, bool>(0L, "ExchangePrincipalCache.Remove.  exchangePrincipal removed with smtp key {0}: {1}", text2, obj != null);
					result = true;
				}
			}
			if (text3 != null)
			{
				ExchangePrincipalWrapper exchangePrincipalWrapper = (ExchangePrincipalWrapper)HttpRuntime.Cache[text3];
				if (exchangePrincipalWrapper != null && (forceRemove || DateTime.UtcNow.Subtract(exchangePrincipalWrapper.CreatedOn).TotalSeconds > (double)Global.ExchangePrincipalCacheTimeoutInSecondsOnError))
				{
					object obj = HttpRuntime.Cache.Remove(text3);
					ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<string, bool>(0L, "ExchangePrincipalCache.Remove.  exchangePrincipal removed with mailboxGuid key {0}: {1}", text3, obj != null);
					result = true;
				}
			}
			if (text4 != null)
			{
				ExchangePrincipalWrapper exchangePrincipalWrapper = (ExchangePrincipalWrapper)HttpRuntime.Cache[text4];
				if (exchangePrincipalWrapper != null && (forceRemove || DateTime.UtcNow.Subtract(exchangePrincipalWrapper.CreatedOn).TotalSeconds > (double)Global.ExchangePrincipalCacheTimeoutInSecondsOnError))
				{
					object obj = HttpRuntime.Cache.Remove(text4);
					ExTraceGlobals.ExchangePrincipalCacheTracer.TraceDebug<string, bool>(0L, "ExchangePrincipalCache.Remove.  exchangePrincipal removed with sid key {0}: {1}", text4, obj != null);
					result = true;
				}
			}
			return result;
		}

		private const string SidKeyPrefix = "_EPSID_";

		private const string SmtpKeyPrefix = "_EPSMTP_";

		private const string MailboxGuidKeyPrefix = "_EPMBXGUID_";

		private const string ArchiveKeyPrefix = "_A_";

		private static Lazy<ADObjectId> servicesRootOrgId = new Lazy<ADObjectId>(() => ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), LazyThreadSafetyMode.PublicationOnly);
	}
}
