using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Hybrid;

namespace Microsoft.Exchange.Management.Deployment.HybridConfigurationDetection
{
	internal class RemotePowershellSession : IDisposable
	{
		public RemotePowershellSession(string targetServer, PowershellConnectionType connectionType, bool useSSL, ILogger logger)
		{
			this.targetServer = targetServer;
			this.connectionType = connectionType;
			this.useSSL = useSSL;
			if (logger == null)
			{
				throw new ArgumentNullException();
			}
			this.logger = logger;
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
				this.logger.Log(Strings.HybridInfoTotalCmdletTime(this.connectionType.ToString(), this.totalProcessedTime.TotalSeconds));
			}
			this.isDisposed = true;
		}

		public void Connect(PSCredential credentials, CultureInfo sessionUiCulture)
		{
			this.CheckDisposed();
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
			this.logger.Log(Strings.HybridInfoOpeningRunspace(uri.ToString()));
			this.runspace.Open();
			this.openedRunspace = true;
		}

		public void RunOneCommand(string command, Dictionary<string, object> parameters, bool ignoreNotFoundErrors)
		{
			this.RunOneCommand<object>(command, parameters, ignoreNotFoundErrors);
		}

		public IEnumerable<T> RunOneCommand<T>(string command, Dictionary<string, object> parameters, bool ignoreNotFoundErrors)
		{
			Collection<PSObject> collection = this.RunCommand(command, parameters, ignoreNotFoundErrors);
			List<T> list = new List<T>();
			if (collection.Count > 0)
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
				throw new InvalidOperationException(Strings.HybridInfoPurePSObjectsNotSupported);
			}
			return list;
		}

		public T RunOneCommandSingleResult<T>(string command, Dictionary<string, object> parameters, bool ignoreNotFoundErrors)
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
			throw new Exception(Strings.TooManyResults);
		}

		public object GetPowershellObjectValueOrNull(string command, string identity, string setting)
		{
			this.logger.Log(Strings.HybridInfoGetObjectValue(setting, identity, command));
			object result = null;
			Dictionary<string, object> parameters = new Dictionary<string, object>();
			Dictionary<string, object> powershellUntypedObjectAsMembers = this.GetPowershellUntypedObjectAsMembers(command, identity, parameters);
			if (!powershellUntypedObjectAsMembers.TryGetValue(setting, out result))
			{
				result = null;
			}
			return result;
		}

		public Dictionary<string, object> GetPowershellUntypedObjectAsMembers(string command, string identity, Dictionary<string, object> parameters)
		{
			if (!string.IsNullOrEmpty(identity))
			{
				parameters.Add("Identity", identity);
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
				throw new HybridConfigurationDetectionException(Strings.ErrorTooManyMatchingResults(identity ?? string.Empty));
			}
			return powershellUntypedObjectsAsMembers[0];
		}

		public List<Dictionary<string, object>> GetPowershellUntypedObjectsAsMembers(string command, string identity, Dictionary<string, object> parameters)
		{
			if (!string.IsNullOrEmpty(identity))
			{
				parameters.Add("Identity", identity);
			}
			Collection<PSObject> collection = this.RunCommand(command, parameters, true);
			if (collection == null || collection.Count == 0)
			{
				this.logger.Log(Strings.HybridInfoObjectNotFound);
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

		public void SetPowershellObjectProperty(string command, string identity, string property, object value)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>(2);
			if (!string.IsNullOrEmpty(identity))
			{
				dictionary.Add("Identity", identity);
			}
			dictionary.Add(property, value);
			this.RunCommand(command, dictionary, false);
		}

		public void CreatePowershellObject(string command, Dictionary<string, object> parameters)
		{
			this.RunCommand(command, parameters, false);
		}

		private Collection<PSObject> RunCommand(string cmdlet, Dictionary<string, object> parameters, bool ignoreNotFoundErrors)
		{
			this.CheckDisposed();
			Collection<PSObject> result;
			using (PowerShell powerShell = PowerShell.Create())
			{
				powerShell.Runspace = this.runspace;
				StringBuilder stringBuilder = new StringBuilder(1024);
				powerShell.AddCommand(cmdlet);
				if (parameters != null && parameters.Count<KeyValuePair<string, object>>() > 0)
				{
					powerShell.AddParameters(parameters);
					foreach (KeyValuePair<string, object> keyValuePair in parameters)
					{
						stringBuilder.Append(string.Format(" -{0} '{1}'", keyValuePair.Key, keyValuePair.Value));
					}
				}
				ExDateTime now = ExDateTime.Now;
				try
				{
					this.logger.Log(Strings.HybridInfoCmdletStart(this.connectionType.ToString(), cmdlet, stringBuilder.ToString()));
					Collection<PSObject> collection = powerShell.Invoke();
					if (powerShell.Streams.Error.Count > 0)
					{
						foreach (ErrorRecord errorRecord in powerShell.Streams.Error)
						{
							if (!errorRecord.CategoryInfo.Reason.Equals("ManagementObjectNotFoundException") || !ignoreNotFoundErrors)
							{
								this.logger.Log(Strings.ErrorWhileRunning(errorRecord.Exception.ToString()));
								throw errorRecord.Exception;
							}
						}
					}
					result = collection;
				}
				catch (Exception innerException)
				{
					Exception ex = new Exception(cmdlet, innerException);
					throw ex;
				}
				finally
				{
					TimeSpan t = ExDateTime.Now.Subtract(now);
					this.totalProcessedTime += t;
					this.logger.Log(Strings.HybridInfoCmdletEnd(this.connectionType.ToString(), cmdlet, t.TotalMilliseconds.ToString()));
				}
			}
			return result;
		}

		private void CheckDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("RemotePowershellProxy");
			}
		}

		private const string ExchangeShellUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

		private const string WSManShellUri = "http://schemas.microsoft.com/powershell/Microsoft.PowerShell";

		private const string OnPremUriFormat = "{0}{1}/powershell?serializationLevel=Full";

		private const string TenantUriFormat = "{0}{1}/powershell-liveid?serializationLevel=Full";

		private const string WSManUriFormat = "{0}{1}/wsman";

		private const string Identity = "Identity";

		private const int MaximumConnectionRedirectionCount = 5;

		private readonly string targetServer;

		private readonly PowershellConnectionType connectionType;

		private bool useSSL;

		private Runspace runspace;

		private bool openedRunspace;

		private bool isDisposed;

		private TimeSpan totalProcessedTime;

		private ILogger logger;
	}
}
