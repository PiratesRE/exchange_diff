using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CommandColumnHeader : ColumnHeader
	{
		[DefaultValue(true)]
		public override bool AllowHTML
		{
			get
			{
				return true;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		[DefaultValue(null)]
		public string ButtonCssClass { get; set; }

		[DefaultValue(null)]
		[TypeConverter(typeof(StringArrayConverter))]
		public string[] Commands { get; set; }

		[DefaultValue(false)]
		public bool UseCheckBox { get; set; }

		[DefaultValue(false)]
		public bool UseCommandText { get; set; }

		public override string ToJavaScript()
		{
			if (this.Commands.IsNullOrEmpty())
			{
				throw new Exception("At least one command must be set to Commands property of CommandColumnHeader.");
			}
			if (this.UseCheckBox && this.Commands.Length != 2)
			{
				throw new Exception("Two commands must be set to Commands property of CommandColumnHeader when UseCheckBox is true.");
			}
			if (this.UseCheckBox && string.IsNullOrEmpty(base.Name))
			{
				throw new Exception("The column must be bound to a Boolean property (set Name property) if UseCheckBox is true.");
			}
			if (!this.UseCheckBox && !this.UseCommandText && !string.IsNullOrEmpty(base.Name) && this.Commands.Length != 1)
			{
				throw new Exception("Only one command can be set to Commands property of CommandColumnHeader if the column is data bound and UseCheckBox is false.");
			}
			return string.Format("new CommandColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",{6},{7},{8},\"{9}\",{10},\"{11}\",{12})", new object[]
			{
				base.Name,
				base.SortExpression,
				base.FormatString,
				base.TextAlign.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(base.EmptyText),
				this.ButtonCssClass,
				this.Commands.ToJsonString(null),
				this.UseCommandText.ToJavaScript(),
				this.UseCheckBox.ToJavaScript(),
				base.Text,
				this.Width.Value,
				this.Width.Type.ToJavaScript(),
				base.Features
			});
		}
	}
}
