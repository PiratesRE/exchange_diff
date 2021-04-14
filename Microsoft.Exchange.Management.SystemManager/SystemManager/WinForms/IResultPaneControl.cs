using System;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IResultPaneControl : IPersistComponentSettings
	{
		ExchangeSettings SharedSettings { get; set; }

		string Status { get; set; }

		event EventHandler StatusChanged;

		bool IsModified { get; set; }

		event EventHandler IsModifiedChanged;

		CommandCollection ResultPaneCommands { get; }

		CommandCollection ExportListCommands { get; }

		CommandCollection ViewModeCommands { get; }

		CommandCollection SelectionCommands { get; }

		Command RefreshCommand { get; }

		IRefreshableNotification RefreshableDataSource { get; set; }

		event EventHandler RefreshableDataSourceChanged;

		DataObject SelectionDataObject { get; }

		ICollection SelectedObjects { get; }

		string SelectionHelpTopic { get; }

		event HelpEventHandler HelpRequested;

		bool HasSelection { get; }

		void OnSetActive();

		void OnKillActive();
	}
}
