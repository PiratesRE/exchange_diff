using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInRegistryReader
	{
		private LinkedInRegistryReader()
		{
			this.AppId = string.Empty;
			this.AppSecret = string.Empty;
			this.AccessTokenEndpoint = string.Empty;
			this.ConnectionsEndpoint = string.Empty;
			this.ProfileEndpoint = string.Empty;
			this.RemoveAppEndpoint = string.Empty;
			this.RequestTokenEndpoint = string.Empty;
			this.WebRequestTimeout = TimeSpan.Zero;
			this.WebProxyUri = string.Empty;
		}

		public static LinkedInRegistryReader Read()
		{
			LinkedInRegistryReader result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PeopleConnect\\LinkedIn"))
				{
					if (registryKey == null)
					{
						result = new LinkedInRegistryReader();
					}
					else
					{
						result = new LinkedInRegistryReader
						{
							AppId = (string)registryKey.GetValue("AppId", string.Empty),
							AppSecret = (string)registryKey.GetValue("AppSecret", string.Empty),
							RequestTokenEndpoint = (string)registryKey.GetValue("RequestTokenEndpoint", string.Empty),
							AccessTokenEndpoint = (string)registryKey.GetValue("AccessTokenEndpoint", string.Empty),
							ConnectionsEndpoint = (string)registryKey.GetValue("ConnectionsEndpoint", string.Empty),
							ProfileEndpoint = (string)registryKey.GetValue("ProfileEndpoint", string.Empty),
							RemoveAppEndpoint = (string)registryKey.GetValue("RemoveAppEndpoint", string.Empty),
							ConsentRedirectEndpoint = (string)registryKey.GetValue("ConsentRedirectEndpoint", string.Empty),
							WebRequestTimeout = TimeSpan.FromSeconds((double)((int)registryKey.GetValue("WebRequestTimeoutSeconds", 0))),
							WebProxyUri = (string)registryKey.GetValue("WebProxyUri", string.Empty)
						};
					}
				}
			}
			catch (SecurityException e)
			{
				result = LinkedInRegistryReader.TraceErrorAndReturnEmptyConfiguration(e);
			}
			catch (IOException e2)
			{
				result = LinkedInRegistryReader.TraceErrorAndReturnEmptyConfiguration(e2);
			}
			catch (UnauthorizedAccessException e3)
			{
				result = LinkedInRegistryReader.TraceErrorAndReturnEmptyConfiguration(e3);
			}
			return result;
		}

		public string AppId { get; private set; }

		public string AppSecret { get; private set; }

		public string RequestTokenEndpoint { get; private set; }

		public string AccessTokenEndpoint { get; private set; }

		public string ConnectionsEndpoint { get; private set; }

		public string ProfileEndpoint { get; private set; }

		public string RemoveAppEndpoint { get; private set; }

		public string ConsentRedirectEndpoint { get; private set; }

		public TimeSpan WebRequestTimeout { get; private set; }

		public string WebProxyUri { get; private set; }

		private static LinkedInRegistryReader TraceErrorAndReturnEmptyConfiguration(Exception e)
		{
			LinkedInRegistryReader.Tracer.TraceError<Exception>(0L, "LinkedInRegistryReader.Read: caught exception {0}", e);
			return new LinkedInRegistryReader();
		}

		private const string AppIdValueKey = "AppId";

		private const string AppSecretKey = "AppSecret";

		private const string RequestTokenEndpointKey = "RequestTokenEndpoint";

		private const string AccessTokenEndpointKey = "AccessTokenEndpoint";

		private const string ConnectionsEndpointKey = "ConnectionsEndpoint";

		private const string ProfileEndpointKey = "ProfileEndpoint";

		private const string RemoveAppEndpointKey = "RemoveAppEndpoint";

		private const string ConsentRedirectEndpointValueName = "ConsentRedirectEndpoint";

		private const string WebRequestTimeoutSecondsKey = "WebRequestTimeoutSeconds";

		private const string WebProxyUriKey = "WebProxyUri";

		private const string LinkedInKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PeopleConnect\\LinkedIn";

		private static readonly Trace Tracer = ExTraceGlobals.LinkedInTracer;
	}
}
