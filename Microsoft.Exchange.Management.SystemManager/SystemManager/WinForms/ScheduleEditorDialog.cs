using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public partial class ScheduleEditorDialog : ExchangeDialog
	{
		public ScheduleEditorDialog()
		{
			this.InitializeComponent();
			this.Text = Strings.ScheduleEditorDialogTitle;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Schedule Schedule
		{
			get
			{
				return this.scheduleEditor.Schedule;
			}
			set
			{
				this.scheduleEditor.Schedule = value;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			int num = base.ClientSize.Height - base.ButtonsPanel.Height;
			int height = this.tableLayoutPanel.Height;
			int num2 = num - height;
			base.Height -= num2;
			base.OnLoad(e);
		}
	}
}
