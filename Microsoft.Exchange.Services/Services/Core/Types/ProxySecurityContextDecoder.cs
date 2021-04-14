using System;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ProxySecurityContextDecoder
	{
		public static SerializedSecurityAccessToken Decode(ProxyHeaderValue proxyHeaderValue)
		{
			switch (proxyHeaderValue.ProxyHeaderType)
			{
			case ProxyHeaderType.SuggesterSid:
				break;
			case ProxyHeaderType.FullToken:
				try
				{
					proxyHeaderValue.ValidateSize();
				}
				catch (InvalidProxySecurityContextException exception)
				{
					throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
				}
				using (MemoryStream memoryStream = new MemoryStream(proxyHeaderValue.Value))
				{
					memoryStream.Position = 0L;
					using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
					{
						SerializedSecurityAccessToken serializedSecurityAccessToken = null;
						using (MemoryStream memoryStream2 = new MemoryStream())
						{
							byte[] buffer = new byte[1024];
							for (;;)
							{
								int num = gzipStream.Read(buffer, 0, 1024);
								if (num == 0)
								{
									break;
								}
								memoryStream2.Write(buffer, 0, num);
							}
							memoryStream2.Flush();
							serializedSecurityAccessToken = SerializedSecurityAccessToken.FromBytes(memoryStream2.GetBuffer());
						}
						ProxyTokenCache.Singleton.ForceAdd(serializedSecurityAccessToken.UserSid, serializedSecurityAccessToken);
						RequestDetailsLogger.Current.AppendGenericInfo("SecurityTokenType", string.Format("FullToken: {0}", serializedSecurityAccessToken.UserSid));
						return serializedSecurityAccessToken;
					}
				}
				break;
			default:
				RequestDetailsLogger.Current.AppendGenericInfo("SecurityToken", "Unknown");
				return null;
			}
			SecurityIdentifier securityIdentifier;
			try
			{
				securityIdentifier = new SecurityIdentifier(proxyHeaderValue.Value, 0);
			}
			catch (ArgumentException innerException)
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidProxySecurityContextException(innerException), FaultParty.Sender);
			}
			SerializedSecurityAccessToken serializedSecurityAccessToken2 = ProxyTokenCache.Singleton.Get(securityIdentifier.ToString());
			RequestDetailsLogger.Current.AppendGenericInfo("SecurityTokenType", string.Format("SuggesterSid: {0}", securityIdentifier.ToString()));
			if (serializedSecurityAccessToken2 == null)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[ProxySecurityContextDecoder::Decode] Received a suggester sid, but the full token is not in the cache.  Failing the request.");
				throw FaultExceptionUtilities.CreateFault(new ProxyTokenExpiredException(), FaultParty.Sender);
			}
			return serializedSecurityAccessToken2;
		}
	}
}
