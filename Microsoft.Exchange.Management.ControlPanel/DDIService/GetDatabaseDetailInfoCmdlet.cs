using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.DDIService
{
	public class GetDatabaseDetailInfoCmdlet : CmdletActivity
	{
		public GetDatabaseDetailInfoCmdlet()
		{
			base.AllowExceuteThruHttpGetRequest = true;
		}

		protected GetDatabaseDetailInfoCmdlet(GetDatabaseDetailInfoCmdlet activity) : base(activity)
		{
		}

		public override Activity Clone()
		{
			return new GetDatabaseDetailInfoCmdlet(this);
		}

		public override RunResult Run(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind, Workflow.UpdateTableDelegate updateTableDelegate)
		{
			DDIHelper.Trace(TraceType.InfoTrace, "Executing :" + base.GetType().Name);
			object[] array = (object[])input["DeferLoadNames"];
			object[] array2 = (object[])input["DeferLoadIdentities"];
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			dataTable.BeginLoadData();
			for (int i = 0; i < array2.Length; i++)
			{
				object value = array2[i];
				object arg = array[i];
				base.Command = base.PowershellFactory.CreatePSCommand().AddCommand(base.GetCommandText(store));
				base.Command.AddParameter("Identity", string.Format("{0}\\*", arg));
				DDIHelper.Trace<IPSCommandWrapper>(TraceType.InfoTrace, base.Command);
				RunResult runResult = new RunResult();
				PowerShellResults<PSObject> powerShellResults;
				base.ExecuteCmdlet(null, runResult, out powerShellResults, false);
				base.StatusReport = powerShellResults;
				runResult.ErrorOccur = !base.StatusReport.Succeeded;
				if (!runResult.ErrorOccur && powerShellResults.Output != null && powerShellResults.Output.Length > 0)
				{
					string value2 = Strings.StatusUnknown.ToString();
					bool flag = false;
					int num = 0;
					PSObject[] output = powerShellResults.Output;
					int j = 0;
					while (j < output.Length)
					{
						PSObject psobject = output[j];
						DatabaseCopyStatusEntry databaseCopyStatusEntry = (DatabaseCopyStatusEntry)psobject.BaseObject;
						CopyStatus status = databaseCopyStatusEntry.Status;
						if (status == CopyStatus.Failed || status == CopyStatus.ServiceDown)
						{
							goto IL_186;
						}
						switch (status)
						{
						case CopyStatus.DisconnectedAndHealthy:
						case CopyStatus.FailedAndSuspended:
						case CopyStatus.DisconnectedAndResynchronizing:
						case CopyStatus.Misconfigured:
							goto IL_186;
						}
						IL_18C:
						if (databaseCopyStatusEntry.ActiveCopy)
						{
							switch (databaseCopyStatusEntry.Status)
							{
							case CopyStatus.Mounted:
							case CopyStatus.Mounting:
								value2 = Strings.StatusMounted.ToString();
								flag = true;
								break;
							case CopyStatus.Dismounted:
							case CopyStatus.Dismounting:
								value2 = Strings.StatusDismounted.ToString();
								flag = false;
								break;
							}
						}
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						list.Add(dictionary);
						dictionary["Identity"] = value;
						dictionary["CalcualtedMountStatus"] = value2;
						dictionary["CalcualtedMounted"] = flag;
						dictionary["CalcualtedBadCopies"] = num;
						j++;
						continue;
						IL_186:
						num++;
						goto IL_18C;
					}
				}
			}
			dataTable.Clear();
			foreach (Dictionary<string, object> dictionary2 in list)
			{
				DataRow dataRow = dataTable.NewRow();
				dataTable.Rows.Add(dataRow);
				dataRow["Identity"] = dictionary2["Identity"];
				dataRow["CalcualtedMountStatus"] = dictionary2["CalcualtedMountStatus"];
				dataRow["CalcualtedMounted"] = dictionary2["CalcualtedMounted"];
				dataRow["CalcualtedBadCopies"] = dictionary2["CalcualtedBadCopies"];
			}
			dataTable.EndLoadData();
			return new RunResult();
		}

		protected override void DoPreRunCore(DataRow input, DataTable dataTable, DataObjectStore store, Type codeBehind)
		{
		}

		protected override string GetVerb()
		{
			return "Get-";
		}
	}
}
