using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeSummaryControl : ExchangeUserControl
	{
		public ExchangeSummaryControl(SummaryControlStyle style, DescriptionControl descriptionControl)
		{
			this.InitializeComponent();
			switch (style)
			{
			case SummaryControlStyle.VariableDescriptionSize:
				this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
				this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
				break;
			case SummaryControlStyle.Percentage:
				this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
				this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
				break;
			case SummaryControlStyle.Percentage40:
				this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40f));
				this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60f));
				break;
			}
			this.summaryControlStyle = style;
			this.descriptionControl = descriptionControl;
			this.objectInfoCollection.ListChanged += this.objectInfoCollection_ListChanged;
			this.objectInfoCollection.ListChanging += this.objectInfoCollection_ListChanging;
		}

		public ExchangeSummaryControl() : this(SummaryControlStyle.VariableDescriptionSize, DescriptionControl.TextBox)
		{
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ChangeNotifyingCollection<GeneralPageSummaryInfo> SummaryInfoCollection
		{
			get
			{
				return this.objectInfoCollection;
			}
		}

		private void objectInfoCollection_ListChanged(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemAdded)
			{
				this.summaryPanel_InsertItem(e.NewIndex, this.SummaryInfoCollection[e.NewIndex]);
				return;
			}
			if (e.ListChangedType == ListChangedType.ItemChanged)
			{
				this.summaryPanel_RemoveItem(e.NewIndex, this.SummaryInfoCollection[e.NewIndex]);
				this.summaryPanel_InsertItem(e.NewIndex, this.SummaryInfoCollection[e.NewIndex]);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				for (int i = 0; i < this.SummaryInfoCollection.Count; i++)
				{
					this.summaryPanel_InsertItem(i, this.SummaryInfoCollection[i]);
				}
			}
		}

		private void objectInfoCollection_ListChanging(object sender, ListChangedEventArgs e)
		{
			if (e.ListChangedType == ListChangedType.ItemDeleted)
			{
				this.summaryPanel_RemoveItem(e.NewIndex, this.SummaryInfoCollection[e.NewIndex]);
				return;
			}
			if (e.ListChangedType == ListChangedType.Reset)
			{
				for (int i = this.SummaryInfoCollection.Count - 1; i >= 0; i--)
				{
					this.summaryPanel_RemoveItem(i, this.SummaryInfoCollection[i]);
				}
			}
		}

		private void summaryPanel_RemoveItem(int rowIndex, GeneralPageSummaryInfo objectInfo)
		{
			Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(0, rowIndex);
			Control controlFromPosition2 = this.tableLayoutPanel.GetControlFromPosition(1, rowIndex);
			this.tableLayoutPanel.Controls.Remove(controlFromPosition);
			this.tableLayoutPanel.Controls.Remove(controlFromPosition2);
			for (int i = rowIndex + 1; i < this.tableLayoutPanel.RowCount; i++)
			{
				Control controlFromPosition3 = this.tableLayoutPanel.GetControlFromPosition(0, i);
				if (controlFromPosition3 != null)
				{
					this.tableLayoutPanel.SetRow(controlFromPosition3, i - 1);
				}
				controlFromPosition3 = this.tableLayoutPanel.GetControlFromPosition(1, i);
				if (controlFromPosition3 != null)
				{
					this.tableLayoutPanel.SetRow(controlFromPosition3, i - 1);
				}
			}
			this.tableLayoutPanel.RowStyles.RemoveAt(this.tableLayoutPanel.RowCount - 1);
			this.tableLayoutPanel.RowCount--;
			if (rowIndex == 0 && this.GetControlFromPosition(0, 0) != null)
			{
				this.GetControlFromPosition(0, 0).Margin = this.firstRowMargin;
				this.GetControlFromPosition(1, 0).Margin = this.firstRowMargin;
			}
			objectInfo.BindingSourceChanged -= this.objectInfo_BindingSourceChanged;
			objectInfo.DataSourceChanged -= this.objectInfo_DataSourceChanged;
			objectInfo.PropertyNameChanged -= this.objectInfo_PropertyNameChanged;
			objectInfo.PropertyEmptyValueChanged -= this.objectInfo_PropertyEmptyValueChanged;
		}

		private void summaryPanel_InsertItem(int rowIndex, GeneralPageSummaryInfo objectInfo)
		{
			Label label = new Label();
			label.Name = objectInfo.PropertyName.Replace('.', '_') + "Label";
			label.Text = objectInfo.Text;
			label.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
			label.AutoSize = true;
			if (this.summaryControlStyle == SummaryControlStyle.VariableDescriptionSize)
			{
				int width = (int)Math.Floor(0.7 * (double)(this.DefaultSize.Width - base.Padding.Horizontal));
				label.MaximumSize = new Size(width, 0);
			}
			Control control = this.GetDescriptionControl(objectInfo);
			this.tableLayoutPanel.RowCount++;
			this.tableLayoutPanel.RowStyles.Insert(rowIndex, new RowStyle());
			for (int i = this.tableLayoutPanel.RowCount - 2; i >= rowIndex; i--)
			{
				Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(0, i);
				if (controlFromPosition != null)
				{
					this.tableLayoutPanel.SetRow(controlFromPosition, i + 1);
				}
				controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(1, i);
				if (controlFromPosition != null)
				{
					this.tableLayoutPanel.SetRow(controlFromPosition, i + 1);
				}
			}
			label.Margin = ((rowIndex == 0) ? this.firstRowMargin : this.nonFirstRowMargin);
			control.Margin = ((rowIndex == 0) ? this.firstRowMargin : this.nonFirstRowMargin);
			if (rowIndex == 0 && this.GetControlFromPosition(0, 0) != null)
			{
				this.GetControlFromPosition(0, 0).Margin = this.nonFirstRowMargin;
				this.GetControlFromPosition(1, 0).Margin = this.nonFirstRowMargin;
			}
			this.tableLayoutPanel.Controls.Add(label, 0, rowIndex);
			this.tableLayoutPanel.Controls.Add(control, 1, rowIndex);
			label.DataBindings.Add(new Binding("Text", objectInfo, "Text"));
			if (objectInfo.BindingSource != null && objectInfo.BindingSource.DataSource != null)
			{
				this.objectInfo_UpdateBinding(objectInfo);
			}
			objectInfo.BindingSourceChanged += this.objectInfo_BindingSourceChanged;
			objectInfo.DataSourceChanged += this.objectInfo_DataSourceChanged;
			objectInfo.PropertyNameChanged += this.objectInfo_PropertyNameChanged;
			objectInfo.PropertyEmptyValueChanged += this.objectInfo_PropertyEmptyValueChanged;
		}

		private Control GetDescriptionControl(GeneralPageSummaryInfo objectInfo)
		{
			Control result = null;
			switch (this.descriptionControl)
			{
			case DescriptionControl.Label:
				result = new Label
				{
					Name = objectInfo.PropertyName.Replace('.', '_') + "TextBox",
					Dock = DockStyle.Top,
					AutoSize = true
				};
				break;
			case DescriptionControl.TextBox:
				result = new ExchangeTextBox
				{
					Name = objectInfo.PropertyName.Replace('.', '_') + "TextBox",
					ReadOnly = true,
					BorderStyle = BorderStyle.None,
					Dock = DockStyle.Top,
					FormatMode = objectInfo.FormatMode
				};
				break;
			}
			return result;
		}

		private void objectInfo_DataSourceChanged(object sender, EventArgs e)
		{
			this.objectInfo_UpdateBinding((GeneralPageSummaryInfo)sender);
		}

		private void objectInfo_PropertyNameChanged(object sender, EventArgs e)
		{
			this.objectInfo_UpdateBinding((GeneralPageSummaryInfo)sender);
		}

		private void objectInfo_PropertyEmptyValueChanged(object sender, EventArgs e)
		{
			this.objectInfo_UpdateBinding((GeneralPageSummaryInfo)sender);
		}

		private void objectInfo_BindingSourceChanged(object sender, EventArgs e)
		{
			this.objectInfo_UpdateBinding((GeneralPageSummaryInfo)sender);
		}

		private void objectInfo_UpdateBinding(GeneralPageSummaryInfo objectInfo)
		{
			Control controlFromPosition = this.tableLayoutPanel.GetControlFromPosition(1, this.SummaryInfoCollection.IndexOf(objectInfo));
			if (controlFromPosition != null)
			{
				Binding binding = controlFromPosition.DataBindings["Text"];
				if (binding != null)
				{
					controlFromPosition.DataBindings.Remove(binding);
				}
				if (objectInfo.BindingSource != null && objectInfo.BindingSource.DataSource != null)
				{
					binding = new Binding("Text", objectInfo.BindingSource, objectInfo.PropertyName, true, DataSourceUpdateMode.Never, string.Empty);
					controlFromPosition.DataBindings.Add(binding);
					binding.Format += delegate(object sender, ConvertEventArgs e)
					{
						if (binding.DataSourceUpdateMode == DataSourceUpdateMode.Never && (e.Value == null || string.IsNullOrEmpty(e.Value.ToString())))
						{
							e.Value = objectInfo.PropertyEmptyValue;
						}
					};
					if (objectInfo.BindingSource.DataSource is Type)
					{
						controlFromPosition.Text = objectInfo.PropertyEmptyValue;
						return;
					}
					binding.ReadValue();
				}
			}
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			return this.tableLayoutPanel.GetPreferredSize(proposedSize);
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new TableLayoutPanel();
			base.SuspendLayout();
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.Dock = DockStyle.Top;
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.TabIndex = 0;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ExchangeSummaryControl";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(443, 496);
			}
		}

		public int RowCount
		{
			get
			{
				return this.tableLayoutPanel.RowCount;
			}
		}

		public Control GetControlFromPosition(int columnIndex, int rowIndex)
		{
			return this.tableLayoutPanel.GetControlFromPosition(columnIndex, rowIndex);
		}

		private SummaryControlStyle summaryControlStyle;

		private DescriptionControl descriptionControl;

		private TableLayoutPanel tableLayoutPanel;

		private ChangeNotifyingCollection<GeneralPageSummaryInfo> objectInfoCollection = new ChangeNotifyingCollection<GeneralPageSummaryInfo>();

		private Padding firstRowMargin = new Padding(0, 0, 0, 0);

		private Padding nonFirstRowMargin = new Padding(0, 12, 0, 0);
	}
}
