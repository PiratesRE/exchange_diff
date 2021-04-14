using System;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Sqm
{
	public class SqmSession
	{
		static SqmSession()
		{
			ExWatson.TryRegistryKeyGetValue<string>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", "LabName", string.Empty, out SqmSession.labName);
			if (string.IsNullOrEmpty(SqmSession.labName))
			{
				SqmSession.labName = (ExWatson.MsInternal ? "MSInternal" : "Customer");
			}
		}

		public SqmSession(SqmAppID sqmAppID, SqmSession.Scope scope) : this(sqmAppID, scope, 0U, 0U, 0U, 0U)
		{
			try
			{
				Version installedVersion = ExchangeSetupContext.InstalledVersion;
				if (installedVersion != null)
				{
					this.majorVersion = (uint)installedVersion.Major;
					this.minorVersion = (uint)installedVersion.Minor;
					this.majorBuild = (uint)installedVersion.Build;
					this.minorBuild = (uint)installedVersion.Revision;
				}
			}
			catch (SetupVersionInformationCorruptException)
			{
			}
		}

		public SqmSession(SqmAppID sqmAppID, SqmSession.Scope scope, uint majorVersion, uint minorVersion) : this(sqmAppID, scope, majorVersion, minorVersion, 0U, 0U)
		{
		}

		public SqmSession(SqmAppID sqmAppID, SqmSession.Scope scope, uint majorVersion, uint minorVersion, uint majorBuild, uint minorBuild)
		{
			this.sqmAppID = sqmAppID;
			this.Name = (Enum.GetName(typeof(SqmAppID), sqmAppID) ?? ("Default" + (int)sqmAppID));
			switch (scope)
			{
			case SqmSession.Scope.AppDomain:
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					this.physicalSessionName = string.Concat(new object[]
					{
						this.Name,
						currentProcess.Id,
						"_",
						AppDomain.CurrentDomain.Id.ToString()
					});
					goto IL_F0;
				}
				break;
			case SqmSession.Scope.Process:
				break;
			default:
				goto IL_E4;
			}
			using (Process currentProcess2 = Process.GetCurrentProcess())
			{
				this.physicalSessionName = this.Name + currentProcess2.Id;
				goto IL_F0;
			}
			IL_E4:
			this.physicalSessionName = this.Name;
			IL_F0:
			this.majorVersion = majorVersion;
			this.minorVersion = minorVersion;
			this.majorBuild = majorBuild;
			this.minorBuild = minorBuild;
		}

		public uint SessionHandle { get; set; }

		public virtual uint MaximumDataFileSize
		{
			get
			{
				return 60000U;
			}
		}

		public virtual uint MaximumApproachingLimit
		{
			get
			{
				return 15000U;
			}
		}

		public string Name { get; private set; }

		public string DataFileDirectory
		{
			get
			{
				if (string.IsNullOrEmpty(this.dataFileDirectory))
				{
					string text = null;
					string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Exchange Server\\v15");
					try
					{
						Directory.CreateDirectory(path);
						this.dataFileDirectory = path;
					}
					catch (IOException ex)
					{
						text = ex.Message;
					}
					catch (UnauthorizedAccessException ex2)
					{
						text = ex2.Message;
					}
					if (!string.IsNullOrEmpty(text))
					{
						ExTraceGlobals.SqmTracer.TraceDebug<string>(60751, 0L, "Failed to create data file directory for SQM session. Error: {0}", text);
					}
					if (string.IsNullOrEmpty(this.dataFileDirectory))
					{
						this.Enabled = false;
					}
				}
				return this.dataFileDirectory;
			}
		}

		public bool Enabled
		{
			get
			{
				return this.IsOpened && SqmLibWrap.SqmGetEnabled(this.SessionHandle);
			}
			set
			{
				if (this.IsOpened)
				{
					SqmLibWrap.SqmSetEnabled(this.SessionHandle, value);
				}
			}
		}

		public bool IsOpened
		{
			get
			{
				return this.SessionHandle != 0U;
			}
		}

		internal uint SessionSize
		{
			get
			{
				return this.sessionSize;
			}
			set
			{
				this.sessionSize = value;
			}
		}

		protected uint MajorVersion
		{
			get
			{
				return this.majorVersion;
			}
			set
			{
				this.majorVersion = value;
			}
		}

		protected uint MinorVersion
		{
			get
			{
				return this.minorVersion;
			}
			set
			{
				this.minorVersion = value;
			}
		}

		private string LdapHostName
		{
			get
			{
				if (this.ldapHostName == null)
				{
					string text = null;
					try
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange"))
						{
							if (registryKey != null)
							{
								object value = registryKey.GetValue("LdapPort");
								if (value != null)
								{
									this.ldapHostName = Environment.MachineName + ":" + value.ToString() + "/";
								}
							}
							else
							{
								this.ldapHostName = string.Empty;
							}
						}
					}
					catch (SecurityException ex)
					{
						text = ex.Message;
					}
					catch (UnauthorizedAccessException ex2)
					{
						text = ex2.Message;
					}
					if (!string.IsNullOrEmpty(text))
					{
						ExTraceGlobals.SqmTracer.TraceDebug<string>(44367, 0L, "Failed to get Ldap host name. Error: {0}", text);
					}
				}
				return this.ldapHostName;
			}
		}

		public void Open()
		{
			if (!this.IsOpened)
			{
				this.SessionHandle = SqmLibWrap.SqmGetSession(this.physicalSessionName, this.MaximumDataFileSize, 0U);
				if (!this.IsOpened)
				{
					this.SessionHandle = SqmLibWrap.SqmGetSession(this.physicalSessionName, this.MaximumDataFileSize, 1U);
					if (this.IsOpened)
					{
						this.OnOpen();
						this.OnCreate();
					}
				}
				else
				{
					if (!this.Enabled)
					{
						ExTraceGlobals.SqmTracer.TraceDebug(60559, 0L, "Calling SqmStartSession from Open");
						SqmLibWrap.SqmStartSession(this.SessionHandle);
						this.Enabled = this.GetOptInStatus();
					}
					this.OnOpen();
				}
				this.SetCommonDataPoints();
			}
		}

		public void Close()
		{
			if (this.IsOpened)
			{
				this.OnClosing();
				this.UpdateData(true, true);
				SqmLibWrap.SqmWaitForUploadComplete(0U, 2U);
				this.SessionHandle = 0U;
				if (this.dataWriteMutex != null)
				{
					this.dataWriteMutex.Close();
				}
			}
		}

		public void SetDataPoint(SqmDataID dataID, int value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSet(this.SessionHandle, (uint)dataID, (uint)value);
				return 4U;
			});
		}

		public void SetDataPoint(SqmDataID dataID, uint value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSet(this.SessionHandle, (uint)dataID, value);
				return 4U;
			});
		}

		public void SetDataPoint(SqmDataID dataID, bool value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSetBool(this.SessionHandle, (uint)dataID, value ? 1U : 0U);
				return 4U;
			});
		}

		public void SetDataPoint(SqmDataID dataID, string value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSetString(this.SessionHandle, (uint)dataID, value);
				return (uint)Encoding.Unicode.GetByteCount(value);
			});
		}

		public void SetBitsDataPoint(SqmDataID dataID, uint value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSetBits(this.SessionHandle, (uint)dataID, value);
				return 4U;
			});
		}

		public void SetIfMaxDataPoint(SqmDataID dataID, uint value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSetIfMax(this.SessionHandle, (uint)dataID, value);
				return 4U;
			});
		}

		public void SetIfMinDataPoint(SqmDataID dataID, uint value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmSetIfMin(this.SessionHandle, (uint)dataID, value);
				return 4U;
			});
		}

		public void IncrementDataPoint(SqmDataID dataID, uint value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmIncrement(this.SessionHandle, (uint)dataID, value);
				return 4U;
			});
		}

		public void AddToAverageDataPoint(SqmDataID dataID, uint value)
		{
			this.InternalSetDataPoint(delegate
			{
				SqmLibWrap.SqmAddToAverage(this.SessionHandle, (uint)dataID, value);
				return 4U;
			});
		}

		public void AddToStreamDataPoint(SqmDataID dataID, params object[] list)
		{
			this.InternalSetDataPoint(delegate
			{
				uint num = 0U;
				foreach (object obj in list)
				{
					if (obj is bool)
					{
						SqmLibWrap.SqmAddToStreamDWord(this.SessionHandle, (uint)dataID, (uint)list.Length, ((bool)obj) ? 1U : 0U);
						num += 4U;
					}
					else if (obj is int)
					{
						SqmLibWrap.SqmAddToStreamDWord(this.SessionHandle, (uint)dataID, (uint)list.Length, (uint)((int)obj));
						num += 4U;
					}
					else if (obj is uint)
					{
						SqmLibWrap.SqmAddToStreamDWord(this.SessionHandle, (uint)dataID, (uint)list.Length, (uint)obj);
						num += 4U;
					}
					else if (obj is string)
					{
						SqmLibWrap.SqmAddToStreamString(this.SessionHandle, (uint)dataID, (uint)list.Length, (string)obj);
						num += (uint)Encoding.Unicode.GetByteCount((string)obj);
					}
					else
					{
						ExTraceGlobals.SqmTracer.TraceDebug<string>(62799, 0L, "Unexpected object type: {0}", obj.GetType().Name);
					}
				}
				return num;
			});
		}

		protected virtual void OnOpen()
		{
			this.dataWriteMutex = new Mutex(false, this.physicalSessionName);
		}

		protected virtual void OnCreate()
		{
			ExTraceGlobals.SqmTracer.TraceDebug(35983, 0L, "Calling SqmStartSession from OnCreate");
			SqmLibWrap.SqmStartSession(this.SessionHandle);
			this.UpdateData(false, false);
			Guid guid;
			if (!SqmLibWrap.SqmReadSharedMachineId(out guid))
			{
				SqmLibWrap.SqmCreateNewId(out guid);
				SqmLibWrap.SqmWriteSharedMachineId(ref guid);
				SqmLibWrap.SqmReadSharedMachineId(out guid);
			}
			SqmLibWrap.SqmSetMachineId(this.SessionHandle, ref guid);
			Guid guid2;
			if (!SqmLibWrap.SqmReadSharedUserId(out guid2))
			{
				SqmLibWrap.SqmCreateNewId(out guid2);
				SqmLibWrap.SqmWriteSharedUserId(ref guid2);
				SqmLibWrap.SqmReadSharedUserId(out guid2);
			}
			SqmLibWrap.SqmSetUserId(this.SessionHandle, ref guid2);
			SqmLibWrap.SqmSetAppId(this.SessionHandle, (uint)this.sqmAppID);
			SqmLibWrap.SqmSetAppVersion(this.SessionHandle, this.majorVersion, this.minorVersion);
		}

		protected virtual void OnClosing()
		{
		}

		protected virtual bool GetOptInStatus()
		{
			if (this.LdapHostName == null)
			{
				return false;
			}
			bool result = false;
			string text = null;
			try
			{
				string arg;
				using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}RootDse", this.LdapHostName)))
				{
					arg = (string)directoryEntry.Properties["configurationNamingContext"].Value;
				}
				this.configNCUri = string.Format("LDAP://{0}{1}", this.LdapHostName, arg);
				using (DirectorySearcher directorySearcher = new DirectorySearcher(new DirectoryEntry(this.configNCUri)))
				{
					string machineName = Environment.MachineName;
					if (machineName != null)
					{
						directorySearcher.Filter = string.Format("(&(objectClass=msExchExchangeServer)(cn={0}))", machineName);
						directorySearcher.PropertiesToLoad.Clear();
						directorySearcher.PropertiesToLoad.Add("msExchCustomerFeedbackEnabled");
						SearchResult searchResult = directorySearcher.FindOne();
						if (searchResult != null)
						{
							ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["msExchCustomerFeedbackEnabled"];
							if (resultPropertyValueCollection != null && resultPropertyValueCollection.Count > 0 && resultPropertyValueCollection[0] is bool)
							{
								result = (bool)resultPropertyValueCollection[0];
							}
						}
					}
				}
			}
			catch (COMException ex)
			{
				text = ex.Message;
			}
			catch (ActiveDirectoryObjectNotFoundException ex2)
			{
				text = ex2.Message;
			}
			catch (ActiveDirectoryOperationException ex3)
			{
				text = ex3.Message;
			}
			catch (ActiveDirectoryServerDownException ex4)
			{
				text = ex4.Message;
			}
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.SqmTracer.TraceDebug<string>(52559, 0L, "Failed to get opt-in status. Error: {0}", text);
			}
			return result;
		}

		private static bool UploadCallback(uint hr, string filePath, uint httpResponse)
		{
			ExTraceGlobals.SqmTracer.TraceDebug<string, uint, uint>(36175, 0L, "Cmdlet SQM infra: Data file {0} was uploaded with result {1} and http response {2}", filePath, hr, httpResponse);
			return httpResponse == 200U;
		}

		private void UpdateData(bool flushToDisk, bool closing)
		{
			try
			{
				this.dataWriteMutex.WaitOne();
				if (this.IsOpened && !string.IsNullOrEmpty(this.DataFileDirectory))
				{
					if (flushToDisk && this.SessionSize > 0U)
					{
						ExTraceGlobals.SqmTracer.TraceDebug(46223, 0L, "Calling SqmEndSession");
						SqmLibWrap.SqmEndSession(this.SessionHandle, Path.Combine(this.DataFileDirectory, this.physicalSessionName + "%02d.sqm"), 10U, 2U);
						if (!closing)
						{
							ExTraceGlobals.SqmTracer.TraceDebug(52367, 0L, "Calling SqmStartSession from UpdateData");
							SqmLibWrap.SqmStartSession(this.SessionHandle);
							this.SessionSize = 0U;
							this.SetCommonDataPoints();
						}
					}
					if (!closing)
					{
						this.Enabled = this.GetOptInStatus();
						if (this.Enabled)
						{
							ExTraceGlobals.SqmTracer.TraceDebug(62607, 0L, "Calling SqmStartUpload");
							SqmLibWrap.SqmStartUpload(Path.Combine(this.DataFileDirectory, this.Name + "*.sqm"), null, "https://sqm.microsoft.com/sqm/exchange/sqmserver.dll", 6U, SqmSession.uploadCallback);
						}
					}
				}
			}
			finally
			{
				this.dataWriteMutex.ReleaseMutex();
			}
		}

		private Guid GetOrganization()
		{
			if (this.organizationGuid != Guid.Empty)
			{
				return this.organizationGuid;
			}
			if (this.LdapHostName == null)
			{
				return Guid.Empty;
			}
			string text = null;
			try
			{
				string arg;
				using (DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://{0}RootDse", this.LdapHostName)))
				{
					arg = (string)directoryEntry.Properties["configurationNamingContext"].Value;
				}
				this.configNCUri = string.Format("LDAP://{0}{1}", this.LdapHostName, arg);
				using (DirectorySearcher directorySearcher = new DirectorySearcher(new DirectoryEntry(this.configNCUri)))
				{
					directorySearcher.Filter = string.Format("(objectClass=msExchOrganizationContainer)", new object[0]);
					directorySearcher.PropertiesToLoad.Clear();
					directorySearcher.PropertiesToLoad.Add("objectGUID");
					SearchResult searchResult = directorySearcher.FindOne();
					if (searchResult != null)
					{
						ResultPropertyValueCollection resultPropertyValueCollection = searchResult.Properties["objectGUID"];
						if (resultPropertyValueCollection != null && resultPropertyValueCollection.Count > 0)
						{
							this.organizationGuid = new Guid((byte[])resultPropertyValueCollection[0]);
						}
					}
				}
			}
			catch (COMException ex)
			{
				text = ex.Message;
			}
			catch (ActiveDirectoryObjectNotFoundException ex2)
			{
				text = ex2.Message;
			}
			catch (ActiveDirectoryOperationException ex3)
			{
				text = ex3.Message;
			}
			catch (ActiveDirectoryServerDownException ex4)
			{
				text = ex4.Message;
			}
			catch (FormatException ex5)
			{
				text = ex5.Message;
			}
			catch (OverflowException ex6)
			{
				text = ex6.Message;
			}
			if (!string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.SqmTracer.TraceDebug<string>(46415, 0L, "Failed to get the organization GUID. Error: {0}", text);
			}
			return this.organizationGuid;
		}

		private void SetOrganizationDataPoint()
		{
			Guid organization = this.GetOrganization();
			if (organization != Guid.Empty)
			{
				this.SetDataPoint(SqmDataID.COMMON_DW_ORGID, organization.GetHashCode());
			}
		}

		private void SetVersionDataPoints()
		{
			this.SetDataPoint(SqmDataID.CMN_DYN_MAJORBUILD, this.majorBuild.ToString());
			this.SetDataPoint(SqmDataID.CMN_DYN_MINORBUILD, this.minorBuild.ToString());
		}

		private void SetDeploymentTypeDataPoint()
		{
			this.SetDataPoint(SqmDataID.CMN_DYN_EXDEPLOYMENTTYPE, Datacenter.GetExchangeSku().ToString());
		}

		private void SetLabNameDataPoint()
		{
			this.SetDataPoint(SqmDataID.CMN_DYN_LABNAME, SqmSession.labName);
		}

		private void SetCommonDataPoints()
		{
			this.SetOrganizationDataPoint();
			this.SetVersionDataPoints();
			this.SetDeploymentTypeDataPoint();
			this.SetLabNameDataPoint();
		}

		private void InternalSetDataPoint(SqmSession.SetDataPointDelegate setDataPoint)
		{
			try
			{
				this.dataWriteMutex.WaitOne();
				if (this.Enabled)
				{
					uint size = setDataPoint();
					this.AddSessionSize(size);
				}
			}
			finally
			{
				this.dataWriteMutex.ReleaseMutex();
			}
		}

		private void AddSessionSize(uint size)
		{
			this.SessionSize += size;
			if (this.SessionSize >= this.MaximumApproachingLimit)
			{
				ExTraceGlobals.SqmTracer.TraceDebug(38223, 0L, "Session size exceeds MaximumApproachingLimit, uploading data and starting a new session.");
				this.UpdateData(true, false);
			}
		}

		private const uint DefaultMaximumDataFileSize = 60000U;

		private const uint DefaultMaximumApproachingLimit = 15000U;

		private const string SqmUploadUrl = "https://sqm.microsoft.com/sqm/exchange/sqmserver.dll";

		private const string DefaultSessionName = "Default";

		private static SqmLibWrap.SqmUploadCallback uploadCallback = new SqmLibWrap.SqmUploadCallback(SqmSession.UploadCallback);

		private static string labName = string.Empty;

		private SqmAppID sqmAppID;

		private uint sessionSize;

		private uint majorVersion;

		private uint minorVersion;

		private uint majorBuild;

		private uint minorBuild;

		private Mutex dataWriteMutex;

		private string physicalSessionName;

		private Guid organizationGuid = Guid.Empty;

		private string ldapHostName;

		private string configNCUri;

		private string dataFileDirectory;

		private delegate uint SetDataPointDelegate();

		public enum Scope
		{
			AppDomain,
			Process,
			System
		}
	}
}
