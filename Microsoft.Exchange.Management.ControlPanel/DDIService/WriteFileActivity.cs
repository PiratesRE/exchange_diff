using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Security;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class WriteFileActivity : Activity
	{
		public WriteFileActivity() : base("WriteFileActivity")
		{
		}

		protected WriteFileActivity(WriteFileActivity activity) : base(activity)
		{
			this.InputVariable = activity.InputVariable;
			this.OutputFileNameVariable = activity.OutputFileNameVariable;
		}

		public override Activity Clone()
		{
			return new WriteFileActivity(this);
		}

		[DDIMandatoryValue]
		public string InputVariable { get; set; }

		[DDIMandatoryValue]
		public string OutputFileNameVariable { get; set; }

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			DataRow dataRow = dataTable.Rows[0];
			string value = (string)input[this.InputVariable];
			string text = (string)input[this.OutputFileNameVariable];
			RunResult runResult = new RunResult();
			try
			{
				runResult.ErrorOccur = true;
				if (!text.EndsWith(WriteFileActivity.textExtension))
				{
					text += WriteFileActivity.textExtension;
				}
				using (StreamWriter streamWriter = new StreamWriter(File.Open(text, FileMode.CreateNew)))
				{
					streamWriter.WriteLine(value);
				}
				runResult.ErrorOccur = false;
			}
			catch (UnauthorizedAccessException e)
			{
				this.HandleFileSaveException(e);
			}
			catch (DirectoryNotFoundException e2)
			{
				this.HandleFileSaveException(e2);
			}
			catch (PathTooLongException e3)
			{
				this.HandleFileSaveException(e3);
			}
			catch (SecurityException e4)
			{
				this.HandleFileSaveException(e4);
			}
			catch (IOException e5)
			{
				this.HandleFileSaveException(e5);
			}
			return runResult;
		}

		public override PowerShellResults[] GetStatusReport(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (base.ErrorBehavior == ErrorBehavior.SilentlyContinue)
			{
				this.errorRecords.Clear();
			}
			PowerShellResults[] array = new PowerShellResults[]
			{
				new PowerShellResults()
			};
			array[0].ErrorRecords = this.errorRecords.ToArray();
			return array;
		}

		private void HandleFileSaveException(Exception e)
		{
			this.errorRecords.Add(new ErrorRecord(e));
		}

		private static readonly string textExtension = ".txt";

		private List<ErrorRecord> errorRecords = new List<ErrorRecord>();
	}
}
