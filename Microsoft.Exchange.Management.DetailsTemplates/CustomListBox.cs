using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	[Designer(typeof(CustomControlDesigner))]
	internal sealed class CustomListBox : ListBox, IDetailsTemplateControlBound
	{
		public ScrollBars ScrollBars
		{
			get
			{
				return this.detailsTemplateControl.ScrollBars;
			}
			set
			{
				this.detailsTemplateControl.ScrollBars = value;
				this.UpdateScrollBars();
			}
		}

		[TypeConverter(typeof(MAPITypeConverter))]
		public string AttributeName
		{
			get
			{
				return this.detailsTemplateControl.AttributeName;
			}
			set
			{
				this.detailsTemplateControl.AttributeName = value;
			}
		}

		[Browsable(false)]
		public DetailsTemplateControl DetailsTemplateControl
		{
			get
			{
				return this.detailsTemplateControl;
			}
			set
			{
				this.detailsTemplateControl = (value as ListboxControl);
				this.UpdateScrollBars();
			}
		}

		private void UpdateScrollBars()
		{
			base.MultiColumn = false;
			base.HorizontalScrollbar = false;
			base.ScrollAlwaysVisible = false;
			switch (this.detailsTemplateControl.ScrollBars)
			{
			case ScrollBars.Horizontal:
				base.MultiColumn = true;
				base.HorizontalScrollbar = true;
				base.ScrollAlwaysVisible = true;
				return;
			case ScrollBars.Vertical:
				base.ScrollAlwaysVisible = true;
				return;
			case ScrollBars.Both:
				base.HorizontalScrollbar = true;
				base.ScrollAlwaysVisible = true;
				return;
			default:
				return;
			}
		}

		private ListboxControl detailsTemplateControl = new ListboxControl();
	}
}
