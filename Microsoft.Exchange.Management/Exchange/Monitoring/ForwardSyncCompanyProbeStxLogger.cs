using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Monitoring
{
	internal class ForwardSyncCompanyProbeStxLogger : StxLoggerBase
	{
		public ForwardSyncCompanyProbeStxLogger()
		{
			base.ExtendedFields.AddRange(new List<FieldInfo>
			{
				new FieldInfo(StxLoggerBase.MandatoryFields.Count, "OrganizationStatus"),
				new FieldInfo(StxLoggerBase.MandatoryFields.Count + 1, "OrganizationName")
			});
		}

		internal override string LogTypeName
		{
			get
			{
				return "ForwardSyncCompanyProbe Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ForwardSyncCompanyProbe";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ForwardSyncCompanyProbe_";
			}
		}
	}
}
