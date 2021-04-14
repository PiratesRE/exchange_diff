using System;
using Microsoft.ManagementConsole;

namespace Microsoft.Exchange.Management.SnapIn
{
	public interface ITestStub
	{
		void InstallStub(NamespaceSnapInBase snapin);
	}
}
