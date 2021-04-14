using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewRemoteDomain : BaseForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (!base.IsPostBack)
			{
				new LanguageList();
				PropertiesContentPanel propertiesContentPanel = (PropertiesContentPanel)this.newEditRemoteDomainPropertySheet.Controls[0];
				DropDownList dropDownList = (DropDownList)propertiesContentPanel.FindControl("ddMIMECharacterSet");
				DropDownList dropDownList2 = (DropDownList)propertiesContentPanel.FindControl("ddNonMIMECharacterSet");
				DomainContentConfig.CharacterSetInfo[] characterSetList = DomainContentConfig.CharacterSetList;
				IEnumerable<ListItem> source = from aCharacterSet in characterSetList
				select new ListItem(aCharacterSet.CharsetDescription, aCharacterSet.CharacterSetName);
				dropDownList2.Items.AddRange(source.ToArray<ListItem>());
				dropDownList.Items.AddRange(source.ToArray<ListItem>());
			}
		}

		protected PropertyPageSheet newEditRemoteDomainPropertySheet;
	}
}
