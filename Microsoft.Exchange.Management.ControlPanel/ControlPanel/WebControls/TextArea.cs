using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:TextArea runat=server></{0}:TextArea>")]
	public class TextArea : TextBox
	{
		protected override void OnPreRender(EventArgs e)
		{
			if (this.MaxLength > 0)
			{
				if (Util.IsIE() || Util.IsSafari())
				{
					base.Attributes.Add("onkeydown", "return TextAreaUtil.TextAreaOnKeyDownEvent(this);");
					base.Attributes.Add("onpaste", "TextAreaUtil.TextAreaOnPasteEvent(this);");
				}
				else if (Util.IsFirefox())
				{
					base.Attributes.Add("oninput", "TextAreaUtil.TextAreaOnInputEvent(this);");
				}
				else
				{
					base.Attributes.Add("onkeydown", "return TextAreaUtil.TextAreaOnKeyDownEvent(this);");
					base.Attributes.Add("onpaste", "TextAreaUtil.TextAreaOnPasteEvent(this);");
					base.Attributes.Add("oninput", "TextAreaUtil.TextAreaOnInputEvent(this);");
				}
				base.Attributes.Add("onmouseover", "TextAreaUtil.TextAreaOnMouseOverEvent(this);");
				base.Attributes.Add("maxLength", this.MaxLength.ToString());
			}
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.RenderBeginTag(writer);
			if (this.TextMode == TextBoxMode.MultiLine)
			{
				HttpUtility.HtmlEncode(this.Text, writer);
			}
			this.RenderEndTag(writer);
		}

		public override TextBoxMode TextMode
		{
			get
			{
				return TextBoxMode.MultiLine;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public override int Rows
		{
			get
			{
				return base.Rows;
			}
			set
			{
				if (value < 5)
				{
					throw new ArgumentException("You may get the bug(Exchange14:$93146) again if you specify Rows < 5 in TextArea.", "TextArea.Rows");
				}
				base.Rows = value;
			}
		}
	}
}
