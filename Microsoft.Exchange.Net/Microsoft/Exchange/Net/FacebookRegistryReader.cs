using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FacebookRegistryReader
	{
		private FacebookRegistryReader()
		{
			this.AppId = string.Empty;
			this.AppSecret = string.Empty;
			this.AuthorizationEndpoint = string.Empty;
			this.GraphTokenEndpoint = string.Empty;
			this.WebRequestTimeout = TimeSpan.Zero;
			this.SkipContactUpload = false;
			this.ContinueOnContactUploadFailure = true;
			this.WaitForContactUploadCommit = false;
			this.NotifyOnEachContactUpload = false;
			this.MaximumContactsToUpload = 1000;
		}

		public static FacebookRegistryReader Read()
		{
			FacebookRegistryReader result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(FacebookRegistryReader.FacebookKey))
				{
					if (registryKey == null)
					{
						result = new FacebookRegistryReader();
					}
					else
					{
						result = new FacebookRegistryReader
						{
							AppId = (string)registryKey.GetValue("AppId", string.Empty),
							AppSecret = (string)registryKey.GetValue("AppSecret", string.Empty),
							AuthorizationEndpoint = (string)registryKey.GetValue("AuthorizationEndpoint", string.Empty),
							ConsentRedirectEndpoint = (string)registryKey.GetValue("ConsentRedirectEndpoint", string.Empty),
							GraphTokenEndpoint = (string)registryKey.GetValue("GraphTokenEndpoint", string.Empty),
							GraphApiEndpoint = (string)registryKey.GetValue("GraphApiEndpoint", string.Empty),
							WebRequestTimeout = TimeSpan.FromSeconds((double)((int)registryKey.GetValue("WebRequestTimeoutSeconds", 0))),
							SkipContactUpload = Convert.ToBoolean(registryKey.GetValue("SkipContactUpload", false)),
							ContinueOnContactUploadFailure = Convert.ToBoolean(registryKey.GetValue("ContinueOnContactUploadFailure", true)),
							WaitForContactUploadCommit = Convert.ToBoolean(registryKey.GetValue("WaitForContactUploadCommit", false)),
							NotifyOnEachContactUpload = Convert.ToBoolean(registryKey.GetValue("NotifyOnEachContactUpload", false)),
							MaximumContactsToUpload = (int)registryKey.GetValue("MaximumContactsToUpload", 1000)
						};
					}
				}
			}
			catch (SecurityException e)
			{
				result = FacebookRegistryReader.TraceErrorAndReturnEmptyConfiguration(e);
			}
			catch (IOException e2)
			{
				result = FacebookRegistryReader.TraceErrorAndReturnEmptyConfiguration(e2);
			}
			catch (UnauthorizedAccessException e3)
			{
				result = FacebookRegistryReader.TraceErrorAndReturnEmptyConfiguration(e3);
			}
			return result;
		}

		public string AppId { get; private set; }

		public string AppSecret { get; private set; }

		public string AuthorizationEndpoint { get; private set; }

		public string GraphTokenEndpoint { get; private set; }

		public string GraphApiEndpoint { get; private set; }

		public string ConsentRedirectEndpoint { get; private set; }

		public TimeSpan WebRequestTimeout { get; private set; }

		public bool SkipContactUpload { get; private set; }

		public bool ContinueOnContactUploadFailure { get; private set; }

		public bool WaitForContactUploadCommit { get; private set; }

		public bool NotifyOnEachContactUpload { get; private set; }

		public int MaximumContactsToUpload { get; private set; }

		private static FacebookRegistryReader TraceErrorAndReturnEmptyConfiguration(Exception e)
		{
			FacebookRegistryReader.Tracer.TraceError<Exception>(0L, "FacebookRegistryReader.Read: caught exception {0}", e);
			return new FacebookRegistryReader();
		}

		private const string AppIdValueName = "AppId";

		private const string AppSecretValueName = "AppSecret";

		private const string AuthorizationEndpointName = "AuthorizationEndpoint";

		private const string GraphTokenEndpointValueName = "GraphTokenEndpoint";

		private const string GraphApiEndpointValueName = "GraphApiEndpoint";

		private const string ConsentRedirectEndpointValueName = "ConsentRedirectEndpoint";

		private const string WebRequestTimeoutSecondsValueName = "WebRequestTimeoutSeconds";

		private const string SkipContactUploadName = "SkipContactUpload";

		private const string ContinueOnContactUploadFailureName = "ContinueOnContactUploadFailure";

		private const string WaitForContactUploadCommitName = "WaitForContactUploadCommit";

		private const string NotifyOnEachContactUploadName = "NotifyOnEachContactUpload";

		private const string MaximumContactsToUploadName = "MaximumContactsToUpload";

		private const bool SkipContactUploadDefaultValue = false;

		private const bool ContinueOnContactUploadFailureDefaultValue = true;

		private const bool WaitForContactUploadCommitDefaultValue = false;

		private const bool NotifyOnEachContactUploadDefaultValue = false;

		private const int MaximumContactsToUploadDefaultValue = 1000;

		private static readonly Trace Tracer = ExTraceGlobals.FacebookTracer;

		private static readonly string FacebookKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\PeopleConnect\\Facebook";
	}
}
