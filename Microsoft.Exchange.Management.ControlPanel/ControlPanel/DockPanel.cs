using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource("DockPanel", "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	[RequiredScript(typeof(CommonToolkitScripts))]
	public class DockPanel : ScriptControlBase
	{
		public DockPanel() : base(HtmlTextWriterTag.Div)
		{
		}

		[Browsable(false)]
		[TemplateInstance(TemplateInstance.Single)]
		[DefaultValue(null)]
		[Description("Property Pane Content")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(PropertiesContentPanel))]
		public virtual ITemplate Content
		{
			get
			{
				return this.content;
			}
			set
			{
				this.content = value;
			}
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (this.Content != null)
			{
				this.contentContainer = new PropertiesContentPanel();
				this.contentContainer.ID = "contentContainer";
				this.contentContainer.Height = Unit.Percentage(100.0);
				this.Controls.Add(this.contentContainer);
				this.Content.InstantiateIn(this.contentContainer);
			}
		}

		internal const string ContentContainerID = "contentContainer";

		public const string DockAttribute = "dock";

		public const string Top = "top";

		public const string Bottom = "bottom";

		public const string Fill = "fill";

		private PropertiesContentPanel contentContainer;

		private ITemplate content;
	}
}
