using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class OwaProxyRequestHandler : OwaEcpProxyRequestHandler<OwaService>
	{
		protected override string ProxyLogonUri
		{
			get
			{
				return "proxyLogon.owa";
			}
		}

		protected override ClientAccessType ClientAccessType
		{
			get
			{
				return ClientAccessType.Internal;
			}
		}

		protected override bool WillAddProtocolSpecificCookiesToClientResponse
		{
			get
			{
				return true;
			}
		}

		internal static void TryAddUnAuthenticatedPLTRequestPostDataToUriQueryOfIISLog(HttpContext httpContext)
		{
			if (httpContext != null && OwaProxyRequestHandler.IsPLTPostRequest(httpContext.Request) && httpContext.Response != null && httpContext.Response.StatusCode == 440 && httpContext.Request.InputStream != null)
			{
				try
				{
					using (StreamReader streamReader = new StreamReader(httpContext.Request.InputStream))
					{
						string param = streamReader.ReadToEnd();
						httpContext.Response.AppendToLog(param);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		internal static void AddProxyUriHeader(HttpRequest request, WebHeaderCollection headers)
		{
			headers["X-OWA-ProxyUri"] = new UriBuilder
			{
				Scheme = request.Url.Scheme,
				Port = request.Url.Port,
				Host = request.Url.Host,
				Path = request.ApplicationPath
			}.Uri.ToString();
		}

		protected override StreamProxy BuildRequestStreamProxy(StreamProxy.StreamProxyType streamProxyType, Stream source, Stream target, byte[] buffer)
		{
			if (OwaProxyRequestHandler.IsPLTPostRequest(base.ClientRequest))
			{
				return new OwaPLTStreamProxy(streamProxyType, source, target, buffer, this);
			}
			return base.BuildRequestStreamProxy(streamProxyType, source, target, buffer);
		}

		protected override void SetProtocolSpecificServerRequestParameters(HttpWebRequest serverRequest)
		{
			base.SetProtocolSpecificServerRequestParameters(serverRequest);
			object obj = base.HttpContext.Items["Flags"];
			if (obj != null)
			{
				int num = (int)obj;
				if ((num & 4) != 4)
				{
					serverRequest.Headers["X-LogonType"] = "Public";
				}
				if ((num & 1) == 1 && !base.ClientRequest.Url.AbsolutePath.StartsWith("/owa/attachment.ashx", StringComparison.OrdinalIgnoreCase) && !base.ClientRequest.Url.AbsolutePath.StartsWith("/owa/integrated/attachment.ashx", StringComparison.OrdinalIgnoreCase))
				{
					serverRequest.UserAgent = "Mozilla/5.0 (Windows NT; owaauth)";
				}
			}
		}

		protected override void AddProtocolSpecificHeadersToServerRequest(WebHeaderCollection headers)
		{
			IIdentity identity = base.HttpContext.User.Identity;
			CompositeIdentity compositeIdentity = base.HttpContext.User.Identity as CompositeIdentity;
			if (compositeIdentity != null)
			{
				identity = compositeIdentity.PrimaryIdentity;
			}
			if (!base.ProxyToDownLevel || identity is OAuthIdentity || identity is OAuthPreAuthIdentity || identity is MSAIdentity)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[OwaProxyRequestHandler::AddProtocolSpecificHeadersToServerRequest]: Skip adding downlevel proxy headers.");
			}
			else
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[OwaProxyRequestHandler::AddProtocolSpecificHeadersToServerRequest]: User identity type is {0}.", identity.GetType().FullName);
				headers["X-OWA-ProxySid"] = identity.GetSecurityIdentifier().ToString();
				OwaProxyRequestHandler.AddProxyUriHeader(base.ClientRequest, headers);
				headers["X-OWA-ProxyVersion"] = HttpProxyGlobals.ApplicationVersion;
			}
			if (UrlUtilities.IsCmdWebPart(base.ClientRequest) && !OwaProxyRequestHandler.IsOwa15Url(base.ClientRequest))
			{
				headers["X-OWA-ProxyWebPart"] = "1";
			}
			headers["RPSPUID"] = (string)base.HttpContext.Items["RPSPUID"];
			headers["RPSOrgIdPUID"] = (string)base.HttpContext.Items["RPSOrgIdPUID"];
			headers["logonLatency"] = (string)base.HttpContext.Items["logonLatency"];
			if (base.IsExplicitSignOn)
			{
				headers["X-OWA-ExplicitLogonUser"] = HttpUtility.UrlDecode(base.ExplicitSignOnAddress);
			}
			base.AddProtocolSpecificHeadersToServerRequest(headers);
		}

		protected override bool ShouldCopyHeaderToServerRequest(string headerName)
		{
			return !string.Equals(headerName, "X-OWA-ProxyUri", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "X-OWA-ProxyVersion", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "X-OWA-ProxySid", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "X-LogonType", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "X-OWA-ProxyWebPart", StringComparison.OrdinalIgnoreCase) && !string.Equals(headerName, "RPSPUID", StringComparison.OrdinalIgnoreCase) && base.ShouldCopyHeaderToServerRequest(headerName);
		}

		protected override void CopySupplementalCookiesToClientResponse()
		{
			if (HttpProxySettings.DFPOWAVdirProxyEnabled.Value)
			{
				this.SetDFPOwaVdirCookie();
			}
			base.CopySupplementalCookiesToClientResponse();
		}

		protected override Uri GetTargetBackEndServerUrl()
		{
			Uri targetBackEndServerUrl = base.GetTargetBackEndServerUrl();
			if (HttpProxySettings.DFPOWAVdirProxyEnabled.Value)
			{
				string text = base.ClientRequest.QueryString[OwaProxyRequestHandler.DFPOWAVdirParam];
				HttpCookie httpCookie = base.ClientRequest.Cookies["X-DFPOWA-Vdir"];
				if (!base.ClientRequest.Url.AbsolutePath.EndsWith("/logoff.owa", StringComparison.OrdinalIgnoreCase))
				{
					string text2 = string.Empty;
					if (httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value))
					{
						text2 = httpCookie.Value;
					}
					if (!string.IsNullOrEmpty(text))
					{
						text = text.Trim();
						if (OwaProxyRequestHandler.DFPOWAValidVdirValues.Contains(text, StringComparer.OrdinalIgnoreCase))
						{
							text2 = text;
						}
					}
					if (!string.IsNullOrEmpty(text2))
					{
						return UrlUtilities.FixDFPOWAVdirUrlForBackEnd(targetBackEndServerUrl, text2);
					}
				}
			}
			return UrlUtilities.FixIntegratedAuthUrlForBackEnd(targetBackEndServerUrl);
		}

		protected override AnchorMailbox ResolveAnchorMailbox()
		{
			if (base.AuthBehavior.IsFullyAuthenticated())
			{
				return this.LegacyResolveAnchorMailbox();
			}
			base.HasPreemptivelyCheckedForRoutingHint = true;
			string text = base.HttpContext.Request.Headers["X-UpnAnchorMailbox"];
			AnchorMailbox anchorMailbox;
			if (string.IsNullOrWhiteSpace(text))
			{
				anchorMailbox = base.CreateAnchorMailboxFromRoutingHint();
			}
			else
			{
				base.Logger.SafeSet(HttpProxyMetadata.RoutingHint, "OwaEcpUpn");
				anchorMailbox = new LiveIdMemberNameAnchorMailbox(text, null, this);
			}
			string text2 = base.ClientRequest.Headers["X-OWA-ExplicitLogonUser"];
			if (anchorMailbox == null)
			{
				if (base.UseRoutingHintForAnchorMailbox)
				{
					if (!string.IsNullOrEmpty(text2) && SmtpAddress.IsValidSmtpAddress(text2))
					{
						base.IsExplicitSignOn = true;
						base.ExplicitSignOnAddress = text2;
						base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP-Header");
						anchorMailbox = new SmtpAnchorMailbox(text2, this);
					}
					else
					{
						text2 = this.TryGetExplicitLogonNode(ExplicitLogonNode.Second);
						if (!string.IsNullOrEmpty(text2))
						{
							if (SmtpAddress.IsValidSmtpAddress(text2))
							{
								base.IsExplicitSignOn = true;
								base.ExplicitSignOnAddress = text2;
								base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP");
								anchorMailbox = new SmtpAnchorMailbox(text2, this);
							}
							else if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.ExplicitDomain.Enabled) && SmtpAddress.IsValidDomain(text2))
							{
								text2 = this.TryGetExplicitLogonNode(ExplicitLogonNode.Third);
								if (text2 != null)
								{
									base.IsExplicitSignOn = true;
									base.ExplicitSignOnAddress = text2;
									base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP");
									anchorMailbox = new SmtpAnchorMailbox(text2, this);
								}
							}
						}
					}
				}
				if (anchorMailbox == null)
				{
					anchorMailbox = base.ResolveAnchorMailbox();
				}
				else
				{
					base.IsAnchorMailboxFromRoutingHint = true;
					this.originalAnchorMailboxFromExplicitLogon = anchorMailbox;
				}
			}
			else if (!string.IsNullOrWhiteSpace(text2))
			{
				if (!string.Equals(anchorMailbox.SourceObject.ToString(), text2, StringComparison.InvariantCultureIgnoreCase))
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "ExplicitLogonMismatch", string.Format("{0}~{1}", anchorMailbox.SourceObject, text2));
				}
				this.originalAnchorMailboxFromExplicitLogon = anchorMailbox;
			}
			UserBasedAnchorMailbox userBasedAnchorMailbox = anchorMailbox as UserBasedAnchorMailbox;
			if (userBasedAnchorMailbox != null)
			{
				userBasedAnchorMailbox.MissingDatabaseHandler = new Func<ADRawEntry, ADObjectId>(this.ResolveMailboxDatabase);
			}
			return anchorMailbox;
		}

		protected ADObjectId ResolveMailboxDatabase(ADRawEntry activeDirectoryRawEntry)
		{
			if (activeDirectoryRawEntry == null)
			{
				throw new ArgumentNullException("activeDirectoryRawEntry");
			}
			SmtpProxyAddress smtpProxyAddress = (SmtpProxyAddress)activeDirectoryRawEntry[ADRecipientSchema.ExternalEmailAddress];
			if (smtpProxyAddress != null)
			{
				OrganizationId key = (OrganizationId)activeDirectoryRawEntry[ADObjectSchema.OrganizationId];
				OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(key);
				if (!((SmtpAddress)smtpProxyAddress).IsValidAddress)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[OwaProxyRequestHandler::ResolveMailboxDatabase]: ExternalEmailAddress configured is invalid.");
				}
				else
				{
					OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(((SmtpAddress)smtpProxyAddress).Domain);
					if (organizationRelationship != null && organizationRelationship.TargetOwaURL != null)
					{
						string absoluteUri = organizationRelationship.TargetOwaURL.AbsoluteUri;
						ExTraceGlobals.VerboseTracer.TraceDebug<string>((long)this.GetHashCode(), "[OwaProxyRequestHandler::ResolveMailboxDatabase]: Stop processing and redirect to {0}.", absoluteUri);
						base.Logger.AppendGenericInfo("ExternalRedir", absoluteUri);
						throw new ServerSideTransferException(absoluteUri, LegacyRedirectTypeOptions.Manual);
					}
					ExTraceGlobals.VerboseTracer.TraceDebug((long)this.GetHashCode(), "[OwaProxyRequestHandler::ResolveMailboxDatabase]: Unable to find OrganizationRelationShip or its TargetOwaUrl is not configured.");
					base.Logger.AppendGenericInfo("ExternalRedir", "Org-Relationship or targetOwaUrl not found.");
				}
			}
			return null;
		}

		protected override void ResetForRetryOnError()
		{
			this.originalAnchorMailboxFromExplicitLogon = null;
			base.ResetForRetryOnError();
		}

		protected override void UpdateOrInvalidateAnchorMailboxCache(Guid mdbGuid, string resourceForest)
		{
			if (this.originalAnchorMailboxFromExplicitLogon != null && this.originalAnchorMailboxFromExplicitLogon != base.AnchoredRoutingTarget.AnchorMailbox && !string.IsNullOrEmpty(resourceForest))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<AnchorMailbox, Guid>((long)this.GetHashCode(), "[OwaProxyRequestHandler::UpdateOrInvalidateAnchorMailboxCache]: Updating anchor mailbox cache for original anchor mailbox {0}, mapping to Mailbox Database {1}.", this.originalAnchorMailboxFromExplicitLogon, mdbGuid);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(base.Logger, "UpdateAnchorMbxCache", string.Format("{0}~{1}~{2}", this.originalAnchorMailboxFromExplicitLogon, mdbGuid, resourceForest));
				this.originalAnchorMailboxFromExplicitLogon.UpdateCache(new AnchorMailboxCacheEntry
				{
					Database = new ADObjectId(mdbGuid, resourceForest)
				});
				return;
			}
			base.UpdateOrInvalidateAnchorMailboxCache(mdbGuid, resourceForest);
		}

		protected override void HandleLogoffRequest()
		{
			if (base.ClientRequest != null && base.ClientResponse != null && base.ClientRequest.Url.AbsolutePath.EndsWith("/logoff.owa", StringComparison.OrdinalIgnoreCase))
			{
				if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoFormBasedAuthentication.Enabled)
				{
					FbaModule.InvalidateKeyCache(base.ClientRequest);
				}
				Utility.DeleteFbaAuthCookies(base.ClientRequest, base.ClientResponse);
			}
		}

		protected override BackEndServer GetE12TargetServer(BackEndServer mailboxServer)
		{
			if (!Utilities.IsPartnerHostedOnly && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoCrossForestServerLocate.Enabled)
			{
				Uri e12ExternalUrl = HttpProxyBackEndHelper.GetE12ExternalUrl<OwaService>(mailboxServer);
				if (e12ExternalUrl != null)
				{
					throw new HttpException(302, e12ExternalUrl.ToString());
				}
			}
			return base.GetE12TargetServer(mailboxServer);
		}

		protected override bool IsRoutingError(HttpWebResponse response)
		{
			string text;
			return base.TryGetSpecificHeaderFromResponse(response, "OwaProxyRequestHandler::IsRoutingError", "X-OWA-Error", Constants.IllegalCrossServerConnectionExceptionType, out text) || base.TryGetSpecificHeaderFromResponse(response, "OwaProxyRequestHandler::IsRoutingError", "X-OWA-Error", "Microsoft.Exchange.Data.Storage.WrongServerException", out text) || base.TryGetSpecificHeaderFromResponse(response, "OwaProxyRequestHandler::IsRoutingError", "X-OWA-Error", "Microsoft.Exchange.Data.Storage.DatabaseNotFoundException", out text) || base.IsRoutingError(response);
		}

		private static bool IsOwa15Url(HttpRequest request)
		{
			if (string.IsNullOrEmpty(request.Url.Query))
			{
				return true;
			}
			foreach (string name in OwaProxyRequestHandler.Owa15ParameterNames)
			{
				if (!string.IsNullOrEmpty(request.QueryString[name]))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsPLTPostRequest(HttpRequest httpRequest)
		{
			return httpRequest != null && httpRequest.Url != null && httpRequest.Url.Query != null && httpRequest.Url.ToString().IndexOf("plt1.ashx", StringComparison.OrdinalIgnoreCase) >= 0 && httpRequest.Url.Query.IndexOf("cId", StringComparison.OrdinalIgnoreCase) >= 0 && httpRequest.HttpMethod == "POST";
		}

		private void SetDFPOwaVdirCookie()
		{
			if (HttpProxySettings.DFPOWAVdirProxyEnabled.Value)
			{
				string text = base.ClientRequest.QueryString[OwaProxyRequestHandler.DFPOWAVdirParam];
				HttpCookie httpCookie = base.ClientRequest.Cookies["X-DFPOWA-Vdir"];
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Trim();
					if (!OwaProxyRequestHandler.DFPOWAValidVdirValues.Contains(text, StringComparer.OrdinalIgnoreCase))
					{
						return;
					}
					bool flag = httpCookie != null && !string.Equals(text, httpCookie.Value);
					if (httpCookie == null || flag)
					{
						HttpCookie httpCookie2 = new HttpCookie("X-DFPOWA-Vdir", text);
						httpCookie2.HttpOnly = false;
						httpCookie2.Secure = base.ClientRequest.IsSecureConnection;
						base.ClientResponse.Cookies.Add(httpCookie2);
					}
				}
			}
		}

		private AnchorMailbox LegacyResolveAnchorMailbox()
		{
			AnchorMailbox anchorMailbox = null;
			if (base.UseRoutingHintForAnchorMailbox)
			{
				string text = base.ClientRequest.Headers["X-OWA-ExplicitLogonUser"];
				if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text))
				{
					base.IsExplicitSignOn = true;
					base.ExplicitSignOnAddress = text;
					base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP-Header");
					anchorMailbox = new SmtpAnchorMailbox(text, this);
				}
				else
				{
					text = this.TryGetExplicitLogonNode(ExplicitLogonNode.Second);
					if (!string.IsNullOrEmpty(text))
					{
						if (SmtpAddress.IsValidSmtpAddress(text))
						{
							base.IsExplicitSignOn = true;
							base.ExplicitSignOnAddress = text;
							base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP");
							anchorMailbox = new SmtpAnchorMailbox(text, this);
						}
						else if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.ExplicitDomain.Enabled) && SmtpAddress.IsValidDomain(text))
						{
							string domain = text;
							text = this.TryGetExplicitLogonNode(ExplicitLogonNode.Third);
							if (text == null)
							{
								base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-Domain");
								anchorMailbox = new DomainAnchorMailbox(domain, this);
							}
							else
							{
								base.IsExplicitSignOn = true;
								base.ExplicitSignOnAddress = text;
								base.Logger.Set(HttpProxyMetadata.RoutingHint, "ExplicitLogon-SMTP");
								anchorMailbox = new SmtpAnchorMailbox(text, this);
							}
						}
					}
				}
			}
			if (anchorMailbox == null)
			{
				anchorMailbox = base.ResolveAnchorMailbox();
			}
			else
			{
				base.IsAnchorMailboxFromRoutingHint = true;
				this.originalAnchorMailboxFromExplicitLogon = anchorMailbox;
			}
			UserBasedAnchorMailbox userBasedAnchorMailbox = anchorMailbox as UserBasedAnchorMailbox;
			if (userBasedAnchorMailbox != null)
			{
				userBasedAnchorMailbox.MissingDatabaseHandler = new Func<ADRawEntry, ADObjectId>(this.ResolveMailboxDatabase);
			}
			return anchorMailbox;
		}

		private const string OwaLogonTypeHeader = "X-LogonType";

		private const string OwaLogonTypeHeaderPublicValue = "Public";

		private const string OwaProxyLogonUri = "proxyLogon.owa";

		private const string AttachmentUrl = "/owa/attachment.ashx";

		private const string IntegratedAttachmentUrl = "/owa/integrated/attachment.ashx";

		private const string DownlevelUserAgent = "Mozilla/5.0 (Windows NT; owaauth)";

		private const string LiveIdPuid = "RPSPUID";

		private const string OrgIdPuid = "RPSOrgIdPUID";

		private const string XOwaErrorHeaderName = "X-OWA-Error";

		private const string LogonLatencyName = "logonLatency";

		private const string WrongServerException = "Microsoft.Exchange.Data.Storage.WrongServerException";

		private const string DatabaseNotFoundException = "Microsoft.Exchange.Data.Storage.DatabaseNotFoundException";

		public static readonly string DFPOWAVdirParam = "vdir";

		public static readonly string[] DFPOWAValidVdirValues = new string[]
		{
			"OWA",
			"DFPOWA",
			"DFPOWA1",
			"DFPOWA2",
			"DFPOWA3",
			"DFPOWA4",
			"DFPOWA5"
		};

		private static readonly string[] Owa15ParameterNames = new string[]
		{
			OwaProxyRequestHandler.DFPOWAVdirParam,
			"owa15",
			"appcache",
			"diag",
			"layout",
			"offline",
			"prefetch",
			"server",
			"sync",
			"tracelevel",
			"viewmodel",
			"wa"
		};

		private AnchorMailbox originalAnchorMailboxFromExplicitLogon;
	}
}
