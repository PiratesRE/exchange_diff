using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxControlToolkit.Design
{
	public class TypedControlIDConverter<T> : ControlIDConverter
	{
		protected override bool FilterControl(Control control)
		{
			return typeof(T).IsInstanceOfType(control);
		}
	}
}
