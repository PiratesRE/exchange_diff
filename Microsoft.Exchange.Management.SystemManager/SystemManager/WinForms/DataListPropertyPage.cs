using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataListPropertyPage : ExchangePropertyPageControl
	{
		public DataListPropertyPage()
		{
			this.InitializeComponent();
		}

		public DataListControl DataListControl
		{
			get
			{
				return this.dataListControl;
			}
		}

		protected DataListView DataListView
		{
			get
			{
				return this.DataListControl.DataListView;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.dataListControl = new DataListControl();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoLayout = true;
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.ContainerType = ContainerType.PropertyPage;
			this.tableLayoutPanel.Controls.Add(this.dataListControl, 0, 0);
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Margin = new Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(13, 12, 16, 12);
			this.tableLayoutPanel.RowCount = 1;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(418, 396);
			this.tableLayoutPanel.TabIndex = 0;
			this.dataListControl.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.dataListControl.AutoSize = true;
			this.dataListControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.dataListControl.DataListView.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			this.dataListControl.DataListView.DataSourceRefresher = null;
			this.dataListControl.DataListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			this.dataListControl.DataListView.Location = new Point(3, 0);
			this.dataListControl.DataListView.Margin = new Padding(3, 0, 0, 0);
			this.dataListControl.DataListView.Name = "dataListView";
			this.dataListControl.DataListView.Size = new Size(386, 372);
			this.dataListControl.DataListView.TabIndex = 4;
			this.dataListControl.DataListView.UseCompatibleStateImageBehavior = false;
			this.dataListControl.Location = new Point(13, 12);
			this.dataListControl.Margin = new Padding(0);
			this.dataListControl.Name = "dataListControl";
			this.dataListControl.Size = new Size(389, 372);
			this.dataListControl.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 13f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "DataListPropertyPage";
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private IContainer components;

		private AutoTableLayoutPanel tableLayoutPanel;

		private DataListControl dataListControl;
	}
}
