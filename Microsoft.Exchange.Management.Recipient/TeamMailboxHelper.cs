using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.LinkedFolder;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.SharePoint.Client;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class TeamMailboxHelper
	{
		internal TeamMailboxHelper(TeamMailbox teamMailbox, ADRawEntry actAsUser, OrganizationId actAsUserOrgId, IRecipientSession adSession, TeamMailboxGetDataObject<ADUser> getAdUser)
		{
			if (teamMailbox == null)
			{
				throw new ArgumentNullException("teamMailbox");
			}
			if (actAsUser == null)
			{
				throw new ArgumentNullException("actAsUser");
			}
			if (actAsUserOrgId == null)
			{
				throw new ArgumentNullException("actAsUserOrgId");
			}
			if (adSession == null)
			{
				throw new ArgumentNullException("adSession");
			}
			if (getAdUser == null)
			{
				throw new ArgumentNullException("getAdUser");
			}
			this.teamMailbox = teamMailbox;
			this.actAsUser = actAsUser;
			this.actAsUserOrgId = actAsUserOrgId;
			this.adSession = adSession;
			this.getAdUser = getAdUser;
		}

		internal static string GetAggreatedIds(IList<RecipientIdParameter> ids)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (ids != null)
			{
				foreach (RecipientIdParameter recipientIdParameter in ids)
				{
					stringBuilder.Append(string.Format("{0}; ", recipientIdParameter.ToString()));
				}
			}
			return stringBuilder.ToString();
		}

		internal static string GetAggreatedUsers(IList<ADUser> users)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (users != null)
			{
				foreach (ADUser aduser in users)
				{
					stringBuilder.Append(string.Format("{0} ({1}); ", aduser.DisplayName, aduser.PrimarySmtpAddress));
				}
			}
			return stringBuilder.ToString();
		}

		internal List<ADObjectId> RecipientIds2ADObjectIds(RecipientIdParameter[] ids, out IList<RecipientIdParameter> unresolvedIds, out IList<ADUser> unqualifiedUsers)
		{
			if (ids == null)
			{
				throw new ArgumentNullException("ids");
			}
			List<ADObjectId> list = new List<ADObjectId>();
			unresolvedIds = new List<RecipientIdParameter>();
			unqualifiedUsers = new List<ADUser>();
			foreach (RecipientIdParameter recipientIdParameter in ids)
			{
				Exception ex;
				ADUser aduser = this.ResolveUser(recipientIdParameter, out ex);
				if (aduser == null)
				{
					unresolvedIds.Add(recipientIdParameter);
				}
				else if (!TeamMailboxMembershipHelper.IsUserQualifiedType(aduser))
				{
					unqualifiedUsers.Add(aduser);
				}
				else
				{
					list.Add(aduser.ObjectId);
				}
			}
			return list;
		}

		internal ADUser ResolveUser(ADObjectId id, out Exception ex)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			return TeamMailboxADUserResolver.Resolve(this.adSession, id, out ex);
		}

		private ADUser ResolveUser(RecipientIdParameter id, out Exception ex)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			ADUser result = null;
			ex = null;
			try
			{
				result = (ADUser)this.getAdUser(id, this.adSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(id.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(id.ToString())), ExchangeErrorCategory.Client);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADExternalException ex3)
			{
				ex = ex3;
			}
			catch (ADOperationException ex4)
			{
				ex = ex4;
			}
			catch (InvalidCastException ex5)
			{
				ex = ex5;
			}
			return result;
		}

		internal static MultiValuedProperty<CultureInfo> CheckSharePointSite(SmtpAddress teamMailboxEmailAddress, ref Uri sharePointUrl, ADRawEntry actAsUser, OrganizationId actAsUserOrgId, ADUser executingUser, out SharePointMemberShip executingUserMembership, out Uri webCollectionUrl, out Guid webId)
		{
			MultiValuedProperty<CultureInfo> multiValuedProperty = new MultiValuedProperty<CultureInfo>();
			executingUserMembership = SharePointMemberShip.Others;
			try
			{
				using (ClientContext clientContext = new ClientContext(sharePointUrl.AbsoluteUri))
				{
					clientContext.RequestTimeout = 60000;
					bool flag;
					TeamMailboxHelper.GetCredentialAndConfigureClientContext(actAsUser, actAsUserOrgId, clientContext, true, out flag);
					Web web = clientContext.Web;
					clientContext.Load<Web>(web, new Expression<Func<Web, object>>[]
					{
						(Web x) => x.AllProperties,
						(Web x) => x.Url,
						(Web x) => (object)x.Language,
						(Web x) => x.Features,
						(Web x) => (object)x.Id
					});
					Feature byId = web.Features.GetById(new Guid("{502A2D54-6102-4757-AAA0-A90586106368}"));
					clientContext.Load<Feature>(byId, new Expression<Func<Feature, object>>[0]);
					Group associatedOwnerGroup = clientContext.Web.AssociatedOwnerGroup;
					Group associatedMemberGroup = clientContext.Web.AssociatedMemberGroup;
					clientContext.Load<UserCollection>(associatedOwnerGroup.Users, new Expression<Func<UserCollection, object>>[0]);
					clientContext.Load<UserCollection>(associatedMemberGroup.Users, new Expression<Func<UserCollection, object>>[0]);
					Site site = clientContext.Site;
					clientContext.Load<Site>(site, new Expression<Func<Site, object>>[]
					{
						(Site x) => x.Url
					});
					bool flag2 = false;
					try
					{
						clientContext.ExecuteQuery();
						if (byId.ServerObjectIsNull != null)
						{
							flag2 = !byId.ServerObjectIsNull.Value;
						}
					}
					catch (UnauthorizedAccessException)
					{
					}
					if (!flag2)
					{
						throw new RecipientTaskException(Strings.ErrorTeamMailboxFeatureNotInstalled(web.Url));
					}
					if (clientContext.ServerVersion.Major < 15)
					{
						throw new RecipientTaskException(Strings.ErrorTeamMailboxSharePointServerVersionInCompatible);
					}
					if (web.AllProperties.FieldValues.ContainsKey("ExchangeTeamMailboxEmailAddress") && (teamMailboxEmailAddress == SmtpAddress.Empty || !string.Equals(teamMailboxEmailAddress.ToString(), (string)web.AllProperties["ExchangeTeamMailboxEmailAddress"], StringComparison.OrdinalIgnoreCase)))
					{
						throw new RecipientTaskException(Strings.ErrorTeamMailboxSharePointSiteAlreadyLinkedWithOtherTM(sharePointUrl.ToString(), (string)web.AllProperties["ExchangeTeamMailboxEmailAddress"]));
					}
					webCollectionUrl = TeamMailbox.GetUrl(site.Url);
					webId = web.Id;
					if (webCollectionUrl == null)
					{
						throw new RecipientTaskException(Strings.ErrorSharePointSiteHasNoValidWebCollectionUrl(site.Url));
					}
					multiValuedProperty.Add(new CultureInfo((int)web.Language));
					try
					{
						foreach (User spUser in associatedOwnerGroup.Users)
						{
							if (TeamMailboxHelper.ExchangeSharePointUserMatch(executingUser, spUser))
							{
								executingUserMembership = SharePointMemberShip.Owner;
								break;
							}
						}
						if (executingUserMembership == SharePointMemberShip.Others)
						{
							foreach (User spUser2 in associatedMemberGroup.Users)
							{
								if (TeamMailboxHelper.ExchangeSharePointUserMatch(executingUser, spUser2))
								{
									executingUserMembership = SharePointMemberShip.Member;
									break;
								}
							}
						}
					}
					catch (CollectionNotInitializedException)
					{
						executingUserMembership = SharePointMemberShip.Others;
					}
					string uriString = null;
					if (!MaintenanceSynchronizer.TryEscapeAndGetWellFormedUrl(web.Url, out uriString))
					{
						throw new RecipientTaskException(Strings.ErrorSharePointSiteHasNoValidUrl(web.Url));
					}
					Uri uri = new Uri(uriString);
					if (uri != sharePointUrl)
					{
						sharePointUrl = uri;
					}
				}
			}
			catch (ClientRequestException ex)
			{
				throw new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(sharePointUrl.AbsoluteUri, ex.Message));
			}
			catch (ServerException ex2)
			{
				throw new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSiteWithCorrelationId(sharePointUrl.AbsoluteUri, ex2.Message, ex2.ServerErrorTraceCorrelationId));
			}
			catch (TimeoutException ex3)
			{
				throw new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(sharePointUrl.AbsoluteUri, ex3.Message));
			}
			catch (IOException ex4)
			{
				throw new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(sharePointUrl.AbsoluteUri, ex4.Message));
			}
			catch (WebException e)
			{
				SharePointException ex5 = new SharePointException(sharePointUrl.AbsoluteUri, e, false);
				throw new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(sharePointUrl.AbsoluteUri, ex5.DiagnosticInfo));
			}
			catch (FormatException ex6)
			{
				throw new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(sharePointUrl.AbsoluteUri, ex6.Message));
			}
			return multiValuedProperty;
		}

		internal static ICredentials GetCredentialAndConfigureClientContext(ADRawEntry actAsUser, OrganizationId actAsUserOrgId, ClientContext context, bool createAppTokenOnly, out bool isOAuthCredential)
		{
			if (!createAppTokenOnly && actAsUser == null)
			{
				throw new ArgumentNullException("actAsUser");
			}
			if (actAsUserOrgId == null)
			{
				throw new ArgumentNullException("actAsUserOrgId");
			}
			int num = 0;
			isOAuthCredential = true;
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchangeRPC\\ParametersSystem", RegistryKeyPermissionCheck.ReadSubTree);
				if (registryKey != null)
				{
					num = (int)registryKey.GetValue("TMPublishHttpDebugEnabled", 0);
					if ((int)registryKey.GetValue("TMOAuthEnabled", 1) == 0)
					{
						isOAuthCredential = false;
						return null;
					}
				}
			}
			catch (SecurityException)
			{
				return null;
			}
			catch (IOException)
			{
				return null;
			}
			catch (UnauthorizedAccessException)
			{
				return null;
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			ICredentials credentials;
			if (createAppTokenOnly)
			{
				credentials = OAuthCredentials.GetOAuthCredentialsForAppToken(actAsUserOrgId, "PlaceHolder");
			}
			else
			{
				ADObjectId id = actAsUser.Id;
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(id);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 552, "GetCredentialAndConfigureClientContext", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\RecipientTasks\\TeamMailbox\\TeamMailboxHelper.cs");
				ADUser aduser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(id);
				credentials = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(actAsUserOrgId, aduser, null);
			}
			if (context != null)
			{
				context.Credentials = credentials;
				context.FormDigestHandlingEnabled = false;
				context.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
				{
					args.WebRequestExecutor.RequestHeaders.Add(HttpRequestHeader.Authorization, "Bearer");
					args.WebRequestExecutor.RequestHeaders.Add("client-request-id", Guid.NewGuid().ToString());
					args.WebRequestExecutor.RequestHeaders.Add("return-client-request-id", "true");
					args.WebRequestExecutor.WebRequest.PreAuthenticate = true;
					args.WebRequestExecutor.WebRequest.UserAgent = Utils.GetUserAgentStringForSiteMailboxRequests();
				};
				if (num == 1)
				{
					context.ExecutingWebRequest += delegate(object sender, WebRequestEventArgs args)
					{
						args.WebRequestExecutor.WebRequest.Proxy = new WebProxy("127.0.0.1", 8888);
					};
				}
			}
			return credentials;
		}

		internal bool LinkSharePointSite(Uri sharePointSiteUrl, bool siteChecked, bool forceToUnlink = false)
		{
			LinkSharePointSiteResult linkSharePointSiteResult = LinkSharePointSiteResult.ResultNotSet;
			string empty = string.Empty;
			if (sharePointSiteUrl != null && this.teamMailbox.SharePointUrl != null && this.teamMailbox.SharePointUrl != sharePointSiteUrl)
			{
				linkSharePointSiteResult = this.InternalLinkSharePointSite(null, out empty, false, forceToUnlink);
			}
			if (linkSharePointSiteResult == LinkSharePointSiteResult.ResultNotSet || linkSharePointSiteResult == LinkSharePointSiteResult.Success || linkSharePointSiteResult == LinkSharePointSiteResult.CurrentlyNotLinked)
			{
				linkSharePointSiteResult = this.InternalLinkSharePointSite(sharePointSiteUrl, out empty, siteChecked, forceToUnlink);
			}
			switch (linkSharePointSiteResult)
			{
			case LinkSharePointSiteResult.NotSiteOwner:
				throw (sharePointSiteUrl == null) ? new RecipientTaskException(Strings.ErrorTeamMailboxCanNotUnLinkSharePointByNonSiteOwner(this.teamMailbox.SharePointUrl.ToString())) : new RecipientTaskException(Strings.ErrorTeamMailboxCanNotLinkSharePointByNonSiteOwner(sharePointSiteUrl.ToString()));
			case LinkSharePointSiteResult.SPServerVersionNotCompatible:
				throw new RecipientTaskException(Strings.ErrorTeamMailboxSharePointServerVersionInCompatible);
			case LinkSharePointSiteResult.NotTeamMailboxOwner:
				throw new RecipientTaskException(Strings.ErrorTeamMailboxCanNotLinkSharePointByNonOwner);
			case LinkSharePointSiteResult.LinkedByOthers:
				throw new RecipientTaskException(Strings.ErrorTeamMailboxSharePointSiteAlreadyLinkedWithOtherTM((this.teamMailbox.SharePointUrl != null) ? this.teamMailbox.SharePointUrl.ToString() : "N/A", empty));
			}
			return linkSharePointSiteResult == LinkSharePointSiteResult.Success;
		}

		private LinkSharePointSiteResult InternalLinkSharePointSite(Uri sharePointUrl, out string sharePointTeamMailboxKey, bool siteChecked, bool forceToUnlink)
		{
			LinkSharePointSiteResult linkSharePointSiteResult = LinkSharePointSiteResult.Success;
			sharePointTeamMailboxKey = "N/A";
			bool flag = sharePointUrl == null;
			Uri uri = sharePointUrl ?? this.teamMailbox.SharePointUrl;
			Exception ex = null;
			if (flag)
			{
				if (this.teamMailbox.SharePointUrl == null)
				{
					return LinkSharePointSiteResult.CurrentlyNotLinked;
				}
				sharePointUrl = this.teamMailbox.SharePointUrl;
			}
			try
			{
				using (ClientContext clientContext = new ClientContext(sharePointUrl.AbsoluteUri))
				{
					clientContext.RequestTimeout = 60000;
					bool flag2;
					TeamMailboxHelper.GetCredentialAndConfigureClientContext(this.actAsUser, this.actAsUserOrgId, clientContext, true, out flag2);
					Web web = clientContext.Web;
					clientContext.Load<Web>(web, new Expression<Func<Web, object>>[]
					{
						(Web x) => x.AllProperties,
						(Web x) => (object)x.Id
					});
					Site site = clientContext.Site;
					clientContext.Load<Site>(site, new Expression<Func<Site, object>>[]
					{
						(Site x) => x.Url
					});
					clientContext.ExecuteQuery();
					if (!siteChecked && clientContext.ServerVersion.Major < 15)
					{
						return LinkSharePointSiteResult.SPServerVersionNotCompatible;
					}
					string text = this.teamMailbox.PrimarySmtpAddress.ToString();
					if (web.AllProperties.FieldValues.ContainsKey("ExchangeTeamMailboxEmailAddress"))
					{
						sharePointTeamMailboxKey = (string)web.AllProperties["ExchangeTeamMailboxEmailAddress"];
						if (!string.Equals(text, sharePointTeamMailboxKey, StringComparison.OrdinalIgnoreCase))
						{
							linkSharePointSiteResult = LinkSharePointSiteResult.LinkedByOthers;
						}
						else if (flag)
						{
							web.AllProperties["ExchangeTeamMailboxEmailAddress"] = null;
							web.Update();
							clientContext.ExecuteQuery();
							this.teamMailbox.SharePointUrl = null;
							this.teamMailbox.SetSharePointSiteInfo(null, Guid.Empty);
							this.teamMailbox.SharePointLinkedBy = null;
						}
						else
						{
							if (this.actAsUser.Id.Equals(this.teamMailbox.SharePointLinkedBy))
							{
								linkSharePointSiteResult = LinkSharePointSiteResult.AlreadyLinkedBySelf;
							}
							else
							{
								this.teamMailbox.SharePointLinkedBy = this.actAsUser.Id;
							}
							this.teamMailbox.SharePointUrl = sharePointUrl;
						}
					}
					else if (flag)
					{
						this.teamMailbox.SharePointUrl = null;
						this.teamMailbox.SetSharePointSiteInfo(null, Guid.Empty);
						this.teamMailbox.SharePointLinkedBy = null;
					}
					else
					{
						Uri url = TeamMailbox.GetUrl(site.Url);
						if (url == null)
						{
							throw new RecipientTaskException(Strings.ErrorSharePointSiteHasNoValidWebCollectionUrl(site.Url));
						}
						web.AllProperties["ExchangeTeamMailboxEmailAddress"] = text;
						web.AllProperties["ExchangeTeamMailboxSharePointUrl"] = sharePointUrl.AbsoluteUri;
						web.AllProperties["ExchangeTeamMailboxSiteID"] = web.Id.ToString();
						web.AllProperties["ExchangeTeamMailboxSiteCollectionUrl"] = site.Url;
						web.Update();
						clientContext.ExecuteQuery();
						this.teamMailbox.SharePointUrl = sharePointUrl;
						this.teamMailbox.SetSharePointSiteInfo(url, web.Id);
						this.teamMailbox.SharePointLinkedBy = this.actAsUser.Id;
					}
				}
			}
			catch (ClientRequestException ex2)
			{
				ex = new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(uri.AbsoluteUri, ex2.Message));
			}
			catch (ServerException ex3)
			{
				ex = new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSiteWithCorrelationId(uri.AbsoluteUri, ex3.Message, ex3.ServerErrorTraceCorrelationId));
			}
			catch (TimeoutException ex4)
			{
				ex = new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(uri.AbsoluteUri, ex4.Message));
			}
			catch (IOException ex5)
			{
				ex = new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(uri.AbsoluteUri, ex5.Message));
			}
			catch (WebException e)
			{
				SharePointException ex6 = new SharePointException(uri.AbsoluteUri, e, false);
				ex = new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(uri.AbsoluteUri, ex6.DiagnosticInfo));
			}
			catch (FormatException ex7)
			{
				ex = new RecipientTaskException(Strings.ErrorTeamMailboxContactSharePointSite(uri.AbsoluteUri, ex7.Message));
			}
			if (flag && forceToUnlink)
			{
				if (ex != null || linkSharePointSiteResult == LinkSharePointSiteResult.LinkedByOthers)
				{
					this.teamMailbox.SharePointUrl = null;
					this.teamMailbox.SetSharePointSiteInfo(null, Guid.Empty);
					this.teamMailbox.SharePointLinkedBy = null;
					linkSharePointSiteResult = LinkSharePointSiteResult.Success;
				}
			}
			else if (ex != null)
			{
				throw ex;
			}
			return linkSharePointSiteResult;
		}

		private static bool ExchangeSharePointUserMatch(ADUser exUser, User spUser)
		{
			if (spUser.UserId != null && spUser.UserId.NameId != null)
			{
				string nameId = spUser.UserId.NameId;
				if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					try
					{
						SecurityIdentifier securityIdentifier = new SecurityIdentifier(nameId);
						return securityIdentifier.Equals(exUser.Sid) || securityIdentifier.Equals(exUser.MasterAccountSid);
					}
					catch (ArgumentException)
					{
					}
					return false;
				}
				NetID other;
				if (NetID.TryParse(nameId, out other))
				{
					return exUser.NetID.Equals(other);
				}
			}
			return false;
		}

		internal static string GenerateUniqueAliasForSiteMailbox(IRecipientSession recipientSession, OrganizationId organizationId, string preferredAlias, string prefix, bool isMicrosoftHostedOnlyDatacenter, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate writeError)
		{
			string text = WindowsLiveIDLocalPartConstraint.RemoveInvalidPartOfWindowsLiveID(preferredAlias);
			if (!string.IsNullOrEmpty(text) && text.Length > 3)
			{
				logHandler(Strings.VerboseGenerateAliasBySiteDisplayName(preferredAlias));
				text = RecipientTaskHelper.GenerateUniqueAlias(recipientSession, organizationId, (!string.IsNullOrEmpty(prefix)) ? (prefix + text) : text, logHandler, 63);
			}
			else
			{
				int num = 1000;
				if (string.IsNullOrEmpty(prefix))
				{
					prefix = (isMicrosoftHostedOnlyDatacenter ? "SMO-" : "SM-");
				}
				do
				{
					text = TeamMailboxHelper.GenerateRandomString();
					text = prefix + text;
					logHandler(Strings.VerboseGenerateAliasByRandomString(preferredAlias, text));
					if (RecipientTaskHelper.IsAliasUnique(recipientSession, organizationId, null, text, logHandler, writeError, ExchangeErrorCategory.Client))
					{
						break;
					}
					text = string.Empty;
				}
				while (num-- > 0);
			}
			if (string.IsNullOrEmpty(text))
			{
				writeError(new ErrorCannotGenerateSiteMailboxAliasException(), ExchangeErrorCategory.Client, null);
			}
			return text;
		}

		private static string GenerateRandomString()
		{
			int num = 4;
			ExDateTime utcNow = ExDateTime.UtcNow;
			StringBuilder stringBuilder = new StringBuilder(7 + num + 1);
			stringBuilder.AppendFormat("{0:D2}{1:D3}", utcNow.Year % 100, utcNow.DayOfYear);
			while (num-- > 0)
			{
				stringBuilder.Append((char)(65 + TeamMailboxHelper.siteMailboxAliasRandom.Next(26)));
			}
			return stringBuilder.ToString();
		}

		private const string SharePointTeamMailboxKey = "ExchangeTeamMailboxEmailAddress";

		private const string SharePointUrlKey = "ExchangeTeamMailboxSharePointUrl";

		private const string SharePointSiteIdKey = "ExchangeTeamMailboxSiteID";

		private const string SharePointSiteCollectionUrlKey = "ExchangeTeamMailboxSiteCollectionUrl";

		private TeamMailbox teamMailbox;

		private ADRawEntry actAsUser;

		private OrganizationId actAsUserOrgId;

		private IRecipientSession adSession;

		private TeamMailboxGetDataObject<ADUser> getAdUser;

		private static readonly Random siteMailboxAliasRandom = new Random();
	}
}
