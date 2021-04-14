using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ResultsLoaderProfile : ICloneable, IHasPermission
	{
		public ResultsLoaderProfile(string displayName, bool hideIcon, string imageProperty, string commandText, DataTable inputTable, DataTable dataTable, ResultsColumnProfile[] displayedColumnCollection, ICommandBuilder commandBuilder) : this(displayName, hideIcon, imageProperty, inputTable, dataTable, displayedColumnCollection)
		{
			MonadAdapterFiller filler = new MonadAdapterFiller(commandText, commandBuilder);
			this.AddTableFiller(filler);
		}

		public ResultsLoaderProfile(string displayName, string imageProperty, string commandText, DataTable inputTable, DataTable dataTable, ResultsColumnProfile[] displayedColumnCollection, ICommandBuilder commandBuilder) : this(displayName, false, imageProperty, commandText, inputTable, dataTable, displayedColumnCollection, commandBuilder)
		{
		}

		public ResultsLoaderProfile(string displayName, bool hideIcon, string imageProperty, DataTable inputTable, DataTable dataTable, ResultsColumnProfile[] displayedColumnCollection) : this(new UIPresentationProfile(displayedColumnCollection)
		{
			DisplayName = displayName,
			HideIcon = hideIcon,
			ImageProperty = imageProperty
		}, inputTable, dataTable)
		{
		}

		public ResultsLoaderProfile(UIPresentationProfile uiPresentationProfile, DataTable inputTable, DataTable dataTable)
		{
			this.UIPresentationProfile = (uiPresentationProfile ?? new UIPresentationProfile());
			this.dataTable = dataTable;
			this.inputTable = inputTable;
			this.inputTable.Rows.Add(inputTable.NewRow());
			this.tableFillers = new List<AbstractDataTableFiller>();
			this.TableFillers = new ReadOnlyCollection<AbstractDataTableFiller>(this.tableFillers);
			this.CommandsProfile = new ResultCommandsProfile();
		}

		public ResultsLoaderProfile(DataTable inputTable, DataTable dataTable) : this(null, inputTable, dataTable)
		{
		}

		public void AddTableFiller(AbstractDataTableFiller filler, string runnableLambdaExpression)
		{
			this.tableFillers.Add(filler);
			this.runnableLambdaExpressions[filler] = runnableLambdaExpression;
		}

		public void AddTableFiller(AbstractDataTableFiller filler)
		{
			this.AddTableFiller(filler, string.Empty);
		}

		public void InsertTableFiller(int index, AbstractDataTableFiller filler)
		{
			this.tableFillers.Insert(index, filler);
			this.runnableLambdaExpressions[filler] = string.Empty;
		}

		public void RemoveTableFiller(AbstractDataTableFiller filler)
		{
			this.tableFillers.Remove(filler);
		}

		public void ClearFiller()
		{
			this.tableFillers.Clear();
		}

		public ReadOnlyCollection<AbstractDataTableFiller> TableFillers { get; private set; }

		public string GetRunnableLambdaExpression(AbstractDataTableFiller filler)
		{
			if (!this.runnableLambdaExpressions.ContainsKey(filler))
			{
				return string.Empty;
			}
			return this.runnableLambdaExpressions[filler];
		}

		public bool IsRunnable(AbstractDataTableFiller filler)
		{
			bool flag = true;
			string runnableLambdaExpression = this.GetRunnableLambdaExpression(filler);
			if (!string.IsNullOrEmpty(runnableLambdaExpression))
			{
				ColumnExpression expression = ExpressionCalculator.BuildColumnExpression(runnableLambdaExpression);
				flag = (bool)ExpressionCalculator.CalculateLambdaExpression(expression, typeof(bool), null, this.InputTable.Rows[0]);
			}
			if (flag && this.IsResolving)
			{
				flag = (this.PipelineObjects != null && this.PipelineObjects.Length > 0);
			}
			return flag;
		}

		[DefaultValue(0)]
		public FillType FillType { get; set; }

		public int BatchSize
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

		public bool IsResolving { get; set; }

		[DefaultValue("")]
		public string DistinguishIdentity { get; set; }

		public string NameProperty
		{
			get
			{
				return this.nameProperty;
			}
			set
			{
				this.nameProperty = value;
			}
		}

		public string ResolveProperty
		{
			get
			{
				return this.resolveProperty;
			}
			set
			{
				this.resolveProperty = value;
			}
		}

		public List<object> ResolvedObjects
		{
			get
			{
				return this.resolvedObjects;
			}
			set
			{
				this.resolvedObjects = value;
			}
		}

		public string WholeObjectProperty
		{
			get
			{
				return this.wholeObjectProperty;
			}
			set
			{
				this.wholeObjectProperty = value;
			}
		}

		public DataTable InputTable
		{
			get
			{
				return this.inputTable;
			}
		}

		public IPartialOrderComparer InputTablePartialOrderComparer { get; set; }

		public DataTable DataTable
		{
			get
			{
				return this.dataTable;
			}
		}

		public IDataColumnsCalculator DataColumnsCalculator { get; set; }

		public void BuildCommand(AbstractDataTableFiller filler)
		{
			if (filler is MonadAdapterFiller)
			{
				(filler as MonadAdapterFiller).IsResolving = this.IsResolving;
			}
			if (this.IsResolving && filler.CommandBuilder != null)
			{
				filler.CommandBuilder.ResolveProperty = this.ResolveProperty;
			}
			if ((this.ScopeSupportingLevel == ScopeSupportingLevel.NoScoping && !this.UseTreeViewForm) || this.IsResolving)
			{
				filler.BuildCommand(this.SearchText, this.PipelineObjects, this.InputTable.Rows[0]);
				return;
			}
			filler.BuildCommandWithScope(this.SearchText, this.PipelineObjects, this.InputTable.Rows[0], this.Scope);
		}

		public ResultCommandsProfile CommandsProfile { get; set; }

		public object Clone()
		{
			return this.CloneInternal(false);
		}

		public object CloneWithSharedInputTable()
		{
			return this.CloneInternal(true);
		}

		private ResultsLoaderProfile CloneInternal(bool shareInputTable)
		{
			ResultsLoaderProfile resultsLoaderProfile = new ResultsLoaderProfile(new UIPresentationProfile(this.DisplayedColumnCollection), shareInputTable ? this.inputTable : this.inputTable.Copy(), this.dataTable);
			resultsLoaderProfile.Name = this.Name;
			resultsLoaderProfile.DisplayName = this.DisplayName;
			resultsLoaderProfile.HideIcon = this.HideIcon;
			resultsLoaderProfile.ImageProperty = this.ImageProperty;
			resultsLoaderProfile.SearchText = this.searchText;
			resultsLoaderProfile.PipelineObjects = this.PipelineObjects;
			resultsLoaderProfile.ScopeSupportingLevel = this.ScopeSupportingLevel;
			resultsLoaderProfile.Scope = this.Scope;
			resultsLoaderProfile.UseTreeViewForm = this.UseTreeViewForm;
			resultsLoaderProfile.IsResolving = this.IsResolving;
			resultsLoaderProfile.ResolveProperty = this.ResolveProperty;
			resultsLoaderProfile.WholeObjectProperty = this.WholeObjectProperty;
			resultsLoaderProfile.NameProperty = this.NameProperty;
			resultsLoaderProfile.LoadableFromProfilePredicate = this.LoadableFromProfilePredicate;
			resultsLoaderProfile.PostRefreshAction = this.PostRefreshAction;
			resultsLoaderProfile.SerializationLevel = this.SerializationLevel;
			resultsLoaderProfile.MultiSelect = this.MultiSelect;
			resultsLoaderProfile.FillType = this.FillType;
			foreach (AbstractDataTableFiller abstractDataTableFiller in this.TableFillers)
			{
				resultsLoaderProfile.AddTableFiller(abstractDataTableFiller.Clone() as AbstractDataTableFiller, this.GetRunnableLambdaExpression(abstractDataTableFiller));
			}
			resultsLoaderProfile.BatchSize = this.BatchSize;
			resultsLoaderProfile.CommandsProfile = new ResultCommandsProfile();
			resultsLoaderProfile.CommandsProfile.ResultPaneCommands.AddRange(this.CommandsProfile.ResultPaneCommands);
			resultsLoaderProfile.CommandsProfile.CustomSelectionCommands.AddRange(this.CommandsProfile.CustomSelectionCommands);
			resultsLoaderProfile.CommandsProfile.DeleteSelectionCommands.AddRange(this.CommandsProfile.DeleteSelectionCommands);
			resultsLoaderProfile.CommandsProfile.ShowSelectionPropertiesCommands.AddRange(this.CommandsProfile.ShowSelectionPropertiesCommands);
			resultsLoaderProfile.AutoGenerateColumns = this.AutoGenerateColumns;
			resultsLoaderProfile.DataColumnsCalculator = this.DataColumnsCalculator;
			resultsLoaderProfile.DistinguishIdentity = this.DistinguishIdentity;
			return resultsLoaderProfile;
		}

		public string SearchText
		{
			get
			{
				return this.searchText;
			}
			set
			{
				this.searchText = value;
			}
		}

		public object[] PipelineObjects
		{
			get
			{
				return this.pipelineObjects;
			}
			set
			{
				this.pipelineObjects = value;
			}
		}

		public object Scope
		{
			get
			{
				return this.rootId;
			}
			set
			{
				this.rootId = value;
			}
		}

		public DataTable CreateResultsDataTable()
		{
			DataTable dataTable = this.DataTable.Clone();
			dataTable.TableName = (string.IsNullOrEmpty(this.DataTable.TableName) ? base.GetType().Name : this.DataTable.TableName);
			dataTable.Locale = CultureInfo.InvariantCulture;
			return dataTable;
		}

		public void InputValue(string columnName, object value)
		{
			if (!this.InputTable.Columns.Contains(columnName))
			{
				throw new ArgumentException("The column {0} you try to access doesn't exist.", columnName);
			}
			this.TryInputValue(columnName, value);
		}

		public bool TryInputValue(string columnName, object value)
		{
			if (this.InputTable.Columns.Contains(columnName))
			{
				if (value == null && this.InputTable.Columns[columnName].DataType.IsValueType)
				{
					value = DBNull.Value;
				}
				this.InputTable.Rows[0][columnName] = value;
				return true;
			}
			return false;
		}

		public object GetValue(string columnName)
		{
			if (!this.InputTable.Columns.Contains(columnName))
			{
				throw new ArgumentException("The column {0} you try to access doesn't exist.", columnName);
			}
			object obj = this.InputTable.Rows[0][columnName];
			if (DBNull.Value.Equals(obj))
			{
				return null;
			}
			return obj;
		}

		public bool IsLoadable(DataRow row)
		{
			if (this.LoadableFromProfilePredicate == null)
			{
				throw new InvalidOperationException("LoadableFromPredicate has no value!");
			}
			return this.LoadableFromProfilePredicate.IsLoadableFrom(this, row);
		}

		public ILoadableFromProfile LoadableFromProfilePredicate { get; set; }

		public PostRefreshActionBase PostRefreshAction { get; set; }

		public UIPresentationProfile UIPresentationProfile { get; private set; }

		public bool AutoGenerateColumns
		{
			get
			{
				return this.UIPresentationProfile.AutoGenerateColumns;
			}
			set
			{
				this.UIPresentationProfile.AutoGenerateColumns = value;
			}
		}

		public ResultsColumnProfile[] DisplayedColumnCollection
		{
			get
			{
				return this.UIPresentationProfile.DisplayedColumnCollection;
			}
		}

		public ExchangeColumnHeader[] CreateColumnHeaders()
		{
			return this.UIPresentationProfile.CreateColumnHeaders();
		}

		public bool HideIcon
		{
			get
			{
				return this.UIPresentationProfile.HideIcon;
			}
			set
			{
				this.UIPresentationProfile.HideIcon = value;
			}
		}

		internal ObjectSchema FilterObjectSchema
		{
			get
			{
				ObjectSchema result = this.UIPresentationProfile.FilterObjectSchema;
				if (this.UIPresentationProfile.FilterLanguage == FilterLanguage.Ado)
				{
					IList<PropertyDefinition> list = new List<PropertyDefinition>();
					foreach (FilterablePropertyDescription filterablePropertyDescription in this.UIPresentationProfile.FilterableProperties.Values)
					{
						list.Add(filterablePropertyDescription.PropertyDefinition);
					}
					result = new ADOFilterObjectSchema(list);
				}
				return result;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.UIPresentationProfile.DisplayName;
			}
			set
			{
				this.UIPresentationProfile.DisplayName = value;
			}
		}

		public string ImageProperty
		{
			get
			{
				return this.UIPresentationProfile.ImageProperty;
			}
			set
			{
				this.UIPresentationProfile.ImageProperty = value;
			}
		}

		public string SortProperty
		{
			get
			{
				return this.UIPresentationProfile.SortProperty;
			}
			set
			{
				this.UIPresentationProfile.SortProperty = value;
			}
		}

		public bool UseTreeViewForm
		{
			get
			{
				return this.UIPresentationProfile.UseTreeViewForm;
			}
			set
			{
				this.UIPresentationProfile.UseTreeViewForm = value;
			}
		}

		public string HelpTopic
		{
			get
			{
				return this.UIPresentationProfile.HelpTopic;
			}
			set
			{
				this.UIPresentationProfile.HelpTopic = value;
			}
		}

		public ScopeSupportingLevel ScopeSupportingLevel
		{
			get
			{
				return this.UIPresentationProfile.ScopeSupportingLevel;
			}
			set
			{
				this.UIPresentationProfile.ScopeSupportingLevel = value;
			}
		}

		public ExchangeRunspaceConfigurationSettings.SerializationLevel SerializationLevel
		{
			get
			{
				return this.UIPresentationProfile.SerializationLevel;
			}
			set
			{
				this.UIPresentationProfile.SerializationLevel = value;
			}
		}

		public bool MultiSelect
		{
			get
			{
				return this.UIPresentationProfile.MultiSelect;
			}
			set
			{
				this.UIPresentationProfile.MultiSelect = value;
			}
		}

		public string Name { get; set; }

		public bool HasPermission()
		{
			bool flag = false;
			if (this.FillType == null)
			{
				using (List<AbstractDataTableFiller>.Enumerator enumerator = this.tableFillers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						AbstractDataTableFiller abstractDataTableFiller = enumerator.Current;
						MonadAdapterFiller monadAdapterFiller = abstractDataTableFiller as MonadAdapterFiller;
						if (monadAdapterFiller == null || monadAdapterFiller.HasPermission())
						{
							flag = true;
							break;
						}
					}
					goto IL_9B;
				}
			}
			flag = true;
			foreach (AbstractDataTableFiller abstractDataTableFiller2 in this.tableFillers)
			{
				MonadAdapterFiller monadAdapterFiller2 = abstractDataTableFiller2 as MonadAdapterFiller;
				if (monadAdapterFiller2 != null && !monadAdapterFiller2.HasPermission())
				{
					flag = false;
					break;
				}
			}
			IL_9B:
			if (string.Equals(this.Name, "MailboxMigration", StringComparison.OrdinalIgnoreCase))
			{
				flag = (EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope("Get-MoveRequest", new string[]
				{
					"Identity",
					"ResultSize"
				}) && EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope("Get-MoveRequestStatistics", new string[]
				{
					"Identity"
				}));
			}
			else if (string.Equals(this.Name, "Database", StringComparison.OrdinalIgnoreCase))
			{
				flag = ((EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope("Get-MailboxDatabase", new string[]
				{
					"Identity"
				}) && EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope("Get-MailboxDatabaseCopyStatus", new string[]
				{
					"Identity",
					"Server"
				})) || EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope("Get-PublicFolderDatabase", new string[]
				{
					"Identity",
					"Status"
				}));
			}
			else if (flag && string.Equals(this.Name, "DisconnectedMailbox", StringComparison.OrdinalIgnoreCase))
			{
				flag = EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope("Get-ExchangeServer", new string[]
				{
					"Identity"
				});
			}
			return flag;
		}

		private const string ColumnDoesnotExist = "The column {0} you try to access doesn't exist.";

		private DataTable inputTable;

		private DataTable dataTable;

		private string searchText;

		private object[] pipelineObjects;

		private object rootId;

		private string resolveProperty;

		private string nameProperty = "Name";

		private string wholeObjectProperty;

		private List<AbstractDataTableFiller> tableFillers;

		private Dictionary<AbstractDataTableFiller, string> runnableLambdaExpressions = new Dictionary<AbstractDataTableFiller, string>();

		private int batchSize = ResultsLoaderProfile.DefaultBatchSize;

		public static readonly int DefaultBatchSize = 100;

		private List<object> resolvedObjects = new List<object>();
	}
}
