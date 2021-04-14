using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class AutoTableLayoutPanel : TableLayoutPanel
	{
		[DefaultValue(false)]
		[Description("Enable the ability to support auto layout")]
		[Category("Layout")]
		public bool AutoLayout
		{
			get
			{
				return this.autoLayout;
			}
			set
			{
				if (this.autoLayout != value)
				{
					this.autoLayout = value;
					if (value)
					{
						base.PerformLayout(this, "AutoLayout");
					}
				}
			}
		}

		protected override void OnControlAdded(ControlEventArgs e)
		{
			this.needAdjustLayout = true;
			e.Control.VisibleChanged += this.Control_VisibleChanged;
			base.OnControlAdded(e);
		}

		private void Control_VisibleChanged(object sender, EventArgs e)
		{
			this.needAdjustLayout = true;
		}

		protected override void OnControlRemoved(ControlEventArgs e)
		{
			this.needAdjustLayout = true;
			e.Control.VisibleChanged -= this.Control_VisibleChanged;
			base.OnControlRemoved(e);
		}

		protected override void OnFontChanged(EventArgs e)
		{
			this.needAdjustLayout = true;
			base.OnFontChanged(e);
		}

		public AutoTableLayoutPanel()
		{
			base.Name = "AutoTableLayoutPanel";
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			if (this.autoLayout && this.needAdjustLayout)
			{
				this.AdjustLayout(levent);
				this.UpdateTLP();
				this.needAdjustLayout = false;
			}
			base.OnLayout(levent);
		}

		private void AdjustLayout(LayoutEventArgs e)
		{
			AlignUnitsCollection alignUnitsCollectionFromTLP = AlignUnitsCollection.GetAlignUnitsCollectionFromTLP(this);
			foreach (IAlignRule alignRule in AlignSettings.RulesList)
			{
				alignRule.Apply(alignUnitsCollectionFromTLP);
			}
			foreach (AlignUnit alignUnit in alignUnitsCollectionFromTLP.Units)
			{
				alignUnit.Control.Margin = LayoutHelper.Scale(alignUnit.ResultMargin + new Padding(0, alignUnitsCollectionFromTLP.RowDeltaValue[alignUnit.Row], 0, 0), this.scaleFactor);
			}
		}

		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			base.ScaleControl(factor, specified);
			this.scaleFactor = factor;
		}

		private void UpdateTLP()
		{
			this.Dock = DockStyle.Top;
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			base.Margin = Padding.Empty;
			this.refreshPadding();
			for (int i = 0; i < base.RowCount; i++)
			{
				if (ContainerType.Control != this.containerType)
				{
					base.RowStyles[i].SizeType = SizeType.AutoSize;
				}
			}
		}

		[Category("Layout")]
		[Description("Enable the ability to set pre-defined Padding")]
		[DefaultValue(ContainerType.Control)]
		public ContainerType ContainerType
		{
			get
			{
				return this.containerType;
			}
			set
			{
				this.containerType = value;
				if (value != ContainerType.Control)
				{
					base.SuspendLayout();
					this.refreshPadding();
					base.ResumeLayout(false);
					base.PerformLayout();
				}
			}
		}

		private void refreshPadding()
		{
			switch (this.containerType)
			{
			case ContainerType.Wizard:
				this.defaultPadding = new Padding(0, 8, 16, 0);
				break;
			case ContainerType.PropertyPage:
				this.defaultPadding = new Padding(13, 12, 16, 12);
				break;
			case ContainerType.Control:
				this.defaultPadding = this.Padding;
				break;
			}
			this.Padding = this.defaultPadding;
		}

		public new Padding Padding
		{
			get
			{
				return this.originPadding;
			}
			set
			{
				this.originPadding = value;
				base.Padding = LayoutHelper.RTLPadding(this.originPadding, this);
			}
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			base.Padding = LayoutHelper.RTLPadding(this.originPadding, this);
		}

		private bool autoLayout;

		private bool needAdjustLayout = true;

		private SizeF scaleFactor = new SizeF(1f, 1f);

		private ContainerType containerType = ContainerType.Control;

		private Padding defaultPadding;

		private Padding originPadding;
	}
}
