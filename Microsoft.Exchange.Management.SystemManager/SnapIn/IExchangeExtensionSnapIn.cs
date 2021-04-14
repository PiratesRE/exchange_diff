using System;
using System.Drawing;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn
{
	public interface IExchangeExtensionSnapIn
	{
		void AddRootNode(OrganizationType type, string displayName, Icon icon);

		void HideRootNode();

		void ShowRootNode();

		void RemoveRootNode();

		void ForceSaveSetting();

		SharedDataItem SharedDataItem { get; }

		SharedDataItem CallbackSharedDataItem { get; }
	}
}
