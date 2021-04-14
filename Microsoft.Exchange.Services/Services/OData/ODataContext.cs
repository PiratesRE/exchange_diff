using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.OData.Model;
using Microsoft.Exchange.Services.OData.Web;
using Microsoft.OData.Core.UriParser;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;

namespace Microsoft.Exchange.Services.OData
{
	internal class ODataContext : DisposeTrackableBase
	{
		public ODataContext(HttpContext httpContext, Uri requestUri, ServiceModel serviceModel, ODataPathWrapper odataPath, ODataUriParser odataUriParser)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			ArgumentValidator.ThrowIfNull("requestUri", requestUri);
			ArgumentValidator.ThrowIfNull("serviceModel", serviceModel);
			ArgumentValidator.ThrowIfNull("odataPath", odataPath);
			ArgumentValidator.ThrowIfNull("odataUriParser", odataUriParser);
			HttpContext.Current = httpContext;
			this.HttpContext = httpContext;
			this.RequestUri = requestUri;
			this.ServiceModel = serviceModel;
			this.ODataPath = odataPath;
			this.ODataQueryOptions = new ODataQueryOptions(httpContext, odataUriParser);
			this.InitializeCallContext();
		}

		public CallContext CallContext { get; private set; }

		public ServiceModel ServiceModel { get; private set; }

		public Uri RequestUri { get; private set; }

		public ODataPathWrapper ODataPath { get; private set; }

		public IEdmModel EdmModel
		{
			get
			{
				return this.ServiceModel.EdmModel;
			}
		}

		public ODataQueryOptions ODataQueryOptions { get; private set; }

		public IEdmEntityType EntityType
		{
			get
			{
				return this.ODataPath.EntityType;
			}
		}

		public IEdmNavigationSource NavigationSource
		{
			get
			{
				return this.ODataPath.NavigationSource;
			}
		}

		public HttpContext HttpContext { get; private set; }

		public ADUser TargetMailbox { get; private set; }

		public NameValueCollection QueryString
		{
			get
			{
				return this.HttpContext.Request.QueryString;
			}
		}

		public RequestDetailsLogger RequestDetailsLogger
		{
			get
			{
				return this.CallContext.ProtocolLog;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.CallContext != null)
			{
				this.CallContext.Dispose();
				this.CallContext = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ODataContext>(this);
		}

		private void InitializeCallContext()
		{
			string impersonatedUser;
			string targetSmtpString;
			this.FindImpersonatedOrExplicitTargetUser(out impersonatedUser, out targetSmtpString);
			this.CallContext = CallContext.CreateForOData(this.HttpContext, impersonatedUser);
			this.TargetMailbox = this.CallContext.AccessingADUser;
			this.HandleExplicitTargetAccess(targetSmtpString);
			ExchangePrincipal exchangePrincipal;
			if (ExchangePrincipalCache.TryGetFromCache(this.TargetMailbox.Sid, this.CallContext.ADRecipientSessionContext, out exchangePrincipal) && !string.Equals(exchangePrincipal.MailboxInfo.Location.ServerFqdn, LocalServer.GetServer().Fqdn, StringComparison.OrdinalIgnoreCase))
			{
				WrongServerException ex = new WrongServerException(ServerStrings.PrincipalFromDifferentSite, exchangePrincipal.MailboxInfo.GetDatabaseGuid(), exchangePrincipal.MailboxInfo.Location.ServerFqdn, exchangePrincipal.MailboxInfo.Location.ServerVersion, null);
				string value = ex.RightServerToString();
				this.HttpContext.Response.Headers[WellKnownHeader.XDBMountedOnServer] = value;
				this.HttpContext.Response.Headers["X-BEServerException"] = typeof(IllegalCrossServerConnectionException).FullName;
				throw ex;
			}
			if (!this.CallContext.CallerHasAccess())
			{
				throw new ODataAuthorizationException(CoreResources.ErrorODataAccessDisabled);
			}
		}

		private void FindImpersonatedOrExplicitTargetUser(out string impersonatedUser, out string explicitTargetUser)
		{
			impersonatedUser = null;
			explicitTargetUser = null;
			if (this.ODataPath.PathSegments.Count > 2)
			{
				EntitySetSegment entitySetSegment = this.ODataPath.FirstSegment as EntitySetSegment;
				if (entitySetSegment != null && entitySetSegment.EntitySet.Name.Equals(EntitySets.Users.Name))
				{
					KeySegment keySegment = this.ODataPath.PathSegments[1] as KeySegment;
					if (keySegment != null)
					{
						string idKey = keySegment.GetIdKey();
						if ("Me".Equals(idKey, StringComparison.OrdinalIgnoreCase))
						{
							return;
						}
						OAuthIdentity oauthIdentity = this.HttpContext.User.Identity as OAuthIdentity;
						if (oauthIdentity != null && oauthIdentity.OAuthApplication.ApplicationType == OAuthApplicationType.V1App && oauthIdentity.IsAppOnly)
						{
							impersonatedUser = idKey;
							return;
						}
						explicitTargetUser = idKey;
					}
				}
			}
		}

		private void HandleExplicitTargetAccess(string targetSmtpString)
		{
			if (string.IsNullOrEmpty(targetSmtpString))
			{
				return;
			}
			ProxyAddress a = this.CallContext.AccessingADUser.EmailAddresses.FirstOrDefault((ProxyAddress x) => string.Equals(x.AddressString, targetSmtpString, StringComparison.OrdinalIgnoreCase));
			if (!(a == null))
			{
				return;
			}
			this.RequestDetailsLogger.AppendGenericInfo("ODataTargetMailbox", targetSmtpString);
			ADUser targetMailbox;
			if (ADIdentityInformationCache.Singleton.TryGetADUser(targetSmtpString, this.CallContext.ADRecipientSessionContext, out targetMailbox))
			{
				this.TargetMailbox = targetMailbox;
				this.CallContext.OwaExplicitLogonUser = this.TargetMailbox.PrimarySmtpAddress.ToString();
				return;
			}
			throw new InvalidUserException(targetSmtpString);
		}
	}
}
