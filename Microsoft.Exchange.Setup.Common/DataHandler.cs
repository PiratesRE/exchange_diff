using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Setup.Common
{
	public class DataHandler
	{
		public DataHandler()
		{
		}

		public DataHandler(ICloneable dataSource) : this()
		{
			this.DataSource = dataSource.Clone();
		}

		public DataHandler(bool breakOnError) : this()
		{
			this.BreakOnError = breakOnError;
		}

		public bool IsObjectReadOnly
		{
			get
			{
				return this.isObjectReadOnly;
			}
			set
			{
				this.isObjectReadOnly = value;
				if (!this.isObjectReadOnly)
				{
					this.objectReadOnlyReason = string.Empty;
				}
			}
		}

		public string ObjectReadOnlyReason
		{
			get
			{
				return this.objectReadOnlyReason;
			}
			set
			{
				this.objectReadOnlyReason = value;
			}
		}

		protected virtual void CheckObjectReadOnly()
		{
		}

		public bool HasDataHandlers
		{
			get
			{
				return this.dataHandlers != null && this.dataHandlers.Count > 0;
			}
		}

		public IList<DataHandler> DataHandlers
		{
			get
			{
				if (this.dataHandlers == null)
				{
					this.dataHandlers = new List<DataHandler>();
				}
				return this.dataHandlers;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		public List<object> SavedResults
		{
			get
			{
				if (this.savedResults == null)
				{
					this.savedResults = new List<object>();
				}
				return this.savedResults;
			}
		}

		internal void Read(CommandInteractionHandler interactionHandler, string pageName)
		{
			if (!this.Cancelled)
			{
				CancelEventArgs cancelEventArgs = new CancelEventArgs();
				this.OnReadingData(cancelEventArgs);
				if (!cancelEventArgs.Cancel)
				{
					this.OnReadData(interactionHandler, pageName);
					this.CheckObjectReadOnly();
				}
			}
		}

		protected virtual void OnReadingData(CancelEventArgs e)
		{
			if (this.ReadingData != null)
			{
				this.ReadingData(this, e);
			}
		}

		public event CancelEventHandler ReadingData;

		internal virtual void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			if (this.HasDataHandlers)
			{
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (this.Cancelled)
					{
						break;
					}
					dataHandler.Read(interactionHandler, pageName);
				}
				this.DataSource = this.DataHandlers[0].DataSource;
			}
		}

		internal void Save(CommandInteractionHandler interactionHandler)
		{
			if (!this.ReadOnly)
			{
				if (this.HasWorkUnits)
				{
					foreach (WorkUnit workUnit in this.WorkUnits)
					{
						workUnit.ResetStatus();
					}
				}
				if (!this.HasWorkUnits)
				{
					this.UpdateWorkUnits();
				}
				if (!this.Cancelled)
				{
					CancelEventArgs cancelEventArgs = new CancelEventArgs();
					this.OnSavingData(cancelEventArgs);
					if (!cancelEventArgs.Cancel)
					{
						this.OnSaveData(interactionHandler);
						if (!this.HasWorkUnits || !this.WorkUnits.HasFailures)
						{
							this.ClearParameterNames();
						}
					}
				}
			}
		}

		protected virtual void OnSavingData(CancelEventArgs e)
		{
			if (this.SavingData != null)
			{
				this.SavingData(this, e);
			}
		}

		public event CancelEventHandler SavingData;

		internal virtual void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			if (this.HasDataHandlers)
			{
				this.SavedResults.Clear();
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (dataHandler.IsModified())
					{
						if (this.Cancelled)
						{
							break;
						}
						dataHandler.ProgressReport += this.OnProgressReport;
						try
						{
							dataHandler.Save(interactionHandler);
						}
						finally
						{
							dataHandler.ProgressReport -= this.OnProgressReport;
						}
						this.SavedResults.AddRange(dataHandler.SavedResults);
						if (this.BreakOnError && !dataHandler.IsSucceeded)
						{
							break;
						}
					}
				}
			}
		}

		internal virtual string CommandToRun
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (dataHandler.IsModified())
					{
						string commandToRun = dataHandler.CommandToRun;
						if (!string.IsNullOrEmpty(commandToRun))
						{
							stringBuilder.Append(commandToRun);
						}
					}
				}
				return stringBuilder.ToString();
			}
		}

		protected bool BreakOnError
		{
			get
			{
				return this.breakOnError;
			}
			set
			{
				this.breakOnError = value;
			}
		}

		public virtual ValidationError[] Validate()
		{
			return this.ValidateOnly(null);
		}

		public virtual ValidationError[] ValidateOnly(object objectToBeValidated)
		{
			List<ValidationError> list = new List<ValidationError>();
			IConfigurable configurable = this.DataSource as IConfigurable;
			if (configurable != null && (objectToBeValidated == null || configurable == objectToBeValidated))
			{
				ValidationError[] array = configurable.Validate();
				if (array != null)
				{
					list.AddRange(array);
				}
			}
			if (this.HasDataHandlers)
			{
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (dataHandler.DataSource != this.DataSource && (objectToBeValidated == null || dataHandler.DataSource == objectToBeValidated))
					{
						ValidationError[] array2 = dataHandler.Validate();
						if (array2 != null)
						{
							list.AddRange(array2);
						}
					}
				}
			}
			return list.ToArray();
		}

		public object DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
			}
		}

		public bool HasWorkUnits
		{
			get
			{
				return this.workUnits != null && this.workUnits.Count > 0;
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

		public virtual void UpdateWorkUnits()
		{
			if (this.HasDataHandlers)
			{
				if (this.HasWorkUnits)
				{
					this.WorkUnits.Clear();
				}
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (dataHandler.IsModified())
					{
						dataHandler.UpdateWorkUnits();
						if (dataHandler.HasWorkUnits)
						{
							foreach (WorkUnit item in dataHandler.WorkUnits)
							{
								this.WorkUnits.Add(item);
							}
						}
					}
				}
			}
		}

		public virtual string CompletionStatus
		{
			get
			{
				if (this.HasWorkUnits)
				{
					return this.WorkUnits.Description;
				}
				return "";
			}
		}

		public virtual string CompletionDescription
		{
			get
			{
				return " ";
			}
		}

		public virtual string InProgressDescription
		{
			get
			{
				return " ";
			}
		}

		public virtual bool IsSucceeded
		{
			get
			{
				return this.WorkUnits.AllCompleted;
			}
		}

		public bool Cancelled
		{
			get
			{
				return this.cancelled;
			}
		}

		public virtual void Cancel()
		{
			this.cancelled = true;
			if (this.HasDataHandlers)
			{
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					dataHandler.Cancel();
				}
			}
		}

		public void ResetCancel()
		{
			this.cancelled = false;
			if (this.HasDataHandlers)
			{
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					dataHandler.ResetCancel();
				}
			}
		}

		public virtual bool CanCancel
		{
			get
			{
				return true;
			}
		}

		public virtual bool OverrideCorruptedValuesWithDefault()
		{
			bool flag = false;
			if (this.HasDataHandlers)
			{
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (dataHandler.DataSource != this.DataSource && dataHandler.DataSource is ADObject)
					{
						flag |= dataHandler.OverrideCorruptedValuesWithDefault();
					}
				}
			}
			return flag;
		}

		public virtual bool IsCorrupted
		{
			get
			{
				bool flag = false;
				IConfigurable configurable = this.DataSource as IConfigurable;
				if (configurable != null)
				{
					flag = !configurable.IsValid;
				}
				if (this.HasDataHandlers)
				{
					foreach (DataHandler dataHandler in this.DataHandlers)
					{
						flag |= dataHandler.IsCorrupted;
					}
				}
				return flag;
			}
		}

		protected virtual bool IsModified()
		{
			if (this.HasDataHandlers)
			{
				foreach (DataHandler dataHandler in this.DataHandlers)
				{
					if (dataHandler.IsModified())
					{
						return true;
					}
				}
				return true;
			}
			return true;
		}

		protected HashSet<string> ParameterNames
		{
			get
			{
				if (this.parameterNames == null)
				{
					this.parameterNames = new HashSet<string>();
				}
				return this.parameterNames;
			}
		}

		internal void ClearParameterNames()
		{
			this.ParameterNames.Clear();
		}

		internal virtual void SpecifyParameterNames(Dictionary<object, List<string>> bindingMembers)
		{
			if (this.HasDataHandlers)
			{
				using (IEnumerator<DataHandler> enumerator = this.DataHandlers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataHandler dataHandler = enumerator.Current;
						dataHandler.SpecifyParameterNames(bindingMembers);
					}
					return;
				}
			}
			if (this.DataSource != null && bindingMembers.ContainsKey(this.DataSource))
			{
				this.ParameterNames.UnionWith(bindingMembers[this.DataSource]);
			}
		}

		internal void SpecifyParameterNames(string parameterName)
		{
			this.SpecifyParameterNames(new List<string>
			{
				parameterName
			});
		}

		internal void SpecifyParameterNames(List<string> parameterNames)
		{
			if (this.DataSource != null)
			{
				this.SpecifyParameterNames(new Dictionary<object, List<string>>
				{
					{
						this.DataSource,
						parameterNames
					}
				});
			}
		}

		internal event EventHandler<ProgressReportEventArgs> ProgressReport;

		internal void OnProgressReport(object sender, ProgressReportEventArgs e)
		{
			if (this.ProgressReport != null)
			{
				this.ProgressReport(sender, e);
			}
		}

		internal virtual bool TimeConsuming
		{
			get
			{
				return false;
			}
		}

		public virtual string ModifiedParametersDescription
		{
			get
			{
				return null;
			}
		}

		private bool cancelled;

		private bool readOnly;

		private object dataSource;

		private List<object> savedResults;

		private List<DataHandler> dataHandlers;

		private bool isObjectReadOnly;

		private string objectReadOnlyReason = string.Empty;

		private bool breakOnError = true;

		private WorkUnitCollection workUnits;

		private HashSet<string> parameterNames;
	}
}
