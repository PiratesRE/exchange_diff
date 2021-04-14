using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DefaultProperty("Components")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	[ClientScriptResource("SlabTable", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[ParseChildren(true, "Components")]
	public class SlabTable : DockPanel
	{
		public SlabTable()
		{
			this.CssClass = "baseFrm";
			this.Components = new List<SlabComponent>();
			this.DockEnabled = true;
		}

		public string HelpId
		{
			get
			{
				return this.helpId;
			}
			set
			{
				this.helpId = value;
			}
		}

		public bool UseGlobalSaveButton { get; set; }

		public bool DockEnabled { get; set; }

		internal bool IsSingleSlabPage { get; set; }

		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<SlabComponent> Components { get; private set; }

		protected override void OnInit(EventArgs e)
		{
			this.DispatchComponents();
			this.Refactor();
			this.PopulateChildControls();
			this.AddCssFileLinks();
			base.OnInit(e);
			this.ReconfigFVA();
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddProperty("DockEnabled", this.DockEnabled);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.PrepareSlabsForScreenReader();
			this.ShowButtonPanelIfNeeded();
			if (this.saveButtonClientID != null)
			{
				base.Attributes.Add("onkeydown", string.Format("javascript:return EcpSlabTable_FireSaveButton(event, '{0}')", this.saveButtonClientID));
			}
			base.Render(writer);
		}

		private void DispatchComponents()
		{
			this.columns = new List<SlabColumn>();
			this.topRows = new List<SlabRow>();
			this.bottomRows = new List<SlabRow>();
			int num = 0;
			foreach (SlabComponent slabComponent in this.Components)
			{
				SlabColumn slabColumn = slabComponent as SlabColumn;
				if (slabColumn != null)
				{
					if (num == 0)
					{
						num = 1;
					}
					else if (num == 2)
					{
						throw new InvalidOperationException("Don't add SlabRow between SlabColumns! SlabRow can only be added before or after SlabClolumns.");
					}
					this.columns.Add(slabColumn);
				}
				else if (num == 0)
				{
					this.topRows.Add((SlabRow)slabComponent);
				}
				else
				{
					num = 2;
					this.bottomRows.Add((SlabRow)slabComponent);
				}
			}
		}

		private void Refactor()
		{
			double num = 0.0;
			double num2 = 0.0;
			bool flag = false;
			foreach (SlabRow slabRow in this.topRows)
			{
				slabRow.Refactor();
			}
			foreach (SlabRow slabRow2 in this.bottomRows)
			{
				slabRow2.Refactor();
			}
			for (int i = this.columns.Count - 1; i >= 0; i--)
			{
				SlabColumn slabColumn = this.columns[i];
				slabColumn.Refactor();
				bool flag2 = false;
				if (slabColumn.Slabs.Count == 0)
				{
					this.columns.RemoveAt(i);
					flag2 = true;
				}
				if (slabColumn.Width.Type == UnitType.Percentage)
				{
					num += slabColumn.Width.Value;
					if (flag2)
					{
						flag = true;
					}
					else
					{
						num2 += slabColumn.Width.Value;
					}
				}
			}
			if (flag && num2 != 0.0)
			{
				double num3 = num / num2;
				foreach (SlabColumn slabColumn2 in this.columns)
				{
					if (slabColumn2.Width.Type == UnitType.Percentage)
					{
						slabColumn2.Width = new Unit(slabColumn2.Width.Value * num3, UnitType.Percentage);
					}
				}
			}
		}

		private void PopulateChildControls()
		{
			this.Controls.Clear();
			Table table = this.PopulateSlabColumnAndRows();
			table.Attributes.Add("role", "presentation");
			bool flag = this.AddCaptionPanelOrHelpControl();
			this.contentPanel = new Panel();
			this.contentPanel.CssClass = "slbTblCtnPnl";
			if (flag)
			{
				Panel panel = this.contentPanel;
				panel.CssClass += " topSpc";
			}
			this.contentPanel.Attributes.Add("dock", "fill");
			this.contentPanel.Controls.Add(table);
			this.Controls.Add(this.contentPanel);
			this.showCloseButton = this.CheckCloseButton();
			this.AddButtonPanel();
		}

		private void PrepareSlabsForScreenReader()
		{
			foreach (SlabFrame slabFrame in this.slabFrames)
			{
				SlabControl slab = slabFrame.Slab;
				slabFrame.Attributes["tabindex"] = "0";
				slabFrame.Attributes["role"] = "region";
				slabFrame.Attributes["aria-labelledby"] = slabFrame.CaptionLabel.ClientID;
			}
		}

		private Table PopulateSlabColumnAndRows()
		{
			Table table = new Table();
			table.CssClass = "slbTbl";
			table.CellPadding = 0;
			table.CellSpacing = 0;
			bool showHelp = ((EcpContentPage)this.Page).ShowHelp;
			this.PopulateSlabRows(table, this.topRows);
			bool flag = this.slabFrames.Count == 0;
			if (!flag)
			{
				SlabFrame slabFrame = this.slabFrames[0];
				slabFrame.CssClass += " slbTp";
			}
			bool flag2 = table.Rows.Count > 0;
			int columnSpan = 0;
			if (this.columns.Count > 0)
			{
				TableRow tableRow = new TableRow();
				table.Rows.Add(tableRow);
				this.PopulateSlabColumns(tableRow, showHelp, flag);
				if (flag2)
				{
					this.AddDummyTopRow(table, tableRow);
				}
				columnSpan = tableRow.Cells.Count;
			}
			this.PopulateSlabRows(table, this.bottomRows);
			if (this.columns.Count > 0)
			{
				foreach (TableCell tableCell in this.fullRowCells)
				{
					tableCell.ColumnSpan = columnSpan;
				}
			}
			return table;
		}

		private void AddDummyTopRow(Table table, TableRow columnRow)
		{
			TableRow tableRow = new TableRow();
			tableRow.Height = new Unit(0.0, UnitType.Pixel);
			foreach (object obj in columnRow.Cells)
			{
				TableCell tableCell = (TableCell)obj;
				TableCell tableCell2 = new TableCell();
				tableCell2.Width = tableCell.Width;
				tableRow.Cells.Add(tableCell2);
			}
			table.Rows.AddAt(0, tableRow);
		}

		private void PopulateSlabRows(Table table, List<SlabRow> rows)
		{
			foreach (SlabRow slabRow in rows)
			{
				foreach (Control control in slabRow.Content)
				{
					TableRow tableRow = new TableRow();
					table.Rows.Add(tableRow);
					TableCell tableCell = new TableCell();
					tableRow.Cells.Add(tableCell);
					this.fullRowCells.Add(tableCell);
					SlabControl slabControl = control as SlabControl;
					if (slabControl != null)
					{
						SlabFrame slabFrame = new SlabFrame(slabControl);
						SlabFrame slabFrame2 = slabFrame;
						slabFrame2.CssClass += " slbLf";
						if (slabControl.UsePropertyPageStyle)
						{
							SlabFrame slabFrame3 = slabFrame;
							slabFrame3.CssClass += " slbPrpg";
						}
						tableCell.Controls.Add(slabFrame);
						this.slabFrames.Add(slabFrame);
					}
					else
					{
						tableCell.Controls.Add(control);
					}
				}
			}
		}

		private void PopulateSlabColumns(TableRow row, bool showHelp, bool noSlabInTopRow)
		{
			row.Attributes.Add("srow", "f");
			int num = 0;
			bool flag = false;
			int num2 = 0;
			foreach (SlabColumn slabColumn in this.columns)
			{
				bool flag2 = num2 == 0;
				int count = this.columns.Count;
				TableCell tableCell = new TableCell();
				UnitType type = slabColumn.Width.Type;
				if (type == UnitType.Percentage)
				{
					num += (int)slabColumn.Width.Value;
				}
				else
				{
					if (!slabColumn.Width.IsEmpty)
					{
						throw new NotSupportedException("Slab column width of type other than percentage is not supported.");
					}
					flag = true;
				}
				tableCell.Width = slabColumn.Width;
				tableCell.VerticalAlign = VerticalAlign.Top;
				row.Cells.Add(tableCell);
				int num3 = 0;
				foreach (SlabControl slabControl in slabColumn.Slabs)
				{
					SlabFrame slabFrame = new SlabFrame(slabControl);
					if (flag2)
					{
						SlabFrame slabFrame2 = slabFrame;
						slabFrame2.CssClass += " slbLf";
					}
					if (slabControl.UsePropertyPageStyle)
					{
						SlabFrame slabFrame3 = slabFrame;
						slabFrame3.CssClass += " slbPrpg";
					}
					if (noSlabInTopRow && num3 == 0)
					{
						SlabFrame slabFrame4 = slabFrame;
						slabFrame4.CssClass += " slbTp";
					}
					tableCell.Controls.Add(slabFrame);
					this.slabFrames.Add(slabFrame);
					num3++;
				}
				num2++;
			}
			if (!flag && num < 100)
			{
				TableCell tableCell2 = new TableCell();
				tableCell2.Width = new Unit((double)(100 - num), UnitType.Percentage);
				row.Cells.Add(tableCell2);
			}
		}

		private bool CheckCloseButton()
		{
			int num = 0;
			for (int i = 0; i < this.slabFrames.Count; i++)
			{
				SlabFrame slabFrame = this.slabFrames[i];
				if (slabFrame.Slab.ShowCloseButton)
				{
					num++;
				}
			}
			return num > 0 && num == this.slabFrames.Count;
		}

		private bool AddCaptionPanelOrHelpControl()
		{
			bool flag = this.slabFrames.Count == 1;
			bool showHelp = ((EcpContentPage)this.Page).ShowHelp;
			bool result = false;
			if (flag)
			{
				SlabFrame slabFrame = this.slabFrames[0];
				slabFrame.ShowHelp = showHelp;
				slabFrame.PublishHelp = true;
				if (this.IsSingleSlabPage)
				{
					slabFrame.Attributes.Add("fill", "100");
				}
			}
			else if (showHelp)
			{
				CaptionPanel captionPanel = new CaptionPanel();
				captionPanel.HelpId = this.HelpId;
				captionPanel.Attributes.Add("dock", "top");
				captionPanel.ShowCaption = false;
				captionPanel.ShowHelp = true;
				this.Controls.Add(captionPanel);
				result = true;
			}
			else
			{
				HelpControl helpControl = new HelpControl();
				helpControl.HelpId = this.HelpId;
				helpControl.ShowHelp = false;
				helpControl.NeedPublishHelpLinkWhenHidden = true;
				this.Controls.Add(helpControl);
			}
			return result;
		}

		[Conditional("DEBUG")]
		private void CheckOnePageHasAtMostOnePrimarySlab()
		{
			if (!((EcpContentPage)this.Page).ShowHelp)
			{
				int num = 0;
				foreach (SlabFrame slabFrame in this.slabFrames)
				{
					if (slabFrame.Slab.IsPrimarySlab)
					{
						num++;
					}
				}
			}
		}

		[Conditional("DEBUG")]
		private void CheckControlsHaveSameFeatureSet()
		{
			FeatureSet? featureSet = null;
			foreach (SlabFrame slabFrame in this.slabFrames)
			{
				if (featureSet == null)
				{
					featureSet = new FeatureSet?(slabFrame.Slab.FeatureSet);
				}
				else if (featureSet.Value != slabFrame.Slab.FeatureSet)
				{
					break;
				}
			}
		}

		private void AddButtonPanel()
		{
			this.saveSlabFrames = new List<SlabFrame>();
			List<WebServiceMethod> list = new List<WebServiceMethod>();
			foreach (SlabFrame slabFrame in this.slabFrames)
			{
				if (slabFrame.PropertiesControl != null && slabFrame.PropertiesControl.HasSaveMethod)
				{
					this.saveSlabFrames.Add(slabFrame);
					slabFrame.InitSaveButton();
					WebServiceMethod saveWebServiceMethod = slabFrame.PropertiesControl.SaveWebServiceMethod;
					if (saveWebServiceMethod != null)
					{
						list.Add(saveWebServiceMethod);
					}
				}
			}
			if (this.ShowGlobalSaveButton() || this.showCloseButton)
			{
				this.buttonsPanel = new ButtonsPanel();
				if (this.showCloseButton)
				{
					this.buttonsPanel.State = ButtonsPanelState.ReadOnly;
					ButtonsPanel buttonsPanel = this.buttonsPanel;
					buttonsPanel.CssClass += " glbClsPnl";
					this.buttonsPanel.CloseWindowOnCancel = true;
				}
				else
				{
					this.buttonsPanel.State = ButtonsPanelState.Save;
					ButtonsPanel buttonsPanel2 = this.buttonsPanel;
					buttonsPanel2.CssClass += " glbSvPnl";
					this.buttonsPanel.SaveWebServiceMethods.AddRange(list);
				}
				this.buttonsPanel.Attributes.Add("dock", "bottom");
				this.Controls.Add(this.buttonsPanel);
			}
			if (this.saveSlabFrames.Count > 0)
			{
				base.Attributes.Add("data-type", "MultiPropertyPageViewModel");
			}
		}

		private void ReconfigFVA()
		{
			if (this.columns.Count == 1 && (this.columns[0].Width.Equals(Unit.Empty) || (this.columns[0].Width.Type == UnitType.Percentage && this.columns[0].Width.Value > 0.95)))
			{
				using (List<SlabControl>.Enumerator enumerator = this.columns[0].Slabs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SlabControl slabControl = enumerator.Current;
						if (this.IsSimpleFormWithFVAEnabled(slabControl))
						{
							WebControl webControl = slabControl.Parent as WebControl;
							WebControl webControl2 = webControl;
							webControl2.CssClass += " reservedSpaceForFVA";
						}
					}
					return;
				}
			}
			if (this.columns.Count > 1)
			{
				foreach (SlabColumn slabColumn in this.columns)
				{
					foreach (SlabControl slabControl2 in slabColumn.Slabs)
					{
						if (this.IsSimpleFormWithFVAEnabled(slabControl2))
						{
							slabControl2.FieldValidationAssistantExtender.Canvas = this.contentPanel.ClientID;
						}
					}
				}
			}
		}

		private bool IsSimpleFormWithFVAEnabled(SlabControl slab)
		{
			return slab.FieldValidationAssistantExtender != null && slab.PropertiesControl != null;
		}

		private void ShowButtonPanelIfNeeded()
		{
			bool flag = false;
			if (this.showCloseButton)
			{
				flag = true;
			}
			else if (this.saveSlabFrames.Count > 0)
			{
				int count = this.saveSlabFrames.Count;
				this.saveSlabFrames = (from frame in this.saveSlabFrames
				where !frame.PropertiesControl.ReadOnly
				select frame).ToList<SlabFrame>();
				if (this.ShowGlobalSaveButton())
				{
					if (count != this.saveSlabFrames.Count)
					{
						List<WebServiceMethod> collection = (from saveFrame in this.saveSlabFrames
						select saveFrame.PropertiesControl.SaveWebServiceMethod).ToList<WebServiceMethod>();
						this.buttonsPanel.SaveWebServiceMethods.Clear();
						this.buttonsPanel.SaveWebServiceMethods.AddRange(collection);
					}
					SlabFrame.SetFocusCssOnSaveButton(this.buttonsPanel);
					flag = true;
					this.saveButtonClientID = this.buttonsPanel.CommitButtonClientID;
				}
				else if (this.saveSlabFrames.Count == 1)
				{
					SlabFrame slabFrame = this.saveSlabFrames[0];
					if (slabFrame.Slab.AlwaysDockSaveButton)
					{
						slabFrame.Attributes.Add("fill", "100");
					}
					slabFrame.ShowSaveButton();
					this.saveButtonClientID = slabFrame.SaveButtonClientID;
				}
			}
			if (flag)
			{
				Panel panel = this.contentPanel;
				panel.CssClass += " btmSpc";
				return;
			}
			if (this.buttonsPanel != null)
			{
				this.buttonsPanel.Visible = false;
			}
		}

		private bool ShowGlobalSaveButton()
		{
			return this.saveSlabFrames.Count > 1 || (this.UseGlobalSaveButton && this.saveSlabFrames.Count == 1);
		}

		private void AddCssFileLinks()
		{
			CommonMaster commonMaster = (CommonMaster)this.Page.Master;
			foreach (SlabFrame slabFrame in this.slabFrames)
			{
				commonMaster.AddCssFiles(slabFrame.Slab.IncludeCssFiles);
			}
		}

		private List<SlabColumn> columns;

		private List<SlabRow> topRows;

		private List<SlabRow> bottomRows;

		private Panel contentPanel;

		private ButtonsPanel buttonsPanel;

		private bool showCloseButton;

		private List<SlabFrame> saveSlabFrames;

		private string saveButtonClientID;

		private List<TableCell> fullRowCells = new List<TableCell>();

		private List<SlabFrame> slabFrames = new List<SlabFrame>();

		private string helpId = string.Empty;
	}
}
