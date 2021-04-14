using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms.Design;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class AutomatedDataHandler : AutomatedDataHandlerBase
	{
		internal IList<ReaderTaskProfile> ReaderTaskProfileList
		{
			get
			{
				return this.readerTaskProfileList;
			}
		}

		internal ICommandExecutionContextFactory ReaderExecutionContextFactory
		{
			get
			{
				return this.readerExecutionContextFactory;
			}
			set
			{
				this.readerExecutionContextFactory = value;
			}
		}

		internal ICommandExecutionContextFactory SaverExecutionContextFactory
		{
			get
			{
				return this.saverExecutionContextFactory;
			}
			set
			{
				this.saverExecutionContextFactory = value;
			}
		}

		public IList<SaverTaskProfile> SaverTaskProfileList
		{
			get
			{
				return this.saverTaskProfileList;
			}
		}

		public AutomatedDataHandler(Assembly resourceContainedAssembly, string resourceName, WorkUnit[] workUnits) : base(resourceContainedAssembly, resourceName)
		{
			this.readerTaskProfileList = base.ProfileBuilder.BuildReaderTaskProfile();
			this.saverTaskProfileList = base.ProfileBuilder.BuildSaverTaskProfile();
			this.pageToReaderTaskMapping = new PageToReaderTaskMapping(this.readerTaskProfileList, base.ProfileBuilder.BuildPageToDataObjectsMapping());
			if (workUnits != null)
			{
				base.EnableBulkEdit = true;
				foreach (ReaderTaskProfile readerTaskProfile in this.readerTaskProfileList)
				{
					readerTaskProfile.Runner = (readerTaskProfile.Runner as Reader).CreateBulkReader(readerTaskProfile.DataObjectName, base.DataObjectStore, base.Table);
				}
				foreach (SaverTaskProfile saverTaskProfile in this.saverTaskProfileList)
				{
					saverTaskProfile.Runner = (saverTaskProfile.Runner as Saver).CreateBulkSaver(workUnits.DeepCopy());
				}
				base.BreakOnError = false;
			}
		}

		internal override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			if (this.pageToReaderTaskMapping.IsExecuted(pageName))
			{
				return;
			}
			if (base.Table.Rows.Count == 0)
			{
				base.Table.Rows.Add(base.Table.NewRow());
			}
			else
			{
				base.Table = base.Table.Copy();
			}
			using (CommandExecutionContext commandExecutionContext = this.CreateExecutionContextForReader())
			{
				WinFormsCommandInteractionHandler winFormsCommandInteractionHandler = interactionHandler as WinFormsCommandInteractionHandler;
				IUIService service = (winFormsCommandInteractionHandler == null) ? null : winFormsCommandInteractionHandler.UIService;
				commandExecutionContext.Open(service);
				foreach (ReaderTaskProfile readerTaskProfile in this.readerTaskProfileList)
				{
					if (readerTaskProfile.IsRunnable(base.Row, base.DataObjectStore) && this.pageToReaderTaskMapping.CanTaskExecuted(pageName, readerTaskProfile.Name))
					{
						readerTaskProfile.BuildParameters(base.Row, base.DataObjectStore);
						commandExecutionContext.Execute(readerTaskProfile, base.Row, base.DataObjectStore);
						base.DataObjectStore.UpdateDataObject(readerTaskProfile.DataObjectName, readerTaskProfile.DataObject);
						base.UpdateTable(base.Row, readerTaskProfile.DataObjectName, true);
					}
				}
			}
			if (this.pageToReaderTaskMapping.Count == 0)
			{
				base.Row.AcceptChanges();
			}
			this.pageToReaderTaskMapping.Execute(pageName);
			base.DataSource = base.Table;
		}

		private CommandExecutionContext CreateExecutionContextForReader()
		{
			if (!base.EnableBulkEdit)
			{
				return this.readerExecutionContextFactory.CreateExecutionContext();
			}
			return new DummyExecutionContext();
		}

		public void InputValue(string columnName, object value)
		{
			base.Table.Columns[columnName].DefaultValue = value;
		}

		public override void UpdateWorkUnits()
		{
			base.WorkUnits.Clear();
			foreach (SaverTaskProfile saverTaskProfile in this.SaverTaskProfileList)
			{
				if (saverTaskProfile.IsRunnable(base.Row, base.DataObjectStore))
				{
					saverTaskProfile.BuildParameters(base.Row, base.DataObjectStore);
					saverTaskProfile.UpdateWorkUnits(base.Row);
					base.WorkUnits.AddRange(saverTaskProfile.WorkUnits);
				}
			}
		}

		public override void Cancel()
		{
			base.Cancel();
			foreach (SaverTaskProfile saverTaskProfile in this.saverTaskProfileList)
			{
				saverTaskProfile.Runner.Cancel();
			}
		}

		internal override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			base.SavedResults.Clear();
			using (CommandExecutionContext commandExecutionContext = this.saverExecutionContextFactory.CreateExecutionContext())
			{
				WinFormsCommandInteractionHandler winFormsCommandInteractionHandler = interactionHandler as WinFormsCommandInteractionHandler;
				IUIService service = (winFormsCommandInteractionHandler == null) ? null : winFormsCommandInteractionHandler.UIService;
				commandExecutionContext.Open(service);
				foreach (SaverTaskProfile saverTaskProfile in this.saverTaskProfileList)
				{
					if (base.Cancelled)
					{
						break;
					}
					if (saverTaskProfile.IsRunnable(base.Row, base.DataObjectStore))
					{
						saverTaskProfile.BuildParameters(base.Row, base.DataObjectStore);
						try
						{
							saverTaskProfile.Runner.ProgressReport += base.OnProgressReport;
							commandExecutionContext.Execute(saverTaskProfile, base.Row, base.DataObjectStore);
						}
						finally
						{
							saverTaskProfile.Runner.ProgressReport -= base.OnProgressReport;
						}
						base.SavedResults.AddRange(saverTaskProfile.SavedResults);
						if (base.BreakOnError && !saverTaskProfile.IgnoreException && !saverTaskProfile.IsSucceeded)
						{
							break;
						}
					}
				}
				if (!base.HasWorkUnits || !base.WorkUnits.HasFailures)
				{
					if (commandExecutionContext.ShouldReload)
					{
						this.pageToReaderTaskMapping.Reset();
					}
					base.DataObjectStore.ClearModifiedColumns();
				}
			}
		}

		internal override string CommandToRun
		{
			get
			{
				this.UpdateWorkUnits();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SaverTaskProfile saverTaskProfile in this.saverTaskProfileList)
				{
					if (saverTaskProfile.IsRunnable(base.Row, base.DataObjectStore))
					{
						saverTaskProfile.BuildParameters(base.Row, base.DataObjectStore);
						stringBuilder.Append(saverTaskProfile.CommandToRun);
					}
				}
				return stringBuilder.ToString();
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				this.UpdateWorkUnits();
				StringBuilder stringBuilder = new StringBuilder();
				foreach (SaverTaskProfile saverTaskProfile in this.saverTaskProfileList)
				{
					if (saverTaskProfile.IsRunnable(base.Row, base.DataObjectStore))
					{
						saverTaskProfile.BuildParameters(base.Row, base.DataObjectStore);
						stringBuilder.Append(saverTaskProfile.ModifiedParametersDescription);
					}
				}
				return stringBuilder.ToString();
			}
		}

		internal override bool TimeConsuming
		{
			get
			{
				return base.EnableBulkEdit;
			}
		}

		internal override bool HasViewPermissionForPage(string pageName)
		{
			if (EnvironmentAnalyzer.IsWorkGroup() || !base.ProfileBuilder.CanEnableUICustomization())
			{
				return true;
			}
			if (this.pageToReaderTaskMapping.ContainsKey(pageName))
			{
				return (from c in this.readerTaskProfileList
				where this.pageToReaderTaskMapping[pageName].Contains(c.Name)
				select c).Any((ReaderTaskProfile c) => c.HasPermission());
			}
			return true;
		}

		internal override bool HasPermissionForProperty(string propertyName, bool canUpdate)
		{
			if (EnvironmentAnalyzer.IsWorkGroup() || !base.ProfileBuilder.CanEnableUICustomization())
			{
				return true;
			}
			ColumnProfile columnProfile = (from DataColumn c in base.Table.Columns
			where c.ColumnName == propertyName
			select c).First<DataColumn>().ExtendedProperties["ColumnProfile"] as ColumnProfile;
			string dataObjectName = columnProfile.DataObjectName;
			IEnumerable<ReaderTaskProfile> source = from c in this.readerTaskProfileList
			where c.DataObjectName == dataObjectName
			select c;
			ReaderTaskProfile readerTaskProfile = null;
			if (source.Count<ReaderTaskProfile>() > 0)
			{
				readerTaskProfile = source.First<ReaderTaskProfile>();
			}
			if (readerTaskProfile != null && !readerTaskProfile.HasPermission())
			{
				return false;
			}
			if (canUpdate)
			{
				if (columnProfile.IgnoreChangeTracking)
				{
					return true;
				}
				IEnumerable<SaverTaskProfile> source2 = from c in this.saverTaskProfileList
				where (c.Runner as Saver).GetConsumedDataObjectName() == dataObjectName && !string.IsNullOrEmpty(dataObjectName)
				select c;
				SaverTaskProfile saverTaskProfile = null;
				if (source2.Count<SaverTaskProfile>() > 0)
				{
					saverTaskProfile = source2.First<SaverTaskProfile>();
				}
				if (saverTaskProfile == null)
				{
					IEnumerable<SaverTaskProfile> enumerable = from c in this.saverTaskProfileList
					where (from p in c.ParameterProfileList
					where p.Reference == propertyName
					select p).Count<ParameterProfile>() > 0
					select c;
					using (IEnumerator<SaverTaskProfile> enumerator = enumerable.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SaverTaskProfile saverTaskProfile2 = enumerator.Current;
							if (!saverTaskProfile2.HasPermission(columnProfile.MappingProperty))
							{
								return false;
							}
						}
						return true;
					}
				}
				if (saverTaskProfile != null && !saverTaskProfile.HasPermission(columnProfile.MappingProperty))
				{
					return false;
				}
			}
			return true;
		}

		private IList<ReaderTaskProfile> readerTaskProfileList;

		private IList<SaverTaskProfile> saverTaskProfileList;

		private PageToReaderTaskMapping pageToReaderTaskMapping;

		private ICommandExecutionContextFactory readerExecutionContextFactory = new MonadCommandExecutionContextFactory();

		private ICommandExecutionContextFactory saverExecutionContextFactory = new MonadCommandExecutionContextFactory();
	}
}
