using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DisconnectedMailbox : SlabControl
	{
		protected override void OnLoad(EventArgs e)
		{
			WebServiceReference webServiceReference = new WebServiceReference("~/DDI/DDIService.svc?schema=DisconnectedMailbox&workflow=GetServerDropDown");
			try
			{
				PowerShellResults<JsonDictionary<object>> list = webServiceReference.GetList(null, null);
				if (list.Output != null && list.Output.Length > 0)
				{
					Microsoft.Exchange.Management.ControlPanel.WebControls.ListView listView = (Microsoft.Exchange.Management.ControlPanel.WebControls.ListView)this.FindControl("disconnectedMailboxListView");
					listView.Views = new List<ListItem>();
					JsonDictionary<object>[] output = list.Output;
					for (int i = 0; i < output.Length; i++)
					{
						Dictionary<string, object> dictionary = output[i];
						listView.Views.Add(new ListItem((string)dictionary["Name"], (string)dictionary["Name"]));
					}
					listView.Views.Sort((ListItem item1, ListItem item2) => item1.Text.CompareTo(item2.Text));
				}
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			base.OnLoad(e);
		}
	}
}
