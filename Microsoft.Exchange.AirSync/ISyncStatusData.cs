using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal interface ISyncStatusData
	{
		string LastSyncRequestRandomString { get; set; }

		byte[] LastCachableWbxmlDocument { get; set; }

		ExDateTime? LastSyncAttemptTime { get; set; }

		ExDateTime? LastSyncSuccessTime { get; set; }

		string LastSyncUserAgent { get; set; }

		bool ClientCanSendUpEmptyRequests { get; set; }

		void AddClientId(string clientId);

		bool ContainsClientId(string clientId);

		bool ContainsClientCategoryHash(int hashName);

		void AddClientCategoryHash(int hashName);

		void ClearClientCategoryHash();

		void SaveAndDispose();
	}
}
