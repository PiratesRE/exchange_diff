using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.InstantMessaging;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa
{
	public class InstantMessagingLocalProbe : ProbeWorkItem
	{
		private IEndpointManager EndpointManager { get; set; }

		private IUtilities Utilities { get; set; }

		protected override void DoWork(CancellationToken cancellationToken)
		{
			ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
			if (exchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				string text = null;
				string text2 = null;
				try
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Find IM Implementation DLL path from registry", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 81);
					using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA\\InstantMessaging"))
					{
						if (registryKey != null)
						{
							InstantMessagingLocalProbe.IMImplementationDllPath = (string)registryKey.GetValue("ImplementationDLLPath", string.Empty);
						}
					}
					if (string.IsNullOrEmpty(InstantMessagingLocalProbe.IMImplementationDllPath))
					{
						throw new Exception("IM Implementation DLL path could not be found from registry.");
					}
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Get IM certificate thumbprint and server name", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 95);
					this.GetIMConfig(out text, out text2);
					base.Result.StateAttribute3 = string.Format("IMImplementationDllPath: {0}", InstantMessagingLocalProbe.IMImplementationDllPath);
					base.Result.StateAttribute4 = string.Format("InstantMessagingServerName: {0}", text);
					base.Result.StateAttribute5 = string.Format("InstantMessagingCertificateThumbprint: {0}", text2);
					X509Certificate2 x509Certificate = Utils.FindCertificate(base.TraceContext, StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, text2);
					if (x509Certificate == null)
					{
						throw new Exception("IM Certificate not found");
					}
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Initialize endpoint manager", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 114);
					if (!this.InitializeEndpointManager(x509Certificate.Issuer, x509Certificate.GetSerialNumber()))
					{
						throw (base.Result.Exception != null) ? new Exception(base.Result.Exception) : new Exception("IM Provider not initialized");
					}
					IEndpoint endpoint = this.EndpointManager.CreateEndpoint("SIP:" + base.Definition.Account, text, 5, null, null);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Begin IM Signin and wait for IM signin callback", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 130);
					IAsyncResult asyncResult = endpoint.BeginSignIn(false, 3500, new AsyncCallback(this.SignInCallback), endpoint);
					asyncResult.AsyncWaitHandle.WaitOne();
				}
				catch (Exception ex)
				{
					base.Result.Exception = ex.ToString();
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, base.Result.Exception, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 139);
				}
				finally
				{
					this.DisposeEndpointManager();
				}
				if (base.Result.ResultType.Equals(ResultType.Failed))
				{
					throw new Exception(base.Result.Exception);
				}
			}
		}

		private void GetIMConfig(out string instantMessagingServerName, out string instantMessagingCertificateThumbprint)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), OrganizationId.ForestWideOrgId, null, false);
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.FullyConsistent, sessionSettings, 166, "GetIMConfig", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs");
			Server server = topologyConfigurationSession.FindLocalServer();
			string propertyValue = string.Format("IIS://{0}/W3SVC/2/ROOT/owa", server.Fqdn);
			ADObjectId descendantId = server.Id.GetDescendantId("Protocols", "HTTP", new string[0]);
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeVirtualDirectorySchema.MetabasePath, propertyValue);
			ADOwaVirtualDirectory[] array = topologyConfigurationSession.Find<ADOwaVirtualDirectory>(descendantId, QueryScope.SubTree, filter, null, 1);
			if (array == null)
			{
				throw new Exception("Owa BE vdir could not be found");
			}
			instantMessagingServerName = array[0].InstantMessagingServerName;
			instantMessagingCertificateThumbprint = array[0].InstantMessagingCertificateThumbprint;
		}

		private bool InitializeEndpointManager(string certificateIssuer, byte[] certificateSerialNumber)
		{
			bool flag = false;
			IEndpointManager endpointManager = null;
			IUtilities utilities = null;
			string text = string.Format(CultureInfo.InvariantCulture, "OWA/{0}", new object[]
			{
				FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
			});
			Type[] array = new Type[]
			{
				typeof(string),
				typeof(byte[]),
				typeof(string)
			};
			object[] array2 = new object[]
			{
				certificateIssuer,
				certificateSerialNumber,
				text
			};
			Type[] array3 = new Type[0];
			object[] array4 = new object[0];
			Type[][] variousConstructorParameters = new Type[][]
			{
				array,
				array3
			};
			object[][] variousConstructorValues = new object[][]
			{
				array2,
				array4
			};
			Type[][] variousConstructorParameters2 = new Type[][]
			{
				array3
			};
			object[][] variousConstructorValues2 = new object[][]
			{
				array4
			};
			bool result;
			try
			{
				Assembly assembly = Assembly.LoadFrom(InstantMessagingLocalProbe.IMImplementationDllPath);
				foreach (Type type in assembly.GetTypes())
				{
					bool flag2;
					InstantMessagingLocalProbe.InitializeType<IEndpointManager>(type, ref endpointManager, variousConstructorParameters, variousConstructorValues, out flag2);
					if (flag2)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Initialize IEndpointManager failed.", null, "InitializeEndpointManager", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 248);
						return false;
					}
					InstantMessagingLocalProbe.InitializeType<IUtilities>(type, ref utilities, variousConstructorParameters2, variousConstructorValues2, out flag2);
					if (flag2)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Initialize IUtilities failed.", null, "InitializeEndpointManager", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 256);
						return false;
					}
				}
				if (endpointManager == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Initialize EndpointManager: No constructor found.", null, "InitializeEndpointManager", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 264);
					result = false;
				}
				else
				{
					foreach (int num in InstantMessagingLocalProbe.mtlsPortNumbers)
					{
						try
						{
							endpointManager.Initialize(null, num);
							flag = true;
							break;
						}
						catch (InstantMessagingException ex)
						{
							base.Result.Exception = ex.ToString();
							WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "Exception during initialization: {0}", base.Result.Exception, null, "InitializeEndpointManager", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 282);
							if (ex.Code != 18105)
							{
								break;
							}
						}
					}
					if (!flag)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Initialize IEndpointManager failed.", null, "InitializeEndpointManager", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 297);
						result = false;
					}
					else
					{
						this.EndpointManager = endpointManager;
						this.Utilities = utilities;
						result = true;
					}
				}
			}
			catch (Exception ex2)
			{
				base.Result.Exception = ex2.ToString();
				result = false;
			}
			return result;
		}

		private static void InitializeType<T>(Type type, ref T foundInstance, Type[][] variousConstructorParameters, object[][] variousConstructorValues, out bool terminalError)
		{
			terminalError = false;
			if (!type.IsClass || !type.IsPublic || type.GetInterface(typeof(T).FullName) == null)
			{
				return;
			}
			if (foundInstance != null)
			{
				terminalError = true;
				return;
			}
			for (int i = 0; i < variousConstructorParameters.Length; i++)
			{
				ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.HasThis, variousConstructorParameters[i], null);
				if (constructor != null)
				{
					foundInstance = (T)((object)Activator.CreateInstance(type, variousConstructorValues[i]));
					return;
				}
			}
		}

		private void DisposeEndpointManager()
		{
			if (this.EndpointManager != null)
			{
				IDisposable disposable = this.EndpointManager as IDisposable;
				if (disposable != null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Disposing EndpointManager", null, "DisposeEndpointManager", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 367);
					disposable.Dispose();
				}
				this.EndpointManager = null;
			}
		}

		private void SignInCallback(IAsyncResult result)
		{
			try
			{
				IEndpoint endpoint = result.AsyncState as IEndpoint;
				if (endpoint != null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, base.TraceContext, "Call End signin", null, "SignInCallback", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 388);
					if (endpoint != null)
					{
						endpoint.EndSignIn(result);
					}
				}
			}
			catch (Exception ex)
			{
				base.Result.Exception = ex.ToString();
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.OWATracer, base.TraceContext, "SigninCallback Exception: {0}", base.Result.Exception, null, "SignInCallback", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\InstantMessaging\\InstantMessagingLocalProbe.cs", 399);
				InstantMessagingException ex2 = ex as InstantMessagingException;
				if (ex2 != null)
				{
					base.Result.StateAttribute12 = string.Format("IM Exception Code: {0}", ex2.Code.ToString());
					base.Result.StateAttribute13 = string.Format("IM Exception SubCode: {0}", ex2.SubCode.ToString());
					base.Result.StateAttribute14 = string.Format("IM Exception Reason: {0}", ex2.Reason);
					base.Result.ResultType = ResultType.Failed;
				}
			}
		}

		private const string UserAgentFormat = "OWA/{0}";

		private const string OwaInstantMessagingRegKeyBase = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA\\InstantMessaging";

		private const string IMImplementationDllPathKey = "ImplementationDLLPath";

		private static string IMImplementationDllPath;

		private static int[] mtlsPortNumbers = new int[]
		{
			5074,
			5073,
			5072
		};
	}
}
