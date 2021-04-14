﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "OutlookProvider", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveOutlookProviderTask : RemoveSystemConfigurationObjectTask<OutlookProviderIdParameter, OutlookProvider>
	{
		protected override ObjectId RootId
		{
			get
			{
				return OutlookProvider.GetParentContainer(base.DataSession as ITopologyConfigurationSession);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveOutlookProvider(this.Identity.ToString());
			}
		}
	}
}
