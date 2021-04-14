using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClientAccess
{
	[Cmdlet("Uninstall", "CafeIisWebServiceExtensions")]
	[LocDescription(Strings.IDs.UninstallCafeIisWebServiceExtensions)]
	public sealed class UninstallCafeIisWebServiceExtensions : UninstallIisWebServiceExtensions
	{
		protected override IisWebServiceExtension this[int i]
		{
			get
			{
				return CafeIisWebServiceExtension.AllExtensions[i];
			}
		}

		protected override int ExtensionCount
		{
			get
			{
				return CafeIisWebServiceExtension.AllExtensions.Length;
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
				return Strings.GetLocalizedString((Strings.IDs)3143233303U);
			}
		}

		protected override string GroupID
		{
			get
			{
				return "MSExchangeCafe";
			}
		}
	}
}
