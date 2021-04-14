using System;
using System.Web.UI;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ImageUrlBinding : ClientControlBinding
	{
		public ImageUrlBinding(Control control, bool neverDirty) : base(control, "src")
		{
			this.neverDirty = neverDirty;
		}

		protected override string ToJavaScriptWhenVisible(IControlResolver resolver)
		{
			return string.Format("new ImageUrlBinding('{0}', {1})", this.ClientID, this.neverDirty ? "true" : "false");
		}

		private readonly bool neverDirty;
	}
}
