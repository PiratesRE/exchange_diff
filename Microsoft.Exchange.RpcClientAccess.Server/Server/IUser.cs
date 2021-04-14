using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal interface IUser : WatsonHelper.IProvideWatsonReportData
	{
		string LegacyDistinguishedName { get; }

		string DisplayName { get; }

		OrganizationId OrganizationId { get; }

		SecurityIdentifier MasterAccountSid { get; }

		SecurityIdentifier ConnectAsSid { get; }

		void BackoffConnect(Exception reason);

		void CheckCanConnect();

		void RegisterActivity();

		int AddReference();

		void Release();

		void InvalidatePrincipalCache();

		ExchangePrincipal GetExchangePrincipal(string legacyDN);

		MiniRecipient MiniRecipient { get; }

		void CorrelateIdentityWithLegacyDN(ClientSecurityContext clientSecurityContext);

		ExOrgInfoFlags ExchangeOrganizationInfo { get; }

		MapiVersionRanges MapiBlockOutlookVersions { get; }

		bool MapiBlockOutlookRpcHttp { get; }

		bool MapiEnabled { get; }

		bool MapiCachedModeRequired { get; }

		bool IsFederatedSystemAttendant { get; }
	}
}
