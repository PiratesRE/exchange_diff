using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPeopleConnectApplicationConfig
	{
		string AppId { get; }

		string AppSecretEncrypted { get; }

		Func<string, string> DecryptAppSecret { get; }

		string AppSecretClearText { get; }

		string AuthorizationEndpoint { get; }

		string GraphTokenEndpoint { get; }

		string GraphApiEndpoint { get; }

		TimeSpan WebRequestTimeout { get; }

		string RequestTokenEndpoint { get; }

		string AccessTokenEndpoint { get; }

		string ProfileEndpoint { get; }

		string ConnectionsEndpoint { get; }

		string RemoveAppEndpoint { get; }

		string ConsentRedirectEndpoint { get; }

		string WebProxyUri { get; }

		bool SkipContactUpload { get; }

		bool ContinueOnContactUploadFailure { get; }

		bool WaitForContactUploadCommit { get; }

		bool NotifyOnEachContactUpload { get; }

		int MaximumContactsToUpload { get; }

		DateTime ReadTimeUtc { get; }

		IPeopleConnectApplicationConfig OverrideWith(IPeopleConnectApplicationConfig other);
	}
}
