using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SaverTaskProfile : TaskProfileBase
	{
		[DDIDictionaryDecorate(UseKeys = false, AttributeType = typeof(DDIDataColumnExistAttribute))]
		[DefaultValue(null)]
		public SavedResultMapping SavedResultMapping
		{
			get
			{
				return this.savedResultMapping;
			}
			set
			{
				this.savedResultMapping = value;
			}
		}

		public List<object> SavedResults
		{
			get
			{
				return this.Saver.SavedResults;
			}
		}

		public bool IsSucceeded
		{
			get
			{
				return this.WorkUnits.AllCompleted;
			}
		}

		public void AddSavedResultMapping(string property, string column)
		{
			this.SavedResultMapping[property] = column;
		}

		internal override void Run(CommandInteractionHandler interactionHandler, DataRow row, DataObjectStore store)
		{
			this.Saver.Run(interactionHandler, row, store);
			if (this.Saver.SavedResults.Count > 0)
			{
				foreach (string text in this.SavedResultMapping.Keys)
				{
					object value = (text != "WholeObjectProperty") ? this.Saver.SavedResults[0].GetType().GetProperty(text).GetValue(this.Saver.SavedResults[0], null) : this.Saver.SavedResults[0];
					row[this.SavedResultMapping[text]] = value;
				}
			}
		}

		private Saver Saver
		{
			get
			{
				return base.Runner as Saver;
			}
		}

		public string CommandToRun
		{
			get
			{
				return this.Saver.CommandToRun;
			}
		}

		public string ModifiedParametersDescription
		{
			get
			{
				return this.Saver.ModifiedParametersDescription;
			}
		}

		internal void UpdateWorkUnits(DataRow row)
		{
			this.Saver.UpdateWorkUnits(row);
		}

		internal WorkUnitCollection WorkUnits
		{
			get
			{
				return this.Saver.WorkUnits as WorkUnitCollection;
			}
		}

		public bool HasPermission(string propertyName)
		{
			return (base.Runner as Saver).HasPermission(propertyName, base.GetEffectiveParameters());
		}

		private SavedResultMapping savedResultMapping = new SavedResultMapping();
	}
}
