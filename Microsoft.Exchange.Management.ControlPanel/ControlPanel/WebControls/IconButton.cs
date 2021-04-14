using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:IconButton runat=\"server\" />")]
	public class IconButton : HtmlButton
	{
		public IconButton()
		{
			this.cmdSprtIcon = new CommandSprite();
			this.lblText = new Label();
			this.CssClass = (Util.IsIE() ? "btn" : "btn btnFF");
			this.SetAttribute("type", "button");
			this.SetAttribute("data-TextTagName", "span");
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			this.Controls.Add(this.cmdSprtIcon);
			this.Controls.Add(this.lblText);
		}

		public CommandSprite CommandSprite
		{
			get
			{
				return this.cmdSprtIcon;
			}
		}

		public Label Label
		{
			get
			{
				return this.lblText;
			}
		}

		public CommandSprite.SpriteId ImageId
		{
			get
			{
				return this.cmdSprtIcon.ImageId;
			}
			set
			{
				this.cmdSprtIcon.ImageId = value;
			}
		}

		public string Text
		{
			get
			{
				return this.lblText.Text;
			}
			set
			{
				this.lblText.Text = value;
			}
		}

		public string CssClass
		{
			get
			{
				return base.Attributes["class"];
			}
			set
			{
				base.Attributes["class"] = value;
			}
		}

		public override string InnerHtml
		{
			get
			{
				return base.InnerHtml;
			}
			set
			{
				throw new NotSupportedException("Setting InnerHtml is not supported for IconButton. Please use the Text property instead.");
			}
		}

		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				throw new NotSupportedException("Setting InnerText is not supported for IconButton. Please use the Text property instead.");
			}
		}

		private CommandSprite cmdSprtIcon;

		private Label lblText;
	}
}
