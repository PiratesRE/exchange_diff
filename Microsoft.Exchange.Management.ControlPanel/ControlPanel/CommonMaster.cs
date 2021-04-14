using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClientScriptResource(null, "Microsoft.Exchange.Management.ControlPanel.Client.Common.js")]
	public class CommonMaster : MasterPage, IMasterPage, IThemable
	{
		protected ContentPlaceHolder ResultPanePlaceHolder { get; set; }

		public ContentPlaceHolder ContentPlaceHolder
		{
			get
			{
				return this.ResultPanePlaceHolder;
			}
		}

		public string Role
		{
			get
			{
				return this.role;
			}
		}

		public FeatureSet FeatureSet { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			string title = this.Page.Title;
			this.role = "document";
			using (SiteMapDataSource siteMapDataSource = new SiteMapDataSource())
			{
				SiteMapNode siteMapNode = siteMapDataSource.Provider.CurrentNode;
				if (siteMapNode != null)
				{
					title = siteMapNode.Title;
					this.role = "application";
					SiteMapNode rootNode = siteMapDataSource.Provider.RootNode;
					while (siteMapNode.ParentNode != null && siteMapNode.ParentNode != rootNode)
					{
						siteMapNode = siteMapNode.ParentNode;
					}
					string pageTitleFormat = Util.GetPageTitleFormat(siteMapNode);
					if (pageTitleFormat != null)
					{
						this.Page.Title = string.Format(pageTitleFormat, title);
					}
				}
			}
		}

		public void AddCssFiles(string cssFiles)
		{
			if (!string.IsNullOrEmpty(cssFiles))
			{
				if (this.cssFiles == null)
				{
					this.cssFiles = new List<string>();
				}
				if (cssFiles.IndexOf(',') > 0)
				{
					string[] array = cssFiles.Split(CommonMaster.CommaList);
					for (int i = 0; i < array.Length; i++)
					{
						this.AddCssFile(array[i]);
					}
					return;
				}
				this.AddCssFile(cssFiles);
			}
		}

		private void AddCssFile(string file)
		{
			if (!this.cssFiles.Contains(file))
			{
				this.cssFiles.Add(file);
			}
		}

		private static readonly char[] CommaList = new char[]
		{
			','
		};

		private string role;

		protected List<string> cssFiles;
	}
}
