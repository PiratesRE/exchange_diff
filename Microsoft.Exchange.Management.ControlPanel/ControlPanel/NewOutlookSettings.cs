using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewOutlookSettings : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.IsPostBack)
			{
				LanguageList langList = new LanguageList();
				PropertiesContentPanel propertiesContentPanel = (PropertiesContentPanel)this.outlooksettings.Controls[0];
				DropDownList dropDownList = (DropDownList)propertiesContentPanel.FindControl("ddLocale");
				HashSet<int> expectedCultureLcids = LanguagePackInfo.expectedCultureLcids;
				IEnumerable<ListItem> source = expectedCultureLcids.Select(delegate(int lcid)
				{
					CultureInfo cultureInfo = new CultureInfo(lcid);
					return new ListItem(RtlUtil.ConvertToDecodedBidiString(langList.GetDisplayValue(cultureInfo.Name), RtlUtil.IsRtl), cultureInfo.Name);
				});
				dropDownList.Items.AddRange((from i in source
				orderby i.Text
				select i).ToArray<ListItem>());
			}
		}

		protected PropertyPageSheet outlooksettings;
	}
}
