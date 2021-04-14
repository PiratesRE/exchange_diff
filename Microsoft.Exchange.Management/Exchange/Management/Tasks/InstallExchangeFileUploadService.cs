﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "ExchangeFileUploadService")]
	public class InstallExchangeFileUploadService : ManageExchangeFileUploadService
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.Install();
			TaskLogger.LogExit();
		}
	}
}
