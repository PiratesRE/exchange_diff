using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal interface IPFTreeTask
	{
		OrganizationIdParameter Organization { get; set; }

		ADObjectId RootOrgContainerId { get; }

		Fqdn DomainController { get; }

		OrganizationId CurrentOrganizationId { get; }

		OrganizationId ExecutingUserOrganizationId { get; }

		IConfigDataProvider DataSession { get; }

		ITopologyConfigurationSession GlobalConfigSession { get; }

		OrganizationId ResolveCurrentOrganization();

		TObject GetDataObject<TObject>(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError) where TObject : IConfigurable, new();

		void WriteVerbose(LocalizedString text);

		void WriteWarning(LocalizedString text);

		void WriteError(Exception exception, ErrorCategory category, object target);
	}
}
