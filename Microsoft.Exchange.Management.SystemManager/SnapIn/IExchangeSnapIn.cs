using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SnapIn
{
	public interface IExchangeSnapIn : IServiceProvider
	{
		void Initialize(IProgressProvider progressProvider);

		int RegisterIcon(string name, Icon icon);

		ExchangeSettings CreateSettings(IComponent owner);

		IUIService ShellUI { get; }

		ExchangeSettings Settings { get; }

		ScopeNodeCollection ScopeNodeCollection { get; }

		string SnapInGuidString { get; }

		string RootNodeDisplayName { get; }

		Icon RootNodeIcon { get; }
	}
}
