using System;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewDistributionGroup : OrgSettingsPage
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			EcpCollectionEditor ecpCollectionEditor = (EcpCollectionEditor)(base.Master as IMasterPage).ContentPlaceHolder.FindControl("Wizard1").FindControl("generalInfoSection").FindControl("ceManagedBy");
			ecpCollectionEditor.IsUsingOwaPeoplePicker = true;
			ecpCollectionEditor.PickerCallerType = PickerCallerType.GroupsOwners;
			EcpCollectionEditor ecpCollectionEditor2 = (EcpCollectionEditor)(base.Master as IMasterPage).ContentPlaceHolder.FindControl("Wizard1").FindControl("generalInfoSection").FindControl("ceMembers");
			ecpCollectionEditor2.IsUsingOwaPeoplePicker = true;
			ecpCollectionEditor2.PickerCallerType = PickerCallerType.GroupsMembers;
			PropertyPageSheet propertyPageSheet = (PropertyPageSheet)(base.Master as IMasterPage).ContentPlaceHolder.FindControl("Wizard1");
			propertyPageSheet.Attributes["vm-IsInOwaOption"] = "true";
		}
	}
}
