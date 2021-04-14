using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.Security.Authorization
{
	internal static class CertificateValidationManager
	{
		public static void RegisterCallback(string componentId, RemoteCertificateValidationCallback callback)
		{
			CertificateValidationManager.RegisterCallback(componentId, callback, false);
		}

		public static void RegisterCallback(string componentId, RemoteCertificateValidationCallback callback, bool forceCallback)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentException("componentId");
			}
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "Entering CertificateValidationManager.RegisterCallback");
			lock (CertificateValidationManager.callbackRegisterLock)
			{
				if (CertificateValidationManager.callbackTable.ContainsKey(componentId))
				{
					ExTraceGlobals.CertificateValidationTracer.TraceDebug<string>(0L, "Callback already registered.  ComponentId={0}.", componentId);
				}
				else
				{
					Dictionary<string, CertificateValidationManager.CallbackPair> dictionary = new Dictionary<string, CertificateValidationManager.CallbackPair>(CertificateValidationManager.callbackTable);
					dictionary[componentId] = new CertificateValidationManager.CallbackPair(callback, forceCallback);
					CertificateValidationManager.callbackTable = dictionary;
					if (CertificateValidationManager.callbackTable.Count == 1)
					{
						RemoteCertificateValidationCallback remoteCertificateValidationCallback = new RemoteCertificateValidationCallback(CertificateValidationManager.MainCertificateValidationCallback);
						ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Remove(ServicePointManager.ServerCertificateValidationCallback, remoteCertificateValidationCallback);
						ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, remoteCertificateValidationCallback);
					}
				}
			}
		}

		public static void SetComponentId(HttpWebRequest request, string componentId)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentException("componentId");
			}
			request.Headers[CertificateValidationManager.ComponentIdHeaderName] = componentId;
		}

		public static void SetComponentId(WebHeaderCollection headers, string componentId)
		{
			if (headers == null)
			{
				throw new ArgumentNullException("headers");
			}
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentException("componentId");
			}
			headers[CertificateValidationManager.ComponentIdHeaderName] = componentId;
		}

		public static string GenerateComponentIdQueryString(string componentId)
		{
			return string.Format("?{0}={1}", CertificateValidationManager.ComponentIdHeaderName, componentId);
		}

		public static bool MainCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "Entering CertificateValidation.CommonCertificateValidationCallback");
			HttpWebRequest httpWebRequest = sender as HttpWebRequest;
			string componentId;
			if (httpWebRequest != null)
			{
				ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "sender is HttpWebRequest");
				componentId = CertificateValidationManager.GetComponentId(httpWebRequest);
			}
			else
			{
				CertificateValidationManager.IComponent component = sender as CertificateValidationManager.IComponent;
				if (component != null)
				{
					ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "sender is IComponent");
					componentId = component.GetComponentId();
				}
				else
				{
					if (!CertificateValidationManager.IsSslError(sslPolicyErrors))
					{
						return true;
					}
					string text = sender.GetType().ToString();
					if (string.IsNullOrEmpty(text))
					{
						text = "<Unknown>";
					}
					ExTraceGlobals.CertificateValidationTracer.TraceDebug<string>(0L, "Unrecognized type passed as sender. Type={0}", text);
					return CertificateValidationManager.DefaultValidateSslPolicyErrors(sslPolicyErrors);
				}
			}
			if (string.IsNullOrEmpty(componentId))
			{
				ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "Component id is not set or empty");
				return CertificateValidationManager.DefaultValidateSslPolicyErrors(sslPolicyErrors);
			}
			ExTraceGlobals.CertificateValidationTracer.TraceDebug<string>(0L, "Looking up callback for component \"{0}\"...", componentId);
			CertificateValidationManager.CallbackPair callbackPair = null;
			if (!CertificateValidationManager.callbackTable.TryGetValue(componentId, out callbackPair))
			{
				return CertificateValidationManager.DefaultValidateSslPolicyErrors(sslPolicyErrors);
			}
			if (callbackPair == null)
			{
				ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "Callback not found, could be a missing call to RegisterCallback");
				throw new InvalidOperationException(string.Format("Couldn not find callback for component with id = \"{0}\"", componentId));
			}
			if (!CertificateValidationManager.IsSslError(sslPolicyErrors) && !callbackPair.ForceCallback)
			{
				return true;
			}
			ExTraceGlobals.CertificateValidationTracer.TraceDebug<string>(0L, "Executing callback...", componentId);
			return callbackPair.Callback(sender, certificate, chain, sslPolicyErrors);
		}

		private static bool IsSslError(SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				ExTraceGlobals.CertificateValidationTracer.TraceDebug(0L, "No SSL policy errors found, exiting...");
				return false;
			}
			return true;
		}

		private static bool DefaultValidateSslPolicyErrors(SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None;
		}

		private static string GetComponentId(HttpWebRequest request)
		{
			string text = request.Headers[CertificateValidationManager.ComponentIdHeaderName];
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (request.RequestUri == null || request.RequestUri.Query == null)
			{
				return text;
			}
			Match match = CertificateValidationManager.ComponentIdRegEx.Match(request.RequestUri.Query);
			if (!match.Success)
			{
				return text;
			}
			return match.Value.Substring(CertificateValidationManager.ComponentIdQueryString.Length);
		}

		private static Regex ComponentIdRegEx
		{
			get
			{
				if (CertificateValidationManager.componentIdRegEx == null)
				{
					lock (CertificateValidationManager.lockInitComponentIdRegEx)
					{
						if (CertificateValidationManager.componentIdRegEx == null)
						{
							CertificateValidationManager.componentIdRegEx = new Regex(CertificateValidationManager.ComponentIdQueryStringPattern, RegexOptions.Compiled);
						}
					}
				}
				return CertificateValidationManager.componentIdRegEx;
			}
		}

		public static readonly string ComponentIdHeaderName = "X-ExCompId";

		public static readonly string ComponentIdQueryString = string.Format("?{0}=", CertificateValidationManager.ComponentIdHeaderName);

		private static readonly string ComponentIdQueryStringPattern = string.Format("\\{0}[\\w]+", CertificateValidationManager.ComponentIdQueryString);

		private static Regex componentIdRegEx = null;

		private static readonly object lockInitComponentIdRegEx = new object();

		private static object callbackRegisterLock = new object();

		private static Dictionary<string, CertificateValidationManager.CallbackPair> callbackTable = new Dictionary<string, CertificateValidationManager.CallbackPair>();

		public interface IComponent
		{
			string GetComponentId();
		}

		private class CallbackPair
		{
			public RemoteCertificateValidationCallback Callback { get; private set; }

			public bool ForceCallback { get; private set; }

			public CallbackPair(RemoteCertificateValidationCallback callback, bool forceCallback)
			{
				this.Callback = callback;
				this.ForceCallback = forceCallback;
			}
		}
	}
}
