using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class SpriteColumnHeader : ColumnHeader
	{
		public SpriteColumnHeader()
		{
			this.SpanCssClass = "ImgColumnSpan";
			this.Width = Unit.Pixel(22);
		}

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
		public virtual string AlternateTextProperty { get; set; }

		[DefaultValue(null)]
		public virtual string DefaultSprite { get; set; }

		public override string ToJavaScript()
		{
			return string.Format("new SpriteColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",{7},\"{8}\",{9})", new object[]
			{
				base.Name,
				base.SortExpression,
				base.FormatString,
				base.TextAlign.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(base.EmptyText),
				HttpUtility.JavaScriptStringEncode(this.AlternateTextProperty),
				base.Text,
				this.Width.Value,
				this.Width.Type.ToJavaScript(),
				base.Features
			});
		}
	}
}
