using System;
using System.Security.Permissions;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ImportContactList : DataSourceService, IImportContactList, IImportObjectService<ImportContactsResult, ImportContactListParameters>
	{
		[PrincipalPermission(SecurityAction.Demand, Role = "MultiTenant+Import-ContactList?Identity&CSV&CSVStream@W:Self|Organization")]
		public PowerShellResults<ImportContactsResult> ImportObject(Identity identity, ImportContactListParameters properties)
		{
			PowerShellResults<ImportContactsResult> powerShellResults = base.SetObject<ImportContactsResult, ImportContactListParameters>("Import-ContactList", identity, properties);
			bool succeeded = powerShellResults.Succeeded;
			return powerShellResults;
		}

		internal const string ImportCmdlet = "Import-ContactList";

		internal const string WriteScope = "@W:Self|Organization";

		private const string Noun = "ContactList";

		private const string ImportObjectRole = "MultiTenant+Import-ContactList?Identity&CSV&CSVStream@W:Self|Organization";
	}
}
