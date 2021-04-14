using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal interface IInstallable
	{
		bool IsUnpacked { get; }

		Version UnpackedVersion { get; }

		bool IsDatacenterUnpacked { get; }

		Version UnpackedDatacenterVersion { get; }

		bool IsInstalled { get; }

		Version InstalledVersion { get; }

		string InstalledPath { get; }

		Task InstallTask { get; }

		Task DisasterRecoveryTask { get; }

		Task UninstallTask { get; }

		ValidatingTask ValidateTask { get; }
	}
}
