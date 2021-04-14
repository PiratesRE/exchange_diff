using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Probes
{
	public sealed class ForwardFullSyncProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name)
		{
			return new ProbeDefinition
			{
				AssemblyPath = ForwardFullSyncProbe.AssemblyPath,
				TypeName = ForwardFullSyncProbe.TypeName,
				Name = name
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!ForwardSyncEventlogUtil.IsForwardSyncActiveServer())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardFullSyncProbe:: DoWork(): This server is Passive", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardFullSyncProbe.cs", 75);
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			using (LocalPowerShellProvider localPowerShellProvider = new LocalPowerShellProvider())
			{
				try
				{
					Dictionary<string, string> parameters = new Dictionary<string, string>();
					Collection<PSObject> collection = localPowerShellProvider.RunExchangeCmdlet<string>("Get-MSOTenantSyncRequest", parameters, base.TraceContext, true);
					foreach (PSObject psobject in collection)
					{
						string text = psobject.Properties["ExternalDirectoryOrganizationId"].Value.ToString();
						this.AppendToLog(true, 0, string.Format("ForwardFullSyncProbe. This org {0} is in fullsync queue", text), "GetMSOTenantSyncRequest");
						parameters = new Dictionary<string, string>
						{
							{
								"Identity",
								text
							}
						};
						Collection<PSObject> collection2 = localPowerShellProvider.RunExchangeCmdlet<string>("Get-Organization", parameters, base.TraceContext, true);
						this.AppendToLog(true, 0, string.Format("ForwardFullSyncProbe. GetOrganization {0}", text), "GetOrg");
						OrganizationId organizationId = (OrganizationId)collection2[0].Properties["OrganizationId"].Value;
						string value = collection2[0].Properties["OriginatingServer"].Value.ToString();
						parameters = new Dictionary<string, string>
						{
							{
								"Identity",
								organizationId.ConfigurationUnit.DistinguishedName
							},
							{
								"Server",
								value
							},
							{
								"Properties",
								"msDS-ReplAttributeMetaData"
							}
						};
						Collection<PSObject> collection3 = localPowerShellProvider.RunExchangeCmdlet<string>("Get-ADObject", parameters, base.TraceContext, true);
						MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>(collection3[0].Properties["msDS-ReplAttributeMetaData"].Value);
						this.AppendToLog(true, 0, "ForwardFullSyncProbe. Get Metadata", "GetADObject");
						StringBuilder stringBuilder2 = new StringBuilder();
						foreach (string text2 in multiValuedProperty)
						{
							if (text2.Contains("msExchMSOForwardSync"))
							{
								stringBuilder2.Append(text2);
								this.AppendToLog(true, 0, text2, "GetMeta");
							}
						}
						string text3 = stringBuilder2.ToString();
						if (string.IsNullOrEmpty(text3))
						{
							base.Result.StateAttribute1 = "Didn't get replication meta data. return";
							this.AppendToLog(true, 0, base.Result.StateAttribute1, "GetEmpty");
							return;
						}
						XmlDocument xmlDocument = new XmlDocument();
						text3 = "<root>" + text3 + "</root>";
						text3 = text3.Replace('\0', ' ');
						xmlDocument.LoadXml(text3);
						XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("pszAttributeName");
						XmlNodeList elementsByTagName2 = xmlDocument.GetElementsByTagName("ftimeLastOriginatingChange");
						bool flag = false;
						for (int i = 0; i < elementsByTagName.Count; i++)
						{
							DateTime dateTime = DateTime.Parse(elementsByTagName2[i].InnerText);
							if ((DateTime.UtcNow - dateTime.ToUniversalTime()).TotalMinutes <= 180.0)
							{
								flag = true;
								break;
							}
							this.AppendToLog(true, 0, string.Format("Organization {0} sync time for {1} at {2}\t", text, elementsByTagName[i].InnerText, dateTime.ToUniversalTime()), "SyncTime");
							ProbeResult result = base.Result;
							result.ExecutionContext += string.Format("Organization {0} sync time for {1} at {2}\t", text, elementsByTagName[i].InnerText, dateTime.ToUniversalTime());
						}
						if (!flag)
						{
							if (stringBuilder.Length == 0)
							{
								stringBuilder.Append("Tenant " + text);
							}
							else
							{
								stringBuilder.Append("; " + text);
							}
						}
						else
						{
							ProbeResult result2 = base.Result;
							result2.ExecutionContext += string.Format("Cookie for this org {0} is up to date.\t", text);
							this.AppendToLog(true, 0, string.Format("ForwardFullSyncProbe. Cookie for this org {0} is up to date.", text), "CompareSyncTime");
						}
					}
				}
				catch (Exception ex)
				{
					this.AppendToLog(false, 1, ex.ToString(), "GetFullSyncTenant");
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.AppendLine(string.Format(" fullsync cookie is(are) not up to date for more than {0} minutes. Please investigate.", 180));
				throw new Exception(stringBuilder.ToString());
			}
		}

		private void AppendToLog(bool isProbeSucceed, int statusCode, string message, string action)
		{
			string hostName = Dns.GetHostName();
			StxLoggerBase.GetLoggerInstance(StxLogType.TestForwardFullSync).BeginAppend(hostName, isProbeSucceed, new TimeSpan(0L), statusCode, message, null, "escalate", action, null);
		}

		private const int FullsyncStuckThresholdMinutes = 180;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ForwardFullSyncProbe).FullName;
	}
}
