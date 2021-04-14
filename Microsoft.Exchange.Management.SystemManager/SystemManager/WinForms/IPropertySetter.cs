using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IPropertySetter
	{
		void Set(object dataObject, object value);
	}
}
