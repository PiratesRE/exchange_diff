using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CulturedName : WebControl, INamingContainer
	{
		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate FirstNameTemplate { get; set; }

		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate InitialTemplate { get; set; }

		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate LastNameTemplate { get; set; }

		public CulturedName()
		{
			this.CssClass = "divEncapsulation";
		}

		protected override void CreateChildControls()
		{
			CulturedHelper.CreateChildControls(this, "FirstName,Initials,LastName", 0, new Dictionary<string, ITemplate>
			{
				{
					"FirstName",
					this.FirstNameTemplate
				},
				{
					"Initials",
					this.InitialTemplate
				},
				{
					"LastName",
					this.LastNameTemplate
				}
			});
		}

		public const string DefaultPattern = "FirstName,Initials,LastName";
	}
}
