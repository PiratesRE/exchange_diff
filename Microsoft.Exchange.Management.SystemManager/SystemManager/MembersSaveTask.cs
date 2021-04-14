using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MembersSaveTask : Saver
	{
		public MembersSaveTask() : this(null, null)
		{
		}

		public MembersSaveTask(string workUnitTextColumn, string workUnitIconColumn) : base(workUnitTextColumn, workUnitIconColumn)
		{
			this.dataHandler = new ExchangeDataHandler(false);
		}

		public override void Cancel()
		{
			this.dataHandler.Cancel();
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.dataHandler.ProgressReport += base.OnProgressReport;
			try
			{
				this.dataHandler.Save(interactionHandler as CommandInteractionHandler);
				if (this.dataHandler.HasWorkUnits && !this.dataHandler.WorkUnits.HasFailures)
				{
					MultiValuedProperty<ADObjectId> multiValuedProperty = row["Members"] as MultiValuedProperty<ADObjectId>;
					if (multiValuedProperty != null)
					{
						multiValuedProperty.ResetChangeTracking();
					}
				}
			}
			finally
			{
				this.dataHandler.ProgressReport -= base.OnProgressReport;
			}
		}

		public override bool IsRunnable(DataRow row, DataObjectStore store)
		{
			MultiValuedProperty<ADObjectId> multiValuedProperty = row["Members"] as MultiValuedProperty<ADObjectId>;
			return multiValuedProperty != null && (multiValuedProperty.Added.Length > 0 || multiValuedProperty.Removed.Length > 0) && base.IsRunnable(row, store);
		}

		public override object WorkUnits
		{
			get
			{
				return this.dataHandler.WorkUnits;
			}
		}

		public override List<object> SavedResults
		{
			get
			{
				return this.dataHandler.SavedResults;
			}
		}

		public override void UpdateWorkUnits(DataRow row)
		{
			ADObjectId identity = row["Identity"] as ADObjectId;
			this.members = (row["Members"] as MultiValuedProperty<ADObjectId>);
			if (this.members != null && (this.members.Added.Length > 0 || this.members.Removed.Length > 0))
			{
				this.dataHandler.DataHandlers.Clear();
				foreach (ADObjectId member in this.members.Added)
				{
					this.dataHandler.DataHandlers.Add(this.CreateDataHandler("Add-DistributionGroupMember", identity, member));
				}
				foreach (ADObjectId member2 in this.members.Removed)
				{
					this.dataHandler.DataHandlers.Add(this.CreateDataHandler("Remove-DistributionGroupMember", identity, member2));
				}
			}
			this.dataHandler.UpdateWorkUnits();
			this.dataHandler.ResetCancel();
		}

		public override string CommandToRun
		{
			get
			{
				return this.dataHandler.CommandToRun;
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (this.members != null)
				{
					if (this.members.Added.Length > 0)
					{
						stringBuilder.AppendLine(string.Format("Members: {0} {1}", Strings.AddMembersToGroup, MonadCommand.FormatParameterValue(this.members.Added)));
					}
					if (this.members.Removed.Length > 0)
					{
						stringBuilder.AppendLine(string.Format("Members: {0} {1}", Strings.RemoveMembersFromGroup, MonadCommand.FormatParameterValue(this.members.Removed)));
					}
				}
				return stringBuilder.ToString();
			}
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
		}

		private SingleTaskDataHandler CreateDataHandler(string commandText, ADObjectId identity, ADObjectId member)
		{
			SingleTaskDataHandler singleTaskDataHandler;
			if (this.workUnits.Length > 0)
			{
				singleTaskDataHandler = new BulkSaveDataHandler(this.workUnits.DeepCopy(), commandText);
			}
			else
			{
				singleTaskDataHandler = new SingleTaskDataHandler(commandText);
				singleTaskDataHandler.Parameters.AddWithValue("Identity", identity);
			}
			singleTaskDataHandler.Parameters.AddWithValue("Member", member);
			return singleTaskDataHandler;
		}

		public override Saver CreateBulkSaver(WorkUnit[] workUnits)
		{
			this.workUnits = workUnits;
			return this;
		}

		internal void UpdateConnection(MonadConnection connection)
		{
			foreach (DataHandler dataHandler in this.dataHandler.DataHandlers)
			{
				SingleTaskDataHandler singleTaskDataHandler = (SingleTaskDataHandler)dataHandler;
				singleTaskDataHandler.Command.Connection = connection;
			}
		}

		private DataHandler dataHandler;

		private WorkUnit[] workUnits = new WorkUnit[0];

		private MultiValuedProperty<ADObjectId> members;
	}
}
