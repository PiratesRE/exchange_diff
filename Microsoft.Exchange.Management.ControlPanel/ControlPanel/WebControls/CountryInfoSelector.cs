using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	[ToolboxData("<{0}:CountryInfoSelector runat=server></{0}:CountryInfoSelector>")]
	[DataBindingHandler("System.Web.UI.Design.TextDataBindingHandler, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class CountryInfoSelector : DropDownList
	{
		public bool SkipEmptyCountry
		{
			get
			{
				return this.skipEmptyCountry;
			}
			set
			{
				this.skipEmptyCountry = value;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			List<CountryInfo> list = new List<CountryInfo>(CountryInfo.AllCountryInfos);
			list.Sort();
			if (!this.SkipEmptyCountry)
			{
				this.Items.Add(new ListItem(string.Empty, string.Empty));
			}
			bool isRtl = RtlUtil.IsRtl;
			foreach (CountryInfo countryInfo in list)
			{
				this.Items.Add(new ListItem(RtlUtil.ConvertToDecodedBidiString(countryInfo.LocalizedDisplayName, isRtl), countryInfo.Name));
			}
		}

		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			writer.AddAttribute("role", "combobox");
			base.AddAttributesToRender(writer);
		}

		private bool skipEmptyCountry;
	}
}
