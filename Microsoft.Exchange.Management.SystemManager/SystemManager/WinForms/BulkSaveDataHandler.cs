using System;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class BulkSaveDataHandler : SingleTaskDataHandler
	{
		public BulkSaveDataHandler(WorkUnit[] workUnits, string saveCommand) : base(saveCommand)
		{
			base.WorkUnits.AddRange(workUnits);
		}

		public override void UpdateWorkUnits()
		{
			base.UpdateWorkUnits();
			foreach (WorkUnit workUnit in base.WorkUnits)
			{
				workUnit.ResetStatus();
			}
		}

		internal override string CommandToRun
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				bool flag = true;
				foreach (WorkUnit workUnit in base.WorkUnits)
				{
					if (!flag)
					{
						stringBuilder.Append(",");
					}
					flag = false;
					stringBuilder.Append(MonadCommand.FormatParameterValue(workUnit.Target));
				}
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" | ");
				}
				stringBuilder.Append(base.CommandToRun);
				return stringBuilder.ToString();
			}
		}

		protected override void HandleIdentityParameter()
		{
		}
	}
}
