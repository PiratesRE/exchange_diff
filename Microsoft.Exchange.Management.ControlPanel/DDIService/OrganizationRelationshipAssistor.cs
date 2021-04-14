using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DDIService
{
	public class OrganizationRelationshipAssistor
	{
		private static MultiValuedProperty<string> ToStringMVP(MultiValuedProperty<SmtpDomain> domainsMVP)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			if (domainsMVP != null)
			{
				foreach (SmtpDomain smtpDomain in domainsMVP)
				{
					multiValuedProperty.Add(smtpDomain.ToString());
				}
			}
			multiValuedProperty.Sort();
			return multiValuedProperty;
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			OrganizationRelationship organizationRelationship = store.GetDataObject("OrganizationRelationship") as OrganizationRelationship;
			if (organizationRelationship != null && dataTable.Rows.Count == 1)
			{
				DataRow dataRow = dataTable.Rows[0];
				if (organizationRelationship.FreeBusyAccessLevel == FreeBusyAccessLevel.None)
				{
					dataRow["FreeBusyAccessEnabled"] = false;
					dataRow["FreeBusyAccessLevel"] = FreeBusyAccessLevel.AvailabilityOnly;
				}
				dataRow["DomainNames"] = OrganizationRelationshipAssistor.ToStringMVP(organizationRelationship.DomainNames);
				dataRow["FormattedDomainNames"] = DDIHelper.JoinList<SmtpDomain>(organizationRelationship.DomainNames, (SmtpDomain domain) => domain.Domain);
			}
		}

		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			dataTable.BeginLoadData();
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow["FormattedDomainNames"] = DDIHelper.JoinList<SmtpDomain>(dataRow["DomainNames"] as MultiValuedProperty<SmtpDomain>, (SmtpDomain domain) => domain.Domain);
			}
			dataTable.EndLoadData();
		}

		private const string FreeBusyAccessEnabledColumnName = "FreeBusyAccessEnabled";

		private const string FreeBusyAccessLevelColumnName = "FreeBusyAccessLevel";

		private const string OrganizationRelationShipObjectName = "OrganizationRelationship";

		private const string DomainNamesColumnName = "DomainNames";

		private const string FormattedDomainNamesColumnName = "FormattedDomainNames";
	}
}
