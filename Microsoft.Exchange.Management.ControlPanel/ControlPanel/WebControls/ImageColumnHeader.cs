using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class ImageColumnHeader : ColumnHeader
	{
		public ImageColumnHeader()
		{
			this.SpanCssClass = "ImgColumnSpan";
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
		public virtual string DescriptionProperty { get; set; }

		[DefaultValue("")]
		public virtual Unit ImageHeight { get; set; }

		[DefaultValue("")]
		public virtual Unit ImageWidth { get; set; }

		public override string ToJavaScript()
		{
			return string.Format("new ImageColumnHeader(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",{10},\"{11}\",{12})", new object[]
			{
				base.Name,
				base.SortExpression,
				base.FormatString,
				base.TextAlign.ToJavaScript(),
				HttpUtility.JavaScriptStringEncode(base.EmptyText),
				HttpUtility.JavaScriptStringEncode(this.AlternateTextProperty),
				HttpUtility.JavaScriptStringEncode(this.DescriptionProperty),
				this.ImageHeight.ToString(),
				this.ImageWidth.ToString(),
				base.Text,
				this.Width.Value,
				this.Width.Type.ToJavaScript(),
				base.Features
			});
		}
	}
}
