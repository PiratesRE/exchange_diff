using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics.Components.Management.SystemManager;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	[DefaultEvent("RefreshingChanged")]
	public class DataTableLoader : RefreshableComponent, ISupportFastRefresh, IAcceptExternalInput
	{
		public DataTableLoader() : this(null)
		{
		}

		public DataTableLoader(ObjectPickerProfileLoader profileLoader, string profileName) : this(profileLoader.GetProfile(profileName))
		{
		}

		public DataTableLoader(IResultsLoaderConfiguration config) : this(config.BuildResultsLoaderProfile())
		{
		}

		public DataTableLoader(ResultsLoaderProfile profile)
		{
			this.expectedResultSize = this.DefaultExpectedResultSize;
			this.ResultsLoaderProfile = profile;
		}

		public ResultsLoaderProfile ResultsLoaderProfile
		{
			get
			{
				return this.resultsLoaderProfile;
			}
			private set
			{
				if (value != null)
				{
					this.resultsLoaderProfile = value;
					this.Table = this.ResultsLoaderProfile.CreateResultsDataTable();
					this.Table.Columns.CollectionChanged += delegate(object param0, CollectionChangeEventArgs param1)
					{
						if (this.expressionCalculator != null)
						{
							this.expressionCalculator = null;
						}
					};
					this.BatchSize = this.ResultsLoaderProfile.BatchSize;
					base.RefreshArgument = this.ResultsLoaderProfile;
				}
			}
		}

		[DefaultValue(false)]
		public bool EnforeViewEntireForest { get; set; }

		public virtual void InputValue(string columnName, object value)
		{
			if (this.ResultsLoaderProfile == null)
			{
				throw new InvalidOperationException("ResultsLoaderProfile is null");
			}
			this.ResultsLoaderProfile.InputValue(columnName, value);
		}

		public virtual void RemoveValue(string columnName)
		{
			if (this.ResultsLoaderProfile == null)
			{
				throw new InvalidOperationException("ResultsLoaderProfile is null");
			}
			this.ResultsLoaderProfile.InputValue(columnName, null);
		}

		public virtual object GetValue(string columnName)
		{
			if (this.ResultsLoaderProfile == null)
			{
				throw new InvalidOperationException("ResultsLoaderProfile is null");
			}
			return this.ResultsLoaderProfile.GetValue(columnName);
		}

		[DefaultValue(100)]
		public virtual int BatchSize
		{
			get
			{
				return this.batchSize;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value", value, "value <= 0");
				}
				this.batchSize = value;
			}
		}

		[DefaultValue(100)]
		public virtual int DefaultExpectedResultSize
		{
			get
			{
				return this.defaultExpectedResultSize;
			}
			set
			{
				this.defaultExpectedResultSize = value;
			}
		}

		public int ExpectedResultSize
		{
			get
			{
				return this.expectedResultSize;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException();
				}
				if (this.ExpectedResultSize != value)
				{
					this.expectedResultSize = value;
					this.lastRowCount = value;
					this.OnExpectedResultSizeChanged(EventArgs.Empty);
				}
			}
		}

		private bool ShouldSerializeExpectedResultSize()
		{
			return this.ExpectedResultSize != this.DefaultExpectedResultSize;
		}

		private void ResetExpectedResultSize()
		{
			this.ExpectedResultSize = this.DefaultExpectedResultSize;
		}

		protected virtual void OnExpectedResultSizeChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[DataTableLoader.EventExpectedResultSizeChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ExpectedResultSizeChanged
		{
			add
			{
				base.Events.AddHandler(DataTableLoader.EventExpectedResultSizeChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataTableLoader.EventExpectedResultSizeChanged, value);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LocalizedString ProgressText
		{
			get
			{
				return this.progressText;
			}
			set
			{
				this.progressText = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LocalizedString RefreshCommandText
		{
			get
			{
				return this.refreshCommandText;
			}
			set
			{
				this.refreshCommandText = value;
			}
		}

		public DataTable Table
		{
			get
			{
				return this.table;
			}
			set
			{
				if (this.Table != value)
				{
					if (this.Table != null)
					{
						this.Table.RowChanged -= this.Table_RowChanged;
					}
					if (base.Refreshing)
					{
						throw new InvalidOperationException();
					}
					this.table = value;
					if (this.Table != null)
					{
						this.Table.RowChanged += this.Table_RowChanged;
					}
					this.OnTableChanged(EventArgs.Empty);
				}
			}
		}

		private void Table_RowChanged(object sender, DataRowChangeEventArgs e)
		{
			if (!this.isCalculatingColumn)
			{
				if (e.Action != DataRowAction.Add)
				{
					if (e.Action != DataRowAction.Change)
					{
						return;
					}
				}
				try
				{
					this.isCalculatingColumn = true;
					if (this.ResultsLoaderProfile != null && this.ResultsLoaderProfile.DataColumnsCalculator != null)
					{
						this.ResultsLoaderProfile.DataColumnsCalculator.Calculate(this.ResultsLoaderProfile, this.Table, e.Row);
					}
				}
				finally
				{
					this.isCalculatingColumn = false;
				}
			}
		}

		internal WorkUnitCollection WorkUnits
		{
			get
			{
				if (this.workUnits == null)
				{
					this.workUnits = new WorkUnitCollection();
				}
				return this.workUnits;
			}
		}

		protected virtual DataTable DefaultTable
		{
			get
			{
				return null;
			}
		}

		private bool ShouldSerializeTable()
		{
			return this.Table != this.DefaultTable;
		}

		private void ResetTable()
		{
			this.Table = this.DefaultTable;
		}

		protected virtual void OnTableChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[DataTableLoader.EventTableChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler TableChanged
		{
			add
			{
				base.Events.AddHandler(DataTableLoader.EventTableChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataTableLoader.EventTableChanged, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataColumnCollection Columns
		{
			get
			{
				if (this.Table == null)
				{
					return null;
				}
				return this.Table.Columns;
			}
		}

		protected override void OnRefreshStarting(CancelEventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "-->DataTableLoader.OnRefreshStarting: {0}", this);
			lock (this.WorkUnits)
			{
				this.WorkUnits.Clear();
			}
			if (this.Table == null)
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug((long)this.GetHashCode(), "DataTableLoader.OnRefreshStarting: cancelling since we don't have a table set.");
				e.Cancel = true;
			}
			else
			{
				base.OnRefreshStarting(e);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.OnRefreshStarting: {0}", this);
		}

		protected sealed override RefreshRequestEventArgs CreateFullRefreshRequest(IProgress progress)
		{
			return new DataTableLoader.DataTableLoaderRefreshRequestEventArgs(this.Table.Clone(), true, progress, base.CloneRefreshArgument());
		}

		protected override PartialOrder ComparePartialOrder(RefreshRequestEventArgs leftValue, RefreshRequestEventArgs rightValue)
		{
			if (this.ResultsLoaderProfile != null && this.ResultsLoaderProfile.InputTablePartialOrderComparer != null)
			{
				return this.ResultsLoaderProfile.InputTablePartialOrderComparer.Compare((leftValue.Argument as ResultsLoaderProfile).InputTable, (rightValue.Argument as ResultsLoaderProfile).InputTable);
			}
			return base.ComparePartialOrder(leftValue, rightValue);
		}

		private void ReportDataTable(int rowCount, int expectedRowCount, DataTable dataTable, RefreshRequestEventArgs e, bool resolved, TimeSpan elapsed)
		{
			ResultsLoaderProfile resultsLoaderProfile = e.Argument as ResultsLoaderProfile;
			this.FillColumnsBasedOnLambdaExpression(dataTable, resultsLoaderProfile);
			ColumnValueCalculator.CalculateAll(dataTable);
			DataTable dataTable2 = this.MoveRows(dataTable, null, false);
			if (resultsLoaderProfile != null && resolved && resultsLoaderProfile.IsResolving)
			{
				foreach (object obj in resultsLoaderProfile.PipelineObjects)
				{
					if (this.FindRowByIdentity(dataTable2, obj) != null)
					{
						resultsLoaderProfile.ResolvedObjects.Add(obj);
					}
				}
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction((long)this.GetHashCode(), "-->DataTableLoader.ReportDataTable: report batch: {0}. progressReportTable.Rows.Count:{1}, this.BatchSize:{2}, rowCount:{3}, expectedRowCount:{4}. ElapsedTime:{5}", new object[]
			{
				this,
				dataTable2.Rows.Count,
				this.BatchSize,
				rowCount,
				expectedRowCount,
				elapsed
			});
			if (!e.ReportedProgress)
			{
				ExTraceGlobals.ProgramFlowTracer.TracePerformance<DataTableLoader, string, string>(0L, "Time:{1}. {2} First batch data arrived in worker thread. {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), (this != null) ? this.Table.TableName : string.Empty);
			}
			e.ReportProgress(rowCount, expectedRowCount, this.ProgressText, dataTable2);
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.ReportDataTable: report batch: {0}", this);
		}

		private bool IsPreFillForResolving(AbstractDataTableFiller filler)
		{
			return filler is PreFillADObjectIdFiller;
		}

		protected override void OnDoRefreshWork(RefreshRequestEventArgs e)
		{
			Stopwatch sw = Stopwatch.StartNew();
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "-->DataTableLoader.OnDoRefreshWork: {0}", this);
			this.expressionCalculator = null;
			DataTable dataTable = (e as DataTableLoader.IDataTableLoaderRefreshRequest).DataTable.Clone();
			e.Result = dataTable;
			dataTable.RowChanging += delegate(object param0, DataRowChangeEventArgs param1)
			{
				if (e.CancellationPending)
				{
					e.Cancel = true;
					this.Cancel(e);
					e.Result = dataTable;
				}
			};
			int rowCount = 0;
			int expectedRowCount = Math.Max(this.lastRowCount, this.BatchSize);
			ResultsLoaderProfile profile = e.Argument as ResultsLoaderProfile;
			if (profile != null)
			{
				foreach (AbstractDataTableFiller abstractDataTableFiller in profile.TableFillers)
				{
					abstractDataTableFiller.FillCompleted += delegate(object sender, FillCompletedEventArgs args)
					{
						AbstractDataTableFiller filler = sender as AbstractDataTableFiller;
						if (this.IsPreFillForResolving(filler) || profile.FillType == null)
						{
							expectedRowCount = Math.Max(expectedRowCount, rowCount + this.BatchSize + 1);
							bool flag = !this.IsPreFillForResolving(filler) && profile.FillType == 0;
							this.ReportDataTable(rowCount, expectedRowCount, args.DataTable, e, flag, sw.Elapsed);
							if (profile.IsResolving && profile.PipelineObjects != null && flag)
							{
								foreach (object obj in profile.PipelineObjects)
								{
									if (this.FindRowByIdentity(args.DataTable, obj) != null)
									{
										profile.ResolvedObjects.Add(obj);
									}
								}
								IEnumerable<object> source = from id in profile.PipelineObjects
								where !profile.ResolvedObjects.Contains(id)
								select id;
								profile.PipelineObjects = source.ToArray<object>();
								profile.ResolvedObjects.Clear();
							}
						}
					};
				}
			}
			dataTable.RowChanged += delegate(object sender, DataRowChangeEventArgs rowChangedEvent)
			{
				if (rowChangedEvent.Action == DataRowAction.Add)
				{
					rowCount++;
					ExTraceGlobals.DataFlowTracer.Information<DataTableLoader, int, TimeSpan>((long)this.GetHashCode(), "DataTableLoader.OnDoRefreshWork: {0}, rowCount:{1}. ElapsedTime:{2}", this, rowCount, sw.Elapsed);
					ConvertTypeCalculator.Convert(rowChangedEvent.Row);
					if (dataTable.Rows.Count >= this.BatchSize && (profile == null || profile.FillType == null))
					{
						expectedRowCount = Math.Max(expectedRowCount, rowCount + this.BatchSize + 1);
						this.ReportDataTable(rowCount, expectedRowCount, dataTable, e, true, sw.Elapsed);
					}
				}
			};
			try
			{
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, TimeSpan>((long)this.GetHashCode(), "-->DataTableLoader.OnDoRefreshWork: Fill: {0}. ElapsedTime:{1}", this, sw.Elapsed);
				this.Fill(e);
			}
			catch (MonadDataAdapterInvocationException ex)
			{
				if (!(ex.InnerException is ManagementObjectNotFoundException) && !(ex.InnerException is MapiObjectNotFoundException))
				{
					throw;
				}
			}
			finally
			{
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, TimeSpan>((long)this.GetHashCode(), "<--DataTableLoader.OnDoRefreshWork: Fill: {0}. ElapsedTime:{1}", this, sw.Elapsed);
				e.Result = dataTable;
				ExTraceGlobals.ProgramFlowTracer.TraceFunction((long)this.GetHashCode(), "-->DataTableLoader.OnDoRefreshWork: report last batch: {0}. dataTable.Rows.Count:{1}, this.BatchSize:{2}, rowCount:{3}, expectedRowCount:{4}", new object[]
				{
					this,
					dataTable.Rows.Count,
					this.BatchSize,
					rowCount,
					rowCount + 1
				});
				this.FillColumnsBasedOnLambdaExpression(dataTable, e.Argument as ResultsLoaderProfile);
				ColumnValueCalculator.CalculateAll(dataTable);
				if (!e.ReportedProgress)
				{
					ExTraceGlobals.ProgramFlowTracer.TracePerformance<DataTableLoader, string, string>(0L, "Time:{1}. {2} First batch data arrived in worker thread. {0}", this, ExDateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff"), (this != null) ? this.Table.TableName : string.Empty);
				}
				e.ReportProgress(rowCount, rowCount + 1, this.ProgressText, this.MoveRows(dataTable, null, false));
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.OnDoRefreshWork: report last batch: {0}", this);
			}
			base.OnDoRefreshWork(e);
			e.Result = dataTable;
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, TimeSpan>((long)this.GetHashCode(), "<--DataTableLoader.OnDoRefreshWork: {0}. Total ElapsedTime:{1}", this, sw.Elapsed);
		}

		private ExpressionCalculator GetExpressionCalculator()
		{
			if (this.Table != null && this.expressionCalculator == null)
			{
				this.expressionCalculator = ExpressionCalculator.Parse(this.Table);
			}
			return this.expressionCalculator;
		}

		private void FillColumnsBasedOnLambdaExpression(DataTable dataTable, ResultsLoaderProfile profile)
		{
			if (profile == null)
			{
				return;
			}
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				IList<KeyValuePair<string, object>> list = this.GetExpressionCalculator().CalculateAll(dataRow, profile.InputTable.Rows[0]);
				foreach (KeyValuePair<string, object> keyValuePair in list)
				{
					dataRow[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		private void Fill(RefreshRequestEventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "-->DataTableLoader.Fill: {0}", this);
			ResultsLoaderProfile profile = e.Argument as ResultsLoaderProfile;
			if (profile != null)
			{
				DataTable dataTable = e.Result as DataTable;
				dataTable.RowChanged += delegate(object sender, DataRowChangeEventArgs eventArgs)
				{
					if (eventArgs.Action == DataRowAction.Add)
					{
						this.FillPrimaryKeysBasedOnLambdaExpression(eventArgs.Row, profile);
					}
				};
				DataTable dataTable2 = dataTable.Clone();
				dataTable2.RowChanged += delegate(object sender, DataRowChangeEventArgs eventArgs)
				{
					if (eventArgs.Action == DataRowAction.Add)
					{
						this.FillPrimaryKeysBasedOnLambdaExpression(eventArgs.Row, profile);
					}
				};
				if (!this.EnforeViewEntireForest && !profile.HasPermission())
				{
					goto IL_26F;
				}
				using (DataAdapterExecutionContext dataAdapterExecutionContext = this.executionContextFactory.CreateExecutionContext())
				{
					dataAdapterExecutionContext.Open(base.UIService, this.WorkUnits, this.EnforeViewEntireForest, profile);
					foreach (AbstractDataTableFiller filler in profile.TableFillers)
					{
						if (profile.IsRunnable(filler))
						{
							if (e.CancellationPending)
							{
								break;
							}
							profile.BuildCommand(filler);
							if (profile.FillType == 1 || this.IsPreFillForResolving(filler))
							{
								dataAdapterExecutionContext.Execute(filler, dataTable2, profile);
								this.MergeChanges(dataTable2, dataTable);
								dataTable2.Clear();
							}
							else
							{
								dataAdapterExecutionContext.Execute(filler, dataTable, profile);
							}
						}
					}
					goto IL_26F;
				}
			}
			MonadCommand monadCommand = e.Argument as MonadCommand;
			if (monadCommand != null)
			{
				this.AttachCommandToMonitorWarnings(monadCommand);
				using (MonadConnection monadConnection = new MonadConnection(PSConnectionInfoSingleton.GetInstance().GetConnectionStringForScript(), new CommandInteractionHandler(), ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject(), PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo(ExchangeRunspaceConfigurationSettings.SerializationLevel.Full)))
				{
					monadConnection.Open();
					monadCommand.Connection = monadConnection;
					using (MonadDataAdapter monadDataAdapter = new MonadDataAdapter(monadCommand))
					{
						DataTable dataTable3 = (DataTable)e.Result;
						if (dataTable3.Columns.Count != 0)
						{
							monadDataAdapter.MissingSchemaAction = MissingSchemaAction.Ignore;
							monadDataAdapter.EnforceDataSetSchema = true;
						}
						ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, MonadCommand>((long)this.GetHashCode(), "-->DataTableLoader.Fill: calling dataAdapter.Fill: {0}. Command:{1}", this, monadCommand);
						monadDataAdapter.Fill(dataTable3);
						ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, MonadCommand>((long)this.GetHashCode(), "<--DataTableLoader.Fill: calling dataAdaptr.Fill: {0}. Command:{1}", this, monadCommand);
					}
				}
				this.DetachCommandFromMonitorWarnings(monadCommand);
			}
			IL_26F:
			this.OnFillTable(e);
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.Fill: {0}", this);
		}

		private void FillPrimaryKeysBasedOnLambdaExpression(DataRow dataRow, ResultsLoaderProfile profile)
		{
			ConvertTypeCalculator.Convert(dataRow);
			if (profile == null)
			{
				return;
			}
			foreach (DataColumn dataColumn in dataRow.Table.PrimaryKey)
			{
				IList<KeyValuePair<string, object>> list = this.GetExpressionCalculator().CalculateSpecifiedColumn(dataColumn.ColumnName, dataRow, profile.InputTable.Rows[0]);
				foreach (KeyValuePair<string, object> keyValuePair in list)
				{
					dataRow[keyValuePair.Key] = keyValuePair.Value;
				}
			}
		}

		private void MergeChanges(DataTable sourceTable, DataTable destinationTable)
		{
			foreach (object obj in sourceTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				DataRow dataRow2 = null;
				if (sourceTable.PrimaryKey != null && sourceTable.PrimaryKey.Length != 0)
				{
					List<object> list = new List<object>();
					foreach (DataColumn column in sourceTable.PrimaryKey)
					{
						list.Add(dataRow[column]);
					}
					dataRow2 = destinationTable.Rows.Find(list.ToArray());
				}
				if (dataRow2 != null)
				{
					using (IEnumerator enumerator2 = sourceTable.Columns.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							DataColumn dataColumn = (DataColumn)obj2;
							if (!dataColumn.DefaultValue.Equals(dataRow[dataColumn.ColumnName]))
							{
								dataRow2[dataColumn.ColumnName] = dataRow[dataColumn.ColumnName];
							}
						}
						continue;
					}
				}
				destinationTable.Rows.Add(dataRow.ItemArray);
			}
		}

		internal void AttachCommandToMonitorWarnings(MonadCommand command)
		{
			lock (this.WorkUnits)
			{
				WorkUnit workUnit;
				if (!this.TryGetWorkUnit(command.CommandText, out workUnit))
				{
					workUnit = new WorkUnit();
					workUnit.Target = command;
					workUnit.Text = command.CommandText;
					this.WorkUnits.Add(workUnit);
					command.WarningReport += this.command_WarningReport;
				}
			}
		}

		internal void DetachCommandFromMonitorWarnings(MonadCommand command)
		{
			lock (this.WorkUnits)
			{
				WorkUnit workUnit;
				if (this.TryGetWorkUnit(command.CommandText, out workUnit))
				{
					workUnit.Status = WorkUnitStatus.Completed;
					command.WarningReport -= this.command_WarningReport;
				}
			}
		}

		private void command_WarningReport(object sender, WarningReportEventArgs e)
		{
			lock (this.WorkUnits)
			{
				WorkUnit workUnit;
				if (this.TryGetWorkUnit(e.Command.CommandText, out workUnit) && workUnit.Target == e.Command)
				{
					workUnit.Warnings.Add(e.WarningMessage);
				}
			}
		}

		private bool TryGetWorkUnit(string text, out WorkUnit workUnit)
		{
			workUnit = null;
			for (int i = 0; i < this.WorkUnits.Count; i++)
			{
				if (this.WorkUnits[i].Text == text)
				{
					workUnit = this.WorkUnits[i];
					break;
				}
			}
			return null != workUnit;
		}

		protected virtual void OnFillTable(RefreshRequestEventArgs e)
		{
			RefreshRequestEventHandler refreshRequestEventHandler = (RefreshRequestEventHandler)base.Events[DataTableLoader.EventFillTable];
			if (refreshRequestEventHandler != null)
			{
				refreshRequestEventHandler(this, e);
			}
		}

		public event RefreshRequestEventHandler FillTable
		{
			add
			{
				base.Events.AddHandler(DataTableLoader.EventFillTable, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataTableLoader.EventFillTable, value);
			}
		}

		private void Cancel(RefreshRequestEventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "-->DataTableLoader.Cancel: {0}", this);
			IDbCommand dbCommand = e.Argument as IDbCommand;
			if (dbCommand != null)
			{
				ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader, IDbCommand>((long)this.GetHashCode(), "DataTableLoader.Cancel: {0} cancelling IDbCommand: {1}", this, dbCommand);
				dbCommand.Cancel();
			}
			ResultsLoaderProfile resultsLoaderProfile = e.Argument as ResultsLoaderProfile;
			if (resultsLoaderProfile != null)
			{
				foreach (AbstractDataTableFiller abstractDataTableFiller in resultsLoaderProfile.TableFillers)
				{
					abstractDataTableFiller.Cancel();
				}
			}
			this.OnCancelFill(e);
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.Cancel: {0}", this);
		}

		protected virtual void OnCancelFill(RefreshRequestEventArgs e)
		{
			RefreshRequestEventHandler refreshRequestEventHandler = (RefreshRequestEventHandler)base.Events[DataTableLoader.EventCancelFill];
			if (refreshRequestEventHandler != null)
			{
				refreshRequestEventHandler(this, e);
			}
		}

		public event RefreshRequestEventHandler CancelFill
		{
			add
			{
				base.Events.AddHandler(DataTableLoader.EventCancelFill, value);
			}
			remove
			{
				base.Events.RemoveHandler(DataTableLoader.EventCancelFill, value);
			}
		}

		private DataTable MoveRows(DataTable sourceTable, DataTable destinationTable, bool forceUseMergeTable)
		{
			if (sourceTable != null)
			{
				int count = sourceTable.Rows.Count;
				Stopwatch stopwatch = Stopwatch.StartNew();
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, bool>((long)this.GetHashCode(), "-->DataTableLoader.MoveRows: {0}. IsBackgroundThread:{1}", this, Thread.CurrentThread.IsBackground);
				if (forceUseMergeTable || (destinationTable != null && destinationTable.Columns.Count == 0))
				{
					destinationTable = ((destinationTable == null) ? sourceTable.Clone() : destinationTable);
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.MoveRows: {0} merging tables.", this);
					destinationTable.Merge(sourceTable);
				}
				else
				{
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.MoveRows: {0} BeginLoadData.", this);
					destinationTable = ((destinationTable == null) ? sourceTable.Clone() : destinationTable);
					destinationTable.BeginLoadData();
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.MoveRows: {0} Moving Rows.", this);
					DataRowCollection rows = destinationTable.Rows;
					DataRowCollection rows2 = sourceTable.Rows;
					for (int i = 0; i < count; i++)
					{
						rows.Add(rows2[i].ItemArray);
					}
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.MoveRows: {0} Moving Extended Properties.", this);
					foreach (object key in sourceTable.ExtendedProperties.Keys)
					{
						destinationTable.ExtendedProperties[key] = sourceTable.ExtendedProperties[key];
					}
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.MoveRows: {0} EndLoadData.", this);
					try
					{
						destinationTable.EndLoadData();
					}
					catch (ConstraintException ex)
					{
						List<string> list = new List<string>();
						foreach (object obj in destinationTable.Columns)
						{
							DataColumn dataColumn = (DataColumn)obj;
							list.Add(dataColumn.ColumnName);
						}
						throw new ConstraintException(ex.Message + "\r\nAll columns of this data table are:" + string.Join(" ", list.ToArray()), ex);
					}
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.MoveRows: {0} Clearing source table.", this);
					sourceTable.Clear();
				}
				ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, int, TimeSpan>((long)this.GetHashCode(), "<--DataTableLoader.MoveRows: {0}. sourceRowsCount:{1}, Elapsed Time: {2}", this, count, stopwatch.Elapsed);
			}
			return destinationTable;
		}

		protected override void OnProgressChanged(RefreshProgressChangedEventArgs e)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "-->DataTableLoader.OnProgressChanged: {0}", this);
			if (!e.CancellationPending)
			{
				this.RemoveExistingRows(e);
				DataTable dataTable = (DataTable)e.UserState;
				if (dataTable != null)
				{
					this.MoveRows(dataTable, this.Table, true);
				}
				base.OnProgressChanged(e);
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.OnProgressChanged: {0}", this);
		}

		protected virtual void RemoveExistingRows(RefreshProgressChangedEventArgs e)
		{
			if (e.IsFirstProgressReport)
			{
				ResultsLoaderProfile resultsLoaderProfile = e.RequestArgument as ResultsLoaderProfile;
				if (resultsLoaderProfile != null && resultsLoaderProfile.LoadableFromProfilePredicate != null)
				{
					for (int i = this.Table.Rows.Count - 1; i >= 0; i--)
					{
						DataRow row = this.Table.Rows[i];
						if (resultsLoaderProfile.IsLoadable(row))
						{
							this.Table.Rows.Remove(row);
						}
					}
					return;
				}
				if (e.IsFullRefresh)
				{
					ExTraceGlobals.ProgramFlowTracer.TraceDebug<DataTableLoader>((long)this.GetHashCode(), "DataTableLoader.OnProgressChanged: clearing table as this is the first progress report of this refresh. {0}", this);
					this.Table.Clear();
					return;
				}
				PartialRefreshRequestEventArgs partialRefreshRequestEventArgs = e.Request as PartialRefreshRequestEventArgs;
				if (partialRefreshRequestEventArgs != null)
				{
					DataTable dataTable = (DataTable)e.UserState;
					foreach (object identity in partialRefreshRequestEventArgs.Identities)
					{
						if (this.FindRowByIdentity(dataTable, identity) == null)
						{
							DataRow dataRow = this.FindRowByIdentity(this.Table, identity);
							if (dataRow != null)
							{
								this.Table.Rows.Remove(dataRow);
							}
						}
					}
				}
			}
		}

		internal DataRow FindRowByIdentity(DataTable table, object identity)
		{
			DataRow result = null;
			if (table != null && identity != null)
			{
				object[] array = this.ConvertToPrimaryKeysFromIdentity(identity);
				if (array != null)
				{
					result = table.Rows.Find(array);
				}
			}
			return result;
		}

		protected override void OnRefreshCompleted(RunWorkerCompletedEventArgs e)
		{
			this.lastRowCount = this.Table.Rows.Count;
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, int>((long)this.GetHashCode(), "*--DataTableLoader.OnRefreshCompleted: {0}. lastRowCount:{1}", this, this.lastRowCount);
			base.OnRefreshCompleted(e);
		}

		void ISupportFastRefresh.Refresh(IProgress progress, object id)
		{
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, object>((long)this.GetHashCode(), "-->DataTableLoader.RefreshSingleRow: {0}. ID:{1}", this, id);
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			this.Refresh(progress, new object[]
			{
				id
			}, 0);
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.RefreshSingleRow: {0}", this);
		}

		void ISupportFastRefresh.Refresh(IProgress progress, object[] identities, RefreshRequestPriority priority)
		{
			if (identities == null)
			{
				throw new ArgumentNullException("identities");
			}
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader, int>((long)this.GetHashCode(), "-->DataTableLoader.PartialRefresh: {0}. ID Count:{1}", this, identities.Length);
			if (this.table.PrimaryKey.Length == 0)
			{
				throw new InvalidOperationException("Must specify PrimaryKey of DataTable");
			}
			base.RefreshCore(progress, this.CreatePartialRowRefreshRequest(identities, progress, priority));
			ExTraceGlobals.ProgramFlowTracer.TraceFunction<DataTableLoader>((long)this.GetHashCode(), "<--DataTableLoader.PartialRefresh: {0}", this);
		}

		void ISupportFastRefresh.Remove(object identity)
		{
			this.FastRemoveImplement(identity);
		}

		protected virtual void FastRemoveImplement(object identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identities");
			}
			if (this.Table.PrimaryKey.Length == 0)
			{
				throw new InvalidOperationException();
			}
			object[] array = this.ConvertToPrimaryKeysFromIdentity(identity);
			if (array != null)
			{
				DataRow dataRow = this.Table.Rows.Find(array);
				if (dataRow != null)
				{
					this.Table.Rows.Remove(dataRow);
				}
			}
		}

		protected virtual object[] ConvertToPrimaryKeysFromIdentity(object identity)
		{
			if (identity is ADObjectId)
			{
				return new object[]
				{
					(identity as ADObjectId).ObjectGuid.ToString()
				};
			}
			if (this.ResultsLoaderProfile == null || string.IsNullOrEmpty(this.ResultsLoaderProfile.DistinguishIdentity))
			{
				return new object[]
				{
					identity
				};
			}
			DataRow dataRow = null;
			foreach (object obj in this.Table.Rows)
			{
				DataRow dataRow2 = (DataRow)obj;
				if (dataRow2[this.ResultsLoaderProfile.DistinguishIdentity].Equals(identity))
				{
					dataRow = dataRow2;
					break;
				}
			}
			if (dataRow != null)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < this.Table.PrimaryKey.Length; i++)
				{
					list.Add(dataRow[this.Table.PrimaryKey[i]]);
				}
				return list.ToArray();
			}
			return null;
		}

		private RefreshRequestEventArgs CreatePartialRowRefreshRequest(object[] ids, IProgress progress, RefreshRequestPriority priority)
		{
			object argument;
			if (this.TryToGetPartialRefreshArgument(ids, out argument))
			{
				return new DataTableLoader.DataTableLoaderPartialRefreshRequestEventArgs(this.Table.Clone(), progress, argument, ids, priority);
			}
			return this.CreateFullRefreshRequest(progress);
		}

		protected virtual bool TryToGetPartialRefreshArgument(object[] ids, out object partialRefreshArgument)
		{
			partialRefreshArgument = null;
			if (ids != null)
			{
				if (this.ResultsLoaderProfile != null)
				{
					ResultsLoaderProfile resultsLoaderProfile = null;
					if (1 != ids.Length || this.ResultsLoaderProfile.InputTable.Columns.Contains("Identity"))
					{
						resultsLoaderProfile = (this.ResultsLoaderProfile.Clone() as ResultsLoaderProfile);
						resultsLoaderProfile.TryInputValue("IsFullRefresh", false);
						if (1 == ids.Length)
						{
							resultsLoaderProfile.InputValue("Identity", ids[0]);
						}
						else if (this.ResultsLoaderProfile.InputTable.Columns.Contains("IdentityList"))
						{
							resultsLoaderProfile.InputValue("IdentityList", ids);
						}
						else
						{
							resultsLoaderProfile.PipelineObjects = ids;
						}
					}
					partialRefreshArgument = resultsLoaderProfile;
				}
				else
				{
					partialRefreshArgument = ids;
				}
			}
			return null != partialRefreshArgument;
		}

		protected override void DoPostRefreshAction(RefreshRequestEventArgs refreshRequest)
		{
			ResultsLoaderProfile resultsLoaderProfile = refreshRequest.Argument as ResultsLoaderProfile;
			if (resultsLoaderProfile != null && resultsLoaderProfile.PostRefreshAction != null)
			{
				resultsLoaderProfile.PostRefreshAction.DoPostRefreshAction(this, refreshRequest);
			}
		}

		public IProgress CreateProgress(string operationName)
		{
			IProgressProvider progressProvider = (IProgressProvider)this.GetService(typeof(IProgressProvider));
			if (progressProvider != null)
			{
				return progressProvider.CreateProgress(operationName);
			}
			return NullProgress.Value;
		}

		public override string ToString()
		{
			return base.GetType().Name;
		}

		private const int DefaultBatchSize = 100;

		private int batchSize = 100;

		private int expectedResultSize;

		private int lastRowCount;

		private LocalizedString progressText = LocalizedString.Empty;

		private LocalizedString refreshCommandText = LocalizedString.Empty;

		private DataTable table;

		private WorkUnitCollection workUnits;

		private ExpressionCalculator expressionCalculator;

		private IDataAdapterExecutionContextFactory executionContextFactory = new MonadDataAdapterExecutionContextFactory();

		private ResultsLoaderProfile resultsLoaderProfile;

		private int defaultExpectedResultSize = 100;

		private static readonly object EventExpectedResultSizeChanged = new object();

		private bool isCalculatingColumn;

		private static readonly object EventTableChanged = new object();

		private static readonly object EventFillTable = new object();

		private static readonly object EventCancelFill = new object();

		private interface IDataTableLoaderRefreshRequest
		{
			DataTable DataTable { get; }
		}

		private class DataTableLoaderRefreshRequestEventArgs : RefreshRequestEventArgs, DataTableLoader.IDataTableLoaderRefreshRequest
		{
			public DataTableLoaderRefreshRequestEventArgs(DataTable dataTable, bool isFullRefresh, IProgress progress, object argument) : base(isFullRefresh, progress, argument)
			{
				this.DataTable = dataTable;
			}

			public DataTable DataTable
			{
				get
				{
					return this.dataTable;
				}
				private set
				{
					this.dataTable = value;
				}
			}

			private DataTable dataTable;
		}

		private class DataTableLoaderPartialRefreshRequestEventArgs : PartialRefreshRequestEventArgs, DataTableLoader.IDataTableLoaderRefreshRequest
		{
			public DataTableLoaderPartialRefreshRequestEventArgs(DataTable dataTable, IProgress progress, object argument, object[] ids) : this(dataTable, progress, argument, ids, 0)
			{
			}

			public DataTableLoaderPartialRefreshRequestEventArgs(DataTable dataTable, IProgress progress, object argument, object[] ids, RefreshRequestPriority priority) : base(progress, argument, ids, priority)
			{
				this.DataTable = dataTable;
			}

			public DataTable DataTable
			{
				get
				{
					return this.dataTable;
				}
				private set
				{
					this.dataTable = value;
				}
			}

			private DataTable dataTable;
		}
	}
}
