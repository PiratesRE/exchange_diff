using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ConditionsEditorControl : BindableUserControl
	{
		public ConditionsEditorControl()
		{
			this.InitializeComponent();
			this.conditionListView.VirtualMode = false;
			this.conditionListView.AutoGenerateColumns = false;
			this.conditionListView.AvailableColumns.Add("Description", "Description", true);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public IList<ConditionDescriptor> ConditionDescriptors
		{
			get
			{
				return this.conditionListView.DataSource as IList<ConditionDescriptor>;
			}
			set
			{
				if (this.conditionListView.DataSource != value)
				{
					this.conditionListView.DataSource = value;
					this.InitializePhrasePresentationControl();
				}
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BindingList<PhraseDescriptor> PhraseDescriptors
		{
			get
			{
				return this.phraseDescriptors;
			}
			set
			{
				if (this.PhraseDescriptors != value)
				{
					this.DetachEventsWithPhraseDescriptors();
					this.phraseDescriptors = value;
					this.InitializePhrasePresentationControl();
					this.AttachEventsWithPhraseDescriptors();
				}
			}
		}

		private void AttachEventsWithPhraseDescriptors()
		{
			if (this.PhraseDescriptors != null)
			{
				this.PhraseDescriptors.ListChanged += this.phraseDescriptors_ListChanged;
				foreach (PhraseDescriptor phraseDescriptor in this.PhraseDescriptors)
				{
					phraseDescriptor.PhraseEditingPropertyUpdated += this.PhraseDescriptor_PhraseEditingPropertyUpdated;
				}
			}
		}

		private void DetachEventsWithPhraseDescriptors()
		{
			if (this.PhraseDescriptors != null)
			{
				this.PhraseDescriptors.ListChanged -= this.phraseDescriptors_ListChanged;
				foreach (PhraseDescriptor phraseDescriptor in this.PhraseDescriptors)
				{
					phraseDescriptor.PhraseEditingPropertyUpdated -= this.PhraseDescriptor_PhraseEditingPropertyUpdated;
				}
			}
		}

		private void PhraseDescriptor_PhraseEditingPropertyUpdated(object sender, PropertyChangedEventArgs e)
		{
			base.UpdateError();
			this.OnEditingPropertyUpdated(e.PropertyName);
		}

		public event PropertyChangedEventHandler EditingPropertyUpdated;

		private void OnEditingPropertyUpdated(string propertyName)
		{
			if (this.EditingPropertyUpdated != null)
			{
				this.EditingPropertyUpdated(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public bool HasConditionsSelected
		{
			get
			{
				bool result = false;
				if (this.ConditionDescriptors != null && this.PhraseDescriptors != null)
				{
					foreach (ConditionDescriptor conditionDescriptor in this.ConditionDescriptors)
					{
						if (this.GetPhraseByIndex(conditionDescriptor.Index).Used)
						{
							result = true;
							break;
						}
					}
				}
				return result;
			}
		}

		private void phraseDescriptors_ListChanged(object sender, ListChangedEventArgs e)
		{
			BindingList<PhraseDescriptor> bindingList = sender as BindingList<PhraseDescriptor>;
			if (e.ListChangedType == ListChangedType.ItemChanged)
			{
				this.UpdatePhrasePresentation(bindingList[e.NewIndex]);
			}
		}

		private void InitializePhrasePresentationControl()
		{
			if (this.PhraseDescriptors != null && this.ConditionDescriptors != null)
			{
				this.ClearPhrasePresentationControl();
				foreach (PhraseDescriptor phraseDescriptor in this.PhraseDescriptors)
				{
					this.UpdatePhrasePresentation(phraseDescriptor);
				}
			}
		}

		private void ClearPhrasePresentationControl()
		{
			this.sentencePanel.SuspendLayout();
			try
			{
				for (int i = this.sentencePanel.Controls.Count - 1; i >= 0; i--)
				{
					Control control = this.sentencePanel.Controls[i];
					if (control is ConditionsEditorControl.PhrasePresentationControl)
					{
						this.sentencePanel.Controls.Remove(control);
						control.Dispose();
					}
				}
			}
			finally
			{
				this.sentencePanel.ResumeLayout(true);
			}
		}

		private void conditionListView_UpdateItem(object sender, ItemCheckedEventArgs e)
		{
			PhraseDescriptor phraseByListViewItem = this.GetPhraseByListViewItem(e.Item);
			if (phraseByListViewItem != null)
			{
				e.Item.Checked = phraseByListViewItem.Used;
			}
		}

		private void conditionListView_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			ConditionDescriptor conditionDescriptor = this.conditionListView.GetRowFromItem(this.conditionListView.Items[e.Index]) as ConditionDescriptor;
			if (conditionDescriptor != null)
			{
				PhraseDescriptor phraseByIndex = this.GetPhraseByIndex(conditionDescriptor.Index);
				if (phraseByIndex != null)
				{
					phraseByIndex.Used = (e.NewValue == CheckState.Checked);
				}
			}
			base.UpdateError();
		}

		[DefaultValue("")]
		public string SentenceDescriptionText
		{
			get
			{
				return this.sentenceDescriptionLabel.Text;
			}
			set
			{
				this.sentenceDescriptionLabel.Text = value;
			}
		}

		[DefaultValue("")]
		public string FirstStepText
		{
			get
			{
				return this.step1Label.Text;
			}
			set
			{
				this.step1Label.Text = value;
			}
		}

		[DefaultValue("")]
		public string SecondStepText
		{
			get
			{
				return this.step2Label.Text;
			}
			set
			{
				this.step2Label.Text = value;
			}
		}

		private void UpdatePhrasePresentation(PhraseDescriptor phraseDescriptor)
		{
			if (phraseDescriptor != null)
			{
				this.UpdateConditionItem(phraseDescriptor);
				ConditionsEditorControl.PhrasePresentationControl phrasePresentationControl = this.GetPhrasePresentationControlByPhraseDescriptor(phraseDescriptor);
				if (phraseDescriptor.Used && phrasePresentationControl == null)
				{
					this.sentencePanel.SuspendLayout();
					phrasePresentationControl = new ConditionsEditorControl.PhrasePresentationControl(phraseDescriptor);
					this.sentencePanel.Controls.Add(phrasePresentationControl);
					this.sentencePanel.Controls.SetChildIndex(phrasePresentationControl, this.GetIndexOfPhrasePresentationControl(phrasePresentationControl));
					this.UpdatePhrasePresentationControlPrefix();
					this.sentencePanel.ResumeLayout(true);
					this.sentencePanel.ScrollControlIntoView(phrasePresentationControl);
					return;
				}
				if (!phraseDescriptor.Used && phrasePresentationControl != null)
				{
					this.sentencePanel.SuspendLayout();
					this.sentencePanel.Controls.Remove(phrasePresentationControl);
					this.UpdatePhrasePresentationControlPrefix();
					this.sentencePanel.ResumeLayout(true);
					phrasePresentationControl.Dispose();
				}
			}
		}

		private void UpdatePhrasePresentationControlPrefix()
		{
			int previousGroupID = int.MinValue;
			for (int i = this.sentencePanel.Controls.Count - 1; i >= 0; i--)
			{
				ConditionsEditorControl.PhrasePresentationControl phrasePresentationControl = this.sentencePanel.Controls[i] as ConditionsEditorControl.PhrasePresentationControl;
				if (phrasePresentationControl != null && phrasePresentationControl.PhraseDescriptor != null)
				{
					phrasePresentationControl.UpdateText(previousGroupID);
					previousGroupID = phrasePresentationControl.PhraseDescriptor.GroupID;
				}
			}
		}

		private void UpdateConditionItem(PhraseDescriptor phraseDescriptor)
		{
			if (this.ConditionDescriptors != null && phraseDescriptor != null)
			{
				ConditionDescriptor conditionByIndex = this.GetConditionByIndex(phraseDescriptor.Index);
				if (conditionByIndex != null)
				{
					ListViewItem itemFromRow = this.conditionListView.GetItemFromRow(conditionByIndex);
					if (itemFromRow != null)
					{
						itemFromRow.Checked = phraseDescriptor.Used;
					}
				}
			}
		}

		private int GetIndexOfPhrasePresentationControl(ConditionsEditorControl.PhrasePresentationControl phraseControl)
		{
			int i;
			for (i = 0; i < this.sentencePanel.Controls.Count - 1; i++)
			{
				ConditionsEditorControl.PhrasePresentationControl phrasePresentationControl = this.sentencePanel.Controls[i] as ConditionsEditorControl.PhrasePresentationControl;
				if (phrasePresentationControl == null || phraseControl.PhraseDescriptor.Index >= phrasePresentationControl.PhraseDescriptor.Index)
				{
					break;
				}
			}
			return i;
		}

		protected override UIValidationError[] GetValidationErrors()
		{
			List<UIValidationError> list = new List<UIValidationError>();
			if (this.ConditionDescriptors != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (ConditionDescriptor conditionDescriptor in this.ConditionDescriptors)
				{
					PhraseDescriptor phraseByIndex = this.GetPhraseByIndex(conditionDescriptor.Index);
					if (phraseByIndex != null)
					{
						ValidationError[] array = phraseByIndex.Validate();
						foreach (ValidationError validationError in array)
						{
							stringBuilder.AppendLine(validationError.Description);
						}
					}
				}
				if (stringBuilder.Length > 0)
				{
					list.Add(new UIValidationError(new LocalizedString(stringBuilder.ToString()), this.sentencePanel));
				}
			}
			return list.ToArray();
		}

		private PhraseDescriptor GetPhraseByListViewItem(ListViewItem listViewItem)
		{
			PhraseDescriptor result = null;
			ConditionDescriptor conditionDescriptor = this.conditionListView.GetRowFromItem(listViewItem) as ConditionDescriptor;
			if (conditionDescriptor != null)
			{
				result = this.GetPhraseByIndex(conditionDescriptor.Index);
			}
			return result;
		}

		private PhraseDescriptor GetPhraseByIndex(int index)
		{
			PhraseDescriptor result = null;
			if (this.PhraseDescriptors != null)
			{
				foreach (PhraseDescriptor phraseDescriptor in this.PhraseDescriptors)
				{
					if (phraseDescriptor.Index == index)
					{
						result = phraseDescriptor;
						break;
					}
				}
			}
			return result;
		}

		private ConditionDescriptor GetConditionByIndex(int index)
		{
			ConditionDescriptor result = null;
			if (this.ConditionDescriptors != null)
			{
				foreach (ConditionDescriptor conditionDescriptor in this.ConditionDescriptors)
				{
					if (conditionDescriptor.Index == index)
					{
						result = conditionDescriptor;
						break;
					}
				}
			}
			return result;
		}

		private ConditionsEditorControl.PhrasePresentationControl GetPhrasePresentationControlByPhraseDescriptor(PhraseDescriptor phrase)
		{
			ConditionsEditorControl.PhrasePresentationControl result = null;
			foreach (object obj in this.sentencePanel.Controls)
			{
				Control control = (Control)obj;
				ConditionsEditorControl.PhrasePresentationControl phrasePresentationControl = control as ConditionsEditorControl.PhrasePresentationControl;
				if (phrasePresentationControl != null && phrasePresentationControl.PhraseDescriptor == phrase)
				{
					result = phrasePresentationControl;
					break;
				}
			}
			return result;
		}

		private void InitializeComponent()
		{
			this.tableLayoutPanel = new AutoTableLayoutPanel();
			this.step1Label = new Label();
			this.step2Label = new Label();
			this.sentencePanel = new ExchangeUserControl();
			this.sentenceDescriptionLabel = new AutoHeightLabel();
			this.conditionListView = new DataListView();
			((ISupportInitialize)base.BindingSource).BeginInit();
			this.tableLayoutPanel.SuspendLayout();
			this.sentencePanel.SuspendLayout();
			base.SuspendLayout();
			this.tableLayoutPanel.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
			this.tableLayoutPanel.AutoSize = true;
			this.tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel.ColumnCount = 1;
			this.tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
			this.tableLayoutPanel.Controls.Add(this.step1Label, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.step2Label, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.sentencePanel, 0, 3);
			this.tableLayoutPanel.Controls.Add(this.conditionListView, 0, 1);
			this.tableLayoutPanel.Location = new Point(0, 0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.Padding = new Padding(0, 0, 16, 0);
			this.tableLayoutPanel.RowCount = 4;
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new RowStyle());
			this.tableLayoutPanel.Size = new Size(400, 342);
			this.tableLayoutPanel.TabIndex = 0;
			this.step1Label.AutoSize = true;
			this.step1Label.Dock = DockStyle.Top;
			this.step1Label.Location = new Point(0, 0);
			this.step1Label.Margin = new Padding(0);
			this.step1Label.Name = "step1Label";
			this.step1Label.Size = new Size(384, 13);
			this.step1Label.TabIndex = 0;
			this.step2Label.AutoSize = true;
			this.step2Label.Dock = DockStyle.Top;
			this.step2Label.Location = new Point(0, 203);
			this.step2Label.Margin = new Padding(0, 9, 0, 0);
			this.step2Label.Name = "step2Label";
			this.step2Label.Size = new Size(384, 13);
			this.step2Label.TabIndex = 2;
			this.sentencePanel.AutoScroll = true;
			this.sentencePanel.BackColor = SystemColors.Window;
			this.sentencePanel.BorderStyle = BorderStyle.Fixed3D;
			this.sentencePanel.Controls.Add(this.sentenceDescriptionLabel);
			this.sentencePanel.Dock = DockStyle.Top;
			this.sentencePanel.Location = new Point(3, 219);
			this.sentencePanel.Margin = new Padding(3, 3, 0, 3);
			this.sentencePanel.MaximumSize = new Size(1024, 200);
			this.sentencePanel.MinimumSize = new Size(100, 100);
			this.sentencePanel.Name = "sentencePanel";
			this.sentencePanel.Size = new Size(381, 120);
			this.sentencePanel.TabIndex = 3;
			this.sentenceDescriptionLabel.Dock = DockStyle.Top;
			this.sentenceDescriptionLabel.LinkArea = new LinkArea(0, 0);
			this.sentenceDescriptionLabel.Location = new Point(0, 0);
			this.sentenceDescriptionLabel.Margin = new Padding(0);
			this.sentenceDescriptionLabel.Name = "sentenceDescriptionLabel";
			this.sentenceDescriptionLabel.Size = new Size(377, 16);
			this.sentenceDescriptionLabel.TabIndex = 0;
			this.conditionListView.CheckBoxes = true;
			this.conditionListView.DataSourceRefresher = null;
			this.conditionListView.Dock = DockStyle.Top;
			this.conditionListView.HeaderStyle = ColumnHeaderStyle.None;
			this.conditionListView.Location = new Point(3, 16);
			this.conditionListView.Margin = new Padding(3, 3, 0, 3);
			this.conditionListView.MaximumSize = new Size(1024, 230);
			this.conditionListView.MinimumSize = new Size(0, 100);
			this.conditionListView.MultiSelect = false;
			this.conditionListView.Name = "conditionListView";
			this.conditionListView.Size = new Size(381, 175);
			this.conditionListView.TabIndex = 1;
			this.conditionListView.UseCompatibleStateImageBehavior = false;
			this.conditionListView.UpdateItem += this.conditionListView_UpdateItem;
			this.conditionListView.ItemCheck += this.conditionListView_ItemCheck;
			base.Controls.Add(this.tableLayoutPanel);
			base.Name = "ConditionsEditorControl";
			base.Size = new Size(400, 344);
			((ISupportInitialize)base.BindingSource).EndInit();
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			this.sentencePanel.ResumeLayout(false);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		private BindingList<PhraseDescriptor> phraseDescriptors;

		private AutoTableLayoutPanel tableLayoutPanel;

		private Label step1Label;

		private Label step2Label;

		private ExchangeUserControl sentencePanel;

		private DataListView conditionListView;

		private AutoHeightLabel sentenceDescriptionLabel;

		private class PhrasePresentationControl : LinkLabelCommand
		{
			public PhrasePresentationControl()
			{
				base.Name = "PhrasePresentationControl";
				this.ImageAlign = ContentAlignment.MiddleRight;
				this.TextAlign = ContentAlignment.MiddleLeft;
				this.DoubleBuffered = true;
				this.Padding = this.defaultPadding;
				this.Dock = DockStyle.Top;
				base.ListSeparator = this.DefaultListSeparator;
			}

			public PhrasePresentationControl(PhraseDescriptor phrase) : this()
			{
				if (phrase == null)
				{
					throw new ArgumentNullException("phrase");
				}
				this.phraseDescriptor = phrase;
				base.TabIndex = this.PhraseDescriptor.Index;
				base.ListSeparator = this.DefaultListSeparator;
				base.Text = this.PhraseDescriptor.MarkupText;
				base.DataSource = this.PhraseDescriptor.DataSource;
			}

			public void UpdateText(int previousGroupID)
			{
				base.SuspendLayout();
				bool flag = previousGroupID == this.phraseDescriptor.GroupID;
				if (flag)
				{
					base.Text = string.Format(Strings.PhraseDescriptorFormat, this.PhraseDescriptor.PhrasePresentationPrefix, this.PhraseDescriptor.MarkupText);
				}
				else
				{
					base.Text = this.PhraseDescriptor.MarkupText;
				}
				this.Padding = (flag ? this.indentPadding : this.defaultPadding);
				base.ResumeLayout(true);
			}

			public PhraseDescriptor PhraseDescriptor
			{
				get
				{
					return this.phraseDescriptor;
				}
			}

			[DefaultValue(ContentAlignment.MiddleRight)]
			public ContentAlignment ImageAlign
			{
				get
				{
					return base.ImageAlign;
				}
				set
				{
					base.ImageAlign = value;
				}
			}

			[DefaultValue(ContentAlignment.MiddleLeft)]
			public ContentAlignment TextAlign
			{
				get
				{
					return base.TextAlign;
				}
				set
				{
					base.TextAlign = value;
				}
			}

			[DefaultValue(DockStyle.Top)]
			public DockStyle Dock
			{
				get
				{
					return base.Dock;
				}
				set
				{
					base.Dock = value;
				}
			}

			public Padding Padding
			{
				get
				{
					return base.Padding;
				}
				set
				{
					base.Padding = value;
				}
			}

			private bool ShouldSerializePadding()
			{
				return !this.Padding.Equals(this.defaultPadding);
			}

			private void ResetPadding()
			{
				this.Padding = this.defaultPadding;
			}

			protected override string DefaultListSeparator
			{
				get
				{
					if (this.PhraseDescriptor != null)
					{
						return this.PhraseDescriptor.ListSeparator;
					}
					return string.Empty;
				}
			}

			protected override void OnTextChanged(EventArgs e)
			{
				base.OnTextChanged(e);
				if (this.PhraseDescriptor != null)
				{
					if (!this.phraseDescriptor.IsValuesOfEditingPropertiesValid)
					{
						base.Image = ConditionsEditorControl.PhrasePresentationControl.edit;
						return;
					}
					base.Image = null;
				}
			}

			protected override void OnLinkClicked(LinkLabelLinkClickedEventArgs e)
			{
				string text = (string)e.Link.LinkData;
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(base.DataSource)[text];
				UITypeEditor uitypeEditor = this.CreateEditor(propertyDescriptor);
				if (propertyDescriptor != null && base.DataSource != null && uitypeEditor != null)
				{
					IServiceProvider serviceProvider = (base.FindForm() as IServiceProvider) ?? new ServiceContainer();
					TypeDescriptorContext context = new TypeDescriptorContext(serviceProvider, base.DataSource, propertyDescriptor);
					base.SuspendUpdates();
					try
					{
						object value = propertyDescriptor.GetValue(base.DataSource);
						value = uitypeEditor.EditValue(context, serviceProvider, value);
						this.PhraseDescriptor.SetDataSourceProperty(text, value);
					}
					finally
					{
						base.ResumeUpdates();
					}
				}
			}

			private UITypeEditor CreateEditor(PropertyDescriptor property)
			{
				UITypeEditor result = null;
				if (this.PhraseDescriptor != null)
				{
					result = this.PhraseDescriptor.CreateEditor(property);
				}
				return result;
			}

			private static Bitmap edit = IconLibrary.ToSmallBitmap(Icons.Edit);

			private Padding defaultPadding = new Padding(0, 0, 20, 0);

			private Padding indentPadding = new Padding(8, 0, 20, 0);

			private PhraseDescriptor phraseDescriptor;
		}
	}
}
