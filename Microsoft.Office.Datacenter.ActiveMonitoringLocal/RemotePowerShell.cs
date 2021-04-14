using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class RemotePowerShell
	{
		private RemotePowerShell()
		{
		}

		private Dictionary<string, Tuple<RemoteRunspaceFactory, string>> RemoteRunspaceFactories
		{
			get
			{
				if (this.remoteRunspaceFactories == null)
				{
					this.remoteRunspaceFactories = new Dictionary<string, Tuple<RemoteRunspaceFactory, string>>();
				}
				return this.remoteRunspaceFactories;
			}
		}

		public static RemotePowerShell CreateRemotePowerShellByCertificate(Uri uri, string certificateSubjectDN, bool resolveUri = false)
		{
			X509Certificate2 cert = RemotePowerShell.FindCertificate(StoreLocation.LocalMachine, StoreName.My, certificateSubjectDN);
			RemotePowerShell rps = new RemotePowerShell();
			RemotePowerShell.AddRemoteRunspaceFactories addRemoteRunspaceFactories = delegate(Uri uriToAdd)
			{
				if (!rps.RemoteRunspaceFactories.ContainsKey(uriToAdd.AbsoluteUri))
				{
					RemoteConnectionInfo remoteConnectionInfo = new RemoteConnectionInfo(uriToAdd, cert.Thumbprint, "http://schemas.microsoft.com/powershell/Microsoft.Exchange", null, AuthenticationMechanism.Default, true, 3);
					rps.RemoteRunspaceFactories.Add(uriToAdd.AbsoluteUri, new Tuple<RemoteRunspaceFactory, string>(new RemoteRunspaceFactory(new InitialSessionStateFactory(), new BasicPSHostFactory(typeof(RunspaceHost)), remoteConnectionInfo), uri.AbsoluteUri));
				}
			};
			if (resolveUri)
			{
				using (IEnumerator enumerator = new HostnameResolver(uri.Host).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						IPAddress vip = (IPAddress)obj;
						addRemoteRunspaceFactories(RemotePowerShell.ReconstructUriFromIP(vip, uri));
					}
					goto IL_96;
				}
			}
			addRemoteRunspaceFactories(uri);
			IL_96:
			return rps;
		}

		public static RemotePowerShell CreateRemotePowerShellByCertificate(string[] uriStrings, string certificateSubjectDN, bool resolveUri = false)
		{
			if (uriStrings == null || uriStrings.Length == 0)
			{
				throw new ArgumentException("Cannot be null or empty.", "uriStrings");
			}
			X509Certificate2 cert = RemotePowerShell.FindCertificate(StoreLocation.LocalMachine, StoreName.My, certificateSubjectDN);
			RemotePowerShell rps = new RemotePowerShell();
			List<Exception> list = null;
			foreach (string text in uriStrings)
			{
				try
				{
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, RemotePowerShell.traceContext, "RemotePowerShell.CreateRemotePowerShellByCertificate: Creating Uri for '{0}'...", text, null, "CreateRemotePowerShellByCertificate", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\RemotePowershell\\RemotePowerShell.cs", 176);
					Uri uri = new Uri(text);
					RemotePowerShell.AddRemoteRunspaceFactories addRemoteRunspaceFactories = delegate(Uri uriToAdd)
					{
						if (!rps.RemoteRunspaceFactories.ContainsKey(uriToAdd.AbsoluteUri))
						{
							RemoteConnectionInfo remoteConnectionInfo = new RemoteConnectionInfo(uriToAdd, cert.Thumbprint, "http://schemas.microsoft.com/powershell/Microsoft.Exchange", null, AuthenticationMechanism.Default, true, 3);
							rps.RemoteRunspaceFactories.Add(uriToAdd.AbsoluteUri, new Tuple<RemoteRunspaceFactory, string>(new RemoteRunspaceFactory(new InitialSessionStateFactory(), new BasicPSHostFactory(typeof(RunspaceHost)), remoteConnectionInfo), uri.AbsoluteUri));
						}
					};
					if (resolveUri)
					{
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, RemotePowerShell.traceContext, "RemotePowerShell.CreateRemotePowerShellByCertificate: Resolving endpoint '{0}'...", uri.AbsoluteUri, null, "CreateRemotePowerShellByCertificate", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\RemotePowershell\\RemotePowerShell.cs", 212);
						using (IEnumerator enumerator = new HostnameResolver(uri.Host).GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								IPAddress vip = (IPAddress)obj;
								addRemoteRunspaceFactories(RemotePowerShell.ReconstructUriFromIP(vip, uri));
							}
							goto IL_13A;
						}
					}
					addRemoteRunspaceFactories(uri);
					IL_13A:;
				}
				catch (Exception ex)
				{
					string message = string.Format("Failed to process endpoint URI '{0}': {1}", text, ex.ToString());
					WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, RemotePowerShell.traceContext, message, null, "CreateRemotePowerShellByCertificate", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\RemotePowershell\\RemotePowerShell.cs", 233);
					if (list == null)
					{
						list = new List<Exception>();
					}
					list.Add(new Exception(message));
				}
			}
			if (rps.RemoteRunspaceFactories.Count == 0)
			{
				throw new AggregateException(string.Format("No valid endpoints can be resolved from the specified uriStrings ('{0}').", uriStrings), list);
			}
			return rps;
		}

		public static RemotePowerShell CreateRemotePowerShellByCredential(Uri uri, string userName, string password, bool resolveUri = false)
		{
			RemotePowerShell rps = new RemotePowerShell();
			RemotePowerShell.AddRemoteRunspaceFactories addRemoteRunspaceFactories = delegate(Uri uriToAdd)
			{
				if (!rps.RemoteRunspaceFactories.ContainsKey(uriToAdd.AbsoluteUri))
				{
					RemoteConnectionInfo remoteConnectionInfo = new RemoteConnectionInfo(uriToAdd, new PSCredential(userName, RemotePowerShell.ConvertToSecureString(password)), "http://schemas.microsoft.com/powershell/Microsoft.Exchange", null, AuthenticationMechanism.Basic, true, 3);
					rps.RemoteRunspaceFactories.Add(uriToAdd.AbsoluteUri, new Tuple<RemoteRunspaceFactory, string>(new RemoteRunspaceFactory(new InitialSessionStateFactory(), new BasicPSHostFactory(typeof(RunspaceHost)), remoteConnectionInfo), uri.AbsoluteUri));
				}
			};
			if (resolveUri)
			{
				using (IEnumerator enumerator = new HostnameResolver(uri.Host).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						IPAddress vip = (IPAddress)obj;
						addRemoteRunspaceFactories(RemotePowerShell.ReconstructUriFromIP(vip, uri));
					}
					goto IL_96;
				}
			}
			addRemoteRunspaceFactories(uri);
			IL_96:
			return rps;
		}

		public Collection<PSObject> InvokePSCommand(PSCommand command)
		{
			List<Exception> list = null;
			using (BasicRunspaceCache basicRunspaceCache = new BasicRunspaceCache())
			{
				foreach (KeyValuePair<string, Tuple<RemoteRunspaceFactory, string>> keyValuePair in this.RemoteRunspaceFactories)
				{
					string key = keyValuePair.Key;
					RemoteRunspaceFactory item = keyValuePair.Value.Item1;
					string item2 = keyValuePair.Value.Item2;
					WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, RemotePowerShell.traceContext, "RemotePowerShell.InvokePSCommand: Invoking via endpoint '{0}' ('{1}')...", key, item2, null, "InvokePSCommand", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\RemotePowershell\\RemotePowerShell.cs", 330);
					try
					{
						RunspaceMediator runspaceMediator = new RunspaceMediator(item, basicRunspaceCache);
						using (RunspaceProxy runspaceProxy = new RunspaceProxy(runspaceMediator, true))
						{
							PowerShellProxy powerShellProxy = new PowerShellProxy(runspaceProxy, command);
							Collection<PSObject> result = powerShellProxy.Invoke<PSObject>();
							if (powerShellProxy.Errors != null && powerShellProxy.Errors.Count > 0)
							{
								Exception[] array = new Exception[powerShellProxy.Errors.Count];
								for (int i = 0; i < powerShellProxy.Errors.Count; i++)
								{
									if (ErrorCategory.InvalidArgument == powerShellProxy.Errors[i].CategoryInfo.Category)
									{
										throw powerShellProxy.Errors[i].Exception;
									}
									array[i] = powerShellProxy.Errors[i].Exception;
								}
								throw new AggregateException(array);
							}
							return result;
						}
					}
					catch (Exception ex)
					{
						string message = string.Format("Failed to invoke via endpoint '{0}' ('{1}'): {2}", key, item2, ex.ToString());
						WTFDiagnostics.TraceError(ExTraceGlobals.CommonComponentsTracer, RemotePowerShell.traceContext, message, null, "InvokePSCommand", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\RemotePowershell\\RemotePowerShell.cs", 378);
						if (list == null)
						{
							list = new List<Exception>();
						}
						if (ex is RemoteException && ErrorCategory.InvalidArgument == ((RemoteException)ex).ErrorRecord.CategoryInfo.Category)
						{
							throw;
						}
						list.Add(new Exception(message));
					}
				}
			}
			if (list != null)
			{
				throw new AggregateException("Tried all available endpoints without success.", list);
			}
			throw new Exception(string.Format("No results found (number of endpoints: {0}).", this.RemoteRunspaceFactories.Count));
		}

		private static X509Certificate2 FindCertificate(StoreLocation location, StoreName name, string findValue)
		{
			if (string.IsNullOrWhiteSpace(findValue))
			{
				throw new ArgumentException("Cannot be null or white-space characters.", "findValue");
			}
			X509Store x509Store = new X509Store(name, location);
			X509Certificate2 result;
			try
			{
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, findValue, true);
				if (x509Certificate2Collection == null || x509Certificate2Collection.Count != 1)
				{
					x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, findValue, true);
					if (x509Certificate2Collection == null || x509Certificate2Collection.Count != 1)
					{
						throw new Exception(string.Format("Unable to find a valid certificate by either thumbprint or subject DN '{0}', '{1}', '{2}'.", location, name, findValue));
					}
				}
				result = x509Certificate2Collection[0];
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		private static Uri ReconstructUriFromIP(IPAddress vip, Uri uri)
		{
			string format = (vip.AddressFamily == AddressFamily.InterNetworkV6) ? "{0}://[{1}]{2}" : "{0}://{1}{2}";
			return new Uri(string.Format(format, uri.Scheme, vip.ToString(), uri.AbsolutePath));
		}

		public unsafe static SecureString ConvertToSecureString(string password)
		{
			if (password == null || password.Length == 0)
			{
				return new SecureString();
			}
			IntPtr intPtr2;
			IntPtr intPtr = intPtr2 = password;
			if (intPtr != 0)
			{
				intPtr2 = (IntPtr)((int)intPtr + RuntimeHelpers.OffsetToStringData);
			}
			char* value = intPtr2;
			SecureString secureString = new SecureString(value, password.Length);
			secureString.MakeReadOnly();
			return secureString;
		}

		internal const string DefaultShellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

		private static TracingContext traceContext = TracingContext.Default;

		private Dictionary<string, Tuple<RemoteRunspaceFactory, string>> remoteRunspaceFactories;

		private delegate void AddRemoteRunspaceFactories(Uri uriToAdd);
	}
}
