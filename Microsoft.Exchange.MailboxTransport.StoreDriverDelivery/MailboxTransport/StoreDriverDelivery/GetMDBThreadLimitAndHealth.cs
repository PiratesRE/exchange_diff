using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal delegate int GetMDBThreadLimitAndHealth(Guid mbGuid, out int databaseHealthMeasure, out List<KeyValuePair<string, double>> monitorHealthValues);
}
