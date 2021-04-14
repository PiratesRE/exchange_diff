using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class Bookmark : WebControl
	{
		public Bookmark() : base(HtmlTextWriterTag.Div)
		{
			this.CssClass = "bookmark";
			base.Attributes.Add("data-control", "Bookmark");
		}

		public void AddEntry(string anchor, string title, string workflowName, string visibilityBinding)
		{
			if (string.IsNullOrEmpty(anchor))
			{
				throw new ArgumentNullException("anchor");
			}
			if (string.IsNullOrEmpty(title))
			{
				ArgumentNullException ex = new ArgumentNullException("title");
				ex.Data["SectionID"] = anchor;
				throw ex;
			}
			this.anchors.Add(anchor);
			this.titles.Add(HttpUtility.HtmlEncode(title));
			this.workflows.Add(workflowName);
			if (!string.IsNullOrEmpty(visibilityBinding))
			{
				this.sectionVisibilityBinding.Add(anchor, visibilityBinding);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			this.RenderBeginTag(writer);
			int num = 0;
			foreach (string text in this.anchors)
			{
				string text2 = this.titles[num];
				string text3 = this.workflows[num];
				string text4 = this.sectionVisibilityBinding.ContainsKey(text) ? string.Format("data-control=\"Panel\" data-visible=\"{0}\"", this.sectionVisibilityBinding[text]) : string.Empty;
				writer.Write("<span {4}><a name=\"{0}\" id=\"bookmarklink_{1}\" role=\"tab\" ecp_index=\"{1}\" workflow=\"{2}\">{3}</a><div class=\"bmSplit\" ></div></span>", new object[]
				{
					text,
					num,
					text3,
					text2,
					text4
				});
				num++;
			}
			writer.Write("<div id=\"ptr\" class=\"ptr CommonSprite ArrowExpand\" style=\"display:none\" ></div>");
			this.RenderEndTag(writer);
		}

		private List<string> anchors = new List<string>();

		private List<string> titles = new List<string>();

		private List<string> workflows = new List<string>();

		private Dictionary<string, string> sectionVisibilityBinding = new Dictionary<string, string>();
	}
}
