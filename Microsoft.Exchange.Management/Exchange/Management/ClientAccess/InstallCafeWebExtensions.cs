using System;
using System.Management.Automation;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClientAccess
{
	[LocDescription(Strings.IDs.InstallCafeIisWebServiceExtensions)]
	[Cmdlet("Install", "CafeIisWebServiceExtensions")]
	public sealed class InstallCafeWebExtensions : InstallIisWebServiceExtensions
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
