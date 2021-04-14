using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SnapIn.Esm.Toolbox
{
	[NodeType("9a51f92e-ada8-4f51-915b-36bd68a47882", Description = "Exchange Toolbox Node")]
	public sealed class ToolboxNode : ExchangeScopeNode
	{
		public ToolboxNode() : this(false)
		{
		}

		public ToolboxNode(bool isEdgeServerRoleOnly) : base(true)
		{
			ToolboxNode <>4__this = this;
			base.DisplayName = Strings.Toolbox;
			base.Icon = Icons.Toolbox;
			base.HelpTopic = HelpId.ToolboxNode.ToString();
			base.ViewDescriptions.Add(ExchangeFormView.CreateViewDescription(typeof(ToolboxResultPane)));
			RefreshableComponent refreshableComponent = new RefreshableComponent();
			base.DataSource = refreshableComponent;
			refreshableComponent.DoRefreshWork += delegate(object sender, RefreshRequestEventArgs e)
			{
				DataList<Tool> dataList = new DataList<Tool>();
				DataProvider dataProvider = new RegistryDataProvider(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\AdminTools\\Toolbox", ConfigurationContext.Setup.InstallPath);
				dataProvider.Query();
				for (int i = 0; i < dataProvider.Tools.Count; i++)
				{
					Tool tool = dataProvider.Tools[i];
					if (tool.Name != string.Empty && Tool.IsToolDuplicate(dataList, tool))
					{
						tool.ValidTool = false;
						tool.AddErrorMessage(Strings.DuplicateTool(tool.DataSource));
					}
					else if (!isEdgeServerRoleOnly || !tool.NonEdgeTool)
					{
						dataList.Add(tool);
					}
				}
				e.Result = dataList;
			};
			refreshableComponent.RefreshCompleted += delegate(object sender, RunWorkerCompletedEventArgs e)
			{
				if (e.Cancelled)
				{
					return;
				}
				DataList<Tool> dataList = (DataList<Tool>)e.Result;
				this.ValidTools.Clear();
				DataList<Tool> dataList2 = new DataList<Tool>();
				for (int i = 0; i < dataList.Count; i++)
				{
					if (dataList[i].ValidTool)
					{
						this.ValidTools.Add(dataList[i]);
					}
					else
					{
						dataList2.Add(dataList[i]);
					}
				}
				if (dataList2.Count > 0)
				{
					List<WorkUnit> list = new List<WorkUnit>();
					for (int j = 0; j < dataList2.Count; j++)
					{
						WorkUnit workUnit = new WorkUnit();
						workUnit.Text = dataList2[j].DataSource;
						Exception ex = new Exception(dataList2[j].ErrorMessage);
						workUnit.Errors.Add(new ErrorRecord(ex, LocalizedException.GenerateErrorCode(ex).ToString("X"), ErrorCategory.InvalidOperation, null));
						workUnit.Status = WorkUnitStatus.Failed;
						list.Add(workUnit);
					}
					UIService.ShowError(Strings.ErrorDialogMessage, string.Empty, list.ToArray(), base.ShellUI);
				}
			};
		}

		public DataList<Tool> ValidTools
		{
			get
			{
				return this.validTools;
			}
			private set
			{
				this.validTools = value;
			}
		}

		public override void InitializeView(Control control, IProgress status)
		{
			(control as ToolboxResultPane).RefreshableDataSource = base.DataSource;
			(control as ToolboxResultPane).ListControl.DataSource = this.ValidTools;
			(control as ToolboxResultPane).ListControl.ShowGroups = true;
			base.DataSource.Refresh(status);
			base.InitializeView(control, status);
		}

		public const string NodeGuid = "9a51f92e-ada8-4f51-915b-36bd68a47882";

		public const string NodeDescription = "Exchange Toolbox Node";

		private DataList<Tool> validTools = new DataList<Tool>();
	}
}
