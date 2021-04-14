using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EncodingLabel : Label
	{
		protected override void RenderContents(HtmlTextWriter writer)
		{
			if (this.IsRequiredField)
			{
				this.Text = ClientStrings.RequiredFieldIndicator + this.Text;
			}
			if (this.SkipEncoding)
			{
				writer.Write(this.Text);
				return;
			}
			HttpUtility.HtmlEncode(this.Text, writer);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			if (Util.IsIE())
			{
				base.Attributes.Add("aria-hidden", "false");
			}
		}

		private Properties FindPropertiesControl()
		{
			Control parent = this.Parent;
			while (parent != null && !(parent is Properties))
			{
				parent = parent.Parent;
			}
			return (Properties)parent;
		}

		private bool IsRequiredField
		{
			get
			{
				return !string.IsNullOrEmpty(base.Attributes["RequiredField"]);
			}
		}

		public bool SkipEncoding { get; set; }
	}
}
