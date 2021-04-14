using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;
using Microsoft.Exchange.Hygiene.Data.DataProvider;
using Microsoft.Exchange.Hygiene.Data.Directory;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public abstract class ConfigDataProviderOperation : DalProbeOperation
	{
		[XmlAttribute]
		public DalType Database { get; set; }

		[XmlAttribute]
		public string DataType { get; set; }

		[XmlAttribute]
		public string OrganizationTag { get; set; }

		public sealed override void Execute(IDictionary<string, object> variables)
		{
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalConfigSession.GetProbeOrganizations(this.OrganizationTag);
			foreach (ProbeOrganizationInfo probeOrganizationInfo in probeOrganizations)
			{
				IConfigDataProvider configDataProvider = this.CreateDataProvider(probeOrganizationInfo);
				this.ExecuteConfigDataProviderOperation(configDataProvider, variables);
			}
		}

		protected static IEnumerable<PropertyDefinition> GetPropertyDefinitions(IConfigurable iconfigObj)
		{
			ConfigurableObject configurableObject = iconfigObj as ConfigurableObject;
			if (configurableObject != null)
			{
				return configurableObject.ObjectSchema.AllProperties;
			}
			ConfigurablePropertyBag configurablePropertyBag = (ConfigurablePropertyBag)iconfigObj;
			return configurablePropertyBag.GetPropertyDefinitions(false);
		}

		protected abstract void ExecuteConfigDataProviderOperation(IConfigDataProvider configDataProvider, IDictionary<string, object> variables);

		private IConfigDataProvider CreateDataProvider(ProbeOrganizationInfo probeOrganizationInfo)
		{
			switch (this.Database)
			{
			case DalType.Global:
				throw new NotSupportedException(string.Format("DalType {0} is not supported.", this.Database));
			case DalType.Tenant:
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromTenantCUName(this.OrganizationTag);
				return DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, probeOrganizationInfo.ProbeOrganizationId.ObjectGuid, 115, "CreateDataProvider", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DAL\\Probes\\ConfigDataProviderOperation.cs");
			}
			case DalType.Recipient:
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(probeOrganizationInfo.ProbeOrganizationId.ObjectGuid);
				return DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 118, "CreateDataProvider", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\DAL\\Probes\\ConfigDataProviderOperation.cs");
			}
			default:
				return ConfigDataProviderFactory.Default.Create((DatabaseType)this.Database);
			}
		}
	}
}
