using System;
using System.Configuration;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.UI;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SlabHandler : EcpContentPage
	{
		public SlabHandler()
		{
			base.AppRelativeVirtualPath = "~/SlabHandler.aspx";
		}

		public override void ProcessRequest(HttpContext context)
		{
			string virtualPath = context.Request.Path.Replace(".slab", ".ascx");
			this.slab = (BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(SlabControl)) as SlabControl);
			base.ProcessRequest(context);
		}

		protected override void FrameworkInitialize()
		{
			base.FrameworkInitialize();
			this.MasterPageFile = "~/CommonMaster.Master";
			this.EnableViewState = SlabHandler.pagesSection.EnableViewState;
			base.EnableViewStateMac = SlabHandler.pagesSection.EnableViewStateMac;
			this.EnableEventValidation = SlabHandler.pagesSection.EnableEventValidation;
			this.Theme = SlabHandler.pagesSection.Theme;
			this.InitializeCulture();
			base.AddContentTemplate("ResultPanePlaceHolder", new CompiledTemplateBuilder(new BuildTemplateMethod(this.BuildControl)));
		}

		private void BuildControl(Control ctrl)
		{
			if (this.slab != null)
			{
				this.slab.InitializeAsUserControl(this);
				if (this.slab.LayoutOnly)
				{
					throw new NotSupportedException("LayoutOnly slab control cannot be initialized as an independent slab.");
				}
				base.Title = this.slab.Title;
				SlabTable slabTable = new SlabTable();
				slabTable.HelpId = this.slab.HelpId;
				slabTable.IsSingleSlabPage = true;
				SlabColumn slabColumn = new SlabColumn();
				slabColumn.Slabs.Add(this.slab);
				slabTable.Components.Add(slabColumn);
				base.FeatureSet = this.slab.FeatureSet;
				ctrl.Controls.Add(slabTable);
			}
		}

		private static readonly PagesSection pagesSection = (PagesSection)ConfigurationManager.GetSection("system.web/pages");

		private SlabControl slab;
	}
}
