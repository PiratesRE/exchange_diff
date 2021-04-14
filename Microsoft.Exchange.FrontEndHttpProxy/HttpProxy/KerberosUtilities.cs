using System;
using System.Text;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class KerberosUtilities
	{
		internal static string GenerateKerberosAuthHeader(string host, int traceContext, ref AuthenticationContext authenticationContext, ref string kerberosChallenge)
		{
			byte[] inputBuffer = null;
			byte[] bytes = null;
			if (kerberosChallenge != null)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)traceContext, "[KerberosUtilities::GenerateKerberosAuthHeader]: Context {0}; Reusing existing AuthenticationContext to respond to challenge {1}", traceContext, kerberosChallenge);
				inputBuffer = Encoding.ASCII.GetBytes(kerberosChallenge);
			}
			else
			{
				if (authenticationContext != null)
				{
					authenticationContext.Dispose();
					authenticationContext = null;
				}
				authenticationContext = new AuthenticationContext();
				string text = Constants.SpnPrefixForHttp + host;
				ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)traceContext, "[KerberosUtilities::GenerateKerberosAuthHeader]: Context {0}; SPN {1}", traceContext, text);
				authenticationContext.InitializeForOutboundNegotiate(AuthenticationMechanism.Kerberos, text, null, null);
			}
			SecurityStatus securityStatus = authenticationContext.NegotiateSecurityContext(inputBuffer, out bytes);
			if (securityStatus != SecurityStatus.OK && securityStatus != SecurityStatus.ContinueNeeded)
			{
				ExTraceGlobals.VerboseTracer.TraceError<int, SecurityStatus>((long)traceContext, "[KerberosUtilities::GenerateKerberosAuthHeader]: Context {0}; NegotiateSecurityContext failed with {1}", traceContext, securityStatus);
				throw new HttpException(500, string.Format("NegotiateSecurityContext failed with for host '{0}' with status '{1}'", host, securityStatus));
			}
			kerberosChallenge = null;
			string @string = Encoding.ASCII.GetString(bytes);
			return Constants.PrefixForKerbAuthBlob + @string;
		}

		internal static bool TryFindKerberosChallenge(string wwwAuthenticateHeader, int traceContext, out string kerberosChallenge, out bool foundNegotiatePackageName)
		{
			kerberosChallenge = null;
			foundNegotiatePackageName = false;
			if (wwwAuthenticateHeader != null)
			{
				string[] array = wwwAuthenticateHeader.Split(new char[]
				{
					' ',
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2)
				{
					int i = 0;
					while (i < array.Length)
					{
						string a = array[i];
						if (string.Equals(a, Constants.NegotiatePackageValue, StringComparison.OrdinalIgnoreCase))
						{
							foundNegotiatePackageName = true;
							if (i == array.Length - 1)
							{
								return false;
							}
							kerberosChallenge = array[i + 1];
							ExTraceGlobals.VerboseTracer.TraceDebug<int, string>((long)traceContext, "[KerberosUtilities::TryFindKerberosChallenge]: Context {0}; kerberosChallenge {1}", traceContext, kerberosChallenge);
							return !string.Equals(kerberosChallenge, Constants.NtlmPackageValue, StringComparison.OrdinalIgnoreCase);
						}
						else
						{
							i++;
						}
					}
				}
			}
			ExTraceGlobals.VerboseTracer.TraceError<int>((long)traceContext, "[KerberosUtilities::TryFindKerberosChallenge]: Context {0}; TryFindKerberosChallenge did not find suitable WWW-Authenticate header in server response. Failing!", traceContext);
			return false;
		}
	}
}
