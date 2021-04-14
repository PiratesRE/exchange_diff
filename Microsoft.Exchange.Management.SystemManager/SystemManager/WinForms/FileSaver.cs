using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class FileSaver : Saver
	{
		public FileSaver() : base(string.Empty, string.Empty)
		{
			this.workUnits = new WorkUnitCollection();
			this.workUnit = new WorkUnit(string.Empty, null);
			this.workUnits.Add(this.workUnit);
			this.FilePathParameterName = "FilePath";
			this.savedResults = new List<object>();
		}

		[DDIDataColumnExist]
		public string FilePathParameterName { get; set; }

		public override List<object> SavedResults
		{
			get
			{
				return this.savedResults;
			}
		}

		public override object WorkUnits
		{
			get
			{
				return this.workUnits;
			}
		}

		public override string CommandToRun
		{
			get
			{
				return string.Empty;
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				return string.Empty;
			}
		}

		public override void Cancel()
		{
		}

		protected void OnStart()
		{
			this.workUnit.Errors.Clear();
			this.workUnit.Status = WorkUnitStatus.InProgress;
			this.workUnit.ExecutedCommandText = this.workUnit.Description;
		}

		protected void OnComplete(bool succeeded, Exception exception)
		{
			if (!succeeded)
			{
				int num = LocalizedException.GenerateErrorCode(exception);
				this.workUnit.Errors.Add(new ErrorRecord(exception, num.ToString("X"), ErrorCategory.OperationStopped, null));
			}
			this.workUnit.Status = (succeeded ? WorkUnitStatus.Completed : WorkUnitStatus.Failed);
		}

		public override void UpdateWorkUnits(DataRow row)
		{
			if (!string.IsNullOrEmpty(base.WorkUnitTextColumn))
			{
				this.workUnit.Text = row[base.WorkUnitTextColumn].ToString();
			}
			if (!string.IsNullOrEmpty(base.WorkUnitIconColumn))
			{
				this.workUnit.Icon = WinformsHelper.GetIconFromIconLibrary(row[base.WorkUnitIconColumn].ToString());
			}
		}

		protected WorkUnit workUnit;

		private WorkUnitCollection workUnits;

		private List<object> savedResults;
	}
}
