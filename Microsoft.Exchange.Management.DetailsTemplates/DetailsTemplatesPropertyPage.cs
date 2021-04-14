using System;
using System.Windows.Forms;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DetailsTemplates
{
	internal class DetailsTemplatesPropertyPage : ExchangePropertyPageControl
	{
		internal DetailsTemplatesPropertyPage()
		{
		}

		internal DetailsTemplatesPropertyPage(IServiceProvider services)
		{
			this.designSurface = new DetailsTemplatesSurface(services);
			Control control = this.designSurface.View as Control;
			control.Dock = DockStyle.Fill;
			base.SuspendLayout();
			base.Controls.Add(control);
			base.ResumeLayout();
		}

		internal DetailsTemplatesSurface TemplateSurface
		{
			get
			{
				return this.designSurface;
			}
		}

		protected override void OnSetActive(EventArgs e)
		{
			base.OnSetActive(e);
			DetailsTemplate detailsTemplate = base.BindingSource.DataSource as DetailsTemplate;
			if (detailsTemplate != null && detailsTemplate.IsValid)
			{
				this.designSurface.LoadTemplate(detailsTemplate);
				this.designSurface.DataContext = base.Context;
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			if (base.Enabled)
			{
				base.OnEnabledChanged(e);
			}
		}

		private DetailsTemplatesSurface designSurface;
	}
}
