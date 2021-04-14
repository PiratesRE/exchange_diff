using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ToolboxData("<{0}:Section runat=server></{0}:Section>")]
	[PersistChildren(false)]
	[ParseChildren(true)]
	public class Section : WebControl, INamingContainer
	{
		public Section() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "section";
		}

		public string Title { get; set; }

		public string WorkflowName { get; set; }

		public string ClientVisibilityBinding { get; set; }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		protected SectionContentPanel ContentContainer
		{
			get
			{
				this.EnsureChildControls();
				return this.contentContainer;
			}
		}

		[Description("Section Pane Content")]
		[TemplateContainer(typeof(SectionContentPanel))]
		[TemplateInstance(TemplateInstance.Single)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[Browsable(false)]
		[DefaultValue(null)]
		public virtual ITemplate Content { get; set; }

		public override ControlCollection Controls
		{
			get
			{
				this.EnsureChildControls();
				return base.Controls;
			}
		}

		public IEnumerable<FormView> FormViews
		{
			get
			{
				List<FormView> list = new List<FormView>();
				this.GetFormViews(this.ContentContainer, list);
				return list;
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.Attributes.Add("data-control", "Section");
			base.Attributes.Add("Workflow", this.WorkflowName ?? string.Empty);
			base.OnPreRender(e);
		}

		private void GetFormViews(Control root, List<FormView> formViews)
		{
			foreach (object obj in root.Controls)
			{
				Control control = (Control)obj;
				FormView formView = control as FormView;
				if (formView != null)
				{
					formViews.Add(formView);
				}
				else
				{
					this.GetFormViews(control, formViews);
				}
			}
		}

		protected override void CreateChildControls()
		{
			this.Controls.Clear();
			HtmlAnchor htmlAnchor = new HtmlAnchor();
			htmlAnchor.Name = this.ID;
			htmlAnchor.Attributes["class"] = "dspBlock";
			htmlAnchor.Attributes["tabindex"] = "-1";
			this.Controls.Add(htmlAnchor);
			this.contentContainer = new SectionContentPanel();
			this.contentContainer.ID = "contentContainer";
			this.contentContainer.CssClass = "secCnt";
			this.Controls.Add(this.contentContainer);
			if (this.Content != null)
			{
				this.Content.InstantiateIn(this.contentContainer);
			}
		}

		public override Control FindControl(string id)
		{
			this.EnsureChildControls();
			return base.FindControl(id) ?? this.contentContainer.FindControl(id);
		}

		public override bool Enabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.isEnabled = value;
			}
		}

		public string SetRoles
		{
			get
			{
				return base.Attributes["SetRoles"];
			}
			set
			{
				base.Attributes.Add("SetRoles", value);
			}
		}

		private const string Workflow = "Workflow";

		private SectionContentPanel contentContainer;

		private bool isEnabled = true;
	}
}
