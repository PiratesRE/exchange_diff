using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class Saver : RunnerBase
	{
		public Saver(string workUnitTextColumn, string workUnitIconColumn)
		{
			this.workUnitTextColumn = workUnitTextColumn;
			this.workUnitIconColumn = workUnitIconColumn;
		}

		[DefaultValue(null)]
		[DDIDataColumnExist]
		public string WorkUnitTextColumn
		{
			get
			{
				return this.workUnitTextColumn;
			}
			set
			{
				this.workUnitTextColumn = value;
			}
		}

		[DDIDataColumnExist]
		[DefaultValue(null)]
		public string WorkUnitIconColumn
		{
			get
			{
				return this.workUnitIconColumn;
			}
			set
			{
				this.workUnitIconColumn = value;
			}
		}

		public abstract void UpdateWorkUnits(DataRow row);

		public abstract object WorkUnits { get; }

		public abstract List<object> SavedResults { get; }

		public abstract string CommandToRun { get; }

		public abstract string ModifiedParametersDescription { get; }

		public virtual Saver CreateBulkSaver(WorkUnit[] workunits)
		{
			throw new NotSupportedException();
		}

		public virtual bool HasPermission(string propertyName, IList<ParameterProfile> parameters)
		{
			return true;
		}

		public virtual string GetConsumedDataObjectName()
		{
			return null;
		}

		private string workUnitTextColumn;

		private string workUnitIconColumn;
	}
}
