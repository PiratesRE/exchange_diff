using System;
using System.Windows.Forms;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class QueueViewerTaskCommandAction : ResultsTaskCommandAction
	{
		protected override bool ConfirmOperation(WorkUnitCollectionEventArgs inputArgs)
		{
			bool flag = true;
			bool flag2 = true;
			QueueViewerResultPaneBase queueViewerResultPaneBase = base.DataListViewResultPane as QueueViewerResultPaneBase;
			if (queueViewerResultPaneBase.ListControl.SelectedIndices.Count == queueViewerResultPaneBase.ListControl.Items.Count && !string.IsNullOrEmpty(queueViewerResultPaneBase.ObjectList.FilterControl.Expression))
			{
				using (BulkActionControl bulkActionControl = new BulkActionControl())
				{
					if (queueViewerResultPaneBase.ShowDialog(bulkActionControl) == DialogResult.OK)
					{
						if (bulkActionControl.IsExpandScopeSelected)
						{
							flag2 = false;
							if (base.MultipleSelectionConfirmation != null)
							{
								flag = (DialogResult.Yes == queueViewerResultPaneBase.ShellUI.ShowMessage(base.MultipleSelectionConfirmation(queueViewerResultPaneBase.Datasource.TotalItems), UIService.DefaultCaption, MessageBoxButtons.YesNo));
							}
							if (flag)
							{
								base.Parameters.Remove("Filter");
								base.Parameters.AddWithValue("Filter", queueViewerResultPaneBase.ObjectList.FilterControl.Expression);
								base.Parameters.Remove("server");
								base.Parameters.AddWithValue("server", queueViewerResultPaneBase.ServerName);
							}
						}
					}
					else
					{
						flag = false;
					}
				}
			}
			if (flag2)
			{
				base.Parameters.Remove("Filter");
				base.Parameters.Remove("server");
			}
			else
			{
				inputArgs.WorkUnits.Clear();
			}
			return flag && base.ConfirmOperation(inputArgs);
		}
	}
}
