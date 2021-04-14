using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClientAccess
{
	[LocDescription(Strings.IDs.InstallClientAccessIisWebServiceExtensions)]
	[Cmdlet("Install", "ClientAccessIisWebServiceExtensions")]
	public sealed class InstallClientAccessWebExtensions : InstallIisWebServiceExtensions
	{
		protected override IisWebServiceExtension this[int i]
		{
			get
			{
				return IisWebServiceExtension.AllExtensions[i];
			}
		}

		protected override int ExtensionCount
		{
			get
			{
				return IisWebServiceExtension.AllExtensions.Length;
			}
		}

		protected override string HostName
		{
			get
			{
				return "localhost";
			}
		}

		protected override string GroupDescription
		{
			get
			{
				return Strings.GetLocalizedString(Strings.IDs.ClientAccessIisWebServiceExtensionsDescription);
			}
		}

		protected override string GroupID
		{
			get
			{
				return "MSExchangeClientAccess";
			}
		}
	}
}
