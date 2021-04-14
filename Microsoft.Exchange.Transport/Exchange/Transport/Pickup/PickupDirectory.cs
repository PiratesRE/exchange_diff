using System;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Pickup
{
	internal sealed class PickupDirectory
	{
		public PickupDirectory(PickupType pickupType, IPickupSubmitHandler submitHandler)
		{
			this.pickupType = pickupType;
			this.submitHandler = submitHandler;
			this.eventLogger = new ExEventLog(ExTraceGlobals.PickupTracer.Category, TransportEventLog.GetEventSource());
		}

		public PickupPerfCountersInstance PickupPerformanceCounter
		{
			get
			{
				return this.pickupPerfCounterInstance;
			}
		}

		public IPickupSubmitHandler SubmitHandler
		{
			get
			{
				return this.submitHandler;
			}
		}

		public static string GetRenamedFileName(string fileName, string newExtension)
		{
			if (fileName.Length < 4)
			{
				throw new InvalidOperationException("fileName length should be > 3, but it is " + fileName.Length);
			}
			string str = newExtension;
			string str2 = fileName.Substring(0, fileName.Length - 3);
			for (int i = 0; i < 3; i++)
			{
				string text = str2 + str;
				if (!File.Exists(text))
				{
					return text;
				}
				str = DateTime.UtcNow.Ticks.ToString() + "." + newExtension;
			}
			return str2 + Guid.NewGuid().ToString() + "." + newExtension;
		}

		public static void CreateDirectory(string path, FileSystemAccessRule[] securityRules)
		{
			DirectorySecurity directorySecurity = new DirectorySecurity();
			for (int i = 0; i < securityRules.Length; i++)
			{
				directorySecurity.AddAccessRule(securityRules[i]);
			}
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				directorySecurity.SetOwner(current.User);
			}
			directorySecurity.SetAccessRuleProtection(true, false);
			Directory.CreateDirectory(path, directorySecurity);
		}

		public void Start()
		{
			lock (this.syncObj)
			{
				this.InternalStart();
			}
		}

		public void Stop()
		{
			lock (this.syncObj)
			{
				this.InternalStop();
			}
		}

		public void Reconfigure()
		{
			lock (this.syncObj)
			{
				this.InternalStop();
				this.InternalStart();
			}
		}

		private void InternalStart()
		{
			if (this.pickupPerfCounterInstance == null)
			{
				this.pickupPerfCounterInstance = PickupPerfCounters.GetInstance(this.pickupType.ToString());
			}
			LocalLongFullPath localLongFullPath = (this.pickupType == PickupType.Pickup) ? Components.Configuration.LocalServer.TransportServer.PickupDirectoryPath : Components.Configuration.LocalServer.TransportServer.ReplayDirectoryPath;
			if (null == localLongFullPath)
			{
				this.currentPath = null;
				return;
			}
			string pathName = localLongFullPath.PathName;
			bool flag = !pathName.Equals(this.currentPath, StringComparison.OrdinalIgnoreCase);
			this.currentPath = pathName;
			this.directoryPermissionOk = true;
			this.failedToCreateDirectory = false;
			if (this.CheckDirectory(pathName) && flag)
			{
				this.RenameAllTmpToEml();
			}
			if (this.pickupType == PickupType.Pickup)
			{
				if (flag)
				{
					this.WritePickupPathRegkey();
				}
				this.pickupFileMailer = new PickupFileMailer(this, pathName, this.eventLogger);
			}
			else
			{
				this.pickupFileMailer = new ReplayFileMailer(this, pathName, this.eventLogger);
			}
			this.directoryScanner = new DirectoryScanner(pathName, Components.Configuration.LocalServer.TransportServer.PickupDirectoryMaxMessagesPerMinute, "*.eml", new DirectoryScanner.FileFoundCallBack(this.pickupFileMailer.ProcessFile), new DirectoryScanner.CheckDirectoryCallBack(this.CheckDirectory));
			this.directoryScanner.Start();
		}

		private void InternalStop()
		{
			if (this.directoryScanner != null)
			{
				this.directoryScanner.Stop();
			}
		}

		private void WritePickupPathRegkey()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Pickup"))
				{
					if (registryKey.GetValue("Path") != null && registryKey.GetValueKind("Path") != RegistryValueKind.String)
					{
						registryKey.DeleteValue("Path", false);
					}
					registryKey.SetValue("Path", this.currentPath, RegistryValueKind.String);
				}
			}
			catch (SecurityException ex)
			{
				ExTraceGlobals.PickupTracer.TraceError<SecurityException, string>((long)this.GetHashCode(), "SecurityException {0} trying to add Pickup path {1} to registry.", ex, this.currentPath);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_AccessErrorModifyingPickupRegkey, null, new object[]
				{
					"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Pickup",
					ex
				});
			}
			catch (UnauthorizedAccessException ex2)
			{
				ExTraceGlobals.PickupTracer.TraceError<UnauthorizedAccessException, string>((long)this.GetHashCode(), "SecurityException {0} trying to add Pickup path {1} to registry.", ex2, this.currentPath);
				this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_AccessErrorModifyingPickupRegkey, null, new object[]
				{
					"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Pickup",
					ex2
				});
			}
		}

		private void RenameAllTmpToEml()
		{
			using (FileList fileList = new FileList(this.currentPath, "*.tmp"))
			{
				string text;
				ulong num;
				while (fileList.GetNextFile(out text, out num))
				{
					try
					{
						string renamedFileName = PickupDirectory.GetRenamedFileName(text, "eml");
						File.Move(text, renamedFileName);
					}
					catch (FileNotFoundException arg)
					{
						ExTraceGlobals.PickupTracer.TraceDebug<string, FileNotFoundException>((long)this.GetHashCode(), "Cannot find {0}, exception {1}, leaving file.", text, arg);
					}
					catch (IOException arg2)
					{
						ExTraceGlobals.PickupTracer.TraceDebug<string, IOException>((long)this.GetHashCode(), "Cannot rename {0}, exception {1}, leaving file.", text, arg2);
					}
					catch (UnauthorizedAccessException arg3)
					{
						ExTraceGlobals.PickupTracer.TraceDebug<string, UnauthorizedAccessException>((long)this.GetHashCode(), "Unauthorized to rename {0}, exception {1}", text, arg3);
						this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NoPermissionToRenamePickupFile, this.currentPath, new object[]
						{
							this.currentPath
						});
					}
				}
			}
		}

		private bool CheckDirectory(string fullDirectoryPath)
		{
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(fullDirectoryPath);
				if (!directoryInfo.Exists)
				{
					ExTraceGlobals.PickupTracer.TraceDebug<string>((long)this.GetHashCode(), "Directory {0} does not exist.", fullDirectoryPath);
					if (!this.failedToCreateDirectory)
					{
						this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_DirectoryDoesNotExist, null, new object[]
						{
							fullDirectoryPath
						});
						try
						{
							PickupDirectory.CreateDirectory(fullDirectoryPath, PickupDirectory.DirectoryAccessRules);
							this.directoryPermissionOk = true;
							return true;
						}
						catch (DirectoryNotFoundException ex)
						{
							this.failedToCreateDirectory = true;
							this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_FailedToCreatePickupDirectory, null, new object[]
							{
								fullDirectoryPath,
								ex
							});
							string notificationReason = string.Format("The Microsoft Exchange Transport service failed to create the Pickup directory: {0}. Pickup will not function until the directory is created. The detailed error is {1}.", fullDirectoryPath, ex);
							EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "FailedToCreatePickupDirectory", null, notificationReason, ResultSeverityLevel.Warning, false);
						}
						catch (UnauthorizedAccessException ex2)
						{
							this.failedToCreateDirectory = true;
							this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_FailedToCreatePickupDirectory, null, new object[]
							{
								fullDirectoryPath,
								ex2
							});
							string notificationReason2 = string.Format("The Microsoft Exchange Transport service failed to create the Pickup directory: {0}. Pickup will not function until the directory is created. The detailed error is {1}.", fullDirectoryPath, ex2);
							EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "FailedToCreatePickupDirectory", null, notificationReason2, ResultSeverityLevel.Warning, false);
						}
						catch (IOException ex3)
						{
							this.failedToCreateDirectory = true;
							this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_FailedToCreatePickupDirectory, null, new object[]
							{
								fullDirectoryPath,
								ex3
							});
							string notificationReason3 = string.Format("The Microsoft Exchange Transport service failed to create the Pickup directory: {0}. Pickup will not function until the directory is created. The detailed error is {1}.", fullDirectoryPath, ex3);
							EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "FailedToCreatePickupDirectory", null, notificationReason3, ResultSeverityLevel.Warning, false);
						}
					}
					return false;
				}
				this.failedToCreateDirectory = false;
				SecurityIdentifier securityIdentifier = null;
				using (WindowsIdentity current = WindowsIdentity.GetCurrent(false))
				{
					securityIdentifier = current.User;
				}
				if (securityIdentifier.IsWellKnown(WellKnownSidType.NetworkServiceSid))
				{
					DirectorySecurity accessControl = directoryInfo.GetAccessControl();
					FileSystemRights fileSystemRights = (FileSystemRights)0;
					FileSystemRights fileSystemRights2 = (FileSystemRights)0;
					foreach (object obj in accessControl.GetAccessRules(true, true, typeof(SecurityIdentifier)))
					{
						FileSystemAccessRule fileSystemAccessRule = (FileSystemAccessRule)obj;
						SecurityIdentifier left = fileSystemAccessRule.IdentityReference as SecurityIdentifier;
						if (left != null && left == securityIdentifier)
						{
							if (fileSystemAccessRule.AccessControlType == AccessControlType.Allow)
							{
								fileSystemRights2 |= fileSystemAccessRule.FileSystemRights;
							}
							else
							{
								fileSystemRights |= fileSystemAccessRule.FileSystemRights;
							}
						}
						ExTraceGlobals.PickupTracer.TraceDebug<string, FileSystemRights, IdentityReference>((long)this.GetHashCode(), "Rule {0} {1} access to {2}", (fileSystemAccessRule.AccessControlType == AccessControlType.Allow) ? "grants" : "denies", fileSystemAccessRule.FileSystemRights, fileSystemAccessRule.IdentityReference);
					}
					if ((fileSystemRights & (FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.ReadPermissions)) != (FileSystemRights)0 || (FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.ReadPermissions) != (fileSystemRights2 & (FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.ReadPermissions)))
					{
						if (this.directoryPermissionOk)
						{
							this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NoDirectoryPermission, null, new object[]
							{
								fullDirectoryPath
							});
						}
						this.directoryPermissionOk = false;
						return false;
					}
				}
			}
			catch (UnauthorizedAccessException arg)
			{
				ExTraceGlobals.PickupTracer.TraceDebug<UnauthorizedAccessException>((long)this.GetHashCode(), "No permission to check permissions {0}", arg);
				if (this.directoryPermissionOk)
				{
					this.eventLogger.LogEvent(TransportEventLogConstants.Tuple_NoDirectoryPermission, null, new object[]
					{
						fullDirectoryPath
					});
				}
				this.directoryPermissionOk = false;
				return false;
			}
			this.directoryPermissionOk = true;
			return true;
		}

		public const string BadMailExtension = "bad";

		public const string PickupExtension = "eml";

		public const string PickupTempExtension = "tmp";

		public const string PickupPoisonExtension = "psn";

		private const string PickupRegKeyLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Pickup";

		private const string PickupRegKeyLocationForEventLog = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Pickup";

		private const string PickupPathRegistryName = "Path";

		private static readonly FileSystemAccessRule[] DirectoryAccessRules = new FileSystemAccessRule[]
		{
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow),
			new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.AppendData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.WriteExtendedAttributes | FileSystemRights.DeleteSubdirectoriesAndFiles | FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.ReadPermissions, InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow)
		};

		private ExEventLog eventLogger;

		private PickupFileMailer pickupFileMailer;

		private bool failedToCreateDirectory;

		private bool directoryPermissionOk;

		private PickupPerfCountersInstance pickupPerfCounterInstance;

		private PickupType pickupType;

		private DirectoryScanner directoryScanner;

		private string currentPath;

		private object syncObj = new object();

		private IPickupSubmitHandler submitHandler;
	}
}
