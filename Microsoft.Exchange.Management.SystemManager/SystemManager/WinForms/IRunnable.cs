using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IRunnable
	{
		bool IsRunnable(DataRow row);
	}
}
