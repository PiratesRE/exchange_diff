using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AjaxControlToolkit;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:DetailsPane runat=server></{0}:DetailsPane>")]
	[ClientScriptResource("DetailsPane", "Microsoft.Exchange.Management.ControlPanel.Client.List.js")]
	public class DetailsPane : ScriptControlBase, INamingContainer
	{
		public DetailsPane() : base(HtmlTextWriterTag.Div)
		{
			this.toolbar = new ToolBar();
			this.toolbar.ID = "ToolBar";
			this.toolbar.CssClass = "ListViewToolBar";
			this.ShowPaddingPanels = true;
			Util.RequireUpdateProgressPopUp(this);
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
			if (string.IsNullOrWhiteSpace(this.TypeProperty))
			{
				this.TypeProperty = "Type";
			}
			if (this.ShowPaddingPanels)
			{
				this.topEmptyPanel = new Panel();
				this.topEmptyPanel.ID = "topEmptyPanel";
				this.topEmptyPanel.CssClass = "masterTopEmptyPane";
				this.Controls.Add(this.topEmptyPanel);
				this.middleEmptyPanel = new Panel();
				this.middleEmptyPanel.ID = "middleEmptyPanel";
				this.middleEmptyPanel.CssClass = "masterMiddleEmptyPane";
				this.Controls.Add(this.middleEmptyPanel);
			}
			this.toolbar.ApplyRolesFilter();
			if (this.ShowToolbar)
			{
				this.toolbarPanel = this.CreateToolBarPanel();
				this.Controls.Add(this.toolbarPanel);
			}
			this.contentPanel = new Panel();
			this.contentPanel.ID = "contentPanel";
			if (string.IsNullOrEmpty(this.ContentPaneCssClass))
			{
				this.contentPanel.CssClass = "masterDetailsContentPane";
			}
			else
			{
				this.contentPanel.CssClass = this.ContentPaneCssClass;
			}
			this.frame = new HtmlGenericControl();
			this.frame.ID = "detailsFrame";
			this.frame.Attributes["frameborder"] = "0";
			this.frame.Style.Add(HtmlTextWriterStyle.Width, "100%");
			this.frame.Style.Add(HtmlTextWriterStyle.Height, "100%");
			this.frame.TagName = HtmlTextWriterTag.Iframe.ToString();
			this.frame.Style.Add(HtmlTextWriterStyle.Display, "none");
			this.frame.Attributes["class"] = "detailsFrame";
			if (Util.IsIE())
			{
				this.frame.Attributes["src"] = ThemeResource.BlankHtmlPath;
			}
			this.loadingDiv = this.CreateLoadingDivPanel();
			this.noPermissionDiv = this.CreateNoPermissionDivPanel();
			this.contentPanel.Controls.Add(this.loadingDiv);
			this.contentPanel.Controls.Add(this.frame);
			this.contentPanel.Controls.Add(this.noPermissionDiv);
			this.Controls.Add(this.contentPanel);
			if (this.ShowPaddingPanels)
			{
				this.bottomEmptyPanel = new Panel();
				this.bottomEmptyPanel.ID = "bottomEmptyPanel";
				this.bottomEmptyPanel.CssClass = "masterBottomEmptyPane";
				this.Controls.Add(this.bottomEmptyPanel);
			}
		}

		private Panel CreateToolBarPanel()
		{
			return new Panel
			{
				CssClass = "ToolBarContainer",
				Controls = 
				{
					this.toolbar
				}
			};
		}

		private Panel CreateLoadingDivPanel()
		{
			Panel panel = new Panel();
			panel.ID = "loadingDiv";
			panel.CssClass = "loadingDetails";
			panel.Style[HtmlTextWriterStyle.Display] = "none";
			Image image = new Image();
			image.ImageUrl = ThemeResource.GetThemeResource(this, "progress_sm.gif");
			image.Style[HtmlTextWriterStyle.MarginTop] = "1px";
			image.AlternateText = Strings.Loading;
			Literal literal = new Literal();
			literal.Text = Strings.Loading;
			panel.Controls.Add(image);
			panel.Controls.Add(literal);
			return panel;
		}

		private Panel CreateNoPermissionDivPanel()
		{
			Panel panel = new Panel();
			panel.ID = "noPermissionDiv";
			panel.CssClass = "noPermissionPane";
			panel.Style[HtmlTextWriterStyle.Display] = "none";
			Literal literal = new Literal();
			literal.Text = Strings.NoPermissionToView;
			panel.Controls.Add(literal);
			return panel;
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if (string.IsNullOrEmpty(this.CssClass))
			{
				this.CssClass = "masterDetailsPane";
			}
			else
			{
				this.CssClass += " masterDetailsPane";
			}
			base.AddAttributesToRender(writer);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if (string.IsNullOrEmpty(this.SourceID))
			{
				throw new InvalidOperationException("DetailsPane requires the SourceID parameter to be set");
			}
			Control control = this.NamingContainer.FindControl(this.SourceID);
			if (control != null)
			{
				ListView listView = control as ListView;
				if (listView != null)
				{
					this.ListViewID = listView.ClientID;
				}
				else
				{
					EcpCollectionEditor ecpCollectionEditor = control as EcpCollectionEditor;
					if (ecpCollectionEditor != null)
					{
						this.ListViewID = ecpCollectionEditor.ListViewID;
					}
				}
			}
			if (string.IsNullOrEmpty(this.ListViewID))
			{
				throw new InvalidOperationException(string.Format("Cannot find control that corresponds to SourceID '{0}' passed to the details pane with ID '{1}'. ", this.SourceID, this.ID));
			}
			if (string.IsNullOrEmpty(this.BaseUrl))
			{
				throw new InvalidOperationException("DetailsPane requires the BaseUrl property to be set");
			}
			this.BaseUrl = base.ResolveClientUrl(this.BaseUrl);
			if (string.IsNullOrEmpty(this.FrameTitle))
			{
				throw new InvalidOperationException("DetailsPane requires the FrameTitle property to be set, otherwise there is accessibility issue for screen readers.");
			}
			if (this.TypeMappings.Count != 0)
			{
				foreach (TypeMapping typeMapping in this.TypeMappings)
				{
					if (string.IsNullOrWhiteSpace(typeMapping.BaseUrl))
					{
						throw new InvalidOperationException("BaseUrl in TypeMappings property cannot be empty string");
					}
					typeMapping.BaseUrl = base.ResolveClientUrl(typeMapping.BaseUrl);
					if (typeMapping.InRole == null)
					{
						typeMapping.InRole = new bool?(LoginUtil.CheckUrlAccess(typeMapping.BaseUrl));
					}
				}
			}
		}

		private bool ShowToolbar
		{
			get
			{
				return this.Commands.Count > 0;
			}
		}

		[DefaultValue(true)]
		public bool ShowPaddingPanels { get; set; }

		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue(null)]
		public CommandCollection Commands
		{
			get
			{
				return this.toolbar.Commands;
			}
		}

		public string ListViewID { get; private set; }

		public string BaseUrl { get; set; }

		public string ContentPanelID
		{
			get
			{
				return this.contentPanel.ClientID;
			}
		}

		public string LoadingDivID
		{
			get
			{
				return this.loadingDiv.ClientID;
			}
		}

		public string MiddleEmptyPanelID
		{
			get
			{
				if (this.middleEmptyPanel == null)
				{
					return null;
				}
				return this.middleEmptyPanel.ClientID;
			}
		}

		public string TopEmptyPanelID
		{
			get
			{
				if (this.topEmptyPanel == null)
				{
					return null;
				}
				return this.topEmptyPanel.ClientID;
			}
		}

		public string BottomEmptyPanelID
		{
			get
			{
				if (this.bottomEmptyPanel == null)
				{
					return null;
				}
				return this.bottomEmptyPanel.ClientID;
			}
		}

		public string FrameID
		{
			get
			{
				return this.frame.ClientID;
			}
		}

		public string FrameTitle { get; set; }

		public string SourceID { get; set; }

		public List<TypeMapping> TypeMappings
		{
			get
			{
				return this.typeMappings;
			}
		}

		public string TypeProperty { get; set; }

		public string ArgumentProperty { get; set; }

		public string ContentPaneCssClass { get; set; }

		[DefaultValue(false)]
		public bool SuppressFrameCache { get; set; }

		protected override void BuildScriptDescriptor(ScriptComponentDescriptor descriptor)
		{
			base.BuildScriptDescriptor(descriptor);
			descriptor.AddComponentProperty("ListView", this.ListViewID, this);
			descriptor.AddScriptProperty("BaseTypeMapping", new TypeMapping
			{
				BaseUrl = this.BaseUrl,
				InRole = new bool?(LoginUtil.CheckUrlAccess(this.BaseUrl))
			}.ToJsonString(null));
			if (this.TypeProperty != "Type")
			{
				descriptor.AddProperty("TypeProperty", this.TypeProperty);
			}
			if (!string.IsNullOrWhiteSpace(this.ArgumentProperty))
			{
				descriptor.AddProperty("ArgumentProperty", this.ArgumentProperty);
			}
			descriptor.AddScriptProperty("TypeMappings", this.TypeMappings.ToJsonString(null));
			if (this.SuppressFrameCache)
			{
				descriptor.AddProperty("SuppressFrameCache", true);
			}
			descriptor.AddElementProperty("ContentPanel", this.ContentPanelID, true);
			descriptor.AddElementProperty("LoadingDiv", this.LoadingDivID, true);
			descriptor.AddElementProperty("NoPermissionDiv", this.noPermissionDiv.ClientID, true);
			if (this.ShowPaddingPanels)
			{
				descriptor.AddProperty("ShowPaddingPanels", true);
				descriptor.AddElementProperty("TopEmptyPanel", this.TopEmptyPanelID, true);
				descriptor.AddElementProperty("MiddleEmptyPanel", this.MiddleEmptyPanelID, true);
				descriptor.AddElementProperty("BottomEmptyPanel", this.BottomEmptyPanelID, true);
			}
			descriptor.AddElementProperty("Frame", this.FrameID, true);
			descriptor.AddProperty("FrameTitle", this.FrameTitle, true);
			if (this.toolbar != null && this.ShowToolbar)
			{
				descriptor.AddComponentProperty("Toolbar", this.toolbar.ClientID);
			}
		}

		private Panel toolbarPanel;

		private ToolBar toolbar;

		private Panel contentPanel;

		private Panel loadingDiv;

		private Panel noPermissionDiv;

		private Panel topEmptyPanel;

		private Panel middleEmptyPanel;

		private Panel bottomEmptyPanel;

		private HtmlGenericControl frame;

		private List<TypeMapping> typeMappings = new List<TypeMapping>();
	}
}
