using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:ThemedImage runat=\"server\" />")]
	public class ThemedImage : Image
	{
		[DefaultValue("")]
		public string FileName { get; set; }

		public override string ImageUrl
		{
			get
			{
				return ThemeResource.GetThemeResource(this, this.FileName);
			}
			set
			{
				throw new NotSupportedException("When using the ThemedImage control, use the FileName property to specify the image.");
			}
		}
	}
}
