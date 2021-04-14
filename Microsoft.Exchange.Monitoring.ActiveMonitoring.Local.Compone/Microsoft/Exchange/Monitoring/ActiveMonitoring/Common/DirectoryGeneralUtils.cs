using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Management;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Interop.ActiveDS;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class DirectoryGeneralUtils
	{
		public static string InternalPutDCInMM(string targetFqdn, TracingContext traceContext)
		{
			string text = string.Empty;
			string[] array = targetFqdn.Split(new char[]
			{
				'.'
			});
			string text2 = array[0];
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			bool flag2 = false;
			string ridMaster = DirectoryGeneralUtils.GetRidMaster(targetFqdn);
			stringBuilder.Append(string.Format("RidMaster {0}.", ridMaster));
			string transferDC = DirectoryGeneralUtils.GetTransferDC(targetFqdn, traceContext);
			if (DirectoryGeneralUtils.ReachMMLimit(targetFqdn))
			{
				throw new Exception(string.Format("The unprovisioned DCs in current site has reached MM limit of 60%, so we could not put this DC {0} into MM", targetFqdn));
			}
			List<DirectoryGeneralUtils.FSMORoles> fsmoRolesViaRidMaster = DirectoryGeneralUtils.GetFsmoRolesViaRidMaster(targetFqdn, traceContext);
			if (fsmoRolesViaRidMaster != null && fsmoRolesViaRidMaster.Count > 0)
			{
				flag = true;
				string serverName = string.Empty;
				StringBuilder stringBuilder2 = new StringBuilder();
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Transferring the FSMO roles {0} on DC {1} to target DC {2}.", stringBuilder2, targetFqdn, transferDC), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 117);
				foreach (DirectoryGeneralUtils.FSMORoles fsmoroles in fsmoRolesViaRidMaster)
				{
					stringBuilder2.Append(fsmoroles.ToString());
					stringBuilder2.Append(",");
				}
				stringBuilder.Append(string.Format("Transferring the FSMO roles {0} on DC {1} to target DC {2}.", stringBuilder2, targetFqdn, transferDC));
				try
				{
					DirectoryGeneralUtils.TransferFsmoRole(fsmoRolesViaRidMaster, transferDC, traceContext);
					array = transferDC.Split(new char[]
					{
						'.'
					});
					serverName = array[0];
					List<DirectoryGeneralUtils.FSMORoles> fsmoRolesViaDC = DirectoryGeneralUtils.GetFsmoRolesViaDC(serverName, transferDC, traceContext);
					stringBuilder2.Clear();
					foreach (DirectoryGeneralUtils.FSMORoles fsmoroles2 in fsmoRolesViaRidMaster)
					{
						if (!fsmoRolesViaDC.Contains(fsmoroles2))
						{
							throw new Exception(string.Format("The FSMO role trnasfer from {0} to {1} failed. Oncall please investigate and seize FSMO roles.", targetFqdn, transferDC));
						}
						flag2 = true;
						stringBuilder2.Append(fsmoroles2.ToString());
						stringBuilder2.Append(",");
					}
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Following roles have been transfered {0}.", stringBuilder2.ToString()), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 159);
					stringBuilder.Append(string.Format("Transfered roles {0}.", stringBuilder2.ToString()));
					goto IL_294;
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Received exception when Transferring/Setting FSMO roles {0}.", ex.ToString()), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 167);
					stringBuilder.Append("FSMO role transfering encountered exception. Exception: " + ex.ToString());
					goto IL_294;
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("TargetDC {0} does not have any FSMO roles.", targetFqdn), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 177);
			stringBuilder.Append(string.Format("  Target DC {0} does not have any FSMO roles.", targetFqdn));
			IL_294:
			if (flag && !flag2)
			{
				stringBuilder.Append("Transfer not successful, skipping putting the DC in MM");
			}
			else if (DirectoryGeneralUtils.CheckIfDCProvisioned(targetFqdn))
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, "Clearing the DNSEntry", null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 194);
				try
				{
					DirectoryGeneralUtils.RemoveAndClearDNSEntry(targetFqdn, traceContext);
				}
				catch (Exception ex2)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Received exception when clearing the DNSEntry {0}.", ex2.ToString()), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 204);
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Marking {0} as into MM", targetFqdn), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 210);
				DirectoryGeneralUtils.MarkDCAsMM(targetFqdn);
				text = string.Format("  Finished marking {0} into MM and clear DNSEntry.", targetFqdn);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, text, null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 218);
				stringBuilder.Append(text);
				if (!DirectoryGeneralUtils.CheckIfDCProvisioned(targetFqdn))
				{
					text = string.Format("  Successfully verified that {0} is in MM", targetFqdn);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, text, null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 229);
					stringBuilder.Append(text);
				}
				else
				{
					text = string.Format("  Could not put DC {0} in MM.", targetFqdn);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, text, null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 239);
					stringBuilder.Append(text);
				}
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("{0} is not provisioned, no need to put it into MM.", targetFqdn), null, "InternalPutDCInMM", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 248);
				stringBuilder.Append(string.Format("{0} is not provisioned, no need to put it into MM.", targetFqdn));
			}
			return stringBuilder.ToString();
		}

		internal static bool TransferFSMORoleFromDC(string targetFqdn, TracingContext traceContext)
		{
			bool result = true;
			List<DirectoryGeneralUtils.FSMORoles> fsmoRolesViaRidMaster = DirectoryGeneralUtils.GetFsmoRolesViaRidMaster(targetFqdn, traceContext);
			if (fsmoRolesViaRidMaster != null && fsmoRolesViaRidMaster.Count > 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (DirectoryGeneralUtils.FSMORoles fsmoroles in fsmoRolesViaRidMaster)
				{
					stringBuilder.Append(fsmoroles.ToString());
					stringBuilder.Append(",");
				}
				try
				{
					string transferDC = DirectoryGeneralUtils.GetTransferDC(targetFqdn, traceContext);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Transferring the FSMO roles {0} on DC {1} to target DC {2}.", stringBuilder, targetFqdn, transferDC), null, "TransferFSMORoleFromDC", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 288);
					DirectoryGeneralUtils.TransferFsmoRole(fsmoRolesViaRidMaster, transferDC, traceContext);
					List<DirectoryGeneralUtils.FSMORoles> fsmoRolesViaDC = DirectoryGeneralUtils.GetFsmoRolesViaDC(transferDC, transferDC, traceContext);
					stringBuilder.Clear();
					foreach (DirectoryGeneralUtils.FSMORoles item in fsmoRolesViaRidMaster)
					{
						if (!fsmoRolesViaDC.Contains(item))
						{
							WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("The FSMO role transfer from {0} to {1} failed. Oncall please investigate and seize FSMO roles.", targetFqdn, transferDC), null, "TransferFSMORoleFromDC", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 304);
							result = false;
							break;
						}
					}
					return result;
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Received exception when Transferring/Setting FSMO roles {0}.", ex.ToString()), null, "TransferFSMORoleFromDC", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 316);
					return false;
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("TargetDC {0} does not have any FSMO roles.", targetFqdn), null, "TransferFSMORoleFromDC", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 326);
			return result;
		}

		private static string GetDomainControllerSite(DirectoryEntry dcEntry)
		{
			string text = string.Empty;
			if (dcEntry != null && dcEntry.Properties.Contains("serverReferenceBL") && dcEntry.Properties["serverReferenceBL"].Value != null)
			{
				string text2 = dcEntry.Properties["serverReferenceBL"].Value.ToString();
				if (text2 != null)
				{
					string[] array = Regex.Split(text2, "CN=");
					if (array.Length > 3)
					{
						text = array[3];
						text = text.Substring(0, text.Length - 1);
					}
				}
			}
			return text;
		}

		private static string GetRidMaster(string targetFQDN)
		{
			string ridMaster = string.Empty;
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				ridMaster = DirectoryGeneralUtils.GetRidMasterCore(targetFQDN);
			});
			return ridMaster;
		}

		private static string GetRidMasterCore(string targetFQDN)
		{
			string domainFQDNFromSvr = DirectoryGeneralUtils.GetDomainFQDNFromSvr(targetFQDN);
			string result = string.Empty;
			string ntdsSetting = string.Empty;
			using (DirectoryEntry directoryEntry = new DirectoryEntry("LDAP://" + domainFQDNFromSvr))
			{
				using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://CN=RID Manager$,CN=System," + directoryEntry.Properties["distinguishedName"].Value.ToString()))
				{
					ntdsSetting = directoryEntry2.Properties["FsmoRoleOwner"].Value.ToString();
				}
			}
			string serverCNFromNDTSDN = DirectoryGeneralUtils.GetServerCNFromNDTSDN(ntdsSetting);
			if (!string.IsNullOrEmpty(serverCNFromNDTSDN))
			{
				result = serverCNFromNDTSDN + "." + domainFQDNFromSvr;
			}
			return result;
		}

		private static string GetServerCNFromNDTSDN(string ntdsSetting)
		{
			string text = string.Empty;
			string[] separator = new string[]
			{
				"CN=",
				"DC="
			};
			if (!string.IsNullOrEmpty(ntdsSetting))
			{
				string[] array = ntdsSetting.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				text = array[1];
				text = text.Substring(0, text.Length - 1);
			}
			return text;
		}

		public static string GetDefaultNC(string dcName)
		{
			string defaultNC = string.Empty;
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				defaultNC = DirectoryGeneralUtils.GetDefaultNCCore(dcName);
			});
			return defaultNC;
		}

		private static string GetDefaultNCCore(string dcName)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(dcName))
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", dcName)))
				{
					if (directoryEntry != null && directoryEntry.Properties != null)
					{
						return directoryEntry.Properties["defaultNamingContext"].Value.ToString();
					}
					throw new Exception(string.Format("Check NTDS or DC Health on {0} as its not responding to ldap connections.", dcName));
				}
			}
			using (DirectoryEntry directoryEntry2 = new DirectoryEntry())
			{
				result = directoryEntry2.Properties["distinguishedName"].Value.ToString();
			}
			return result;
		}

		public static string GetConfigNC(string dcName)
		{
			string configNC = string.Empty;
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				configNC = DirectoryGeneralUtils.GetConfigNCCore(dcName);
			});
			return configNC;
		}

		private static string GetConfigNCCore(string dcName)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(dcName))
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", dcName)))
				{
					if (directoryEntry != null && directoryEntry.Properties != null)
					{
						return directoryEntry.Properties["configurationNamingContext"].Value.ToString();
					}
					throw new Exception(string.Format("Check NTDS or DC Health on {0} as its not responding to ldap connections.", dcName));
				}
			}
			using (DirectoryEntry directoryEntry2 = new DirectoryEntry())
			{
				string arg = directoryEntry2.Properties["distinguishedName"].Value.ToString();
				result = string.Format("CN=Configuration,{0}", arg);
			}
			return result;
		}

		private static string GetSchemaNC(string dcName)
		{
			string schemaNC = string.Empty;
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				schemaNC = DirectoryGeneralUtils.GetSchemaNCCore(dcName);
			});
			return schemaNC;
		}

		private static string GetSchemaNCCore(string dcName)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(dcName))
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", dcName)))
				{
					if (directoryEntry != null && directoryEntry.Properties != null)
					{
						return directoryEntry.Properties["schemaNamingContext"].Value.ToString();
					}
					throw new Exception(string.Format("Check NTDS or DC Health on {0} as its not responding to ldap connections.", dcName));
				}
			}
			using (DirectoryEntry directoryEntry2 = new DirectoryEntry())
			{
				string arg = directoryEntry2.Properties["distinguishedName"].Value.ToString();
				result = string.Format("CN=Schema,CN=Configuration,{0}", arg);
			}
			return result;
		}

		private static string GetDCOUEntryLdapPath(string dcName)
		{
			return string.Format("LDAP://{0}/OU=Domain Controllers,{1}", dcName, DirectoryGeneralUtils.GetDefaultNC(dcName));
		}

		private static string GetDCInOUEntryLdapPath(string directoryServer, bool connectRidMaster)
		{
			string text = string.Empty;
			string arg = string.Empty;
			if (connectRidMaster)
			{
				text = DirectoryGeneralUtils.GetRidMaster(directoryServer);
			}
			else
			{
				text = directoryServer;
			}
			string defaultNC = DirectoryGeneralUtils.GetDefaultNC(text);
			if (directoryServer.Contains("."))
			{
				string[] array = directoryServer.Split(new char[]
				{
					'.'
				});
				arg = array[0];
			}
			else
			{
				arg = directoryServer;
			}
			return string.Format("LDAP://{0}/CN={1},OU=Domain Controllers,{2}", text, arg, defaultNC);
		}

		public static string GetLocalFQDN()
		{
			string localFqdn = string.Empty;
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				localFqdn = DirectoryGeneralUtils.GetLocalFQDNCore();
			});
			return localFqdn;
		}

		private static string GetLocalFQDNCore()
		{
			string hostName = Dns.GetHostName();
			IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
			if (hostEntry != null && !string.IsNullOrEmpty(hostEntry.HostName))
			{
				return hostEntry.HostName;
			}
			throw new Exception("Failed to get local FQDN from Host Entry");
		}

		private static string GetDomainFQDNFromSvr(string svrFQDN)
		{
			if (svrFQDN.Contains("."))
			{
				return svrFQDN.Substring(svrFQDN.IndexOf('.') + 1, svrFQDN.Length - svrFQDN.IndexOf('.') - 1);
			}
			throw new Exception(string.Format("Failed to get domain FQDN from given server FQDN {0}. Please make sure given server FQDN in corret format e.g. exch-3456.extest.microsoft.com", svrFQDN));
		}

		public static string GetLocalDomain()
		{
			string localFQDN = DirectoryGeneralUtils.GetLocalFQDN();
			return DirectoryGeneralUtils.GetDomainFQDNFromSvr(localFQDN);
		}

		public static void InvokeCommand(string fileToInvoke, string invokeArguments)
		{
			using (Process process = Process.Start(new ProcessStartInfo(fileToInvoke, invokeArguments)
			{
				CreateNoWindow = true
			}))
			{
				process.WaitForExit();
			}
		}

		public static void StartService(string serviceName)
		{
			DirectoryGeneralUtils.StartStopService(serviceName, DirectoryGeneralUtils.GetLocalFQDN(), true, false);
		}

		public static void StartService(string serviceName, string serverName)
		{
			DirectoryGeneralUtils.StartStopService(serviceName, serverName, true, false);
		}

		public static void StopService(string serviceName)
		{
			DirectoryGeneralUtils.StartStopService(serviceName, DirectoryGeneralUtils.GetLocalFQDN(), false, true);
		}

		public static void StopService(string serviceName, string serverName)
		{
			DirectoryGeneralUtils.StartStopService(serviceName, serverName, false, true);
		}

		public static void RestartService(string serviceName)
		{
			DirectoryGeneralUtils.StartStopService(serviceName, DirectoryGeneralUtils.GetLocalFQDN(), true, true);
		}

		public static void RestartService(string serviceName, string serverName)
		{
			DirectoryGeneralUtils.StartStopService(serviceName, serverName, true, true);
		}

		public static void StartStopService(string serviceName, string serverName, bool start, bool stop)
		{
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				DirectoryGeneralUtils.StartStopServiceCore(serviceName, serverName, start, stop);
			});
		}

		private static void StartStopServiceCore(string serviceName, string serverName, bool start, bool stop)
		{
			using (ServiceController serviceController = new ServiceController(serviceName, serverName))
			{
				if (serviceController == null)
				{
					throw new Exception(string.Format("Serivce  {0} not found on server {1}", serviceName, serverName));
				}
				if (stop && serviceController.Status != ServiceControllerStatus.Stopped)
				{
					serviceController.Stop();
					serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
				}
				if (start && serviceController.Status != ServiceControllerStatus.Running)
				{
					serviceController.Start();
					serviceController.WaitForStatus(ServiceControllerStatus.Running);
				}
			}
		}

		private static List<string> GetNonArunaSites(string targetFQDN)
		{
			List<string> nonArunaSites = new List<string>();
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				nonArunaSites = DirectoryGeneralUtils.GetNonArunaSitesCore(targetFQDN);
			});
			return nonArunaSites;
		}

		private static List<string> GetNonArunaSitesCore(string targetFQDN)
		{
			List<string> list = new List<string>();
			string domainFQDNFromSvr = DirectoryGeneralUtils.GetDomainFQDNFromSvr(targetFQDN);
			string arg = string.Empty;
			DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain, domainFQDNFromSvr);
			DomainController domainController = DomainController.FindOne(context);
			using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", domainController.Name)))
			{
				if (directoryEntry == null || directoryEntry.Properties == null)
				{
					throw new Exception(string.Format("Check NTDS or DC Health on {0} as its not responding to ldap connections.", domainController.Name));
				}
				arg = directoryEntry.Properties["configurationNamingContext"].Value.ToString();
			}
			string path = string.Format("LDAP://{0}/cn=Sites,{1}", domainController.Name, arg);
			using (DirectoryEntry directoryEntry2 = new DirectoryEntry(path))
			{
				using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry2))
				{
					directorySearcher.Filter = "(& (objectclass=site)(msExchTransportSiteFlags:1.2.840.113556.1.4.804:=4))";
					directorySearcher.SearchScope = SearchScope.Subtree;
					directorySearcher.PropertiesToLoad.Add("name");
					SearchResultCollection searchResultCollection = directorySearcher.FindAll();
					if (searchResultCollection == null || searchResultCollection.Count == 0)
					{
						throw new Exception("Can not find Non-Aruna Sites");
					}
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						list.Add(searchResult.Properties["name"][0].ToString());
					}
				}
			}
			return list;
		}

		private static bool CheckDCHealthy(string dcFQDN)
		{
			string a = string.Empty;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}/RootDSE", dcFQDN)))
				{
					if (directoryEntry != null && directoryEntry.Properties != null)
					{
						a = directoryEntry.Properties["isSynchronized"].Value.ToString();
					}
					else
					{
						a = "FALSE";
					}
				}
			}
			catch (Exception)
			{
				a = "FALSE";
			}
			return string.Equals(a, "TRUE", StringComparison.InvariantCultureIgnoreCase);
		}

		private static List<string> GetProvisionedNonArunaDCs(string targetFQDN)
		{
			List<string> provisionedDCs = new List<string>();
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				provisionedDCs = DirectoryGeneralUtils.GetProvisionedNonArunaDCsCore(targetFQDN);
			});
			return provisionedDCs;
		}

		private static List<string> GetProvisionedNonArunaDCsCore(string targetFQDN)
		{
			List<string> list = new List<string>();
			string domainFQDNFromSvr = DirectoryGeneralUtils.GetDomainFQDNFromSvr(targetFQDN);
			string text = string.Empty;
			string text2 = string.Empty;
			DirectoryContext context = new DirectoryContext(DirectoryContextType.Domain, domainFQDNFromSvr);
			DomainController domainController = DomainController.FindOne(context);
			List<string> nonArunaSites = DirectoryGeneralUtils.GetNonArunaSites(targetFQDN);
			string dcouentryLdapPath = DirectoryGeneralUtils.GetDCOUEntryLdapPath(domainController.Name);
			using (DirectoryEntry directoryEntry = new DirectoryEntry(dcouentryLdapPath))
			{
				using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
				{
					directorySearcher.Filter = "(& (objectclass=computer)(msExchProvisioningFlags=0))";
					directorySearcher.SearchScope = SearchScope.Subtree;
					directorySearcher.PropertiesToLoad.Add("dnsHostName");
					directorySearcher.PropertiesToLoad.Add("serverReferenceBL");
					SearchResultCollection searchResultCollection = directorySearcher.FindAll();
					if (searchResultCollection == null || searchResultCollection.Count == 0)
					{
						throw new Exception("Can not find provisioned DCs in local forest");
					}
					foreach (object obj in searchResultCollection)
					{
						SearchResult searchResult = (SearchResult)obj;
						text = searchResult.Properties["serverReferenceBL"][0].ToString();
						if (text != null)
						{
							string[] array = Regex.Split(text, "CN=");
							if (array.Length > 3)
							{
								text2 = array[3];
								text2 = text2.Substring(0, text2.Length - 1);
							}
						}
						if (nonArunaSites.Contains(text2))
						{
							list.Add(searchResult.Properties["dnsHostName"][0].ToString());
						}
					}
				}
			}
			return list;
		}

		private static string GetNtdsSettingDN(string targetDC)
		{
			string arg = string.Empty;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(DirectoryGeneralUtils.GetDCInOUEntryLdapPath(targetDC, false)))
			{
				arg = directoryEntry.Properties["serverReferenceBL"].Value.ToString();
			}
			return string.Format("CN=NTDS Settings,{0}", arg);
		}

		public static bool CheckIfDCProvisionedLocal(string dcName)
		{
			return DirectoryGeneralUtils.CheckIfDCProvisioned(dcName, false);
		}

		public static bool CheckIfDCProvisioned(string dcName)
		{
			return DirectoryGeneralUtils.CheckIfDCProvisioned(dcName, true);
		}

		public static bool CheckIfDCProvisioned(string dcName, bool useRidMaster)
		{
			int num = 0;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(DirectoryGeneralUtils.GetDCInOUEntryLdapPath(dcName, useRidMaster)))
			{
				if (directoryEntry.Properties.Contains("msExchProvisioningFlags"))
				{
					num = (int)directoryEntry.Properties["msExchProvisioningFlags"].Value;
				}
			}
			return num == 0;
		}

		public static void MarkDCAsMM(string dcName)
		{
			DirectoryGeneralUtils.MarkDCWithStatusInAD(dcName, DirectoryGeneralUtils.DCProvisioningStatus.MaintenanceMode);
		}

		public static void MarkDCAsProvisioned(string dcName)
		{
			DirectoryGeneralUtils.MarkDCWithStatusInAD(dcName, DirectoryGeneralUtils.DCProvisioningStatus.Provisioned);
		}

		public static void MarkDCAsDemote(string dcName)
		{
			DirectoryGeneralUtils.MarkDCWithStatusInAD(dcName, DirectoryGeneralUtils.DCProvisioningStatus.Demote);
		}

		private static void MarkDCWithStatusInAD(string dcName, DirectoryGeneralUtils.DCProvisioningStatus provisioningStatus)
		{
			string ridMaster = DirectoryGeneralUtils.GetRidMaster(dcName);
			string defaultNC = DirectoryGeneralUtils.GetDefaultNC(ridMaster);
			string arg = string.Empty;
			int num = 0;
			switch (provisioningStatus)
			{
			case DirectoryGeneralUtils.DCProvisioningStatus.Provisioned:
				num = 0;
				break;
			case DirectoryGeneralUtils.DCProvisioningStatus.MaintenanceMode:
				num = 5;
				break;
			case DirectoryGeneralUtils.DCProvisioningStatus.Demote:
				num = 25000;
				break;
			}
			if (dcName.Contains("."))
			{
				string[] array = dcName.Split(new char[]
				{
					'.'
				});
				arg = array[0];
			}
			else
			{
				arg = dcName;
			}
			string path = string.Format("LDAP://{0}/CN={1},OU=Domain Controllers,{2}", ridMaster, arg, defaultNC);
			using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
			{
				if (directoryEntry.Properties.Contains("msExchProvisioningFlags"))
				{
					directoryEntry.Properties["msExchProvisioningFlags"][0] = num;
				}
				else
				{
					directoryEntry.Properties["msExchProvisioningFlags"].Add(num);
				}
				directoryEntry.CommitChanges();
			}
		}

		private static bool ReachMMLimit(string dcName)
		{
			string strB = string.Empty;
			string strA = string.Empty;
			string empty = string.Empty;
			int num = 0;
			int num2 = 0;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(DirectoryGeneralUtils.GetDCInOUEntryLdapPath(dcName, true)))
			{
				strB = DirectoryGeneralUtils.GetDomainControllerSite(directoryEntry);
			}
			string dcouentryLdapPath = DirectoryGeneralUtils.GetDCOUEntryLdapPath(DirectoryGeneralUtils.GetRidMaster(dcName));
			using (DirectoryEntry directoryEntry2 = new DirectoryEntry(dcouentryLdapPath))
			{
				foreach (object obj in directoryEntry2.Children)
				{
					DirectoryEntry directoryEntry3 = (DirectoryEntry)obj;
					strA = DirectoryGeneralUtils.GetDomainControllerSite(directoryEntry3);
					if (string.Compare(strA, strB, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						num++;
						if (directoryEntry3.Properties.Contains("msExchProvisioningFlags") && (int)directoryEntry3.Properties["msExchProvisioningFlags"].Value == 0)
						{
							num2++;
						}
					}
				}
			}
			double num3 = (double)(num2 - 1) / (double)num;
			return num3 < 0.6;
		}

		private static void RemoveAndClearDNSEntry(string targetFQDN, TracingContext traceContext)
		{
			string domainFQDNFromSvr = DirectoryGeneralUtils.GetDomainFQDNFromSvr(targetFQDN);
			string[] value = new string[]
			{
				"LdapIpAddress",
				"Ldap",
				"LdapAtSite",
				"Pdc",
				"Gc",
				"GcAtSite",
				"DcByGuid",
				"GcIpAddress",
				"DsaCname",
				"Kdc",
				"KdcAtSite",
				"Dc",
				"DcAtSite",
				"Rfc1510Kdc",
				"Rfc1510KdcAtSite",
				"GenericGc",
				"GenericGcAtSite",
				"Rfc1510UdpKdc",
				"Rfc1510Kpwd",
				"Rfc1510UdpKpwd"
			};
			string invokeArguments = string.Format("/dsderegdns:{0} /dom:{1}", targetFQDN, domainFQDNFromSvr);
			DirectoryGeneralUtils.InvokeCommand("nltest.exe", invokeArguments);
			DirectoryGeneralUtils.ClearFromEntryFromCurrentDNSDCs(targetFQDN, traceContext);
			try
			{
				using (RegistryKey registryKey = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, targetFQDN))
				{
					if (registryKey != null)
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey("System\\CurrentControlSet\\Services\\Netlogon\\Parameters"))
						{
							if (registryKey2 != null)
							{
								try
								{
									registryKey2.SetValue("DnsAvoidRegisterRecords", value, RegistryValueKind.MultiString);
								}
								catch (Exception ex)
								{
									WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Got exception while setting regkey {0}", ex.Message), null, "RemoveAndClearDNSEntry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1161);
								}
							}
						}
					}
				}
			}
			catch
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, "Error when trying to create regkey", null, "RemoveAndClearDNSEntry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1173);
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, "Restarting NetLogon service and Stop KDC service", null, "RemoveAndClearDNSEntry", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1179);
			DirectoryGeneralUtils.RestartService("NetLogon", targetFQDN);
			DirectoryGeneralUtils.StopService("kdc", targetFQDN);
		}

		private static void ClearFromEntryFromCurrentDNSDCs(string targetFQDN, TracingContext traceContext)
		{
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				DirectoryGeneralUtils.ClearFromEntryFromCurrentDNSDCsCore(targetFQDN, traceContext);
			});
		}

		private static void ClearFromEntryFromCurrentDNSDCsCore(string targetFQDN, TracingContext traceContext)
		{
			string text = string.Empty;
			string invokeArguments = string.Empty;
			string domainFQDNFromSvr = DirectoryGeneralUtils.GetDomainFQDNFromSvr(targetFQDN);
			string value = Dns.GetHostEntry(targetFQDN).AddressList[0].ToString();
			bool flag = false;
			WqlObjectQuery query = new WqlObjectQuery("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = TRUE");
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(query))
			{
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						using (managementObject)
						{
							try
							{
								string[] array = (string[])managementObject["DnsServerSearchOrder"];
								string[] array2 = (string[])managementObject["IPAddress"];
								foreach (string text2 in array2)
								{
									if (text2.Equals(value))
									{
										flag = true;
									}
								}
								if (flag)
								{
									foreach (string hostNameOrAddress in array)
									{
										IPHostEntry hostEntry = Dns.GetHostEntry(hostNameOrAddress);
										text = hostEntry.HostName;
										if (!text.ToLower().Contains("mgt"))
										{
											invokeArguments = string.Format("/dsderegdns:{0} /dom:{1} /server:{2}", targetFQDN, domainFQDNFromSvr, text);
											DirectoryGeneralUtils.InvokeCommand("nltest.exe", invokeArguments);
										}
									}
								}
							}
							catch (Exception ex)
							{
								WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Error trying to unregister server {0}", ex.ToString()), null, "ClearFromEntryFromCurrentDNSDCsCore", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1258);
							}
						}
					}
				}
			}
		}

		internal static List<DirectoryGeneralUtils.FSMORoles> GetFsmoRolesViaRidMaster(string targetFQDN, TracingContext traceContext)
		{
			string[] array = targetFQDN.Split(new char[]
			{
				'.'
			});
			string serverName = array[0];
			string ridMaster = DirectoryGeneralUtils.GetRidMaster(targetFQDN);
			return DirectoryGeneralUtils.GetFsmoRolesViaDC(serverName, ridMaster, traceContext);
		}

		internal static List<DirectoryGeneralUtils.FSMORoles> GetFsmoRolesViaDC(string serverName, string connectDC, TracingContext traceContext)
		{
			List<DirectoryGeneralUtils.FSMORoles> list = new List<DirectoryGeneralUtils.FSMORoles>();
			string arg = string.Empty;
			string arg2 = string.Empty;
			string arg3 = string.Empty;
			string ridMaster = DirectoryGeneralUtils.GetRidMaster(connectDC);
			string text = string.Empty;
			string text2 = string.Empty;
			if (string.IsNullOrEmpty(ridMaster))
			{
				throw new Exception(string.Format("Can not find Rid Master for connect DC {0} site", connectDC));
			}
			if (ridMaster.Contains("."))
			{
				string[] array = ridMaster.Split(new char[]
				{
					'.'
				});
				text = array[0];
			}
			else
			{
				text = ridMaster;
			}
			if (serverName.Contains("."))
			{
				string[] array2 = serverName.Split(new char[]
				{
					'.'
				});
				text2 = array2[0];
			}
			else
			{
				text2 = serverName;
			}
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Rid Master {0}", ridMaster), serverName, null, "GetFsmoRolesViaDC", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1325);
			if (text.ToLower().Contains(text2.ToLower()) || text2.ToLower().Contains(text.ToLower()))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.RidRole);
			}
			arg = DirectoryGeneralUtils.GetDefaultNC(connectDC);
			arg2 = DirectoryGeneralUtils.GetSchemaNC(connectDC);
			arg3 = DirectoryGeneralUtils.GetConfigNC(connectDC);
			if (DirectoryGeneralUtils.CheckIfFsmoOwner(string.Format("LDAP://{0}/{1}", connectDC, arg), text2, DirectoryGeneralUtils.FSMORoles.PDCRole, traceContext))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.PDCRole);
			}
			if (DirectoryGeneralUtils.CheckIfFsmoOwner(string.Format("LDAP://{0}/{1}", connectDC, arg2), text2, DirectoryGeneralUtils.FSMORoles.SchemaRole, traceContext))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.SchemaRole);
			}
			if (DirectoryGeneralUtils.CheckIfFsmoOwner(string.Format("LDAP://{0}/cn=Infrastructure,{1}", connectDC, arg), text2, DirectoryGeneralUtils.FSMORoles.InfrastructureRole, traceContext))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.InfrastructureRole);
			}
			if (DirectoryGeneralUtils.CheckIfFsmoOwner(string.Format("LDAP://{0}/cn=Partitions,{1}", connectDC, arg3), text2, DirectoryGeneralUtils.FSMORoles.NamingRole, traceContext))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.NamingRole);
			}
			if (DirectoryGeneralUtils.CheckIfFsmoOwner(string.Format("LDAP://{0}/cn=Infrastructure,DC=DomainDnsZones,{1}", connectDC, arg), text2, DirectoryGeneralUtils.FSMORoles.DomainDnsZones, traceContext))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.DomainDnsZones);
			}
			if (DirectoryGeneralUtils.CheckIfFsmoOwner(string.Format("LDAP://{0}/cn=Infrastructure,DC=ForestDnsZones,{1}", connectDC, arg), text2, DirectoryGeneralUtils.FSMORoles.ForestDnsZones, traceContext))
			{
				list.Add(DirectoryGeneralUtils.FSMORoles.ForestDnsZones);
			}
			return list;
		}

		private static bool CheckIfFsmoOwner(string ldapPath, string serverName, DirectoryGeneralUtils.FSMORoles fsmoRole, TracingContext traceContext)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			if (serverName.Contains("."))
			{
				string[] array = serverName.Split(new char[]
				{
					'.'
				});
				text = array[0];
			}
			else
			{
				text = serverName;
			}
			bool result;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(ldapPath))
				{
					if (directoryEntry != null && directoryEntry.Properties.Contains("FsmoRoleOwner") && directoryEntry.Properties["FsmoRoleOwner"].Value != null)
					{
						text3 = directoryEntry.Properties["FsmoRoleOwner"].Value.ToString();
						WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Current setting: {0}: {1}", fsmoRole, text3), null, "CheckIfFsmoOwner", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1403);
						text2 = DirectoryGeneralUtils.GetServerCNFromNDTSDN(text3);
						if (text2.ToLower().Contains(text.ToLower()) || text.ToLower().Contains(text2.ToLower()))
						{
							WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Found role {0} for server {1}", fsmoRole, text), null, "CheckIfFsmoOwner", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1411);
							result = true;
						}
						else
						{
							result = false;
						}
					}
					else
					{
						result = false;
					}
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private static string GetTransferDC(string targetFQDN, TracingContext traceContext)
		{
			List<string> provisionedNonArunaDCs = DirectoryGeneralUtils.GetProvisionedNonArunaDCs(targetFQDN);
			if (provisionedNonArunaDCs.Count < 1)
			{
				throw new Exception("Could not find a DC to transfer FSMO role to");
			}
			bool flag = false;
			Random random = new Random();
			while (!flag)
			{
				string text = provisionedNonArunaDCs[random.Next(provisionedNonArunaDCs.Count)];
				if (DirectoryGeneralUtils.CheckDCHealthy(text) && !string.Equals(text, targetFQDN, StringComparison.InvariantCultureIgnoreCase))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Transfer to DC {0}", text), null, "GetTransferDC", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1458);
					return text;
				}
			}
			throw new Exception("Could not find a DC to transfer FSMO role to");
		}

		private static void TransferFsmoRole(List<DirectoryGeneralUtils.FSMORoles> fsmoRoles, string transferDC, TracingContext traceContext)
		{
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				DirectoryGeneralUtils.TransferFsmoRoleCore(fsmoRoles, transferDC, traceContext);
			});
		}

		private static void TransferFsmoRoleCore(List<DirectoryGeneralUtils.FSMORoles> fsmoRoles, string transferDC, TracingContext traceContext)
		{
			string path = string.Format("LDAP://{0}/RootDSE", transferDC);
			string arg = string.Empty;
			string value = string.Empty;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
			{
				arg = directoryEntry.Properties["defaultNamingContext"].Value.ToString();
				using (DirectoryEntry directoryEntry2 = new DirectoryEntry(string.Format("LDAP://{0}/{1}", transferDC, arg)))
				{
					value = directoryEntry2.Properties["objectSID"].Value.ToString();
				}
				foreach (DirectoryGeneralUtils.FSMORoles fsmoroles in fsmoRoles)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Transfering FSMO role {0} to {1}", fsmoroles, transferDC), null, "TransferFsmoRoleCore", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1503);
					try
					{
						switch (fsmoroles)
						{
						case DirectoryGeneralUtils.FSMORoles.RidRole:
							directoryEntry.Properties["becomeRidMaster"].Add(1);
							directoryEntry.CommitChanges();
							break;
						case DirectoryGeneralUtils.FSMORoles.PDCRole:
							directoryEntry.Properties["becomePdc"].Add(value);
							directoryEntry.CommitChanges();
							break;
						case DirectoryGeneralUtils.FSMORoles.SchemaRole:
							directoryEntry.Properties["becomeSchemaMaster"].Add(1);
							directoryEntry.CommitChanges();
							break;
						case DirectoryGeneralUtils.FSMORoles.InfrastructureRole:
							directoryEntry.Properties["becomeInfrastructureMaster"].Add(1);
							directoryEntry.CommitChanges();
							break;
						case DirectoryGeneralUtils.FSMORoles.NamingRole:
							directoryEntry.Properties["becomeDomainMaster"].Add(1);
							directoryEntry.CommitChanges();
							break;
						case DirectoryGeneralUtils.FSMORoles.DomainDnsZones:
						case DirectoryGeneralUtils.FSMORoles.ForestDnsZones:
							DirectoryGeneralUtils.SeizeFsmoRole(fsmoroles, transferDC, traceContext);
							break;
						}
					}
					catch (Exception ex)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Exception caught when setting {0} on DC {1}, exception {2}", fsmoroles, transferDC, ex.ToString()), null, "TransferFsmoRoleCore", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1541);
					}
				}
				Thread.Sleep(TimeSpan.FromSeconds(10.0));
			}
		}

		private static void SeizeFsmoRole(DirectoryGeneralUtils.FSMORoles fsmoRole, string transferDC, TracingContext traceContext)
		{
			string ntdsSettingDN = DirectoryGeneralUtils.GetNtdsSettingDN(transferDC);
			string arg = string.Empty;
			string arg2 = string.Empty;
			string arg3 = string.Empty;
			string ldapPath = string.Empty;
			arg = DirectoryGeneralUtils.GetDefaultNC(transferDC);
			arg2 = DirectoryGeneralUtils.GetSchemaNC(transferDC);
			arg3 = DirectoryGeneralUtils.GetConfigNC(transferDC);
			switch (fsmoRole)
			{
			case DirectoryGeneralUtils.FSMORoles.RidRole:
				DirectoryGeneralUtils.SetRidMasterAvailablePool(transferDC, traceContext);
				ldapPath = string.Format("LDAP://{0}/cn=RID Manager$,cn=System,{1}", Environment.MachineName, arg);
				break;
			case DirectoryGeneralUtils.FSMORoles.PDCRole:
				ldapPath = string.Format("LDAP://{0}/{1}", Environment.MachineName, arg);
				break;
			case DirectoryGeneralUtils.FSMORoles.SchemaRole:
				ldapPath = string.Format("LDAP://{0}/{1}", Environment.MachineName, arg2);
				break;
			case DirectoryGeneralUtils.FSMORoles.InfrastructureRole:
				ldapPath = string.Format("LDAP://{0}/cn=Infrastructure,{1}", Environment.MachineName, arg);
				break;
			case DirectoryGeneralUtils.FSMORoles.NamingRole:
				ldapPath = string.Format("LDAP://{0}/cn=Partitions,{1}", Environment.MachineName, arg3);
				break;
			case DirectoryGeneralUtils.FSMORoles.DomainDnsZones:
				ldapPath = string.Format("LDAP://{0}/cn=Infrastructure,DC=DomainDnsZones,{1}", Environment.MachineName, arg);
				break;
			case DirectoryGeneralUtils.FSMORoles.ForestDnsZones:
				ldapPath = string.Format("LDAP://{0}/cn=Infrastructure,DC=ForestDnsZones,{1}", Environment.MachineName, arg);
				break;
			}
			DirectoryGeneralUtils.SetFsmoOwner(ldapPath, ntdsSettingDN, fsmoRole, traceContext);
		}

		private static void SetFsmoOwner(string ldapPath, string ntdsSettingDN, DirectoryGeneralUtils.FSMORoles fsmoRole, TracingContext traceContext)
		{
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				DirectoryGeneralUtils.SetFsmoOwnerCore(ldapPath, ntdsSettingDN, fsmoRole, traceContext);
			});
		}

		private static void SetFsmoOwnerCore(string ldapPath, string ntdsSettingDN, DirectoryGeneralUtils.FSMORoles fsmoRole, TracingContext traceContext)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Setting FSMO role {0} as {1} on {2}", fsmoRole, ntdsSettingDN, ldapPath), null, "SetFsmoOwnerCore", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1621);
			using (DirectoryEntry directoryEntry = new DirectoryEntry(ldapPath))
			{
				directoryEntry.Properties["FsmoRoleOwner"][0] = ntdsSettingDN;
				directoryEntry.CommitChanges();
			}
		}

		private static void SetRidMasterAvailablePool(string dcName, TracingContext traceContext)
		{
			DirectoryGeneralUtils.RetryWhileException(delegate
			{
				DirectoryGeneralUtils.SetRidMasterAvailablePoolCore(dcName, traceContext);
			});
		}

		private static void SetRidMasterAvailablePoolCore(string dcName, TracingContext traceContext)
		{
			string defaultNC = DirectoryGeneralUtils.GetDefaultNC(dcName);
			string path = string.Format("LDAP://{0}/cn=RID Manager$,cn=System,{1}", Environment.MachineName, defaultNC);
			string arg = string.Empty;
			string arg2 = string.Empty;
			string path2 = string.Empty;
			int num = 0;
			long num2 = 0L;
			try
			{
				using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
				{
					IADsLargeInteger iadsLargeInteger = (IADsLargeInteger)directoryEntry.Properties["rIDAvailablePool"].Value;
					long num3 = (long)iadsLargeInteger.HighPart;
					long num4 = (long)iadsLargeInteger.LowPart;
					using (DirectoryEntry directoryEntry2 = new DirectoryEntry(DirectoryGeneralUtils.GetDCOUEntryLdapPath(dcName)))
					{
						foreach (object obj in directoryEntry2.Children)
						{
							DirectoryEntry directoryEntry3 = (DirectoryEntry)obj;
							arg = directoryEntry3.Properties["distinguishedName"].Value.ToString();
							arg2 = directoryEntry3.Properties["name"].Value.ToString();
							path2 = string.Format("LDAP://{0}/cn=Rid Set,{1}", arg2, arg);
							try
							{
								using (DirectoryEntry directoryEntry4 = new DirectoryEntry(path2))
								{
									IADsLargeInteger iadsLargeInteger2 = (IADsLargeInteger)directoryEntry4.Properties["rIDAvailablePool"].Value;
									num2 = (long)iadsLargeInteger2.LowPart;
								}
								if (num2 > num4)
								{
									num4 = num2;
								}
							}
							catch
							{
								num++;
							}
						}
					}
					if (num > 6)
					{
						num = 6;
					}
					long num5 = num4 + (num3 << 32) + (long)((4 + num) * 5000);
					iadsLargeInteger.HighPart = (int)(num5 >> 32);
					iadsLargeInteger.LowPart = (int)(num5 & (long)((ulong)-1));
					directoryEntry.Properties["rIDAvailablePool"][0] = iadsLargeInteger;
					directoryEntry.CommitChanges();
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, string.Format("Exception caught when setting RidMasterAvailablePool DC {0}, exception {1}", Environment.MachineName, ex.ToString()), null, "SetRidMasterAvailablePoolCore", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Directory\\DirectoryGeneralUtils.cs", 1709);
			}
		}

		public static bool Retry(Func<bool, bool> retryDelegate, int retryTimes, TimeSpan waitBetweenRetries)
		{
			return DirectoryGeneralUtils.RetryWithIncreasingBackoff(retryDelegate, retryTimes, waitBetweenRetries, 1);
		}

		public static bool RetryWithIncreasingBackoff(Func<bool, bool> retryDelegate, int retryTimes, TimeSpan waitBetweenRetries, int multiplier)
		{
			int num = (int)waitBetweenRetries.TotalMilliseconds;
			while (retryTimes-- > 0)
			{
				bool flag = retryTimes == 0;
				if (retryDelegate(flag))
				{
					return true;
				}
				if (!flag)
				{
					Thread.Sleep(num);
				}
				num *= multiplier;
			}
			return false;
		}

		public static void RetryWhileException(Action action)
		{
			DirectoryGeneralUtils.Retry(delegate(bool lastAttempt)
			{
				try
				{
					action();
					return true;
				}
				catch (Exception)
				{
					if (lastAttempt)
					{
						throw;
					}
				}
				return false;
			}, 10, TimeSpan.FromSeconds(1.0));
		}

		public static List<string> GetTrustedDomains()
		{
			List<string> list = new List<string>();
			string machineName = Environment.MachineName;
			string defaultNC = DirectoryGeneralUtils.GetDefaultNC(machineName);
			string path = string.Format("LDAP://{0}/CN=System,{1}", machineName, defaultNC);
			string text = string.Empty;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
			{
				using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry))
				{
					directorySearcher.Filter = "(objectclass=trustedDomain)";
					directorySearcher.SearchScope = SearchScope.Subtree;
					directorySearcher.PropertiesToLoad.Add("distinguishedName");
					SearchResultCollection searchResultCollection = directorySearcher.FindAll();
					if (searchResultCollection != null && searchResultCollection.Count > 0)
					{
						foreach (object obj in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj;
							text = searchResult.Properties["distinguishedName"][0].ToString();
							text = text.Substring(3);
							text = text.Substring(0, text.IndexOf(','));
							text = string.Format("LDAP://DC={0}", text.Replace(".", ",DC="));
							list.Add(text);
						}
					}
				}
			}
			return list;
		}

		public const string LdapHeader = "LDAP://";

		public const string ProvisioningFlagProperty = "msExchProvisioningFlags";

		public const int RetryMaxCount = 10;

		public const int RetryIntervalSeconds = 1;

		public const double DCInMMLimit = 0.6;

		public enum FSMORoles
		{
			RidRole,
			PDCRole,
			SchemaRole,
			InfrastructureRole,
			NamingRole,
			DomainDnsZones,
			ForestDnsZones
		}

		public enum DCProvisioningStatus
		{
			Provisioned,
			MaintenanceMode,
			Demote
		}
	}
}
