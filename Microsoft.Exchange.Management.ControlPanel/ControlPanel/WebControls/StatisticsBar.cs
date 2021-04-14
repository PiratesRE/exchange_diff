using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:StatisticsBar runat=server></{0}:StatisticsBar>")]
	[ControlValueProperty("Value")]
	[ClientScriptResource("StatisticsBar", "Microsoft.Exchange.Management.ControlPanel.Client.WizardProperties.js")]
	public class StatisticsBar : ScriptControlBase
	{
		public StatisticsBar() : base(HtmlTextWriterTag.Div)
		{
		}

		[DefaultValue(typeof(Unit), "100%")]
		public Unit BarWidth
		{
			get
			{
				return this.barWidth;
			}
			set
			{
				this.barWidth = value;
			}
		}

		[DefaultValue(typeof(Unit), "16px")]
		public Unit BarHeight
		{
			get
			{
				return this.barHeight;
			}
			set
			{
				if (value.Type == UnitType.Pixel && value.Value >= 1.0 && value.Value <= 16.0)
				{
					this.barHeight = value;
					return;
				}
				throw new InvalidOperationException("Height for the bar can only be specified in Pixels and ranging from 1px to 16px.");
			}
		}

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			if (!this.BarWidth.IsEmpty)
			{
				descriptor.AddProperty("BarWidth", this.BarWidth.ToString(CultureInfo.InvariantCulture));
			}
			if (!this.BarHeight.IsEmpty)
			{
				descriptor.AddProperty("BarHeight", this.BarHeight.ToString(CultureInfo.InvariantCulture));
			}
		}

		private Unit barWidth;

		private Unit barHeight;
	}
}
