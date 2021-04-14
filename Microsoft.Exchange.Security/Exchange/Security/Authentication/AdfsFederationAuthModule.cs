using System;
using System.Configuration;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Text;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Net.Wopi;
using Microsoft.IdentityModel.Configuration;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Web;
using Microsoft.IdentityModel.Web.Configuration;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsFederationAuthModule : WSFederationAuthenticationModule
	{
		protected AdfsFederationAuthModule()
		{
			AdfsFederationAuthModule.InitStaticVariables();
			if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled)
			{
				FederatedAuthentication.ServiceConfigurationCreated += this.FederatedAuthentication_ServiceConfigurationCreated;
			}
		}

		internal static bool IsAdfsAuthenticationEnabled { get; private set; }

		internal static bool IsActivityBasedAuthenticationTimeoutEnabled { get; private set; }

		internal static EnhancedTimeSpan ActivityBasedAuthenticationTimeoutInterval { get; private set; }

		internal static TimeSpan TimeBasedAuthenticationTimeoutInterval { get; private set; }

		internal static bool HasOtherAuthenticationMethod { get; set; }

		internal static AdfsIdentifyModelSection Section { get; private set; }

		public override bool CanReadSignInResponse(HttpRequest request, bool onPage)
		{
			if (string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) && this.IsSignInResponse(request))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::CanReadSignInResponse]: Skipping check for existing token and alwayss use the new one if exists.");
				return true;
			}
			Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::CanReadSignInResponse]: Calling base.CanReadSignInResponse().");
			return base.CanReadSignInResponse(request, onPage);
		}

		public override bool IsSignInResponse(HttpRequest request)
		{
			return request.UrlReferrer != null && request.UrlReferrer.IsAbsoluteUri && (request.UrlReferrer.AbsoluteUri.StartsWith(base.Issuer, StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(request.UrlReferrer.AbsolutePath) || string.Equals(request.UrlReferrer.AbsolutePath, "/")) && base.IsSignInResponse(request);
		}

		internal static void InitStaticVariables()
		{
			if (AdfsFederationAuthModule.initialized)
			{
				return;
			}
			bool flag = !string.IsNullOrWhiteSpace(HttpRuntime.AppDomainAppId) && HttpRuntime.AppDomainAppId.EndsWith("/calendar", StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				AdfsFederationAuthModule.IsAdfsAuthenticationEnabled = false;
				AdfsFederationAuthModule.initialized = true;
				return;
			}
			lock (AdfsFederationAuthModule.lockObject)
			{
				if (!AdfsFederationAuthModule.initialized)
				{
					AdfsFederationAuthModule.appDomainAppVirtualPath = HttpRuntime.AppDomainAppVirtualPath + '/';
					ADSessionSettings adsessionSettings = ADSessionSettings.FromRootOrgScopeSet();
					ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, adsessionSettings, 242, "InitStaticVariables", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\AdfsAuth\\AdfsFederationAuthModule.cs");
					PropertyDefinition[] virtualDirectoryPropertyDefinitions = new PropertyDefinition[]
					{
						ADVirtualDirectorySchema.InternalAuthenticationMethodFlags,
						ExchangeWebAppVirtualDirectorySchema.AdfsAuthentication
					};
					ADRawEntry virtualDirectoryObject = Utility.GetVirtualDirectoryObject(Guid.Empty, topologyConfigurationSession, virtualDirectoryPropertyDefinitions);
					AdfsFederationAuthModule.authenticationMethods = (AuthenticationMethodFlags)virtualDirectoryObject[ADVirtualDirectorySchema.InternalAuthenticationMethodFlags];
					AdfsFederationAuthModule.HasOtherAuthenticationMethod = ((AdfsFederationAuthModule.authenticationMethods & (AuthenticationMethodFlags.Fba | AuthenticationMethodFlags.WindowsIntegrated)) != AuthenticationMethodFlags.None);
					AdfsFederationAuthModule.IsAdfsAuthenticationEnabled = (bool)virtualDirectoryObject[ExchangeWebAppVirtualDirectorySchema.AdfsAuthentication];
					Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, "Successfully read ADFS Authentication configurations: HasOtherAutnenticationMethod, IsAdfsAuthenticationEnabled.");
					if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled)
					{
						bool isTestEnvironment = false;
						Utility.TryReadConfigBool("AdfsIsTest", out isTestEnvironment);
						AdfsFederationAuthModule.IsTestEnvironment = isTestEnvironment;
						ADPropertyDefinition[] properties = new ADPropertyDefinition[]
						{
							OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled,
							OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval,
							OrganizationSchema.AdfsAuthenticationRawConfiguration
						};
						ADRawEntry adrawEntry = topologyConfigurationSession.ReadADRawEntry(adsessionSettings.RootOrgId, properties);
						AdfsFederationAuthModule.IsActivityBasedAuthenticationTimeoutEnabled = !(bool)adrawEntry[OrganizationSchema.ActivityBasedAuthenticationTimeoutDisabled];
						AdfsFederationAuthModule.ActivityBasedAuthenticationTimeoutInterval = (EnhancedTimeSpan)adrawEntry[OrganizationSchema.ActivityBasedAuthenticationTimeoutInterval];
						Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, "Successfully read ADFS Authentication configurations: IsActivityBasedAuthenticationTimeoutEnabled, ActivityBasedAuthenticationTimeoutInterval.");
						string text = (string)adrawEntry[OrganizationSchema.AdfsAuthenticationRawConfiguration];
						if (!AdfsAuthenticationConfig.TryDecode(text, out AdfsFederationAuthModule.adfsRawConfiguration) || string.IsNullOrEmpty(AdfsFederationAuthModule.adfsRawConfiguration))
						{
							Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, string.Format("Cannot enable ADFS Authentication because the configuration string is not set. String value: {0}", text ?? "null"));
							AdfsFederationAuthModule.IsAdfsAuthenticationEnabled = false;
						}
						else
						{
							int num;
							if (Utility.TryReadConfigInt("AdfsAuthModuleActivityBasedTimeoutIntervalInSeconds", out num))
							{
								AdfsFederationAuthModule.ActivityBasedAuthenticationTimeoutInterval = TimeSpan.FromSeconds((double)num);
								Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, "ADFS Activity based time out interval found in web.config.");
							}
							int num2;
							if (Utility.TryReadConfigInt("AdfsAuthModuleTimeoutIntervalInSeconds", out num2))
							{
								AdfsFederationAuthModule.TimeBasedAuthenticationTimeoutInterval = TimeSpan.FromSeconds((double)num2);
								Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, "ADFS Time based time out interval found in web.config.");
							}
							else
							{
								AdfsFederationAuthModule.TimeBasedAuthenticationTimeoutInterval = AdfsFederationAuthModule.timeBasedAuthenticationTimeoutIntervalDefault;
							}
						}
						AdfsFederationAuthModule.Section = new AdfsIdentifyModelSection();
						try
						{
							using (XmlReader xmlReader = XmlReader.Create(new StringReader(AdfsFederationAuthModule.adfsRawConfiguration)))
							{
								AdfsFederationAuthModule.Section.Deserialize(xmlReader);
							}
						}
						catch (XmlException ex)
						{
							string message = string.Format("Fail to parse ADFS raw configuration XML: {0}. Input string: {1}", ex.Message, AdfsFederationAuthModule.adfsRawConfiguration);
							Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, message);
							Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceError(0, 0L, message);
							AdfsFederationAuthModule.IsAdfsAuthenticationEnabled = false;
						}
						catch (ConfigurationErrorsException ex2)
						{
							string message2 = string.Format("Fail to parse ADFS raw configuration XML: {0}. Input string: {1}", ex2.Message, AdfsFederationAuthModule.adfsRawConfiguration);
							Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel.ExTraceGlobals.EventLogTracer.TraceInformation(0, 0L, message2);
							Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceError(0, 0L, message2);
							AdfsFederationAuthModule.IsAdfsAuthenticationEnabled = false;
						}
					}
					AdfsFederationAuthModule.initialized = true;
				}
			}
		}

		protected override void InitializeModule(HttpApplication application)
		{
			if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled)
			{
				base.InitializeModule(application);
				base.SecurityTokenReceived += this.AdfsFederationAuthModule_SecurityTokenReceived;
			}
		}

		protected override void OnAuthenticateRequest(object sender, EventArgs eventArgs)
		{
			HttpApplication httpApplication = (HttpApplication)sender;
			HttpContext context = httpApplication.Context;
			if (EDiscoveryExportToolRequestPathHandler.IsEDiscoveryExportToolRequest(context.Request))
			{
				context.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
				return;
			}
			this.InternalOnAuthenticateRequest(sender, eventArgs);
		}

		protected override void OnEndRequest(object sender, EventArgs eventArgs)
		{
		}

		protected override void OnPostAuthenticateRequest(object sender, EventArgs eventArgs)
		{
		}

		protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs eventArgs)
		{
			base.OnRedirectingToIdentityProvider(eventArgs);
			SignInRequestMessage signInRequestMessage = eventArgs.SignInRequestMessage;
			HttpRequest request = HttpContext.Current.Request;
			UriBuilder uriBuilder = new UriBuilder(request.Url.Scheme, request.Url.Host, request.Url.Port, AdfsFederationAuthModule.appDomainAppVirtualPath);
			Uri uri = uriBuilder.Uri;
			signInRequestMessage.Realm = uri.AbsoluteUri;
			if (this.IsSignInResponse(request))
			{
				signInRequestMessage.Freshness = "0";
			}
		}

		protected override void InitializePropertiesFromConfiguration(string serviceName)
		{
			if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled)
			{
				ServiceElement element = AdfsFederationAuthModule.Section.ServiceElements.GetElement(serviceName);
				WSFederationAuthenticationElement wsfederation = element.FederatedAuthentication.WSFederation;
				base.Issuer = wsfederation.Issuer;
				base.Reply = wsfederation.Reply;
				base.RequireHttps = wsfederation.RequireHttps;
				base.Freshness = wsfederation.Freshness;
				base.AuthenticationType = wsfederation.AuthenticationType;
				base.HomeRealm = wsfederation.HomeRealm;
				base.Policy = wsfederation.Policy;
				base.Realm = wsfederation.Realm;
				base.Reply = wsfederation.Reply;
				base.SignOutReply = wsfederation.SignOutReply;
				base.Request = wsfederation.Request;
				base.RequestPtr = wsfederation.RequestPtr;
				base.Resource = wsfederation.Resource;
				base.SignInQueryString = wsfederation.SignInQueryString;
				base.SignOutQueryString = wsfederation.SignOutQueryString;
				base.PassiveRedirectEnabled = wsfederation.PassiveRedirectEnabled;
				base.PersistentCookiesOnPassiveRedirects = wsfederation.PersistentCookiesOnPassiveRedirects;
				if (AdfsFederationAuthModule.IsTestEnvironment)
				{
					base.ServiceConfiguration.CertificateValidationMode = X509CertificateValidationMode.None;
					base.ServiceConfiguration.CertificateValidator = X509CertificateValidator.None;
					return;
				}
				base.ServiceConfiguration.CertificateValidationMode = element.CertificateValidationElement.ValidationMode;
			}
		}

		private void InternalOnAuthenticateRequest(object sender, EventArgs eventArgs)
		{
			Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Entry.");
			HttpContext context = ((HttpApplication)sender).Context;
			HttpRequest request = context.Request;
			HttpResponse response = context.Response;
			if (request.IsAnonymousAuthFolderRequest())
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Request is auth folder anonymous.");
				return;
			}
			if (request.IsAdfsLogoffRequest())
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Is a ADFS logoff request.");
				Utility.DeleteFbaAuthCookies(request, response);
				AdfsTimeBasedLogonCookie.DeleteAdfsAuthCookies(request, response);
				WSFederationAuthenticationModule.FederatedSignOut(this.GetSignOutUri(), null);
				return;
			}
			if (Utility.IsResourceRequest(request.Path) && (!AuthCommon.IsFrontEnd || Utility.IsOwaRequestWithRoutingHint(request) || Utility.HasResourceRoutingHint(request) || Utility.IsAnonymousResourceRequest(request)))
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Request is for a resource that does not require authentication.");
				context.User = new WindowsPrincipal(WindowsIdentity.GetAnonymous());
				return;
			}
			if (WopiRequestPathHandler.IsWopiRequest(context.Request, AuthCommon.IsFrontEnd))
			{
				return;
			}
			bool flag = this.IsSignInResponse(request);
			bool flag2 = request.IsSignOutCleanupRequest();
			if (!request.IsAuthenticatedByAdfs() && !flag && !flag2 && request.IsAuthenticated && !request.ExplicitPreferAdfsAuthentication())
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Already authenticated by another method.");
				return;
			}
			if (request.PreferAdfsAuthentication() || flag || flag2)
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Calling the base class to authenticate the request.");
				try
				{
					base.OnAuthenticateRequest(sender, eventArgs);
				}
				catch (SecurityTokenException arg)
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceError<SecurityTokenException>(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Could not call base.OnAuthenticateRequest: {0}.", arg);
					response.Redirect("/owa/auth/errorfe.aspx?msg=WrongAudienceUriOrBadSigningCert");
				}
				if (flag2)
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Request is signout cleanup request.");
					AdfsTimeBasedLogonCookie.DeleteAdfsAuthCookies(request, response);
					if ((AdfsFederationAuthModule.authenticationMethods & AuthenticationMethodFlags.Fba) > AuthenticationMethodFlags.None)
					{
						Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Redirecting to FBA logout.");
						context.Response.Redirect("logoff.aspx");
					}
					else
					{
						response.Flush();
						response.End();
					}
				}
				else if (request.PreferAdfsAuthentication() && !request.IsAuthenticatedByAdfs())
				{
					if (!request.FilePath.StartsWith(AdfsFederationAuthModule.appDomainAppVirtualPath, StringComparison.OrdinalIgnoreCase))
					{
						Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Redirecting to {0}.", request.RawUrl);
						response.Redirect(request.RawUrl.Insert(request.FilePath.Length, "/"));
					}
					else if (!request.IsAjaxRequest() && !request.IsNotOwaGetOrOehRequest())
					{
						Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Redirecting AJAX request to ADFS.");
						this.RedirectToIdentityProvider("passive", request.RawUrl, base.PersistentCookiesOnPassiveRedirects);
					}
				}
			}
			if (!flag && request.IsAuthenticatedByAdfs())
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Response is not for sign in.");
				AdfsTimeBasedLogonCookie adfsTimeBasedLogonCookie;
				if (!AdfsTimeBasedLogonCookie.TryCreateFromHttpRequest(request, out adfsTimeBasedLogonCookie) || !AdfsTimeBasedLogonCookie.Validate(adfsTimeBasedLogonCookie.LogonTime, AdfsFederationAuthModule.TimeBasedAuthenticationTimeoutInterval))
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Cookie timeout, redirect to logon page.");
					this.Relogon();
					return;
				}
				if (AdfsFederationAuthModule.IsActivityBasedAuthenticationTimeoutEnabled && !AdfsTimeBasedLogonCookie.Validate(adfsTimeBasedLogonCookie.LastActivityTime, AdfsFederationAuthModule.ActivityBasedAuthenticationTimeoutInterval))
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Activity timeout, redirect to logon page.");
					this.Relogon();
					return;
				}
				if (OwaAuthenticationHelper.IsOwaUserActivityRequest(request))
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::InternalOnAuthenticateRequest]: Request is a user action.");
					adfsTimeBasedLogonCookie.Renew();
					adfsTimeBasedLogonCookie.AddToResponse(request, response);
				}
			}
		}

		private void Relogon()
		{
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			HttpResponse response = httpContext.Response;
			FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
			Utility.DeleteFbaAuthCookies(request, response);
			AdfsTimeBasedLogonCookie.DeleteAdfsAuthCookies(request, response);
			string returnUrlForRelogon = this.GetReturnUrlForRelogon(request);
			SignInRequestMessage signInRequestMessage = base.CreateSignInRequest("passive", returnUrlForRelogon, base.PersistentCookiesOnPassiveRedirects);
			RedirectingToIdentityProviderEventArgs redirectingToIdentityProviderEventArgs = new RedirectingToIdentityProviderEventArgs(signInRequestMessage);
			this.OnRedirectingToIdentityProvider(redirectingToIdentityProviderEventArgs);
			if (!redirectingToIdentityProviderEventArgs.Cancel)
			{
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::Relogon]: Redirecting.");
				this.LogoutAndRedirectToLogonPage(returnUrlForRelogon, redirectingToIdentityProviderEventArgs.SignInRequestMessage.RequestUrl);
				httpContext.ApplicationInstance.CompleteRequest();
			}
		}

		private void LogoutAndRedirectToLogonPage(string returnUrl, string requestUrl)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			HttpResponse response = httpContext.Response;
			if (request.IsAjaxRequest() || request.IsNotOwaGetOrOehRequest())
			{
				if (this.IsSignInResponse(request))
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::RedirectToLogonPage]: Explict log off and redirect to login page.");
					WSFederationAuthenticationModule.FederatedSignOut(this.GetSignOutUri(), request.Url);
					return;
				}
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsFederationAuthModule::RedirectToLogonPage]: Request is non GET request. Return 440.");
				response.StatusCode = 440;
				response.StatusDescription = "Login Timeout";
				response.AppendToLog("logoffReason=UnauthenticatedGuest");
				return;
			}
			else
			{
				if (returnUrl.IndexOf("/closewindow.aspx", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsFederationAuthModule::RedirectToLogonPage]: closewindow.aspx found. Redirecting to {0}.", requestUrl);
					response.Redirect(requestUrl);
					return;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("/ecp/");
				stringBuilder.Append("auth/TimeoutLogout.aspx");
				stringBuilder.Append("?");
				stringBuilder.Append("ru");
				stringBuilder.Append("=");
				stringBuilder.Append(HttpUtility.UrlEncode(requestUrl));
				string text = stringBuilder.ToString();
				Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsFederationAuthModule::RedirectToLogonPage]: Redirecting to {0}.", text);
				response.Redirect(text, false);
				return;
			}
		}

		private string GetReturnUrlForRelogon(HttpRequest request)
		{
			string text = null;
			if (this.IsSignInResponse(request))
			{
				text = this.GetReturnUrlFromResponse(request);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = request.RawUrl;
			}
			Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<string>(0L, "[AdfsFederationAuthModule::GetReturnUrlForRelogon]: returnUrl={0}.", text);
			return text;
		}

		private void AdfsFederationAuthModule_SecurityTokenReceived(object sender, SecurityTokenReceivedEventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			HttpRequest request = httpContext.Request;
			HttpResponse response = httpContext.Response;
			SamlSecurityToken samlSecurityToken = e.SecurityToken as SamlSecurityToken;
			if (samlSecurityToken != null)
			{
				DateTime validFrom = samlSecurityToken.ValidFrom;
				foreach (SamlStatement samlStatement in samlSecurityToken.Assertion.Statements)
				{
					SamlAuthenticationStatement samlAuthenticationStatement = samlStatement as SamlAuthenticationStatement;
					if (samlAuthenticationStatement != null)
					{
						TimeSpan timeSpan = validFrom.Subtract(samlAuthenticationStatement.AuthenticationInstant);
						if (!AdfsTimeBasedLogonCookie.Validate(timeSpan, AdfsFederationAuthModule.TimeBasedAuthenticationTimeoutInterval) || (AdfsFederationAuthModule.IsActivityBasedAuthenticationTimeoutEnabled && !AdfsTimeBasedLogonCookie.Validate(timeSpan, AdfsFederationAuthModule.ActivityBasedAuthenticationTimeoutInterval)))
						{
							e.Cancel = true;
							break;
						}
						break;
					}
				}
			}
			if (e.Cancel)
			{
				this.Relogon();
				return;
			}
			AdfsTimeBasedLogonCookie adfsTimeBasedLogonCookie = AdfsTimeBasedLogonCookie.CreateFromCurrentTime();
			adfsTimeBasedLogonCookie.AddToResponse(request, response);
		}

		private void FederatedAuthentication_ServiceConfigurationCreated(object sender, ServiceConfigurationCreatedEventArgs e)
		{
			e.ServiceConfiguration = new AdfsServiceConfiguration(AdfsFederationAuthModule.Section.ServiceElements.GetElement(string.Empty));
			e.ServiceConfiguration.SecurityTokenHandlers.AddOrReplace(new AdfsSessionSecurityTokenHandler());
		}

		private Uri GetSignOutUri()
		{
			Uri uri = new Uri(base.Issuer);
			if (string.IsNullOrEmpty(uri.Query))
			{
				uri = new Uri(base.Issuer + "?wa=wsignout1.0");
			}
			else
			{
				uri = new Uri(base.Issuer + "&wa=wsignout1.0");
			}
			Microsoft.Exchange.Diagnostics.Components.Clients.ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug<Uri>(0L, "[AdfsFederationAuthModule::GetSignOutUri]: signoutUri={0}.", uri);
			return uri;
		}

		public const string SignOutParameter = "wa=wsignout1.0";

		public const string ActivityBasedAuthenticationTimeoutIntervalKey = "AdfsAuthModuleActivityBasedTimeoutIntervalInSeconds";

		public const string TimeBasedAuthenticationTimeoutIntervalKey = "AdfsAuthModuleTimeoutIntervalInSeconds";

		public const string RedirectUrlParam = "ru";

		public const string FrontEndErrorPage = "/owa/auth/errorfe.aspx";

		private const string CalendarVDirPostfix = "/calendar";

		private const string IsTestWebConfigKey = "AdfsIsTest";

		private static readonly TimeSpan timeBasedAuthenticationTimeoutIntervalDefault = TimeSpan.FromHours(23.0);

		private static AuthenticationMethodFlags authenticationMethods;

		private static bool initialized = false;

		private static bool IsTestEnvironment;

		private static string appDomainAppVirtualPath = null;

		private static string adfsRawConfiguration;

		private static object lockObject = new object();
	}
}
