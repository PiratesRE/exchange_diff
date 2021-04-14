using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class CertificateHelper
	{
		private static void GetOneItem(DataRow row, string serviceDomain, string host)
		{
			if (DBNull.Value.Equals(row[serviceDomain]))
			{
				row[serviceDomain] = host;
				return;
			}
			string text = (string)row[serviceDomain];
			if (text.IndexOf(host) == -1)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(text);
				stringBuilder.Append(",");
				stringBuilder.Append(host);
				row[serviceDomain] = stringBuilder.ToString();
			}
		}

		private static void UpdateServiceDomain(DataTable dataTable, DataObjectStore store, string serviceName)
		{
			DataRow row = dataTable.Rows[0];
			ADVirtualDirectory advirtualDirectory = (ADVirtualDirectory)store.GetDataObject(serviceName + "VirtualDirectory");
			if (advirtualDirectory != null && advirtualDirectory.ExternalUrl != null)
			{
				CertificateHelper.GetOneItem(row, serviceName + "ExternalDomain", advirtualDirectory.ExternalUrl.Host);
			}
			if (advirtualDirectory != null && advirtualDirectory.InternalUrl != null)
			{
				CertificateHelper.GetOneItem(row, serviceName + "InternalDomain", advirtualDirectory.InternalUrl.Host);
			}
		}

		private static void UpdateConfigDomain(DataTable dataTable, DataObjectStore store, string cfgName)
		{
			DataRow row = dataTable.Rows[0];
			PopImapAdConfiguration popImapAdConfiguration = (PopImapAdConfiguration)store.GetDataObject(cfgName + "Configuration");
			if (popImapAdConfiguration != null && !string.IsNullOrEmpty(popImapAdConfiguration.X509CertificateName))
			{
				CertificateHelper.GetOneItem(row, cfgName + "Domain", popImapAdConfiguration.X509CertificateName);
			}
		}

		public static void OnPreGetExchangeServer(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			Identity identity = dataRow["Identity"] as Identity;
			if (identity == null || string.IsNullOrEmpty(identity.RawIdentity))
			{
				throw new ArgumentException("identity");
			}
			string[] array = identity.RawIdentity.Split(new char[]
			{
				'\\'
			});
			if (array == null || array.Length < 2)
			{
				throw new ArgumentException("identity");
			}
			dataRow["Server"] = array[0];
		}

		public static void GetServerListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			inputRow["MaxMajorVersion"] = "15";
			inputRow["MinMajorVersion"] = "15";
			inputRow["ServerRole"] = ServerRole.ClientAccess.ToString() + "," + ServerRole.Mailbox.ToString();
			store.SetModifiedColumns(new List<string>
			{
				"MaxMajorVersion",
				"MinMajorVersion",
				"ServerRole"
			});
			ServerProperties.GetListPostAction(inputRow, dataTable, store);
		}

		public static void GetOWAVirtualDirectoryPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			CertificateHelper.UpdateServiceDomain(dataTable, store, "Owa");
		}

		public static void GetMobileVirtualDirectoryPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			CertificateHelper.UpdateServiceDomain(dataTable, store, "Mobile");
		}

		public static void GetOABVirtualDirectoryPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			CertificateHelper.UpdateServiceDomain(dataTable, store, "OAB");
		}

		public static void GetWebServicesVirtualDirectoryPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			CertificateHelper.UpdateServiceDomain(dataTable, store, "WebServices");
		}

		public static void GetAutoDiscoverVirtualDirectoryPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			ClientAccessServer clientAccessServer = (ClientAccessServer)store.GetDataObject("ClientAccessServer");
			if (clientAccessServer != null && clientAccessServer.AutoDiscoverServiceInternalUri != null)
			{
				CertificateHelper.GetOneItem(row, "AutoInternalDomain", clientAccessServer.AutoDiscoverServiceInternalUri.Host);
			}
		}

		public static void GetAutoDiscoverAcceptDomainPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			object dataObject = store.GetDataObject("AcceptedDomainWholeObject");
			if (dataObject != null && dataObject is IEnumerable)
			{
				foreach (object obj in (dataObject as IEnumerable))
				{
					Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain acceptedDomain = obj as Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain;
					if (acceptedDomain != null)
					{
						CertificateHelper.GetOneItem(row, "AutoExternalDomain", "AutoDiscover." + acceptedDomain.DomainName.Domain);
					}
				}
			}
		}

		public static void GetPOPSettingsPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			CertificateHelper.UpdateConfigDomain(dataTable, store, "Pop");
		}

		public static void GetImapSettingsPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			CertificateHelper.UpdateConfigDomain(dataTable, store, "Imap");
		}

		public static void GetOutlookAnyWherePostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow row = dataTable.Rows[0];
			ADRpcHttpVirtualDirectory adrpcHttpVirtualDirectory = (ADRpcHttpVirtualDirectory)store.GetDataObject("RpcHttpVirtualDirectory");
			if (adrpcHttpVirtualDirectory != null && adrpcHttpVirtualDirectory.ExternalHostname != null)
			{
				CertificateHelper.GetOneItem(row, "OutlookDomain", adrpcHttpVirtualDirectory.ExternalHostname.HostnameString);
			}
		}

		public static void GetAcceptDomainPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				SmtpDomainWithSubdomains smtpDomainWithSubdomains = (SmtpDomainWithSubdomains)dataRow["AcceptDomainName"];
				if (smtpDomainWithSubdomains != null)
				{
					dataRow["AcceptedName"] = smtpDomainWithSubdomains.Domain;
				}
			}
		}

		public static void OnPreNewSelfSignedCertificate(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			dataRow["PlainPassword"] = Guid.NewGuid().ToString().ConvertToSecureString();
		}

		public static void OnPostExportSelfSignedCertificate(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			object dataObject = store.GetDataObject("BinaryFileDataObject");
			if (dataObject != null && dataObject is IEnumerable)
			{
				foreach (object obj in ((IEnumerable)dataObject))
				{
					BinaryFileDataObject binaryFileDataObject = (BinaryFileDataObject)obj;
					if (binaryFileDataObject != null)
					{
						dataRow["FileData"] = binaryFileDataObject.FileData;
						break;
					}
				}
			}
		}

		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				dataRow["ExDTNotAfter"] = ((ExDateTime)((DateTime)dataRow["NotAfter"])).ToShortDateString();
				CertificateHelper.TranslateStatusIntoShortDescription(dataRow);
			}
		}

		public static void GetSDOPostAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			DataRow dataRow = table.Rows[0];
			if (!DBNull.Value.Equals(dataRow["IsSelfSigned"]))
			{
				dataRow["CertType"] = (((bool)dataRow["IsSelfSigned"]) ? Strings.SelfSignedCertificate.ToString() : Strings.CASignedCertificate.ToString());
			}
			if (!DBNull.Value.Equals(dataRow["NotAfter"]))
			{
				dataRow["ExDTNotAfter"] = ((ExDateTime)((DateTime)dataRow["NotAfter"])).ToShortDateString();
			}
			CertificateHelper.TranslateStatusIntoShortDescription(dataRow);
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			DataRow dataRow = table.Rows[0];
			dataRow["ExDTNotAfter"] = ((ExDateTime)((DateTime)dataRow["NotAfter"])).ToShortDateString();
			ExchangeCertificate exchangeCertificate = (ExchangeCertificate)store.GetDataObject("ExchangeCertificate");
			if (exchangeCertificate != null)
			{
				dataRow["SubjectName"] = exchangeCertificate.SubjectName.Name;
			}
		}

		private static void TranslateStatusIntoShortDescription(DataRow row)
		{
			CertificateStatus key = (CertificateStatus)row["Status"];
			row["DisplayStatus"] = CertificateHelper.CertStatusToShortDescDict[key].ToString();
		}

		private static readonly Dictionary<CertificateStatus, LocalizedString> CertStatusToShortDescDict = new Dictionary<CertificateStatus, LocalizedString>
		{
			{
				CertificateStatus.DateInvalid,
				Strings.DateinvalidStatus
			},
			{
				CertificateStatus.Invalid,
				Strings.InvalidStatus
			},
			{
				CertificateStatus.PendingRequest,
				Strings.PendingRequestStatus
			},
			{
				CertificateStatus.RevocationCheckFailure,
				Strings.RevocationCheckFailureStatus
			},
			{
				CertificateStatus.Revoked,
				Strings.RevokedStatus
			},
			{
				CertificateStatus.Unknown,
				Strings.UnknownStatus
			},
			{
				CertificateStatus.Untrusted,
				Strings.UntrustedStatus
			},
			{
				CertificateStatus.Valid,
				Strings.ValidStatus
			}
		};
	}
}
