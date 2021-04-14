using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Remoting;
using System.Management.Automation.Runspaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class RemotePowershellSession : IDisposable
	{
		public RemotePowershellSession(ILogger logger, string targetServer, PowershellConnectionType connectionType, bool useSSL, Func<string, bool> shouldInvokePowershellCommand)
		{
			this.logger = logger;
			this.targetServer = targetServer;
			this.connectionType = connectionType;
			this.useSSL = useSSL;
			this.shouldInvokePowershellCommand = shouldInvokePowershellCommand;
		}

		public void Dispose()
		{
			if (this.isDisposed)
			{
				return;
			}
			if (this.runspace != null)
			{
				if (this.openedRunspace)
				{
					this.runspace.Close();
				}
				this.runspace.Dispose();
				this.logger.LogInformation(HybridStrings.HybridInfoTotalCmdletTime(this.connectionType.ToString(), this.totalProcessedTime.TotalSeconds));
			}
			this.isDisposed = true;
		}

		public void Connect(PSCredential credentials, CultureInfo sessionUiCulture)
		{
			this.CheckDisposed();
			try
			{
				Dns.GetHostEntry(this.targetServer);
			}
			catch (SocketException ex)
			{
				throw new CouldNotResolveServerException(this.targetServer, ex, ex);
			}
			Uri uri = null;
			string shellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";
			switch (this.connectionType)
			{
			case PowershellConnectionType.WSMan:
				uri = new Uri(string.Format("{0}{1}/wsman", this.useSSL ? "https://" : "http://", this.targetServer));
				shellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";
				break;
			case PowershellConnectionType.OnPrem:
				this.useSSL = false;
				uri = new Uri(string.Format("{0}{1}/powershell?serializationLevel=Full", this.useSSL ? "https://" : "http://", this.targetServer));
				break;
			case PowershellConnectionType.Tenant:
			{
				string uriString = string.Format("{0}{1}/powershell-liveid?serializationLevel=Full", this.useSSL ? "https://" : "http://", this.targetServer);
				uri = new Uri(uriString);
				break;
			}
			}
			WSManConnectionInfo wsmanConnectionInfo = new WSManConnectionInfo(uri, shellUri, credentials);
			if (this.connectionType == PowershellConnectionType.Tenant)
			{
				wsmanConnectionInfo.AuthenticationMechanism = AuthenticationMechanism.Basic;
			}
			else if (this.connectionType == PowershellConnectionType.OnPrem)
			{
				wsmanConnectionInfo.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
			}
			PowershellHostUI hostUI = new PowershellHostUI();
			RemotePowershellHost host = new RemotePowershellHost(hostUI);
			wsmanConnectionInfo.Culture = sessionUiCulture;
			wsmanConnectionInfo.MaximumConnectionRedirectionCount = 5;
			this.runspace = RunspaceFactory.CreateRunspace(host, wsmanConnectionInfo);
			this.logger.LogInformation(HybridStrings.HybridInfoOpeningRunspace(uri.ToString()));
			try
			{
				this.runspace.Open();
			}
			catch (PSRemotingTransportException ex2)
			{
				throw new CouldNotOpenRunspaceException(ex2, ex2);
			}
			this.openedRunspace = true;
		}

		public void RunOneCommand(string command, SessionParameters parameters, bool ignoreNotFoundErrors)
		{
			this.RunOneCommand<object>(command, parameters, ignoreNotFoundErrors);
		}

		public IEnumerable<T> RunOneCommand<T>(string command, SessionParameters parameters, bool ignoreNotFoundErrors)
		{
			Collection<PSObject> collection = this.RunCommand(command, parameters, ignoreNotFoundErrors);
			List<T> list = new List<T>();
			if (collection != null && collection.Count > 0)
			{
				if (collection[0] != null && collection[0].BaseObject != null && collection[0].BaseObject is PSCustomObject && MonadCommand.CanDeserialize(collection[0]))
				{
					using (IEnumerator<PSObject> enumerator = collection.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							PSObject psobject = enumerator.Current;
							if (psobject != null)
							{
								list.Add((T)((object)MonadCommand.Deserialize(psobject)));
							}
						}
						return list;
					}
				}
				throw new InvalidOperationException(HybridStrings.HybridInfoPurePSObjectsNotSupported);
			}
			return list;
		}

		public T RunOneCommandSingleResult<T>(string command, SessionParameters parameters, bool ignoreNotFoundErrors)
		{
			IEnumerable<T> source = this.RunOneCommand<T>(command, parameters, ignoreNotFoundErrors);
			if (source.Count<T>() == 0)
			{
				return default(T);
			}
			if (source.Count<T>() == 1)
			{
				return source.First<T>();
			}
			throw new Exception("To many results returned.  Only one result expected.");
		}

		public object GetPowershellObjectValueOrNull(string command, string identity, string setting)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append(HybridStrings.HybridInfoGetObjectValue(setting, identity, command));
			this.logger.LogInformation(stringBuilder.ToString());
			object result = null;
			SessionParameters parameters = new SessionParameters();
			Dictionary<string, object> powershellUntypedObjectAsMembers = this.GetPowershellUntypedObjectAsMembers(command, identity, parameters);
			if (!powershellUntypedObjectAsMembers.TryGetValue(setting, out result))
			{
				result = null;
			}
			return result;
		}

		public Dictionary<string, object> GetPowershellUntypedObjectAsMembers(string command, string identity, SessionParameters parameters)
		{
			if (!string.IsNullOrEmpty(identity))
			{
				parameters.Set("Identity", identity);
			}
			List<Dictionary<string, object>> powershellUntypedObjectsAsMembers = this.GetPowershellUntypedObjectsAsMembers(command, identity, parameters);
			if (powershellUntypedObjectsAsMembers.Count > 1)
			{
				List<string> list = new List<string>(powershellUntypedObjectsAsMembers.Count);
				foreach (Dictionary<string, object> dictionary in powershellUntypedObjectsAsMembers)
				{
					string item = (dictionary["Identity"] != null) ? dictionary["Identity"].ToString() : string.Empty;
					list.Add(item);
				}
				throw new TooManyResultsException(identity ?? string.Empty, HybridStrings.ErrorTooManyMatchingResults(identity ?? string.Empty), null, list);
			}
			return powershellUntypedObjectsAsMembers[0];
		}

		public List<Dictionary<string, object>> GetPowershellUntypedObjectsAsMembers(string command, string identity, SessionParameters parameters)
		{
			if (!string.IsNullOrEmpty(identity))
			{
				parameters.Set("Identity", identity);
			}
			Collection<PSObject> collection = this.RunCommand(command, parameters, true);
			if (collection == null || collection.Count == 0)
			{
				this.logger.LogInformation(HybridStrings.HybridInfoObjectNotFound);
				return null;
			}
			List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
			foreach (PSObject psobject in collection)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				foreach (PSMemberInfo psmemberInfo in psobject.Members)
				{
					dictionary.Add(psmemberInfo.Name, psmemberInfo.Value);
				}
				list.Add(dictionary);
			}
			return list;
		}

		public Collection<PSObject> RunCommand(string cmdlet, SessionParameters parameters)
		{
			return this.RunCommand(cmdlet, parameters, true);
		}

		public Collection<PSObject> RunCommand(string cmdlet, SessionParameters parameters, bool ignoreNotFoundErrors)
		{
			string data = null;
			string text = this.BuildCmdletInvocationForLogging(cmdlet, parameters);
			if (this.shouldInvokePowershellCommand(text))
			{
				this.CheckDisposed();
				using (PowerShell powerShell = PowerShell.Create())
				{
					powerShell.Runspace = this.runspace;
					powerShell.AddCommand(cmdlet);
					if (parameters != null && parameters.Count > 0)
					{
						powerShell.AddParameters(parameters.ToDictionary());
					}
					ExDateTime now = ExDateTime.Now;
					try
					{
						this.logger.LogInformation(HybridStrings.HybridInfoCmdletStart(this.connectionType.ToString(), text, string.Empty));
						Collection<PSObject> collection = powerShell.Invoke();
						if (powerShell.Streams.Error.Count > 0)
						{
							foreach (ErrorRecord errorRecord in powerShell.Streams.Error)
							{
								if (!errorRecord.CategoryInfo.Reason.Equals("ManagementObjectNotFoundException") || !ignoreNotFoundErrors)
								{
									this.logger.LogError(errorRecord.Exception.ToString());
									throw errorRecord.Exception;
								}
							}
						}
						try
						{
							data = RemotePowershellSession.ToText(collection);
						}
						catch
						{
							data = "?";
						}
						return collection;
					}
					catch (Exception innerException)
					{
						Exception ex = new Exception(HybridStrings.ErrorCmdletException(cmdlet).ToString(), innerException);
						throw ex;
					}
					finally
					{
						TimeSpan t = ExDateTime.Now.Subtract(now);
						this.totalProcessedTime += t;
						this.LogInformationAndData(HybridStrings.HybridInfoCmdletEnd(this.connectionType.ToString(), cmdlet, t.TotalMilliseconds.ToString()), data);
					}
				}
			}
			return null;
		}

		private static string ToText(IEnumerable<PSObject> objects)
		{
			return "[" + string.Join(", ", from o in objects
			select RemotePowershellSession.ToText(o)) + "]";
		}

		private static string ToText(PSObject o)
		{
			if (o.ImmediateBaseObject is ArrayList)
			{
				return "[" + string.Join(", ", ((ArrayList)o.ImmediateBaseObject).ToArray().Select(delegate(object i)
				{
					if (i != null)
					{
						return i.ToString();
					}
					return "(null)";
				})) + "]";
			}
			return "{ " + string.Join(", ", from p in o.Properties
			orderby p.Name
			select RemotePowershellSession.ToText(p)) + "}";
		}

		private static string ToText(PSPropertyInfo p)
		{
			return string.Format("\"{0}\": {1}", p.Name, RemotePowershellSession.ToText(p.TypeNameOfValue, p.Value));
		}

		private static string ToText(string type, object value)
		{
			if (value == null)
			{
				return "(null)";
			}
			if (value is PSObject)
			{
				return RemotePowershellSession.ToText((PSObject)value);
			}
			return "\"" + value.ToString() + "\"";
		}

		private string BuildCmdletInvocationForLogging(string cmdlet, SessionParameters parameters)
		{
			StringBuilder stringBuilder = new StringBuilder(cmdlet);
			if (parameters != null && parameters.Count > 0)
			{
				stringBuilder.AppendFormat(" {0}", string.Join(" ", parameters.LoggingText.ToArray<string>()));
			}
			return stringBuilder.ToString();
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("RemotePowershellProxy");
			}
		}

		private void LogInformationAndData(string text, string data)
		{
			if (this.logger is Logger)
			{
				((Logger)this.logger).LogInformation(text, data);
				return;
			}
			this.logger.LogInformation(text);
		}

		private const string ExchangeShellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

		private const string WSManShellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

		private const string OnPremUriFormat = "{0}{1}/powershell?serializationLevel=Full";

		private const string TenantUriFormat = "{0}{1}/powershell-liveid?serializationLevel=Full";

		private const string WSManUriFormat = "{0}{1}/wsman";

		private const int MaximumConnectionRedirectionCount = 5;

		private readonly string targetServer;

		private readonly PowershellConnectionType connectionType;

		private bool useSSL;

		private Runspace runspace;

		private bool openedRunspace;

		private bool isDisposed;

		private readonly ILogger logger;

		private TimeSpan totalProcessedTime;

		private Func<string, bool> shouldInvokePowershellCommand;
	}
}
