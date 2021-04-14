using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class MicroEditColumnHeader : ColumnHeader
	{
		public MicroEditColumnHeader()
		{
			this.Width = 25;
			base.IsSortable = false;
			base.EnableColumnResize = false;
			base.EnableExport = false;
		}

		public string Condition { get; set; }

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

		public override string ToJavaScript()
		{
			return string.Format("new MicroEditColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",{4},\"{5}\",{6},\"{7}\",{8})", new object[]
			{
				base.Name,
				base.SortExpression,
				base.FormatString,
				base.TextAlign,
				string.IsNullOrEmpty(this.Condition) ? "null" : ("function($_) { return " + this.Condition + "}"),
				base.Text,
				this.Width.Value,
				this.Width.Type.ToJavaScript(),
				base.Features
			});
		}
	}
}
