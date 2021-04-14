using System;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole.Advanced;

namespace Microsoft.Exchange.Management.SnapIn
{
	[Serializable]
	public abstract class NotificationSharedDataCommand : ExchangeSharedDataCommand
	{
		public override void ExtensionExecute(IExchangeExtensionSnapIn snapIn)
		{
			try
			{
				snapIn.SharedDataItem.RequestDataUpdate(WinformsHelper.Serialize(this));
			}
			catch (PrimarySnapInDataException)
			{
			}
		}
	}
}
