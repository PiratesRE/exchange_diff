using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class EditMailGroupMaster : MasterPage, IMasterPage
	{
		public ContentPlaceHolder ContentPlaceHolder
		{
			get
			{
				return ((IMasterPage)base.Master).ContentPlaceHolder;
			}
		}
	}
}
