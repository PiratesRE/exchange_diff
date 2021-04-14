using System;
using System.Web.UI;

namespace AjaxControlToolkit
{
	public interface IControlResolver
	{
		Control ResolveControl(string controlId);
	}
}
