using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MapiHttp;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RpcHelper
	{
		public static bool IsRealServerName(string serverName)
		{
			return !serverName.Contains("@");
		}

		public static RpcBindingInfo BuildRpcProxyOnlyBindingInfo(IPropertyBag propertyBag)
		{
			return RpcHelper.BuildRpcBindingInfo(propertyBag, true, false, 6001);
		}

		public static RpcBindingInfo BuildCompleteBindingInfo(IPropertyBag propertyBag, int legacyEndpoint)
		{
			return RpcHelper.BuildRpcBindingInfo(propertyBag, propertyBag.IsPropertyFound(ContextPropertySchema.RpcProxyServer), true, legacyEndpoint);
		}

		public static MapiHttpBindingInfo BuildCompleteMapiHttpBindingInfo(IPropertyBag propertyBag)
		{
			return RpcHelper.BuildMapiHttpBindingInfo(propertyBag);
		}

		public static TaskResult FigureOutErrorInformation(IPropertyBag outputPropertyBag, VerifyRpcProxyResult verifyRpcProxyResult)
		{
			if (!verifyRpcProxyResult.IsSuccessful)
			{
				outputPropertyBag.Set(ContextPropertySchema.Exception, verifyRpcProxyResult.Exception);
				outputPropertyBag.Set(ContextPropertySchema.ErrorDetails, string.Format("Status: {0}\nHttpStatusCode: {1}\nHttpStatusDescription: {2}\nProcessedBody: {3}", new object[]
				{
					(verifyRpcProxyResult.Exception != null) ? verifyRpcProxyResult.Exception.Status.ToString() : "OK",
					verifyRpcProxyResult.ResponseStatusCode,
					verifyRpcProxyResult.ResponseStatusDescription,
					verifyRpcProxyResult.ResponseBody
				}));
				return TaskResult.Failed;
			}
			return TaskResult.Success;
		}

		public static bool DetectShouldUseSsl(RpcProxyPort rpcProxyPort)
		{
			int num = (int)rpcProxyPort;
			return !num.ToString().Contains("8");
		}

		private static RpcBindingInfo BuildRpcBindingInfo(IPropertyBag propertyBag, bool useHttp, bool addRpcAuthentication, int legacyEndpoint)
		{
			RpcBindingInfo rpcBindingInfo = new RpcBindingInfo();
			rpcBindingInfo.RpcServer = propertyBag.Get(ContextPropertySchema.RpcServer);
			rpcBindingInfo.Timeout = propertyBag.Get(ContextPropertySchema.Timeout);
			rpcBindingInfo.UseUniqueBinding = true;
			string authType;
			if (useHttp)
			{
				RpcHelper.AddRpcHttpInfo(rpcBindingInfo, propertyBag, legacyEndpoint);
				authType = RpcHelper.HttpAuthenticationSchemeMapping.Get(propertyBag.Get(ContextPropertySchema.RpcProxyAuthenticationType));
			}
			else
			{
				rpcBindingInfo.UseTcp();
				authType = RpcHelper.AuthenticationServiceMapping.Get(propertyBag.Get(ContextPropertySchema.RpcAuthenticationType));
			}
			ICredentials credentials = propertyBag.Get(ContextPropertySchema.Credentials);
			rpcBindingInfo.Credential = ((credentials != null) ? credentials.GetCredential(rpcBindingInfo.Uri, authType) : null);
			if (addRpcAuthentication)
			{
				RpcHelper.AddRpcAuthenticationInfo(rpcBindingInfo, propertyBag.Get(ContextPropertySchema.RpcAuthenticationType));
			}
			int num = Convert.ToInt32(rpcBindingInfo.Timeout.TotalSeconds);
			int num2 = (num >= 10) ? (num - 10) : num;
			int num3 = num2 / 2;
			rpcBindingInfo.RpcHttpHeaders.Add(WellKnownHeader.FrontEndToBackEndTimeout, num3.ToString());
			return rpcBindingInfo;
		}

		private static MapiHttpBindingInfo BuildMapiHttpBindingInfo(IPropertyBag propertyBag)
		{
			string text = propertyBag.Get(ContextPropertySchema.MapiHttpPersonalizedServerName);
			string serverFqdn = propertyBag.Get(ContextPropertySchema.MapiHttpServer);
			bool ignoreCertificateErrors = propertyBag.Get(ContextPropertySchema.IgnoreInvalidServerCertificateSubject);
			RpcProxyPort rpcProxyPort = propertyBag.Get(ContextPropertySchema.RpcProxyPort);
			bool useSsl = RpcHelper.DetectShouldUseSsl(rpcProxyPort);
			MapiHttpBindingInfo mapiHttpBindingInfo = new MapiHttpBindingInfo(serverFqdn, (int)rpcProxyPort, useSsl, propertyBag.Get(ContextPropertySchema.Credentials), ignoreCertificateErrors, RpcHelper.LooksLikePersonalizedServerName(text) ? text : null);
			WebHeaderCollection webHeaderCollection = propertyBag.Get(ContextPropertySchema.AdditionalHttpHeaders);
			if (webHeaderCollection != null && webHeaderCollection.Count > 0)
			{
				mapiHttpBindingInfo.AdditionalHttpHeaders = webHeaderCollection;
			}
			int val = Convert.ToInt32(propertyBag.Get(ContextPropertySchema.Timeout).TotalSeconds);
			int val2 = Convert.ToInt32(Constants.HttpRequestTimeout.TotalSeconds);
			int num = Math.Min(val, val2);
			int num2 = (num >= 10) ? (num - 10) : num;
			mapiHttpBindingInfo.AdditionalHttpHeaders.Add(WellKnownHeader.FrontEndToBackEndTimeout, num2.ToString());
			return mapiHttpBindingInfo;
		}

		private static bool LooksLikePersonalizedServerName(string mailboxId)
		{
			int num = mailboxId.IndexOf('@');
			return num > 0 && num != mailboxId.Length - 1;
		}

		private static void AddRpcHttpInfo(RpcBindingInfo bindingInfo, IPropertyBag propertyBag, int legacyEndpoint)
		{
			bindingInfo.UseRpcProxy((propertyBag.Get(ContextPropertySchema.RpcEndpoint) == OutlookEndpointSelection.Consolidated) ? 6001 : legacyEndpoint, propertyBag.Get(ContextPropertySchema.RpcProxyServer), propertyBag.Get(ContextPropertySchema.RpcProxyPort));
			bindingInfo.RpcProxyAuthentication = propertyBag.Get(ContextPropertySchema.RpcProxyAuthenticationType);
			string text = propertyBag.Get(ContextPropertySchema.OutlookSessionCookieValue);
			if (!string.IsNullOrEmpty(text))
			{
				string value = "\"" + text + " Client=ACTIVEMONITORING" + "\"";
				bindingInfo.RpcHttpCookies.Add(new Cookie("OutlookSession", value));
			}
			bindingInfo.RpcHttpHeaders.Add(propertyBag.Get(ContextPropertySchema.AdditionalHttpHeaders));
			bindingInfo.WebProxyServer = propertyBag.Get(ContextPropertySchema.WebProxyServer);
			bindingInfo.UseSsl = RpcHelper.DetectShouldUseSsl(bindingInfo.RpcProxyPort);
			bindingInfo.IgnoreInvalidRpcProxyServerCertificateSubject = propertyBag.Get(ContextPropertySchema.IgnoreInvalidServerCertificateSubject);
		}

		private static void AddRpcAuthenticationInfo(RpcBindingInfo bindingInfo, AuthenticationService rpcAuthenticationType)
		{
			bindingInfo.RpcAuthentication = rpcAuthenticationType;
			if (!RpcHelper.IsRealServerName(bindingInfo.RpcServer) && bindingInfo.RpcAuthentication != AuthenticationService.None && bindingInfo.RpcAuthentication != AuthenticationService.Ntlm && bindingInfo.RpcAuthentication != AuthenticationService.Negotiate)
			{
				throw new WrongPropertyValueCombinationException(Strings.WrongAuthForPersonalizedServer);
			}
			bindingInfo.UseRpcEncryption = (bindingInfo.RpcAuthentication != AuthenticationService.None);
		}

		public static readonly ContextProperty[] DependenciesOfBuildRpcProxyOnlyBindingInfo = new ContextProperty[]
		{
			ContextPropertySchema.Credentials.GetOnly(),
			ContextPropertySchema.RpcServer.GetOnly(),
			ContextPropertySchema.RpcEndpoint.GetOnly(),
			ContextPropertySchema.AdditionalHttpHeaders.GetOnly(),
			ContextPropertySchema.RpcProxyServer.GetOnly(),
			ContextPropertySchema.RpcProxyPort.GetOnly(),
			ContextPropertySchema.RpcProxyAuthenticationType.GetOnly(),
			ContextPropertySchema.IgnoreInvalidServerCertificateSubject.GetOnly(),
			ContextPropertySchema.OutlookSessionCookieValue.GetOnly(),
			ContextPropertySchema.WebProxyServer.GetOnly(),
			ContextPropertySchema.Timeout.GetOnly()
		};

		public static readonly ContextProperty[] DependenciesOfBuildCompleteBindingInfo = RpcHelper.DependenciesOfBuildRpcProxyOnlyBindingInfo.Concat(new ContextProperty[]
		{
			ContextPropertySchema.RpcAuthenticationType.GetOnly()
		});

		public static readonly ContextProperty[] DependenciesOfBuildMapiHttpBindingInfo = new ContextProperty[]
		{
			ContextPropertySchema.Credentials.GetOnly(),
			ContextPropertySchema.MapiHttpPersonalizedServerName.GetOnly(),
			ContextPropertySchema.MapiHttpServer.GetOnly(),
			ContextPropertySchema.AdditionalHttpHeaders.GetOnly(),
			ContextPropertySchema.RpcProxyPort.GetOnly(),
			ContextPropertySchema.IgnoreInvalidServerCertificateSubject.GetOnly(),
			ContextPropertySchema.Timeout.GetOnly()
		};

		public static readonly RpcHelper.Mapping<AuthenticationService, string> AuthenticationServiceMapping = new RpcHelper.Mapping<AuthenticationService, string>(null, StringComparer.OrdinalIgnoreCase)
		{
			{
				AuthenticationService.None,
				"Anonymous"
			},
			{
				AuthenticationService.None,
				string.Empty
			},
			{
				AuthenticationService.None,
				null
			},
			{
				AuthenticationService.Negotiate,
				"Negotiate"
			},
			{
				AuthenticationService.Kerberos,
				"Kerberos"
			},
			{
				AuthenticationService.Ntlm,
				"Ntlm"
			}
		};

		public static readonly RpcHelper.Mapping<HttpAuthenticationScheme, string> HttpAuthenticationSchemeMapping = new RpcHelper.Mapping<HttpAuthenticationScheme, string>(null, StringComparer.OrdinalIgnoreCase)
		{
			{
				HttpAuthenticationScheme.Basic,
				"Basic"
			},
			{
				HttpAuthenticationScheme.Negotiate,
				"Negotiate"
			},
			{
				HttpAuthenticationScheme.Ntlm,
				"Ntlm"
			}
		};

		public class Mapping<X, Y> : IEnumerable<KeyValuePair<X, Y>>, IEnumerable
		{
			public Mapping(IEqualityComparer<X> comparerX = null, IEqualityComparer<Y> comparerY = null)
			{
				this.comparerX = (comparerX ?? EqualityComparer<X>.Default);
				this.comparerY = (comparerY ?? EqualityComparer<Y>.Default);
			}

			public void Add(X x, Y y)
			{
				this.inner.Add(new KeyValuePair<X, Y>(x, y));
			}

			public X Get(Y y)
			{
				foreach (KeyValuePair<X, Y> keyValuePair in this.inner)
				{
					if (this.comparerY.Equals(keyValuePair.Value, y))
					{
						return keyValuePair.Key;
					}
				}
				throw RpcHelper.Mapping<X, Y>.BadValueException<Y>(y);
			}

			public Y Get(X x)
			{
				foreach (KeyValuePair<X, Y> keyValuePair in this.inner)
				{
					if (this.comparerX.Equals(keyValuePair.Key, x))
					{
						return keyValuePair.Value;
					}
				}
				throw RpcHelper.Mapping<X, Y>.BadValueException<X>(x);
			}

			public IEnumerator<KeyValuePair<X, Y>> GetEnumerator()
			{
				return this.inner.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private static Exception BadValueException<T>(T value)
			{
				return new ArgumentException(string.Format("{0} {1} is not supported", typeof(T).Name, value));
			}

			private readonly List<KeyValuePair<X, Y>> inner = new List<KeyValuePair<X, Y>>();

			private readonly IEqualityComparer<X> comparerX;

			private readonly IEqualityComparer<Y> comparerY;
		}
	}
}
