using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IDataObjectCreator
	{
		object Create(DataTable table);
	}
}
