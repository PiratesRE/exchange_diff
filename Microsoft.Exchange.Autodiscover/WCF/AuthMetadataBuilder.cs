using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal class AuthMetadataBuilder
	{
		private AuthMetadataBuilder()
		{
		}

		public static AuthMetadataBuilder Singleton
		{
			get
			{
				if (AuthMetadataBuilder.builder == null)
				{
					lock (AuthMetadataBuilder.staticLock)
					{
						if (AuthMetadataBuilder.builder == null)
						{
							AuthMetadataBuilder.builder = new AuthMetadataBuilder();
						}
					}
				}
				return AuthMetadataBuilder.builder;
			}
		}

		private Trace Tracer
		{
			get
			{
				return ExTraceGlobals.AuthMetadataTracer;
			}
		}

		public string Build(Uri requestUrl)
		{
			LocalizedException ex = null;
			string result = null;
			try
			{
				result = this.InternalBuild(requestUrl);
			}
			catch (ADOperationException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataValidationException ex4)
			{
				ex = ex4;
			}
			catch (DataSourceOperationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				this.Tracer.TraceError<LocalizedException>((long)this.GetHashCode(), "[AuthMetadataBuilder.Build] Active Directory operation error occured. Exception: {0}", ex);
				throw new AuthMetadataInternalException(ex.LocalizedString, ex);
			}
			return result;
		}

		private string InternalBuild(Uri requestUrl)
		{
			string result = this.cachedContent;
			if (this.NeedRefreshMetadataDocument() || this.NeedRefreshCacheContent(requestUrl))
			{
				lock (this.instanceLock)
				{
					bool flag2 = false;
					if (this.NeedRefreshMetadataDocument())
					{
						this.Tracer.TraceDebug((long)this.GetHashCode(), "[AuthMetadataBuilder.InternalBuild] Refreshing document.");
						this.lastJsonMetadataDocument = this.BuildJsonMetadataDocument();
						this.lastRefreshTime = ExDateTime.UtcNow;
						flag2 = true;
					}
					if (flag2 || this.NeedRefreshCacheContent(requestUrl))
					{
						this.Tracer.TraceDebug((long)this.GetHashCode(), "[AuthMetadataBuilder.InternalBuild] Refreshing cached content.");
						this.lastJsonMetadataDocument.endpoints[0].location = requestUrl.ToString();
						this.lastRequestUriAuthority = requestUrl.Authority;
						this.cachedContent = this.BuildDocument(this.lastJsonMetadataDocument);
					}
					result = this.cachedContent;
				}
			}
			return result;
		}

		private bool NeedRefreshMetadataDocument()
		{
			return ExDateTime.UtcNow - AuthMetadataBuilder.RefreshInterval > this.lastRefreshTime;
		}

		private bool NeedRefreshCacheContent(Uri requestUrl)
		{
			return !string.Equals(requestUrl.Authority, this.lastRequestUriAuthority, StringComparison.OrdinalIgnoreCase);
		}

		private JsonMetadataDocument BuildJsonMetadataDocument()
		{
			string text = null;
			string serviceName = OAuthConfigHelper.GetServiceName();
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				text = OAuthConfigHelper.GetOrganizationRealm(OrganizationId.ForestWideOrgId);
			}
			else
			{
				text = "*";
			}
			X509Certificate2 currentSigningKey = OAuthConfigHelper.GetCurrentSigningKey();
			X509Certificate2 x509Certificate = null;
			try
			{
				x509Certificate = OAuthConfigHelper.GetPreviousSigningKey();
			}
			catch (InvalidAuthConfigurationException arg)
			{
				this.Tracer.TraceDebug<InvalidAuthConfigurationException>((long)this.GetHashCode(), "[AuthMetadataBuilder.BuildJsonMetadataDocument] failed to get previous signing key with exception: {0}", arg);
			}
			JsonMetadataDocument jsonMetadataDocument = new JsonMetadataDocument();
			jsonMetadataDocument.id = string.Format("_{0}", Guid.NewGuid().ToString("d"));
			jsonMetadataDocument.version = AuthMetadataBuilder.Version;
			jsonMetadataDocument.name = AuthMetadataBuilder.ServiceShortName;
			jsonMetadataDocument.realm = text;
			jsonMetadataDocument.serviceName = serviceName;
			jsonMetadataDocument.issuer = string.Format("{0}@{1}", serviceName, text);
			jsonMetadataDocument.allowedAudiences = new string[]
			{
				jsonMetadataDocument.issuer
			};
			List<JsonKey> list = new List<JsonKey>();
			foreach (X509Certificate2 x509Certificate2 in new X509Certificate2[]
			{
				currentSigningKey,
				x509Certificate
			})
			{
				if (x509Certificate2 != null && x509Certificate2.NotAfter > DateTime.UtcNow)
				{
					JsonKey item = new JsonKey
					{
						usage = AuthMetadataConstants.KeyUsage,
						keyinfo = new JsonKeyInfo
						{
							x5t = OAuthCommon.Base64UrlEncoder.EncodeBytes(x509Certificate2.GetCertHash())
						},
						keyvalue = new JsonKeyValue
						{
							type = AuthMetadataConstants.SigningKeyType,
							value = Convert.ToBase64String(x509Certificate2.GetRawCertData())
						}
					};
					list.Add(item);
				}
			}
			jsonMetadataDocument.keys = list.ToArray();
			jsonMetadataDocument.endpoints = new JsonEndpoint[]
			{
				new JsonEndpoint
				{
					location = null,
					protocol = AuthMetadataConstants.Protocol,
					usage = AuthMetadataConstants.MetadataEndpointUsage
				}
			};
			return jsonMetadataDocument;
		}

		private string BuildDocument(JsonMetadataDocument document)
		{
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			string text = javaScriptSerializer.Serialize(document);
			this.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[AuthMetadataBuilder.BuildDocument] Auth metedata document content: {0}", text);
			return text;
		}

		public static readonly string Version = "1.0";

		public static readonly string ServiceShortName = "Exchange";

		private static readonly TimeSpan RefreshInterval = TimeSpan.FromHours(12.0);

		private static object staticLock = new object();

		private static AuthMetadataBuilder builder = null;

		private object instanceLock = new object();

		private ExDateTime lastRefreshTime = ExDateTime.MinValue;

		private JsonMetadataDocument lastJsonMetadataDocument;

		private string lastRequestUriAuthority;

		private string cachedContent;
	}
}
