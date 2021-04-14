using System;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal sealed class RmsClientManagerComponent : ITransportComponent, IDiagnosable
	{
		void ITransportComponent.Load()
		{
		}

		void ITransportComponent.Unload()
		{
		}

		string ITransportComponent.OnUnhandledException(Exception e)
		{
			return null;
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "RmsClientManager";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			XElement xelement = new XElement(((IDiagnosable)this).GetDiagnosticComponentName());
			if (string.IsNullOrEmpty(parameters.Argument) || string.Equals(parameters.Argument, "help", StringComparison.OrdinalIgnoreCase))
			{
				xelement.Add(new XElement("help", "Supported arguments: config, licenses, templates, help [org=tenantId].  If org is specified, it must be the last argument.  Example: Get-ExchangeDiagnosticInfo -Process EdgeTransport -Component RmsClientManager -Argument \"templates org=contoso.com\""));
			}
			else
			{
				OrganizationId organizationId = OrganizationId.ForestWideOrgId;
				bool flag = parameters.Argument.IndexOf("templates", StringComparison.OrdinalIgnoreCase) != -1;
				bool flag2 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1;
				bool flag3 = parameters.Argument.IndexOf("licenses", StringComparison.OrdinalIgnoreCase) != -1;
				int num = parameters.Argument.IndexOf("org=", StringComparison.OrdinalIgnoreCase);
				if (num != -1)
				{
					string[] array = parameters.Argument.Substring(num).Split(RmsClientManagerComponent.OrgSeparator);
					if (array.Length == 2)
					{
						string text = array[1].Trim();
						if (!string.IsNullOrEmpty(text))
						{
							OrganizationIdParameter organization = new OrganizationIdParameter(text);
							ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
							try
							{
								organizationId = MapiTaskHelper.ResolveTargetOrganization(null, organization, rootOrgContainerIdForLocalForest, OrganizationId.ForestWideOrgId, OrganizationId.ForestWideOrgId);
							}
							catch (ManagementObjectNotFoundException)
							{
								return new XElement("error", "Organization not found.");
							}
						}
					}
				}
				if (flag3)
				{
					xelement.Add(RmsClientManager.GetLicenseDiagnosticInfo(organizationId));
				}
				if (flag)
				{
					xelement.Add(RmsClientManager.GetTemplateDiagnosticInfo(organizationId));
				}
				if (flag2)
				{
					xelement.Add(RmsClientManager.GetConfigDiagnosticInfo(organizationId));
				}
			}
			return xelement;
		}

		private static readonly char[] OrgSeparator = new char[]
		{
			'='
		};
	}
}
