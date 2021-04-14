using System;
using System.Security;
using System.Security.Permissions;
using System.ServiceModel.Activation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public sealed class OrganizationalUnits : DataSourceService
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "Get-OrganizationalUnit?Identity@R:Organization")]
		public PowerShellResults<ExtendedOrganizationalUnit> GetObject(Identity identity)
		{
			PowerShellResults<ExtendedOrganizationalUnit> result;
			try
			{
				result = base.GetObject<ExtendedOrganizationalUnit>("Get-OrganizationalUnit", identity);
			}
			catch (SecurityException innerException)
			{
				result = new PowerShellResults<ExtendedOrganizationalUnit>
				{
					ErrorRecords = new ErrorRecord[]
					{
						new ErrorRecord(new Exception(Strings.MultipleOrganizationalUnit, innerException))
					}
				};
			}
			return result;
		}

		private const string Noun = "OrganizationalUnit";

		internal const string GetCmdlet = "Get-OrganizationalUnit";

		internal const string ReadScope = "@R:Organization";

		private const string GetObjectRole = "Get-OrganizationalUnit?Identity@R:Organization";
	}
}
