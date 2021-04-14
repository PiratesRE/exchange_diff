using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal interface IConnection
	{
		string ActAsLegacyDN { get; }

		ClientSecurityContext AccessingClientSecurityContext { get; }

		ConnectionFlags ConnectionFlags { get; }

		OrganizationId OrganizationId { get; }

		IPAddress ServerIpAddress { get; }

		string ProtocolSequence { get; }

		bool IsWebService { get; }

		void BackoffConnect(Exception reason);

		ExchangePrincipal FindExchangePrincipalByLegacyDN(string legacyDN);

		MiniRecipient MiniRecipient { get; }

		void InvalidateCachedUserInfo();

		void MarkAsDeadAndDropAllAsyncCalls();

		void ExecuteInContext<T>(T input, Action<T> action);

		Fqdn TargetServer { set; }

		bool IsEncrypted { get; }

		CultureInfo CultureInfo { get; }

		int CodePageId { get; }

		ClientInfo ClientInformation { get; }

		string RpcServerTarget { get; }

		bool IsFederatedSystemAttendant { get; }
	}
}
