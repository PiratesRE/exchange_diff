using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Serializable]
	public class FooteredFeatureLauncherListViewItem : FeatureLauncherListViewItem
	{
		public FooteredFeatureLauncherListViewItem(string featureName, string statusPropertyName, Icon icon) : base(featureName, statusPropertyName, icon)
		{
		}

		[DefaultValue("")]
		public string FooterDescription { get; set; }

		[DefaultValue(null)]
		public Bitmap FooterBitmap { get; set; }
	}
}
