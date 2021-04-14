using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.DDIService
{
	public class FederationCodeBehind
	{
		public static void InitializateForGettingState(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			dataTable.Rows[0]["FederationDisabled"] = false;
			dataTable.Rows[0]["FederationPartialDisabled"] = false;
		}

		public static void SetIfFederatedDisable(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			if (!Datacenter.IsMultiTenancyEnabled() && store.GetDataObject("FederationTrust") == null)
			{
				dataTable.Rows[0]["FederationDisabled"] = true;
			}
		}

		public static void SetIfFederatedPartialDisable(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			if (store.GetDataObject("FederatedOrganizationIdentifier") != null && !(bool)store.GetValue("FederatedOrganizationIdentifier", "Enabled") && store.GetValue("FederatedOrganizationIdentifier", "AccountNamespace") != null)
			{
				dataTable.Rows[0]["FederationDisabled"] = true;
				dataTable.Rows[0]["FederationPartialDisabled"] = true;
			}
		}

		public static void ExtractAccountNamespaceAndSharingDomains(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			if (store.GetDataObject("FederatedOrganizationIdentifier") == null)
			{
				return;
			}
			FederatedOrganizationIdWithDomainStatus federatedOrganizationIdWithDomainStatus = (FederatedOrganizationIdWithDomainStatus)store.GetDataObject("FederatedOrganizationIdentifier");
			SmtpDomain smtpDomain = (federatedOrganizationIdWithDomainStatus.AccountNamespace == null) ? null : new SmtpDomain(FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(federatedOrganizationIdWithDomainStatus.AccountNamespace.Domain));
			MultiValuedProperty<FederatedDomain> domains = federatedOrganizationIdWithDomainStatus.Domains;
			dataTable.Rows[0]["HasAccountNamespace"] = false;
			dataTable.Rows[0]["HasFederatedDomains"] = false;
			if (smtpDomain != null)
			{
				List<object> list = new List<object>();
				dataTable.Rows[0]["HasAccountNamespace"] = true;
				dataTable.Rows[0]["Name"] = federatedOrganizationIdWithDomainStatus.AccountNamespace.Domain;
				foreach (FederatedDomain federatedDomain in domains)
				{
					if (federatedDomain.Domain.Domain.Equals(smtpDomain.Domain, StringComparison.InvariantCultureIgnoreCase))
					{
						dataTable.Rows[0]["AccountNamespace"] = (SharingDomain)federatedDomain;
					}
					else
					{
						list.Add((SharingDomain)federatedDomain);
					}
				}
				if (list.Count > 0)
				{
					dataTable.Rows[0]["SharingEnabledDomains"] = list;
					dataTable.Rows[0]["HasFederatedDomains"] = true;
				}
			}
		}

		public static void ExtractPendingAccountNamespaceAndSharingDomains(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			if (store.GetDataObject("PendingFederatedDomain") == null)
			{
				return;
			}
			PendingFederatedDomain pendingFederatedDomain = store.GetDataObject("PendingFederatedDomain") as PendingFederatedDomain;
			SmtpDomain pendingAccountNamespace = pendingFederatedDomain.PendingAccountNamespace;
			SmtpDomain[] pendingDomains = pendingFederatedDomain.PendingDomains;
			if (dataTable.Rows[0]["HasAccountNamespace"] != DBNull.Value && !(bool)dataTable.Rows[0]["HasAccountNamespace"] && pendingAccountNamespace != null)
			{
				dataTable.Rows[0]["AccountNamespace"] = (SharingDomain)pendingAccountNamespace;
			}
			List<object> list = new List<object>();
			if (dataTable.Rows[0]["SharingEnabledDomains"] != DBNull.Value)
			{
				foreach (object item in ((IEnumerable)dataTable.Rows[0]["SharingEnabledDomains"]))
				{
					list.Add(item);
				}
			}
			foreach (SmtpDomain smtpDomain in pendingDomains)
			{
				if (!list.Contains((SharingDomain)smtpDomain))
				{
					list.Add((SharingDomain)smtpDomain);
				}
			}
			if (list.Count > 0)
			{
				dataTable.Rows[0]["SharingEnabledDomains"] = list;
				dataTable.Rows[0]["HasFederatedDomains"] = true;
			}
		}

		public static void PreparePendingDomains(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			List<SmtpDomain> list = new List<SmtpDomain>();
			if (dataTable.Rows[0]["AddedSharingEnabledDomains"] != DBNull.Value)
			{
				foreach (object obj in ((IEnumerable)dataTable.Rows[0]["AddedSharingEnabledDomains"]))
				{
					SmtpDomain item = (SmtpDomain)obj;
					list.Add(item);
				}
			}
			row["PendingDomains"] = list.ToArray();
		}

		public static void GenerateAddAndRemoveCollection(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			if (store.GetDataObject("FederatedOrganizationIdentifier") == null)
			{
				return;
			}
			List<object> list = new List<object>();
			if (dataTable.Rows[0]["SharingEnabledDomains"] != DBNull.Value)
			{
				foreach (object obj in ((IEnumerable)dataTable.Rows[0]["SharingEnabledDomains"]))
				{
					SharingDomain sharingDomain = (SharingDomain)obj;
					list.Add((SmtpDomain)sharingDomain);
				}
			}
			FederatedOrganizationIdWithDomainStatus federatedOrganizationIdWithDomainStatus = (FederatedOrganizationIdWithDomainStatus)store.GetDataObject("FederatedOrganizationIdentifier");
			SmtpDomain smtpDomain = (federatedOrganizationIdWithDomainStatus.AccountNamespace == null) ? null : new SmtpDomain(FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(federatedOrganizationIdWithDomainStatus.AccountNamespace.Domain));
			if (smtpDomain == null && dataTable.Rows[0]["AccountNamespace"] != DBNull.Value)
			{
				smtpDomain = (SmtpDomain)((SharingDomain)dataTable.Rows[0]["AccountNamespace"]);
			}
			if (smtpDomain != null)
			{
				list.Remove(smtpDomain);
			}
			MultiValuedProperty<FederatedDomain> multiValuedProperty = (MultiValuedProperty<FederatedDomain>)store.GetValue("FederatedOrganizationIdentifier", "Domains");
			List<object> list2 = new List<object>();
			if (multiValuedProperty != null)
			{
				foreach (FederatedDomain federatedDomain in multiValuedProperty)
				{
					if (!federatedDomain.Domain.Domain.Equals(smtpDomain.Domain, StringComparison.InvariantCultureIgnoreCase))
					{
						SmtpDomain item = new SmtpDomain(federatedDomain.Domain.Domain);
						if (list.Contains(item))
						{
							list.Remove(item);
						}
						else
						{
							list2.Add(item);
						}
					}
				}
			}
			dataTable.Rows[0]["AddedSharingEnabledDomains"] = list;
			dataTable.Rows[0]["RemovedSharingEnabledDomains"] = list2;
		}

		public static void AddDomainProof(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			string domainName = (string)dataTable.Rows[0]["DomainName"];
			string proof = (string)dataTable.Rows[0]["Proof"];
			SharingDomain sharingDomain = new SharingDomain();
			sharingDomain.DomainName = domainName;
			sharingDomain.SharingStatus = 3;
			sharingDomain.Proof = proof;
			sharingDomain.RawIdentity = sharingDomain.DomainName;
			if (dataTable.Rows[0]["GetFederatedDomainProofResult"] == DBNull.Value)
			{
				dataTable.Rows[0]["GetFederatedDomainProofResult"] = new List<SharingDomain>();
			}
			List<SharingDomain> list = (List<SharingDomain>)dataTable.Rows[0]["GetFederatedDomainProofResult"];
			list.Add(sharingDomain);
		}

		public static void AddDomainProofForFailedAccountNamespace(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			SharingDomain sharingDomain = (SharingDomain)dataTable.Rows[0]["AccountNamespace"];
			string proof = (string)dataTable.Rows[0]["Proof"];
			sharingDomain.Proof = proof;
		}

		public static void AddDomainProofForFailedDomain(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			SharingDomain sharingDomain = (SharingDomain)dataTable.Rows[0]["domain"];
			string proof = (string)dataTable.Rows[0]["Proof"];
			sharingDomain.Proof = proof;
			sharingDomain.RawIdentity = sharingDomain.DomainName;
		}

		public static void CalculateStringForEnabledUI(DataRow row, DataTable dataTable, DataObjectStore store)
		{
			if (store.GetValue("FederatedOrganizationIdentifier", "AccountNamespace") == null)
			{
				dataTable.Rows[0]["EnabledStringType"] = 0;
				return;
			}
			if (((SmtpDomain[])store.GetValue("PendingFederatedDomain", "PendingDomains")).Length > 0)
			{
				dataTable.Rows[0]["EnabledStringType"] = 1;
				return;
			}
			dataTable.Rows[0]["EnabledStringType"] = 2;
		}
	}
}
