using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.CompliancePolicy;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal class SpPolicyCenterSite
	{
		public Uri SpSiteUrl { get; private set; }

		public Uri SpAdminSiteUrl { get; private set; }

		public SpPolicyCenterSite(Uri spSiteUrl, Uri spAdminSiteUrl, ICredentials credentials)
		{
			ArgumentValidator.ThrowIfNull("spSiteUrl", spSiteUrl);
			ArgumentValidator.ThrowIfNull("spAdminSiteUrl", spAdminSiteUrl);
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			this.SpSiteUrl = spSiteUrl;
			this.SpAdminSiteUrl = spAdminSiteUrl;
			this.credentials = credentials;
		}

		public Uri GetPolicyCenterSite(bool throwException = true)
		{
			if (this.spPolicyCenterSiteUrl == null)
			{
				Utils.WrapSharePointCsomCall(this.SpSiteUrl, this.credentials, delegate(ClientContext context)
				{
					SPPolicyStoreProxy sppolicyStoreProxy = new SPPolicyStoreProxy(context, context.Site.RootWeb);
					context.Load<SPPolicyStoreProxy>(sppolicyStoreProxy, new Expression<Func<SPPolicyStoreProxy, object>>[0]);
					context.ExecuteQuery();
					Uri uri;
					if (Uri.TryCreate(sppolicyStoreProxy.PolicyStoreUrl, UriKind.Absolute, out uri))
					{
						this.spPolicyCenterSiteUrl = uri;
						return;
					}
					if (throwException)
					{
						throw new SpCsomCallException(Strings.ErrorInvalidPolicyCenterSiteUrl(sppolicyStoreProxy.PolicyStoreUrl));
					}
				});
			}
			return this.spPolicyCenterSiteUrl;
		}

		public void NotifyUnifiedPolicySync(string notificationId, string syncSvcUrl, string[] changeInfos, bool syncNow, bool fullSyncForTenant)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("notificationId", notificationId);
			ArgumentValidator.ThrowIfNullOrEmpty("syncSvcUrl", syncSvcUrl);
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<string>("changeInfos", changeInfos);
			Utils.WrapSharePointCsomCall(this.GetPolicyCenterSite(true), this.credentials, delegate(ClientContext context)
			{
				SPPolicyStore sppolicyStore = new SPPolicyStore(context, context.Site.RootWeb);
				context.Load<SPPolicyStore>(sppolicyStore, new Expression<Func<SPPolicyStore, object>>[0]);
				sppolicyStore.NotifyUnifiedPolicySync(notificationId, syncSvcUrl, changeInfos, syncNow, fullSyncForTenant);
				context.ExecuteQuery();
			});
		}

		public Uri GeneratePolicyCenterSiteUri(int? salt)
		{
			string text = "/sites/CompliancePolicyCenter";
			if (salt != null)
			{
				text += salt.ToString();
			}
			return new Uri(this.SpSiteUrl, text);
		}

		public bool IsAnExistingSite(Uri siteUrl, out ServerException exception)
		{
			ArgumentValidator.ThrowIfNull("siteUrl", siteUrl);
			bool result = false;
			ServerException caughtException = null;
			Utils.WrapSharePointCsomCall(this.SpAdminSiteUrl, this.credentials, delegate(ClientContext context)
			{
				try
				{
					Tenant tenant = new Tenant(context);
					Site siteByUrl = tenant.GetSiteByUrl(siteUrl.AbsoluteUri);
					context.Load<Site>(siteByUrl, new Expression<Func<Site, object>>[0]);
					context.ExecuteQuery();
					result = true;
				}
				catch (ServerException caughtException)
				{
					caughtException = caughtException;
				}
			});
			exception = caughtException;
			return result;
		}

		public bool IsADeletedSite(Uri siteUrl, out ServerException exception)
		{
			ArgumentValidator.ThrowIfNull("siteUrl", siteUrl);
			bool result = false;
			ServerException caughtException = null;
			Utils.WrapSharePointCsomCall(this.SpAdminSiteUrl, this.credentials, delegate(ClientContext context)
			{
				try
				{
					Tenant tenant = new Tenant(context);
					DeletedSiteProperties deletedSitePropertiesByUrl = tenant.GetDeletedSitePropertiesByUrl(siteUrl.AbsoluteUri);
					context.Load<DeletedSiteProperties>(deletedSitePropertiesByUrl, new Expression<Func<DeletedSiteProperties, object>>[0]);
					context.ExecuteQuery();
					result = true;
				}
				catch (ServerException caughtException)
				{
					caughtException = caughtException;
				}
			});
			exception = caughtException;
			return result;
		}

		public void CreatePolicyCenterSite(Uri policyCenterSiteUrl, string siteOwner, long timeoutInMilliSeconds)
		{
			ArgumentValidator.ThrowIfNull("policyCenterSiteUrl", policyCenterSiteUrl);
			ArgumentValidator.ThrowIfNullOrEmpty("siteOwner", siteOwner);
			Utils.WrapSharePointCsomCall(this.SpAdminSiteUrl, this.credentials, delegate(ClientContext context)
			{
				SiteCreationProperties siteCreationProperties = new SiteCreationProperties
				{
					Url = policyCenterSiteUrl.AbsoluteUri,
					Owner = siteOwner,
					Template = "POLICYCTR#0",
					Title = "Compliance Policy Center"
				};
				Tenant tenant = new Tenant(context);
				SpoOperation spoOperation = tenant.CreateSite(siteCreationProperties);
				context.Load<SpoOperation>(spoOperation, new Expression<Func<SpoOperation, object>>[0]);
				context.ExecuteQuery();
				long num = timeoutInMilliSeconds;
				while (!spoOperation.IsComplete)
				{
					if (num <= 0L || spoOperation.HasTimedout)
					{
						throw new ErrorCreateSiteTimeOutException(policyCenterSiteUrl.AbsoluteUri);
					}
					int num2 = Math.Min(Math.Max(5000, spoOperation.PollingInterval), (int)num);
					num -= (long)num2;
					Thread.Sleep(num2);
					context.Load<SpoOperation>(spoOperation, new Expression<Func<SpoOperation, object>>[0]);
					context.ExecuteQuery();
				}
			});
		}

		private const string PolicyCenterSiteRelativeUrl = "/sites/CompliancePolicyCenter";

		private const string PolicyCenterTemplate = "POLICYCTR#0";

		private const string PolicyCenterTitle = "Compliance Policy Center";

		private ICredentials credentials;

		private Uri spPolicyCenterSiteUrl;
	}
}
