using System;
using System.Collections.Generic;
using Microsoft.Exchange.Migration.DataAccessLayer;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal interface IMigrationNspiClient
	{
		IList<PropRow> QueryRows(ExchangeOutlookAnywhereEndpoint connectionSettings, int? batchSize, int? startIndex, long[] longPropTags);

		PropRow GetRecipient(ExchangeOutlookAnywhereEndpoint connectionSettings, string recipientSmtpAddress, long[] longPropTags);

		void SetRecipient(ExchangeOutlookAnywhereEndpoint connectionSettings, string recipientSmtpAddress, string recipientLegDN, string[] propTagValues, long[] longPropTags);

		IList<PropRow> GetGroupMembers(ExchangeOutlookAnywhereEndpoint connectionSettings, string groupSmtpAddress);

		string GetNewDSA(ExchangeOutlookAnywhereEndpoint connectionSettings);
	}
}
