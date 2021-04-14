using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface ISelectedObjectsProvider
	{
		DataTable SelectedObjects { get; }
	}
}
