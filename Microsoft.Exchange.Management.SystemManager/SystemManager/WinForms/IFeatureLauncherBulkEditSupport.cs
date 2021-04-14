using System;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IFeatureLauncherBulkEditSupport : IBulkEditSupport
	{
		event EventHandler FeatureItemUpdated;
	}
}
