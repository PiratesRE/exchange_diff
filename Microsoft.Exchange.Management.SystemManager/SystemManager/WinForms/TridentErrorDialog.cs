using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class TridentErrorDialog : BaseErrorDialog
	{
		public TridentErrorDialog()
		{
			this.InitializeComponent();
			Label label = new Label();
			label.AutoSize = true;
			label.Dock = DockStyle.Left;
			label.Name = "copyLabel";
			label.Text = Strings.CopyNote;
			base.ButtonsPanel.Controls.Add(label);
			base.ButtonsPanel.Controls.SetChildIndex(label, 0);
			this.tridentsPanel = new WorkUnitsPanel();
			this.tridentsPanel.Name = "tridentsPanel";
			this.tridentsPanel.Dock = DockStyle.Fill;
			this.tridentsPanel.Margin = new Padding(0);
			this.tridentsPanel.TaskState = 2;
			base.ContentPanel.Controls.Add(this.tridentsPanel);
			base.ContentPanel.Controls.SetChildIndex(this.tridentsPanel, 0);
			this.Text = Strings.TridentErrorDialogTitle(UIService.DefaultCaption);
		}

		[DefaultValue(null)]
		internal IList<WorkUnit> Errors
		{
			get
			{
				return this.tridentsPanel.WorkUnits;
			}
			set
			{
				if (value != null)
				{
					foreach (WorkUnit workUnit in value)
					{
						workUnit.CanShowElapsedTime = false;
						workUnit.CanShowExecutedCommand = false;
					}
				}
				this.tridentsPanel.WorkUnits = value;
			}
		}

		protected override void OnIsWarningOnlyChanged()
		{
			base.OnIsWarningOnlyChanged();
			this.Text = (base.IsWarningOnly ? Strings.TridentWarningDialogTitle(UIService.DefaultCaption) : Strings.TridentErrorDialogTitle(UIService.DefaultCaption));
		}

		public override string TechnicalDetails
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("--------------------------------------------------------");
				stringBuilder.AppendLine(this.Text);
				stringBuilder.AppendLine("--------------------------------------------------------");
				stringBuilder.AppendLine(base.Message);
				if (this.Errors != null)
				{
					foreach (WorkUnit workUnit in this.Errors)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(workUnit.Summary);
					}
				}
				stringBuilder.AppendLine("--------------------------------------------------------");
				stringBuilder.AppendLine(Strings.Ok);
				stringBuilder.AppendLine("--------------------------------------------------------");
				return stringBuilder.ToString();
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			this.Errors = null;
			base.OnClosed(e);
		}

		private WorkUnitsPanel tridentsPanel;
	}
}
