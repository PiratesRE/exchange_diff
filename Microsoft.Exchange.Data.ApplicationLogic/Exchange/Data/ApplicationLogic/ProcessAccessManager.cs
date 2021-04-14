using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.ApplicationLogic.Diagnostics;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ProcessAccess;

namespace Microsoft.Exchange.Data.ApplicationLogic
{
	internal class ProcessAccessManager : ProcessAccessRpcServer
	{
		public static void RegisterComponent(IDiagnosable diagnosable)
		{
			if (diagnosable == null)
			{
				throw new ArgumentNullException("diagnosable");
			}
			string diagnosticComponentName = diagnosable.GetDiagnosticComponentName();
			if (string.IsNullOrEmpty(diagnosticComponentName))
			{
				throw new ArgumentNullException("diagnosable.GetDiagnosticComponentName()");
			}
			lock (ProcessAccessManager.RpcServerLockObject)
			{
				ProcessAccessManager.diagnosableComponents[diagnosticComponentName] = diagnosable;
				if (ProcessAccessManager.RpcServer == null)
				{
					ObjectSecurity serverAdminSecurity = ProcessAccessManager.GetServerAdminSecurity();
					if (serverAdminSecurity != null)
					{
						using (Process currentProcess = Process.GetCurrentProcess())
						{
							string annotation;
							if (string.Equals(currentProcess.ProcessName, "w3wp", StringComparison.InvariantCultureIgnoreCase))
							{
								annotation = currentProcess.Id + '\t' + Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);
							}
							else
							{
								annotation = currentProcess.Id + '\t' + currentProcess.ProcessName;
							}
							ProcessAccessManager.RpcServer = (ProcessAccessManager)ProcessAccessRpcServer.RegisterServer(typeof(ProcessAccessManager), serverAdminSecurity, 1, Guid.NewGuid(), annotation, false, 8U);
						}
					}
				}
			}
		}

		public static void RegisterAgentFactory(AgentFactory agentFactory)
		{
			IDiagnosable diagnosable = agentFactory as IDiagnosable;
			if (diagnosable != null)
			{
				ProcessAccessManager.RegisterComponent(diagnosable);
			}
		}

		public static void UnregisterComponent(IDiagnosable diagnosable)
		{
			if (diagnosable == null)
			{
				throw new ArgumentNullException("diagnosable");
			}
			string diagnosticComponentName = diagnosable.GetDiagnosticComponentName();
			if (string.IsNullOrEmpty(diagnosticComponentName))
			{
				throw new ArgumentNullException("diagnosable.GetDiagnosticComponentName()");
			}
			lock (ProcessAccessManager.RpcServerLockObject)
			{
				ProcessAccessManager.diagnosableComponents.Remove(diagnosticComponentName);
				if (ProcessAccessManager.diagnosableComponents.Count == 0 && ProcessAccessManager.RpcServer != null)
				{
					RpcServerBase.StopServer(ProcessAccessRpcServer.RpcIntfHandle);
					ProcessAccessManager.RpcServer = null;
				}
			}
			IDiagnosableExtraData diagnosableExtraData = diagnosable as IDiagnosableExtraData;
			if (diagnosableExtraData != null)
			{
				diagnosableExtraData.OnStop();
			}
		}

		public static string ClientRunProcessCommand(string server, string process, string componentName, string componentArgument, bool componentRestrictedData, bool componentUnlimited, string userIdentity)
		{
			if (string.IsNullOrEmpty(server))
			{
				server = Environment.MachineName;
			}
			string result;
			try
			{
				string text;
				try
				{
					text = ProcessAccessManager.ClientRunProcessCommand(server, ProcessAccessRpcServer.ProcessLocatorGuid, "ProcessLocator", null, false, false, null);
				}
				catch (RpcException rpcException)
				{
					return ProcessAccessManager.ReturnFromRpcException(server, "Query registered processes", rpcException);
				}
				if (text == null)
				{
					result = new XElement("Diagnostics", new XElement("error", string.Format("No processes have registered on {0} to support this task.", server))).ToString(SaveOptions.DisableFormatting);
				}
				else
				{
					XDocument xdocument;
					try
					{
						xdocument = XDocument.Parse(text);
					}
					catch (XmlException)
					{
						return new XElement("Diagnostics", new object[]
						{
							new XElement("error", "Xml parsing error"),
							new XElement("failedXml", text)
						}).ToString(SaveOptions.DisableFormatting);
					}
					XElement root = xdocument.Root;
					XElement xelement = root.Element("ProcessLocator");
					if (string.IsNullOrEmpty(process))
					{
						result = root.ToString(SaveOptions.DisableFormatting);
					}
					else
					{
						Guid processGuid = Guid.Empty;
						int num = 0;
						foreach (XElement xelement2 in xelement.Elements("Process"))
						{
							Guid guid;
							if (GuidHelper.TryParseGuid(xelement2.Element("guid").Value, out guid))
							{
								if (string.Equals(xelement2.Element("id").Value, process, StringComparison.OrdinalIgnoreCase))
								{
									processGuid = guid;
									num++;
								}
								else if (string.Equals(xelement2.Element("name").Value, process, StringComparison.OrdinalIgnoreCase))
								{
									processGuid = guid;
									num++;
								}
							}
						}
						if (num < 1)
						{
							xelement.AddFirst(new XElement("error", string.Format("Specified process is not registered. Currently registered on {0}.", server)));
							result = root.ToString(SaveOptions.DisableFormatting);
						}
						else if (num > 1)
						{
							xelement.AddFirst(new XElement("error", string.Format("Specified process matches more than one registered process on {0}.", server)));
							result = root.ToString(SaveOptions.DisableFormatting);
						}
						else
						{
							try
							{
								result = ProcessAccessManager.ClientRunProcessCommand(server, processGuid, componentName, componentArgument, componentRestrictedData, componentUnlimited, userIdentity);
							}
							catch (RpcException rpcException2)
							{
								result = ProcessAccessManager.ReturnFromRpcException(server, "Query process diagnostics", rpcException2);
							}
						}
					}
				}
			}
			catch (Exception arg)
			{
				result = new XElement("Diagnostics", new XElement("error", string.Format("ProcessAccessManager Exception: {0}", arg))).ToString(SaveOptions.DisableFormatting);
			}
			return result;
		}

		private static string ClientRunProcessCommand(string server, Guid processGuid, string componentName, string componentArgument, bool componentRestrictedData, bool componentUnlimited, string userIdentity)
		{
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			if (!string.IsNullOrEmpty(componentName))
			{
				mdbefPropertyCollection.Add(2466250783U, componentName);
			}
			if (!string.IsNullOrEmpty(componentArgument))
			{
				mdbefPropertyCollection.Add(2466316319U, componentArgument);
			}
			mdbefPropertyCollection.Add(2466381835U, componentRestrictedData);
			mdbefPropertyCollection.Add(2466512907U, componentUnlimited);
			if (!string.IsNullOrEmpty(userIdentity))
			{
				mdbefPropertyCollection.Add(2466447391U, userIdentity);
			}
			byte[] bytes = mdbefPropertyCollection.GetBytes();
			byte[] array;
			using (ProcessAccessRpcClient processAccessRpcClient = new ProcessAccessRpcClient(server, processGuid))
			{
				array = processAccessRpcClient.RunProcessCommand(bytes);
			}
			MdbefPropertyCollection mdbefPropertyCollection2 = MdbefPropertyCollection.Create(array, 0, array.Length);
			object obj = null;
			mdbefPropertyCollection2.TryGetValue(2471559199U, out obj);
			return obj as string;
		}

		public override byte[] RunProcessCommand(byte[] inBlob)
		{
			string componentName = null;
			string argument = null;
			bool allowRestrictedData = false;
			bool unlimited = false;
			string userIdentity = null;
			string text = null;
			MdbefPropertyCollection mdbefPropertyCollection = MdbefPropertyCollection.Create(inBlob, 0, inBlob.Length);
			object obj;
			if (mdbefPropertyCollection.TryGetValue(2466316319U, out obj) && obj is string)
			{
				argument = (string)obj;
			}
			if (mdbefPropertyCollection.TryGetValue(2466250783U, out obj) && obj is string)
			{
				componentName = (string)obj;
			}
			if (mdbefPropertyCollection.TryGetValue(2466381835U, out obj) && obj is bool)
			{
				allowRestrictedData = (bool)obj;
			}
			if (mdbefPropertyCollection.TryGetValue(2466512907U, out obj) && obj is bool)
			{
				unlimited = (bool)obj;
			}
			if (mdbefPropertyCollection.TryGetValue(2466447391U, out obj) && obj is string)
			{
				userIdentity = (string)obj;
			}
			try
			{
				DiagnosableParameters componentParameters = DiagnosableParameters.Create(argument, allowRestrictedData, unlimited, userIdentity);
				text = ProcessAccessManager.RunComponentCommand(componentName, componentParameters);
			}
			catch (XmlException ex)
			{
				StringBuilder stringBuilder = new StringBuilder(100 * ex.Data.Count);
				foreach (object obj2 in ex.Data)
				{
					KeyValuePair<object, object> keyValuePair = (KeyValuePair<object, object>)obj2;
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}:{1}{2}", new object[]
					{
						keyValuePair.Key,
						keyValuePair.Value,
						Environment.NewLine
					});
				}
				text = string.Format(CultureInfo.InvariantCulture, "Message={0}{1}Data={2}{1}Stack={3}", new object[]
				{
					ex.Message,
					Environment.NewLine,
					stringBuilder,
					ex.StackTrace
				});
			}
			catch (Exception ex2)
			{
				text = ex2.ToString();
			}
			MdbefPropertyCollection mdbefPropertyCollection2 = new MdbefPropertyCollection();
			if (text != null)
			{
				mdbefPropertyCollection2.Add(2471559199U, text);
			}
			return mdbefPropertyCollection2.GetBytes();
		}

		private static ObjectSecurity GetServerAdminSecurity()
		{
			FileSecurity securityDescriptor = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 578, "GetServerAdminSecurity", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\ProcessAccessManager.cs");
				Server server = null;
				try
				{
					server = topologyConfigurationSession.FindLocalServer();
				}
				catch (LocalServerNotFoundException)
				{
					return;
				}
				RawSecurityDescriptor rawSecurityDescriptor = server.ReadSecurityDescriptor();
				if (rawSecurityDescriptor != null)
				{
					securityDescriptor = new FileSecurity();
					byte[] array = new byte[rawSecurityDescriptor.BinaryLength];
					rawSecurityDescriptor.GetBinaryForm(array, 0);
					securityDescriptor.SetSecurityDescriptorBinaryForm(array);
					IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 605, "GetServerAdminSecurity", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\ProcessAccessManager.cs");
					SecurityIdentifier exchangeServersUsgSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
					FileSystemAccessRule fileSystemAccessRule = new FileSystemAccessRule(exchangeServersUsgSid, FileSystemRights.ReadData, AccessControlType.Allow);
					securityDescriptor.SetAccessRule(fileSystemAccessRule);
					SecurityIdentifier identity = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
					fileSystemAccessRule = new FileSystemAccessRule(identity, FileSystemRights.ReadData, AccessControlType.Allow);
					securityDescriptor.AddAccessRule(fileSystemAccessRule);
					return;
				}
			}, 3);
			return securityDescriptor;
		}

		private static string RunComponentCommand(string componentName, DiagnosableParameters componentParameters)
		{
			XDocument xdocument = new XDocument();
			XElement xelement = new XElement("Diagnostics");
			xdocument.Add(xelement);
			if (string.Equals(componentName, "ProcessLocator", StringComparison.OrdinalIgnoreCase))
			{
				XElement xelement2 = new XElement("ProcessLocator");
				xelement.Add(xelement2);
				List<KeyValuePair<Guid, string>> registeredProcessGuids = ProcessAccessRpcServer.GetRegisteredProcessGuids();
				int num = 0;
				if (string.Equals(componentParameters.Argument, "debug", StringComparison.OrdinalIgnoreCase))
				{
					using (List<KeyValuePair<Guid, string>>.Enumerator enumerator = registeredProcessGuids.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<Guid, string> pair = enumerator.Current;
							ProcessAccessManager.AddAsXmlElement(xelement2, pair);
							num++;
						}
						goto IL_117;
					}
				}
				HashSet<string> hashSet = new HashSet<string>(registeredProcessGuids.Count);
				foreach (KeyValuePair<Guid, string> pair2 in registeredProcessGuids)
				{
					if (pair2.Key != ProcessAccessRpcServer.ProcessLocatorGuid && !hashSet.Contains(pair2.Value))
					{
						ProcessAccessManager.AddAsXmlElement(xelement2, pair2);
						num++;
						hashSet.Add(pair2.Value);
					}
				}
				IL_117:
				xelement2.AddFirst(new XElement("count", num));
			}
			else
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					DateTime dateTime = currentProcess.StartTime.ToUniversalTime();
					DateTime utcNow = DateTime.UtcNow;
					XElement content = new XElement("ProcessInfo", new object[]
					{
						new XElement("id", currentProcess.Id),
						new XElement("serverName", Environment.MachineName),
						new XElement("startTime", dateTime),
						new XElement("currentTime", utcNow),
						new XElement("lifetime", (utcNow - dateTime).ToString()),
						new XElement("threadCount", currentProcess.Threads.Count),
						new XElement("handleCount", currentProcess.HandleCount),
						new XElement("workingSet", ByteQuantifiedSize.FromBytes((ulong)currentProcess.WorkingSet64))
					});
					xelement.Add(content);
				}
				bool flag = string.IsNullOrEmpty(componentName);
				bool flag2 = componentName == "?";
				if (!flag2 && !flag && !ProcessAccessManager.diagnosableComponents.ContainsKey(componentName))
				{
					XElement content2 = new XElement(componentName, new XElement("message", string.Format("Component \"{0}\" is not supported by this process.", componentName)));
					xelement.Add(content2);
					flag2 = true;
				}
				XElement xelement3 = new XElement("Components");
				xelement.Add(xelement3);
				lock (ProcessAccessManager.RpcServerLockObject)
				{
					if (flag)
					{
						using (IEnumerator<KeyValuePair<string, IDiagnosable>> enumerator3 = ProcessAccessManager.diagnosableComponents.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								KeyValuePair<string, IDiagnosable> keyValuePair = enumerator3.Current;
								xelement3.Add(keyValuePair.Value.GetDiagnosticInfo(componentParameters));
							}
							goto IL_3C2;
						}
					}
					if (flag2)
					{
						using (IEnumerator<KeyValuePair<string, IDiagnosable>> enumerator4 = ProcessAccessManager.diagnosableComponents.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								KeyValuePair<string, IDiagnosable> keyValuePair2 = enumerator4.Current;
								xelement3.Add(new XElement("Component", keyValuePair2.Key));
							}
							goto IL_3C2;
						}
					}
					IDiagnosable diagnosable = ProcessAccessManager.diagnosableComponents[componentName];
					XElement diagnosticInfo = diagnosable.GetDiagnosticInfo(componentParameters);
					xelement3.Add(diagnosticInfo);
					IL_3C2:;
				}
			}
			string result;
			try
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
					{
						xmlTextWriter.Formatting = Formatting.None;
						xdocument.Save(xmlTextWriter);
					}
					result = stringWriter.ToString();
				}
			}
			catch (XmlException ex)
			{
				foreach (XElement xelement4 in xdocument.Descendants())
				{
					ex.Data[xelement4.Name] = xelement4.Value;
				}
				throw;
			}
			return result;
		}

		private static void AddAsXmlElement(XElement parent, KeyValuePair<Guid, string> pair)
		{
			string[] array = pair.Value.Split(new char[]
			{
				' ',
				'\t'
			}, 2, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length == 2)
			{
				string content = array[1];
				string content2 = array[0];
				parent.Add(new XElement("Process", new object[]
				{
					new XElement("guid", pair.Key),
					new XElement("id", content2),
					new XElement("name", content)
				}));
			}
		}

		private static string ReturnFromRpcException(string server, string action, RpcException rpcException)
		{
			XElement xelement = new XElement("Diagnostics");
			if (1753 == rpcException.ErrorCode)
			{
				xelement.Add(new XElement("error", string.Format("No processes have registered on {0} to support this task.", server)));
			}
			else
			{
				xelement.Add(new XElement("error", string.Format("ProcessAccessManager RPC Error {0:X}", rpcException.ErrorCode)));
			}
			xelement.Add(new object[]
			{
				new XElement("action", action),
				new XElement("message", rpcException.Message)
			});
			return xelement.ToString(SaveOptions.DisableFormatting);
		}

		private const string DiagnosticsElementName = "Diagnostics";

		private const uint InArgComponentName = 2466250783U;

		private const uint InArgComponentArgument = 2466316319U;

		private const uint InArgComponentRestrictedData = 2466381835U;

		private const uint InArgComponentUserIdentity = 2466447391U;

		private const uint InArgComponentUnlimited = 2466512907U;

		private const uint OutArgResult = 2471559199U;

		private const int EPT_S_NOT_REGISTERED = 1753;

		private const uint MaxConcurrentCalls = 8U;

		private const string ProcessLocatorComponent = "ProcessLocator";

		private const string ProcessElementName = "Process";

		private const string ProcessGuidElementName = "guid";

		private const string ProcessIdElementName = "id";

		private const string ProcessNameElementName = "name";

		private const char ProcessLocatorSeparator = '=';

		private static readonly string[] LineSeparator = new string[]
		{
			Environment.NewLine
		};

		private static readonly object RpcServerLockObject = new object();

		private static ProcessAccessManager RpcServer;

		private static SynchronizedDictionary<string, IDiagnosable> diagnosableComponents = new SynchronizedDictionary<string, IDiagnosable>(StringComparer.OrdinalIgnoreCase);
	}
}
