using System;
using System.ComponentModel;
using System.Web;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class TrendColumnHeader : ColumnHeader
	{
		public TrendColumnHeader()
		{
			this.AllowHTML = true;
		}

		[DefaultValue(null)]
		public virtual string TrendProperty { get; set; }

		[DefaultValue(null)]
		public virtual string AlternateTextProperty { get; set; }

		public override string ToJavaScript()
		{
			return string.Format("new TrendColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",{8},\"{9}\",{10})", new object[]
			{
				base.Name,
				base.SortExpression,
				base.FormatString,
				base.TextAlign.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(base.EmptyText),
				HttpUtility.JavaScriptStringEncode(this.TrendProperty),
				HttpUtility.JavaScriptStringEncode(this.AlternateTextProperty),
				base.Text,
				this.Width.Value,
				this.Width.Type.ToJavaScript(),
				base.Features
			});
		}
	}
}
