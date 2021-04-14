using System;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class MetabasePropertyNames
	{
		public const string AccessFlags = "AccessFlags";

		public const string AccessSSLFlags = "AccessSSLFlags";

		public const string AppFriendlyName = "AppFriendlyName";

		public const string AppIsolated = "AppIsolated";

		public const string AppPoolId = "AppPoolId";

		public const string AppPoolIdentityType = "AppPoolIdentityType";

		public const string AppPoolQueueLength = "AppPoolQueueLength";

		public const string ManagedPipelineMode = "managedPipelineMode";

		public const string PeriodicRestartTime = "PeriodicRestartTime";

		public const string IdleTimeout = "IdleTimeout";

		public const string AppRoot = "AppRoot";

		public const string AuthFlags = "AuthFlags";

		public const string CacheControlCustom = "CacheControlCustom";

		public const string DirBrowseFlags = "DirBrowseFlags";

		public const string DefaultDoc = "DefaultDoc";

		public const string DefaultDomain = "DefaultDomain";

		public const string DoDynamicCompression = "DoDynamicCompression";

		public const string HttpErrors = "HttpErrors";

		public const string HttpExpires = "HttpExpires";

		public const string HttpRedirect = "HttpRedirect";

		public const string LogonMethod = "LogonMethod";

		public const string KeyType = "KeyType";

		public const string WindowsAuthenticationProviders = "NTAuthenticationProviders";

		public const string Path = "Path";

		public const string ScriptMaps = "ScriptMaps";

		public const string ServerAutoStart = "ServerAutoStart";

		public const string ServerBindings = "ServerBindings";

		public const string SecureBindings = "SecureBindings";

		public const string ServerComment = "ServerComment";

		public const string ServerState = "ServerState";

		public const string SslCertHash = "SSLCertHash";

		public const string WebSvcExtRestrictionList = "WebSvcExtRestrictionList";

		public const string MimeTypes = "MimeMap";

		public const string ASPScriptTimeout = "ASPScriptTimeout";

		public static class Filter
		{
			public const string Flags = "FilterFlags";

			public const string State = "FilterState";

			public const string Enabled = "FilterEnabled";

			public const string Path = "FilterPath";

			public const string LoadOrder = "FilterLoadOrder";
		}

		public static class WindowsAuthenticationProvider
		{
			public const string Ntlm = "NTLM";

			public const string Negotiate = "Negotiate";

			public const string MsoIdSsp = "Negotiate:MSOIDSSP";
		}
	}
}
