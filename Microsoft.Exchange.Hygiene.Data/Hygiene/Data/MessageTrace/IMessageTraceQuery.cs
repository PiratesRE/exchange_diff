using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal interface IMessageTraceQuery
	{
		MessageTrace[] FindPagedTrace(Guid organizationalUnitRoot, DateTime start, DateTime end, string fromEmailPrefix = null, string fromEmailDomain = null, string toEmailPrefix = null, string toEmailDomain = null, string clientMessageId = null, int rowIndex = 0, int rowCount = -1);

		MessageTrace Read(Guid organizationalUnitRoot, Guid messageId);
	}
}
