using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.IisTasks
{
	public abstract class ManageIisWebServiceExtensions : Task
	{
		protected abstract IisWebServiceExtension this[int i]
		{
			get;
		}

		protected abstract int ExtensionCount { get; }

		protected abstract string HostName { get; }

		protected abstract string GroupDescription { get; }

		protected abstract string GroupID { get; }
	}
}
