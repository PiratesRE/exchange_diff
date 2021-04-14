using System;

namespace Microsoft.Exchange.Monitoring
{
	internal class ActiveDirectorySelfCheckStxLogger : StxLoggerBase
	{
		internal override string LogTypeName
		{
			get
			{
				return "ActiveDirectorySelfCheck Logs";
			}
		}

		internal override string LogComponent
		{
			get
			{
				return "ActiveDirectorySelfCheck";
			}
		}

		internal override string LogFilePrefix
		{
			get
			{
				return "Test-ActiveDirectorySelfCheck_";
			}
		}
	}
}
