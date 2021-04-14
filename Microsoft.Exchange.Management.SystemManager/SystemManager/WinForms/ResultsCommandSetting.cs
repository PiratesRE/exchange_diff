using System;
using Microsoft.ManagementGUI.Commands;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ResultsCommandSetting : CommandUtil
	{
		public ResultsCommandProfile Profile
		{
			get
			{
				return base.Profile as ResultsCommandProfile;
			}
			protected internal set
			{
				base.Profile = value;
			}
		}

		public ResultPane ResultPane
		{
			get
			{
				if (this.Profile != null)
				{
					return this.Profile.ResultPane;
				}
				return null;
			}
		}

		public CommandOperation Operation
		{
			get
			{
				return this.operation;
			}
			set
			{
				if (this.Operation != value)
				{
					this.operation = value;
					this.UpdateUseSingleRowRefresh();
				}
			}
		}

		public bool IsSelectionCommand
		{
			get
			{
				return this.isSelectionCommand;
			}
			set
			{
				if (this.IsSelectionCommand != value)
				{
					this.isSelectionCommand = value;
					this.UpdateUseSingleRowRefresh();
					this.UpdateDefaultUpdatingUtil();
				}
			}
		}

		private void UpdateUseSingleRowRefresh()
		{
			this.UseSingleRowRefresh = (this.IsSelectionCommand && this.Operation != CommandOperation.Create);
		}

		public bool RequiresSingleSelection { get; set; }

		public bool RequiresMultiSelection { get; set; }

		public bool IsPropertiesCommand { get; set; }

		public bool IsRemoveCommand { get; set; }

		public bool UseSingleRowRefresh
		{
			get
			{
				return this.useSingleRowRefresh;
			}
			set
			{
				if (this.UseSingleRowRefresh != value)
				{
					this.useSingleRowRefresh = value;
					if (this.UseSingleRowRefresh)
					{
						this.UseFullRefresh = false;
					}
				}
			}
		}

		public bool UseFullRefresh
		{
			get
			{
				return this.useFullRefresh;
			}
			set
			{
				if (this.UseFullRefresh != value)
				{
					this.useFullRefresh = value;
					if (this.UseFullRefresh)
					{
						this.UseSingleRowRefresh = false;
					}
				}
			}
		}

		protected override void OnProfileUpdated()
		{
			base.OnProfileUpdated();
			this.UpdateDefaultUpdatingUtil();
		}

		private void UpdateDefaultUpdatingUtil()
		{
			if (this.Profile != null)
			{
				if (this.IsSelectionCommand && this.Profile.UpdatingUtil == null)
				{
					this.Profile.UpdatingUtil = this.defaultSelectionCommandUpdatingUtil;
					return;
				}
				if (!this.IsSelectionCommand && this.Profile.UpdatingUtil == this.defaultSelectionCommandUpdatingUtil)
				{
					this.Profile.UpdatingUtil = null;
				}
			}
		}

		private CommandOperation operation;

		private bool isSelectionCommand;

		private bool useSingleRowRefresh;

		private bool useFullRefresh;

		private ResultsCommandUpdatingUtil defaultSelectionCommandUpdatingUtil = new SelectionCommandUpdatingUtil();
	}
}
