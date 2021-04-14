using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class StoreSessionCacheBase : IDisposable
	{
		protected SessionAndAuthZ CreateMailboxSessionBasedOnAccessType(ExchangePrincipal mailboxToAccess, CallContext callContext, bool unifiedLogon)
		{
			SessionAndAuthZ sessionAndAuthZ = null;
			try
			{
				sessionAndAuthZ = this.ConnectOnce(mailboxToAccess, callContext, unifiedLogon);
			}
			catch (WrongServerException ex)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "Exception in CreateMailboxSessionBasedOnAccessType. The given EP might be stale. Mailbox: {0}, AccessType: {1}, AccessingAs SID: {2}, Exception: {3}", new object[]
				{
					mailboxToAccess.MailboxInfo.PrimarySmtpAddress,
					callContext.MailboxAccessType,
					callContext.EffectiveCallerSid,
					ex
				});
				this.HandleStaleExchangePrincipal(mailboxToAccess, callContext);
				throw;
			}
			catch (ConnectionFailedPermanentException ex2)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "Exception in CreateMailboxSessionBasedOnAccessType. Mailbox: {0}, AccessType: {1}, AccessingAs SID: {2}, Exception: {3}", new object[]
				{
					mailboxToAccess.MailboxInfo.PrimarySmtpAddress,
					callContext.MailboxAccessType,
					callContext.EffectiveCallerSid,
					ex2
				});
				sessionAndAuthZ = this.ConnectOnce(mailboxToAccess, callContext);
				if (sessionAndAuthZ == null)
				{
					throw;
				}
			}
			catch (ConnectionFailedTransientException ex3)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "Exception in CreateMailboxSessionBasedOnAccessType. Mailbox: {0}, AccessType: {1}, AccessingAs SID: {2}, Exception: {3}", new object[]
				{
					mailboxToAccess.MailboxInfo.PrimarySmtpAddress,
					callContext.MailboxAccessType,
					callContext.EffectiveCallerSid,
					ex3
				});
				sessionAndAuthZ = this.ConnectOnce(mailboxToAccess, callContext);
				if (sessionAndAuthZ == null)
				{
					throw;
				}
			}
			catch (Exception ex4)
			{
				if (ex4 is MailboxInSiteFailoverException || ex4 is MailboxCrossSiteFailoverException)
				{
					ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "Exception in CreateMailboxSessionBasedOnAccessType. ExchangePrincipal cache might be stale. Mailbox: {0}, AccessType: {1}, AccessingAs SID: {2}, Exception: {3}", new object[]
					{
						mailboxToAccess.MailboxInfo.PrimarySmtpAddress,
						callContext.MailboxAccessType,
						callContext.EffectiveCallerSid,
						ex4
					});
					if (Global.SendXBEServerExceptionHeaderToCafeOnFailover)
					{
						ExchangePrincipalCache.Remove(mailboxToAccess);
						this.SendExceptionHeaderIfNeeded(callContext, mailboxToAccess.MailboxInfo.MailboxGuid);
					}
				}
				throw;
			}
			return sessionAndAuthZ;
		}

		protected void SendExceptionHeaderIfNeeded(CallContext callContext, Guid mbxGuid)
		{
			if (callContext.AccessingPrincipal != null && callContext.MailboxAccessType == MailboxAccessType.Normal && callContext.AccessingPrincipal.MailboxInfo.MailboxGuid.Equals(mbxGuid))
			{
				callContext.HttpContext.Response.Headers["X-BEServerException"] = typeof(IllegalCrossServerConnectionException).FullName;
			}
		}

		protected internal SessionAndAuthZ ConnectOnce(ExchangePrincipal mailboxToAccess, CallContext callContext)
		{
			return this.ConnectOnce(mailboxToAccess, callContext, false);
		}

		protected internal SessionAndAuthZ ConnectOnce(ExchangePrincipal mailboxToAccess, CallContext callContext, bool unifiedLogon)
		{
			if (!ExchangeVersionDeterminer.MatchesLocalServerVersion(mailboxToAccess.MailboxInfo.Location.ServerVersion) && !ExchangeVersionDeterminer.AllowCrossVersionAccess(mailboxToAccess.MailboxInfo.Location.ServerVersion))
			{
				throw new WrongServerVersionException(mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			if (!callContext.AuthZBehavior.IsAllowedToOpenMailbox(mailboxToAccess))
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<SecurityIdentifier, SecurityIdentifier, LogonType>(0L, "[MailboxSessionCacheBase::ConnectOnce] user (sid {0}) does not permission to open mailbox (sid {1}) with logon type {2}", callContext.AccessingPrincipal.Sid, mailboxToAccess.Sid, callContext.LogonType);
				throw new ServiceAccessDeniedException((CoreResources.IDs)2771555298U);
			}
			SessionAndAuthZ result;
			try
			{
				StoreSession storeSession = null;
				if (callContext.RequirePrivilegedLogon)
				{
					switch (callContext.LogonTypeSource)
					{
					case LogonTypeSource.OpenAsAdminOrSystemServiceHeader:
						if (mailboxToAccess.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
						{
							if (!callContext.AuthZBehavior.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess))
							{
								ExTraceGlobals.SessionCacheTracer.TraceDebug<SecurityIdentifier, SecurityIdentifier, LogonType>(0L, "[MailboxSessionCacheBase::ConnectOnce] user (sid {0}) does not permission to open mailbox (sid {1}) with logon type {2}", callContext.AccessingPrincipal.Sid, mailboxToAccess.Sid, callContext.LogonType);
								throw new ServiceAccessDeniedException((CoreResources.IDs)2435663882U);
							}
						}
						else if (!mailboxToAccess.Sid.Equals(callContext.AccessingPrincipal.Sid))
						{
							ExTraceGlobals.SessionCacheTracer.TraceDebug<SecurityIdentifier, LogonType, SecurityIdentifier>(0L, "[MailboxSessionCacheBase::ConnectOnce] The request is to open the mailbox (sid {0}) with {1} logon type, however the mailbox it is now trying to open is for sid {2}", callContext.AccessingPrincipal.Sid, callContext.LogonType, mailboxToAccess.Sid);
							throw new AccessDeniedException(LocalizedString.Empty);
						}
						break;
					case LogonTypeSource.ManagementRoleHeader:
						if (!callContext.AuthZBehavior.IsAllowedToPrivilegedOpenMailbox(mailboxToAccess))
						{
							ExTraceGlobals.SessionCacheTracer.TraceDebug<SecurityIdentifier, SecurityIdentifier, LogonType>(0L, "[MailboxSessionCacheBase::ConnectOnce] user (sid {0}) does not permission to open mailbox (sid {1}) with logon type {2}", callContext.AccessingPrincipal.Sid, mailboxToAccess.Sid, callContext.LogonType);
							throw new ServiceAccessDeniedException((CoreResources.IDs)2435663882U);
						}
						break;
					}
					if (mailboxToAccess.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
					{
						storeSession = PublicFolderSession.OpenAsAdmin(mailboxToAccess.MailboxInfo.OrganizationId, null, mailboxToAccess.MailboxInfo.MailboxGuid, null, callContext.ClientCulture, "Client=WebServices", null);
					}
					else if (callContext.LogonType == LogonType.SystemService)
					{
						storeSession = MailboxSession.OpenAsSystemService(mailboxToAccess, callContext.ClientCulture, StoreSessionCacheBase.BuildMapiApplicationId(callContext, null));
					}
					else
					{
						storeSession = MailboxSession.OpenAsAdmin(mailboxToAccess, callContext.ClientCulture, StoreSessionCacheBase.BuildMapiApplicationId(callContext, null));
					}
				}
				else
				{
					RecipientIdentity recipientIdentity = null;
					if (!ADIdentityInformationCache.Singleton.TryGetRecipientIdentity(callContext.EffectiveCallerSid, callContext.ADRecipientSessionContext, out recipientIdentity))
					{
						ExTraceGlobals.SessionCacheTracer.TraceDebug<string, SecurityIdentifier>(0L, "[MailboxSessionCacheBase::ConnectOnce] In {0} access mode, we found that the caller needed delegate access to a mailbox, but the caller is actually a computer account and not a recipient and therefore cannot possibly have delegate rights to your mailbox.  Caller sid: {1}", (callContext.MailboxAccessType == MailboxAccessType.Normal) ? "normal" : "S2S", callContext.EffectiveCallerSid);
						throw new AccessDeniedException(LocalizedString.Empty);
					}
					IADOrgPerson iadorgPerson = recipientIdentity.Recipient as IADOrgPerson;
					if (iadorgPerson == null)
					{
						ExTraceGlobals.SessionCacheTracer.TraceDebug<string, SecurityIdentifier>(0L, "[MailboxSessionCacheBase::ConnectOnce] In {0} access mode, we found that the caller needed delegate access to a mailbox, but searching for the caller by sid returned no records.  Possibly a cross forest trust with no cross-forest contact to speak of.  Caller sid: {1}", (callContext.MailboxAccessType == MailboxAccessType.Normal) ? "normal" : "S2S", callContext.EffectiveCallerSid);
						throw new ServiceAccessDeniedException();
					}
					callContext.EffectiveCaller.MungeTokenForLinkedAccounts(callContext);
					if (mailboxToAccess.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox)
					{
						string clientInfoString = StoreSessionCacheBase.BuildMapiApplicationId(callContext, null);
						if (callContext.AccessingPrincipal is RemoteUserMailboxPrincipal)
						{
							ADUser accessingADUser = callContext.AccessingADUser;
							storeSession = PublicFolderSession.Open(accessingADUser.LegacyExchangeDN, new Participant(accessingADUser), accessingADUser.ExchangeGuid, accessingADUser.DefaultPublicFolderMailbox, mailboxToAccess, callContext.EffectiveCaller.ClientSecurityContext, callContext.ClientCulture, clientInfoString);
						}
						else
						{
							storeSession = PublicFolderSession.Open(callContext.AccessingPrincipal, mailboxToAccess, callContext.EffectiveCaller.ClientSecurityContext, callContext.ClientCulture, clientInfoString);
						}
					}
					else
					{
						if (mailboxToAccess.RecipientTypeDetails == RecipientTypeDetails.TeamMailbox)
						{
							this.UpdateAccessTokenIfNeeded(mailboxToAccess, callContext, iadorgPerson);
						}
						MailboxSession mailboxSessionToClone = this.GetMailboxSessionToClone(callContext, mailboxToAccess.MailboxInfo.MailboxGuid, callContext.ClientCulture, callContext.LogonType, unifiedLogon);
						if (mailboxSessionToClone != null)
						{
							storeSession = mailboxSessionToClone.CloneWithBestAccess(mailboxToAccess, iadorgPerson, callContext.EffectiveCaller.ClientSecurityContext, StoreSessionCacheBase.BuildMapiApplicationId(callContext, null), StoreSessionCacheBase.BuildAuxiliaryIdentity(callContext), unifiedLogon);
						}
						else
						{
							if (Global.EwsClientMailboxSessionCloningHandler != null)
							{
								storeSession = Global.EwsClientMailboxSessionCloningHandler(mailboxToAccess.MailboxInfo.MailboxGuid, callContext.ClientCulture, callContext.LogonType, callContext.OwaUserContextKey, mailboxToAccess, iadorgPerson, callContext.EffectiveCaller.ClientSecurityContext, StoreSessionCacheBase.BuildAuxiliaryIdentity(callContext), unifiedLogon);
							}
							if (storeSession == null)
							{
								storeSession = MailboxSession.OpenWithBestAccess(mailboxToAccess, iadorgPerson, callContext.EffectiveCaller.ClientSecurityContext, callContext.ClientCulture, StoreSessionCacheBase.BuildMapiApplicationId(callContext, null), StoreSessionCacheBase.BuildAuxiliaryIdentity(callContext), unifiedLogon);
							}
						}
					}
				}
				if (!storeSession.IsPublicFolderSession)
				{
					((MailboxSession)storeSession).DisablePrivateItemsFilter();
				}
				EWSSettings.UpdateSessionTimeZoneFromRequestSoapHeader(storeSession);
				SessionAndAuthZ sessionAndAuthZ = new SessionAndAuthZ(storeSession, callContext.EffectiveCaller, callContext.GetSessionCulture(storeSession));
				result = sessionAndAuthZ;
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new InvalidSerializedAccessTokenException(innerException);
			}
			catch (ArgumentException innerException2)
			{
				throw new InvalidSerializedAccessTokenException(innerException2);
			}
			catch (InvalidOperationException innerException3)
			{
				throw new ServiceAccessDeniedException(innerException3);
			}
			return result;
		}

		internal static string BuildMapiApplicationId(CallContext callContext, string action)
		{
			string text = string.IsNullOrEmpty(callContext.UserAgent) ? "UserAgent=[NoUserAgent]" : callContext.UserAgent;
			if (!string.IsNullOrEmpty(callContext.CallerApplicationId))
			{
				text += string.Format("[AppId={0}]", callContext.CallerApplicationId);
			}
			return string.Format("{0};{1};{2}", Global.DefaultMapiClientType, text, action);
		}

		protected internal SessionAndAuthZ ConnectOnceAsExternalPrincipal(ExchangePrincipal mailboxToAccess, ExternalCallContext callContext)
		{
			if (!ExchangeVersionDeterminer.MatchesLocalServerVersion(mailboxToAccess.MailboxInfo.Location.ServerVersion) && !ExchangeVersionDeterminer.AllowCrossVersionAccess(mailboxToAccess.MailboxInfo.Location.ServerVersion))
			{
				throw new WrongServerVersionException(mailboxToAccess.MailboxInfo.PrimarySmtpAddress.ToString());
			}
			SessionAndAuthZ result;
			try
			{
				MailboxSession session = MailboxSession.OpenAsSystemService(mailboxToAccess, callContext.ClientCulture, StoreSessionCacheBase.BuildMapiApplicationId(callContext, null));
				EWSSettings.UpdateSessionTimeZoneFromRequestSoapHeader(session);
				ExternalIdentityToken externalIdentityToken = ExternalIdentityToken.GetExternalIdentityToken(session, callContext.ExternalId);
				if (externalIdentityToken == null)
				{
					throw new ServiceAccessDeniedException();
				}
				AuthZClientInfo authZClientInfo = AuthZClientInfo.FromExternalIdentityToken(externalIdentityToken);
				SessionAndAuthZ sessionAndAuthZ = new SessionAndAuthZ(session, authZClientInfo, callContext.GetSessionCulture(session));
				result = sessionAndAuthZ;
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new ServiceAccessDeniedException(innerException);
			}
			return result;
		}

		protected void SafeConnect(StoreSession storeSession)
		{
			try
			{
				if (!storeSession.IsConnected)
				{
					if (!storeSession.IsPublicFolderSession)
					{
						((MailboxSession)storeSession).DisablePrivateItemsFilter();
					}
					EWSSettings.UpdateSessionTimeZoneFromRequestSoapHeader(storeSession);
					storeSession.Connect();
				}
			}
			catch (ConnectionFailedPermanentException)
			{
				if (!storeSession.IsConnected)
				{
					storeSession.Connect();
				}
			}
			catch (ConnectionFailedTransientException)
			{
				if (!storeSession.IsConnected)
				{
					storeSession.Connect();
				}
			}
		}

		protected void HandleStaleExchangePrincipal(IExchangePrincipal exchangePrincipal, CallContext callContext)
		{
			ExTraceGlobals.SessionCacheTracer.TraceDebug<IExchangePrincipal>(0L, "[MailboxSessionCacheBase::HandleStaleExchangePrincipal] ExchangePrincipal {0} is deemed as stale, going to remove related cache entry", exchangePrincipal);
			ADIdentityInformationCache.Singleton.RemoveUserIdentity(exchangePrincipal.Sid, callContext.ADRecipientSessionContext.OrganizationPrefix);
			ExchangePrincipalCache.Remove(exchangePrincipal);
		}

		protected virtual MailboxSession GetMailboxSessionToClone(CallContext callContext, Guid mailboxOwner, CultureInfo cultureInfo, LogonType logonType, bool unifiedLogon)
		{
			return null;
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected abstract void Dispose(bool isDisposing);

		private static GenericIdentity BuildAuxiliaryIdentity(CallContext callContext)
		{
			if (callContext.MailboxAccessType == MailboxAccessType.ExchangeImpersonation && callContext.OriginalCallerContext.Sid != null && !callContext.OriginalCallerContext.Sid.Equals(callContext.EffectiveCallerSid))
			{
				return new GenericSidIdentity(string.Empty, string.Empty, callContext.OriginalCallerContext.Sid);
			}
			return null;
		}

		private void UpdateAccessTokenIfNeeded(ExchangePrincipal mailboxToAccess, CallContext callContext, IADOrgPerson userPerson)
		{
			Exception ex = null;
			SharepointAccessManager.Instance.UpdateAccessTokenIfNeeded(mailboxToAccess, OauthUtils.GetOauthCredential(userPerson as ADUser), callContext.EffectiveCaller.ClientSecurityContext, out ex, false);
		}

		protected bool isDisposed;
	}
}
