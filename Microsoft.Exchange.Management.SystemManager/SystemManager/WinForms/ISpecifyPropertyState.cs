using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface ISpecifyPropertyState
	{
		void SetPropertyState(string propertyName, PropertyState state, string message);
	}
}
