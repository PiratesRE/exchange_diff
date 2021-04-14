using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring
{
	internal class ForwardSyncCookieStxLogger : StxLoggerBase
	{
		public ForwardSyncCookieStxLogger()
		{
			base.ExtendedFields.AddRange(new List<FieldInfo>
			{
				new FieldInfo(StxLoggerBase.MandatoryFields.Count, "ServiceInstanceName"),
				new FieldInfo(StxLoggerBase.MandatoryFields.Count + 1, "CompanyCookieStructureType"),
				new FieldInfo(StxLoggerBase.MandatoryFields.Count + 2, "CompanyCookieSize"),
				new FieldInfo(StxLoggerBase.MandatoryFields.Count + 3, "RecipientCookieStructureType"),
				new FieldInfo(StxLoggerBase.MandatoryFields.Count + 4, "RecipientCookieSize")
			});
		}

		internal override string LogTypeName
		{
			get
			{
				return "ForwardSyncCookie Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ForwardSyncCookie";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ForwardSyncCookie_";
			}
		}
	}
}
