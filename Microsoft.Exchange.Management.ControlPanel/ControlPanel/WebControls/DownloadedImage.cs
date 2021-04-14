using System;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ClientScriptResource("DownloadedImage", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class DownloadedImage : Image
	{
		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		private bool readOnly;
	}
}
