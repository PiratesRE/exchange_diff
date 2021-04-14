using System;
using System.ComponentModel;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ResultsTaskCommandAction : TaskCommandAction
	{
		public ResultsTaskCommandAction()
		{
			this.PipelineInputProperty = string.Empty;
		}

		public ResultsCommandProfile Profile
		{
			get
			{
				return base.Profile as ResultsCommandProfile;
			}
		}

		public ResultsCommandSetting Setting
		{
			get
			{
				if (this.Profile != null)
				{
					return this.Profile.Setting;
				}
				return null;
			}
		}

		public DataListViewResultPane DataListViewResultPane
		{
			get
			{
				if (this.Profile != null)
				{
					return this.Profile.ResultPane as DataListViewResultPane;
				}
				return null;
			}
		}

		[DefaultValue(false)]
		public bool UseCustomInputRequestedHandler { get; set; }

		public string PipelineInputProperty { get; set; }

		public Type PipelineInputType { get; set; }

		protected override void OnExecuting(out bool cancelled)
		{
			base.OnExecuting(ref cancelled);
			if (this.DataListViewResultPane != null && this.Setting.IsSelectionCommand && this.Setting.Operation == CommandOperation.Delete)
			{
				DataListView listControl = this.DataListViewResultPane.ListControl;
				if (listControl.SelectedIndices.Count > 0 && listControl.SelectedIndices.Count != listControl.Items.Count)
				{
					int num = (listControl.FocusedItem != null) ? listControl.FocusedItem.Index : -1;
					if (num < 0)
					{
						num = listControl.SelectedIndices[listControl.SelectedIndices.Count - 1];
					}
					int num2 = num;
					if (listControl.SelectedIndices.Contains(num))
					{
						num2 = num + 1;
						while (num2 < listControl.Items.Count && listControl.SelectedIndices.Contains(num2))
						{
							num2++;
						}
					}
					if (num2 >= listControl.Items.Count)
					{
						num2 = num - 1;
						while (num2 >= 0 && listControl.SelectedIndices.Contains(num2))
						{
							num2--;
						}
					}
					if (num2 >= 0 && num2 < listControl.Items.Count)
					{
						this.rowIdToSelectAfterDelete = listControl.GetRowIdentity(listControl.GetRowFromItem(listControl.Items[num2]));
					}
				}
			}
		}

		protected sealed override void OnInputRequested(WorkUnitCollectionEventArgs e)
		{
			e.WorkUnits.AddRange(this.OnRequestInputs());
			if (this.DataListViewResultPane != null && this.Setting.IsSelectionCommand)
			{
				if (this.Setting.UseSingleRowRefresh && this.DataListViewResultPane.HasSelection)
				{
					base.RefreshOnFinish = this.DataListViewResultPane.GetSelectionRefreshObjects();
					base.MultiRefreshOnFinish = null;
					return;
				}
				if (this.Setting.UseFullRefresh)
				{
					base.RefreshOnFinish = this.DataListViewResultPane.RefreshableDataSource;
					base.MultiRefreshOnFinish = null;
				}
			}
		}

		protected virtual WorkUnit[] OnRequestInputs()
		{
			if (this.DataListViewResultPane != null && !this.UseCustomInputRequestedHandler)
			{
				string targetPropertyName = string.IsNullOrEmpty(this.PipelineInputProperty) ? this.DataListViewResultPane.ListControl.IdentityProperty : this.PipelineInputProperty;
				return this.DataListViewResultPane.ListControl.GetSelectedWorkUnits(targetPropertyName, this.PipelineInputType);
			}
			return new WorkUnit[0];
		}

		protected sealed override void OnCompleted(WorkUnitCollectionEventArgs e)
		{
			if (this.DataListViewResultPane != null && this.Setting.IsSelectionCommand && this.Setting.Operation == CommandOperation.Delete)
			{
				ISupportFastRefresh supportFastRefresh = this.DataListViewResultPane.RefreshableDataSource as ISupportFastRefresh;
				if (supportFastRefresh != null && base.RefreshOnFinish == null && base.MultiRefreshOnFinish == null)
				{
					foreach (WorkUnit workUnit in e.WorkUnits)
					{
						if (workUnit.Status == WorkUnitStatus.Completed)
						{
							supportFastRefresh.Remove(workUnit.Target);
						}
					}
				}
				if (this.rowIdToSelectAfterDelete != null && !e.WorkUnits.HasFailures)
				{
					this.DataListViewResultPane.ListControl.SelectItemBySpecifiedIdentity(this.rowIdToSelectAfterDelete, false);
				}
				this.rowIdToSelectAfterDelete = null;
			}
			this.OnCompleted(e.WorkUnits.ToArray());
			base.OnCompleted(e);
		}

		protected virtual void OnCompleted(WorkUnit[] workUnits)
		{
		}

		private object rowIdToSelectAfterDelete;
	}
}
