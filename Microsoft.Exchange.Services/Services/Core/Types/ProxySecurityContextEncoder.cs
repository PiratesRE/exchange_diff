using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal static class ProxySecurityContextEncoder
	{
		public static ProxyHeaderValue GetHeaderValueForCAS(ClientSecurityContext clientSecurityContext, WebServicesInfo webService, string smtpAddressOfCaller = null)
		{
			if (!ProxySuggesterSidCache.Singleton.Contains(new SuggesterSidCompositeKey(clientSecurityContext.UserSid, webService.ServerFullyQualifiedDomainName)))
			{
				SerializedSecurityAccessToken serializedSecurityAccessToken = new SerializedSecurityAccessToken();
				clientSecurityContext.SetSecurityAccessToken(serializedSecurityAccessToken);
				int num = (serializedSecurityAccessToken.GroupSids == null) ? 0 : serializedSecurityAccessToken.GroupSids.Length;
				int num2 = (serializedSecurityAccessToken.RestrictedGroupSids == null) ? 0 : serializedSecurityAccessToken.RestrictedGroupSids.Length;
				if (num + num2 > ProxySecurityContextEncoder.GroupSidLimit)
				{
					ProxyEventLogHelper.LogExceededGroupSidLimit(clientSecurityContext.UserSid, ProxySecurityContextEncoder.GroupSidLimit);
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<int>(0L, "[ProxySecurityContextEncoder::GetHeaderValueForCAS] Token contained more than {0} group sids.", ProxySecurityContextEncoder.GroupSidLimit);
					throw new ProxyGroupSidLimitExceededException();
				}
				if (!string.IsNullOrEmpty(smtpAddressOfCaller))
				{
					serializedSecurityAccessToken.SmtpAddress = smtpAddressOfCaller;
				}
				byte[] bytes = serializedSecurityAccessToken.GetBytes();
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
					{
						gzipStream.Write(bytes, 0, bytes.Length);
					}
					memoryStream.Flush();
					return new ProxyHeaderValue(ProxyHeaderType.FullToken, memoryStream.ToArray());
				}
			}
			byte[] array = new byte[clientSecurityContext.UserSid.BinaryLength];
			clientSecurityContext.UserSid.GetBinaryForm(array, 0);
			return new ProxyHeaderValue(ProxyHeaderType.SuggesterSid, array);
		}

		private static readonly int GroupSidLimit = 3000;
	}
}
