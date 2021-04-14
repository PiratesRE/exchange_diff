using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public class CulturedAddress : WebControl, INamingContainer
	{
		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate StreetTemplate { get; set; }

		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate CityTemplate { get; set; }

		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate StateProvinceTemplate { get; set; }

		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate ZipPostalTemplate { get; set; }

		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate CountryTemplate { get; set; }

		public CulturedAddress()
		{
			this.CssClass = "divEncapsulation";
		}

		protected override void CreateChildControls()
		{
			CulturedHelper.CreateChildControls(this, "Street,City,StateProvince,ZipPostal,Country", 1, new Dictionary<string, ITemplate>
			{
				{
					"Street",
					this.StreetTemplate
				},
				{
					"ZipPostal",
					this.ZipPostalTemplate
				},
				{
					"City",
					this.CityTemplate
				},
				{
					"StateProvince",
					this.StateProvinceTemplate
				},
				{
					"Country",
					this.CountryTemplate
				}
			});
		}

		private const string DefaultPattern = "Street,City,StateProvince,ZipPostal,Country";
	}
}
