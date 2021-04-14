using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class NewMailGroupMaster : MasterPage, IMasterPage
	{
		public ContentPlaceHolder ContentPlaceHolder
		{
			get
			{
				return (base.Master as IMasterPage).ContentPlaceHolder;
			}
		}
	}
}
