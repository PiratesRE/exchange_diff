using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface ILoadableFromProfile
	{
		bool IsLoadableFrom(ResultsLoaderProfile profile, DataRow row);
	}
}
