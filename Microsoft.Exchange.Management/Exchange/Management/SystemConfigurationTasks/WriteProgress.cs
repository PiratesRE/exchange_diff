using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal delegate void WriteProgress(LocalizedString activity, LocalizedString statusDescription, int percentCompleted);
}
