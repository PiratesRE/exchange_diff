using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementConsole;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.Services;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SnapIn
{
	public class ExchangeFormView : FormView, INodeSelectionService, ISharedViewDataService
	{
		public static event EventHandler ViewShown;

		public static FormViewDescription CreateViewDescription(Type controlType)
		{
			return new FormViewDescription
			{
				ViewType = typeof(ExchangeFormView),
				ControlType = controlType,
				DisplayName = controlType.Name,
				LanguageIndependentName = controlType.Name
			};
		}

		public ExchangeFormView()
		{
			ExTraceGlobals.ProgramFlowTracer.TracePerformance<ExchangeFormView, string>(0L, "Time:{1}. Start Load View. -->ExchangeFormView.ctor: {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.ctor: {0}", this);
			SynchronizationContext.SetSynchronizationContext(new SynchronizeInvokeSynchronizationContext(base.SnapIn));
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.ctor: {0}", this);
		}

		public IResultPaneControl ResultPane
		{
			get
			{
				return (IResultPaneControl)base.Control;
			}
		}

		protected override void OnInitialize(AsyncStatus status)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.OnInitialize: {0}", this);
			try
			{
				SynchronizationContext.SetSynchronizationContext(new SynchronizeInvokeSynchronizationContext(base.SnapIn));
				this.uiService = new ViewUIService(this);
				this.selectionService = new Selection();
				this.services = new ServiceContainer();
				this.settingsProvider = new ExchangeSettingsProvider();
				this.settingsProvider.Initialize(null, null);
				this.progressProvider = new ScopeNodeProgressProvider(base.ScopeNode);
				this.services.AddService(typeof(SynchronizationContext), SynchronizationContext.Current);
				this.services.AddService(typeof(IUIService), this.uiService);
				this.services.AddService(typeof(ISelectionService), this.selectionService);
				this.services.AddService(typeof(ISettingsProviderService), this.settingsProvider);
				this.services.AddService(typeof(IProgressProvider), this.progressProvider);
				this.services.AddService(typeof(INodeSelectionService), this);
				this.services.AddService(typeof(ISharedViewDataService), this);
				this.components = new ServicedContainer(this.services);
				this.components.Add(base.Control, base.Control.Name);
				IResultPaneControl resultPaneControl = base.Control as IResultPaneControl;
				ExchangeScopeNode exchangeScopeNode = base.ScopeNode as ExchangeScopeNode;
				IExchangeSnapIn exchangeSnapIn = base.SnapIn as IExchangeSnapIn;
				if (resultPaneControl != null && exchangeSnapIn != null)
				{
					resultPaneControl.SharedSettings = exchangeSnapIn.Settings;
				}
				if (exchangeScopeNode != null)
				{
					exchangeScopeNode.Refreshing += this.exchangeScopeNode_Refreshing;
					status.EnableManualCompletion();
					exchangeScopeNode.InitializeView(base.Control, new StatusProgress(status, base.ScopeNode.SnapIn));
				}
				base.Control.Dock = DockStyle.Fill;
				bool showGroupsAsRegions = base.Control is WorkCenter;
				this.selectionCommandsAdapter = new CommandsActionsAdapter(this.services, base.SelectionData.ActionsPaneItems, this.ResultPane.SelectionCommands, showGroupsAsRegions, exchangeSnapIn, this);
				this.resultPaneCommandsAdapter = new CommandsActionsAdapter(this.services, base.ActionsPaneItems, this.ResultPane.ResultPaneCommands, false, exchangeSnapIn, null);
				this.viewModeCommandsAdapter = new CommandsActionsAdapter(this.services, base.ModeActionsPaneItems, this.ResultPane.ViewModeCommands, false, exchangeSnapIn, null);
				base.ActionsPaneItems.Add(new ActionSeparator());
				this.exportListCommandsAdapter = new CommandsActionsAdapter(this.services, base.ActionsPaneItems, this.ResultPane.ExportListCommands, false, exchangeSnapIn, null, true);
				this.selectionService.SelectionChanged += this.selectionService_SelectionChanged;
				this.ResultPane.IsModifiedChanged += this.ResultPane_IsModifiedChanged;
				this.ResultPane_IsModifiedChanged(this.ResultPane, EventArgs.Empty);
				this.ResultPane.HelpRequested += this.ResultPane_HelpRequested;
				base.OnInitialize(status);
			}
			catch (Exception ex)
			{
				if (ExceptionHelper.IsUICriticalException(ex))
				{
					throw;
				}
				base.Control.Enabled = false;
				this.uiService.ShowError(ex);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.OnInitialize: {0}", this);
		}

		protected override void OnShutdown(SyncStatus status)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.OnShutdown: {0}", this);
			try
			{
				if (base.Control != null)
				{
					ExchangeScopeNode exchangeScopeNode = base.ScopeNode as ExchangeScopeNode;
					if (exchangeScopeNode != null)
					{
						exchangeScopeNode.Refreshing -= this.exchangeScopeNode_Refreshing;
					}
					this.ResultPane.HelpRequested -= this.ResultPane_HelpRequested;
					this.ResultPane.IsModifiedChanged -= this.ResultPane_IsModifiedChanged;
					this.selectionService.SelectionChanged -= this.selectionService_SelectionChanged;
					if (this.exportListCommandsAdapter != null)
					{
						this.exportListCommandsAdapter.Dispose();
					}
					this.exportListCommandsAdapter = null;
					if (this.viewModeCommandsAdapter != null)
					{
						this.viewModeCommandsAdapter.Dispose();
					}
					this.viewModeCommandsAdapter = null;
					if (this.resultPaneCommandsAdapter != null)
					{
						this.resultPaneCommandsAdapter.Dispose();
					}
					this.resultPaneCommandsAdapter = null;
					if (this.selectionCommandsAdapter != null)
					{
						this.selectionCommandsAdapter.Dispose();
					}
					this.selectionCommandsAdapter = null;
					if (this.components != null)
					{
						this.components.Dispose();
					}
					this.components = null;
					this.uiService = null;
					this.selectionService = null;
					if (this.services != null)
					{
						this.services.Dispose();
					}
					this.services = null;
					this.settingsProvider = null;
					this.progressProvider = null;
				}
				base.OnShutdown(status);
			}
			catch (Exception ex)
			{
				if (ExceptionHelper.IsUICriticalException(ex))
				{
					throw;
				}
				this.uiService.ShowError(ex);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.OnShutdown: {0}", this);
		}

		private void selectionService_SelectionChanged(object sender, EventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.SelectionChanged: {0}", this);
			this.DelayUpdates();
			SelectedObjects selectedObjects = new SelectedObjects(this.selectionService.GetSelectedComponents());
			DataObject selectionDataObject = this.ResultPane.SelectionDataObject;
			WritableSharedData writableSharedData = new WritableSharedData();
			UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
			foreach (string text in selectionDataObject.GetFormats())
			{
				object data = selectionDataObject.GetData(text);
				if (data is string)
				{
					WritableSharedDataItem writableSharedDataItem = new WritableSharedDataItem(text, false);
					writableSharedDataItem.SetData(unicodeEncoding.GetBytes(data as string));
					writableSharedData.Add(writableSharedDataItem);
					ExTraceGlobals.DataFlowTracer.Information<string, object>(0L, "ExchangeFormView.SelectionChanged: shared data: {0} => {1}", text, data);
				}
				else
				{
					ExTraceGlobals.DataFlowTracer.Information<string, object>(0L, "ExchangeFormView.SelectionChanged: {0}:{1} is not published to MMC because it is not a string.", text, data);
				}
			}
			if (selectedObjects.Count > 0)
			{
				ExTraceGlobals.DataFlowTracer.Information<int>(0L, "ExchangeFormView.SelectionChanged: selectedComponents.Count:{0}", selectedObjects.Count);
				base.SelectionData.Update(selectedObjects, selectedObjects.Count > 1, ExchangeFormView.emptyGuidArray, writableSharedData);
				base.SelectionData.DisplayName = selectionDataObject.GetText();
				ExTraceGlobals.DataFlowTracer.Information<string>(0L, "ExchangeFormView.SelectionChanged: this.SelectionData.DisplayName:{0}", base.SelectionData.DisplayName);
				base.SelectionData.Description = (selectionDataObject.GetData("Description") as string);
				ExTraceGlobals.DataFlowTracer.Information<string>(0L, "ExchangeFormView.SelectionChanged: this.SelectionData.Description:{0}", base.SelectionData.Description);
				base.SelectionData.HelpTopic = (string.IsNullOrEmpty(this.ResultPane.SelectionHelpTopic) ? base.ScopeNode.HelpTopic : this.ResultPane.SelectionHelpTopic);
				ExTraceGlobals.DataFlowTracer.Information<string>(0L, "ExchangeFormView.SelectionChanged: this.SelectionData.HelpTopic:{0}", base.SelectionData.HelpTopic);
			}
			else
			{
				ExTraceGlobals.DataFlowTracer.Information(0L, "ExchangeFormView.SelectionChanged: clearing selection.");
				base.SelectionData.Clear();
				base.SelectionData.HelpTopic = base.ScopeNode.HelpTopic;
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.SelectionChanged: {0}", this);
		}

		private void ResultPane_HelpRequested(object sender, HelpEventArgs hevent)
		{
			if (!hevent.Handled && !string.IsNullOrEmpty(base.ScopeNode.HelpTopic))
			{
				ExchangeHelpService.ShowHelpFromHelpTopicId(base.ScopeNode.HelpTopic);
				hevent.Handled = true;
			}
		}

		private void ResultPane_IsModifiedChanged(object sender, EventArgs e)
		{
			base.IsModified = this.ResultPane.IsModified;
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<bool>(0L, "*--ExchangeFormView.IsModifiedChanged: IsModified:{0}", base.IsModified);
		}

		private void exchangeScopeNode_Refreshing(object sender, EventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.ScopeNodeRefreshing: {0}", this);
			IRefreshable refreshableDataSource = this.ResultPane.RefreshableDataSource;
			IRefreshable dataSource = ((ExchangeScopeNode)base.ScopeNode).DataSource;
			if (refreshableDataSource != null && refreshableDataSource != dataSource && this.ResultPane.RefreshCommand.Enabled)
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug<string, string>(0L, "ExchangeFormView.ScopeNodeRefreshing: refreshing result pane {0} since its data source doesn't match the one in scope node {1}.", base.Control.Name, base.ScopeNode.LanguageIndependentName);
				this.ResultPane.RefreshCommand.Invoke();
			}
			else
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug<string, string>(0L, "ExchangeFormView.ScopeNodeRefreshing: no need to refresh result pane {0} when scope node {1} is refreshed.", base.Control.Name, base.ScopeNode.LanguageIndependentName);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.ScopeNodeRefreshing: {0}", this);
		}

		protected override void OnLoadCustomData(AsyncStatus status, byte[] customData)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.OnLoadCustomData: {0}", this);
			try
			{
				try
				{
					this.settingsProvider.ByteData = customData;
				}
				catch (SerializationException arg)
				{
					ExTraceGlobals.ProgramFlowTracer.TraceError<SerializationException>(0L, "Exception in ExchangeFormView.OnLoadCustomData: {0}", arg);
					this.uiService.ShowMessage(Strings.CannotLoadSettings);
					this.settingsProvider.ByteData = null;
				}
				this.ResultPane.LoadComponentSettings();
			}
			catch (Exception ex)
			{
				if (ExceptionHelper.IsUICriticalException(ex))
				{
					throw;
				}
				this.uiService.ShowError(ex);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.OnLoadCustomData: {0}", this);
		}

		protected override byte[] OnSaveCustomData(SyncStatus status)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.OnSaveCustomData: {0}", this);
			byte[] result = null;
			this.ResultPane.SaveComponentSettings();
			try
			{
				result = this.settingsProvider.ByteData;
			}
			catch (Exception ex)
			{
				ExTraceGlobals.ProgramFlowTracer.TraceError<Exception>(0L, "Exception in ExchangeFormView.OnSaveCustomData Exception: {0}", ex);
				throw new LocalizedException(Strings.CannotSaveSettings, ex);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.OnSaveCustomData: {0}", this);
			return result;
		}

		protected override void OnShow()
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.OnShow: {0}", this);
			ViewSharedData sharedData = this.SharedData;
			if (sharedData != null)
			{
				sharedData.CurrentActiveView = this;
				try
				{
					IAcceptExternalSelection acceptExternalSelection = base.Control as IAcceptExternalSelection;
					if (acceptExternalSelection != null && sharedData.SelectionObject != null)
					{
						ExTraceGlobals.ProgramFlowTracer.TraceDebug<string, object>(0L, "ExchangeFormView.OnShow: requesting {0} to select item {1}.", base.Control.Name, sharedData.SelectionObject);
						acceptExternalSelection.SetSelection(sharedData.SelectionObject);
						sharedData.SelectionObject = null;
					}
					IResultPaneSelectionService resultPaneSelectionService = base.Control as IResultPaneSelectionService;
					if (resultPaneSelectionService != null && sharedData.SelectedResultPaneName != null)
					{
						ExTraceGlobals.ProgramFlowTracer.TraceDebug<string, string>(0L, "ExchangeFormView.OnShow: requesting {0} to select pane {1}.", base.Control.Name, sharedData.SelectedResultPaneName);
						resultPaneSelectionService.SelectResultPaneByName(sharedData.SelectedResultPaneName);
						sharedData.SelectedResultPaneName = null;
					}
				}
				catch (Exception ex)
				{
					if (ExceptionHelper.IsUICriticalException(ex))
					{
						throw;
					}
					this.uiService.ShowError(ex);
				}
				if (ExchangeFormView.ViewShown != null)
				{
					ExchangeFormView.ViewShown(this, EventArgs.Empty);
				}
				this.ResultPane.OnSetActive();
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.OnShow: {0}", this);
			ExTraceGlobals.ProgramFlowTracer.TracePerformance<ExchangeFormView, string>(0L, "Time:{1}. End Load View. <--ExchangeFormView.OnShow: {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"));
		}

		protected override void OnHide()
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.OnHide: {0}", this);
			base.OnHide();
			this.ResultPane.OnKillActive();
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.OnHide: {0}", this);
		}

		public void SelectNodeByName(string nodeName)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView, string>(0L, "*--ExchangeFormView.SelectNodeByName: {0}: {1}", this, nodeName);
			this.SelectNodeFromCollection(nodeName, ((IExchangeSnapIn)base.SnapIn).ScopeNodeCollection);
		}

		public void SelectNodeAndResultPaneByName(string nodeName, string resultPaneName)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView, string, string>(0L, "*--ExchangeFormView.SelectNodeAndResultPaneByName: {0}: {1},{2}", this, nodeName, resultPaneName);
			ViewSharedData sharedData = this.SharedData;
			if (sharedData != null)
			{
				sharedData.SelectedResultPaneName = resultPaneName;
				this.SelectNodeByName(nodeName);
			}
		}

		private void SelectNodeFromCollection(string nodeName, ScopeNodeCollection nodes)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.SelectNodeFromCollection: {0}", this);
			ViewSharedData sharedData = this.SharedData;
			if (sharedData != null)
			{
				foreach (object obj in nodes)
				{
					ScopeNode scopeNode = (ScopeNode)obj;
					if (scopeNode.LanguageIndependentName == nodeName)
					{
						sharedData.CurrentActiveView.SelectScopeNode(scopeNode);
						break;
					}
					if (scopeNode.Children.Count != 0)
					{
						this.SelectNodeFromCollection(nodeName, scopeNode.Children);
					}
				}
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.SelectNodeFromCollection: {0}", this);
		}

		public ViewSharedData SharedData
		{
			get
			{
				if (base.SharedTag == null)
				{
					base.SharedTag = new ViewSharedData();
				}
				return (ViewSharedData)base.SharedTag;
			}
		}

		internal void DelayUpdates()
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.DelayUpdates: {0}", this);
			if (base.Control != null && base.Control.IsHandleCreated)
			{
				if (!this.beginUpdateInvoked)
				{
					ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.DelayUpdates: {0} calling BeginUpdates.", this);
					base.SelectionData.BeginUpdates();
					ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.DelayUpdates: {0} calling BeginUpdates.", this);
					this.beginUpdateInvoked = true;
					base.Control.BeginInvoke(new MethodInvoker(delegate()
					{
						this.beginUpdateInvoked = false;
						ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "-->ExchangeFormView.DelayUpdates: {0} calling EndUpdates.", this);
						base.SelectionData.EndUpdates();
						ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.DelayUpdates: {0} calling EndUpdates.", this);
					}));
				}
				else
				{
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<ExchangeFormView>(0L, "ExchangeFormView.DelayUpdates: {0} skipped because we already invoked BeginUpdate once.", this);
				}
			}
			else
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug<ExchangeFormView>(0L, "ExchangeFormView.DelayUpdates: {0} skipped because control is null or not created.", this);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<ExchangeFormView>(0L, "<--ExchangeFormView.DelayUpdates: {0}", this);
		}

		public override string ToString()
		{
			if (base.Control == null)
			{
				return "(null control in ExchangeFormView)";
			}
			return base.Control.Name;
		}

		private IUIService uiService;

		private ISelectionService selectionService;

		private ServiceContainer services;

		private ServicedContainer components;

		private ExchangeSettingsProvider settingsProvider;

		private IProgressProvider progressProvider;

		private CommandsActionsAdapter selectionCommandsAdapter;

		private CommandsActionsAdapter resultPaneCommandsAdapter;

		private CommandsActionsAdapter viewModeCommandsAdapter;

		private CommandsActionsAdapter exportListCommandsAdapter;

		private static Guid[] emptyGuidArray = new Guid[]
		{
			Guid.Empty
		};

		private bool beginUpdateInvoked;
	}
}
