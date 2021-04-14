using System;
using System.Collections;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.ServicesServerTasks;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "ActiveSyncVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewMobileSyncVirtualDirectory : NewExchangeVirtualDirectory<ADMobileVirtualDirectory>
	{
		public NewMobileSyncVirtualDirectory()
		{
			base.AppPoolId = "MSExchangeSyncAppPool";
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
			this.Name = "Microsoft-Server-ActiveSync";
		}

		[Parameter]
		public bool InstallProxySubDirectory
		{
			get
			{
				return !base.Fields.Contains("InstallProxySubDirectory") || (bool)base.Fields["InstallProxySubDirectory"];
			}
			set
			{
				base.Fields["InstallProxySubDirectory"] = value;
			}
		}

		internal override MetabasePropertyTypes.AppPoolIdentityType AppPoolIdentityType
		{
			get
			{
				return MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMobileSyncVirtualDirectory(base.WebSiteName.ToString(), base.Server.ToString());
			}
		}

		protected override ArrayList CustomizedVDirProperties
		{
			get
			{
				if (this.properties == null)
				{
					this.properties = new ArrayList();
					this.properties.Add(new MetabaseProperty("AppFriendlyName", this.Name));
					this.properties.Add(new MetabaseProperty("AppRoot", base.RetrieveVDirAppRootValue(base.GetAbsolutePath(IisUtility.AbsolutePathType.WebSiteRoot), this.Name)));
					this.properties.Add(new MetabaseProperty("DefaultDoc", "default.aspx"));
					this.properties.Add(new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.Basic));
					this.properties.Add(new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.Script));
					this.properties.Add(new MetabaseProperty("AppIsolated", MetabasePropertyTypes.AppIsolated.Pooled));
					this.properties.Add(new MetabaseProperty("HttpExpires", "D, 0x278d00"));
					this.properties.Add(new MetabaseProperty("DoDynamicCompression", true));
				}
				ExTraceGlobals.TaskTracer.Information<ArrayList>((long)this.GetHashCode(), "Got CustomizedVDirProperties: {0}", this.properties);
				return this.properties;
			}
		}

		private new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			IConfigurable result = base.PrepareDataObject();
			if (base.Role == VirtualDirectoryRole.ClientAccess && this.DataObject.InternalUrl == null)
			{
				this.DataObject.InternalUrl = new Uri(string.Format("https://{0}/{1}", base.OwningServer.Fqdn, "Microsoft-Server-ActiveSync"));
			}
			return result;
		}

		protected override void InternalProcessMetabase()
		{
			TaskLogger.LogEnter();
			base.InternalProcessMetabase();
			if (base.Role == VirtualDirectoryRole.Mailbox)
			{
				this.UpdateVDirScriptMaps();
				this.UpdateCompressionLevel();
			}
			if (DirectoryEntry.Exists(this.DataObject.MetabasePath))
			{
				if (base.Role != VirtualDirectoryRole.Mailbox)
				{
					try
					{
						MobileSyncVirtualDirectoryHelper.InstallIsapiFilter(this.DataObject, true);
					}
					catch (Exception ex)
					{
						TaskLogger.Trace("Exception occurred in InstallIsapiFilter(): {0}", new object[]
						{
							ex.Message
						});
						this.WriteWarning(Strings.ActiveSyncMetabaseIsapiInstallFailure);
						throw;
					}
				}
				try
				{
					if (this.InstallProxySubDirectory)
					{
						MobileSyncVirtualDirectoryHelper.InstallProxySubDirectory(this.DataObject, this);
					}
				}
				catch (Exception ex2)
				{
					TaskLogger.Trace("Exception occurred in InstallProxySubDirectory(): {0}", new object[]
					{
						ex2.Message
					});
					this.WriteWarning(Strings.ActiveSyncMetabaseProxyInstallFailure);
					throw;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResultMetabaseFixup(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			ADMobileVirtualDirectory admobileVirtualDirectory = dataObject as ADMobileVirtualDirectory;
			admobileVirtualDirectory.BasicAuthEnabled = true;
			admobileVirtualDirectory.CompressionEnabled = true;
			string metabasePath = this.DataObject.MetabasePath;
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), this.DataObject.Identity))
			{
				string virtualDirectoryName = (string)directoryEntry.Properties["AppFriendlyName"].Value;
				admobileVirtualDirectory.VirtualDirectoryName = virtualDirectoryName;
				admobileVirtualDirectory.WebSiteSSLEnabled = false;
				admobileVirtualDirectory.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Ignore);
				int? num = (int?)directoryEntry.Properties["AccessSSLFlags"].Value;
				if (num != null)
				{
					if ((num.Value & 8) == 8)
					{
						admobileVirtualDirectory.WebSiteSSLEnabled = true;
					}
					if ((num.Value & 104) == 104)
					{
						admobileVirtualDirectory.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Required);
					}
					else if ((num.Value & 32) == 32)
					{
						admobileVirtualDirectory.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Accepted);
					}
				}
				else
				{
					int startIndex = metabasePath.LastIndexOf('/');
					string iisDirectoryEntryPath = metabasePath.Remove(startIndex);
					using (DirectoryEntry directoryEntry2 = IisUtility.CreateIISDirectoryEntry(iisDirectoryEntryPath, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), this.DataObject.Identity))
					{
						num = (int?)directoryEntry2.Properties["AccessSSLFlags"].Value;
						if (num != null)
						{
							if ((num.Value & 8) == 8)
							{
								admobileVirtualDirectory.WebSiteSSLEnabled = true;
							}
							if ((num.Value & 104) == 104)
							{
								admobileVirtualDirectory.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Required);
							}
							else if ((num.Value & 32) == 32)
							{
								admobileVirtualDirectory.ClientCertAuth = new ClientCertAuthTypes?(ClientCertAuthTypes.Accepted);
							}
						}
					}
				}
			}
			char[] separator = new char[]
			{
				'/'
			};
			string[] array = metabasePath.Split(separator);
			int num2 = array.Length - 2;
			if (num2 <= 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(47);
			for (int i = 0; i < num2; i++)
			{
				stringBuilder.Append(array[i]);
				if (i < num2 - 1)
				{
					stringBuilder.Append('/');
				}
			}
			using (DirectoryEntry directoryEntry3 = IisUtility.CreateIISDirectoryEntry(stringBuilder.ToString(), new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), this.DataObject.Identity))
			{
				string websiteName = (string)directoryEntry3.Properties["ServerComment"].Value;
				admobileVirtualDirectory.WebsiteName = websiteName;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.DataObject.Identity
			});
			if (!base.Fields.IsModified("Path"))
			{
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, (base.Role == VirtualDirectoryRole.ClientAccess) ? "FrontEnd\\HttpProxy\\sync" : "ClientAccess\\sync");
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			TaskLogger.LogExit();
		}

		private void UpdateCompressionLevel()
		{
			string metabasePath = this.DataObject.MetabasePath;
			Gzip.SetIisGzipLevel(IisUtility.WebSiteFromMetabasePath(metabasePath), GzipLevel.High);
			Gzip.SetVirtualDirectoryGzipLevel(this.DataObject.MetabasePath, GzipLevel.High);
			if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
			{
				try
				{
					Gzip.SetIisGzipMimeTypes();
				}
				catch (Exception ex)
				{
					TaskLogger.Trace("Exception occurred in SetIisGzipMimeTypes(): {0}", new object[]
					{
						ex.Message
					});
					this.WriteWarning(Strings.SetIISGzipMimeTypesFailure);
					throw;
				}
			}
		}

		private void UpdateVDirScriptMaps()
		{
			TaskLogger.LogEnter();
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(this.DataObject.MetabasePath, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), this.DataObject.Identity))
			{
				string[] array = new string[directoryEntry.Properties["ScriptMaps"].Count];
				directoryEntry.Properties["ScriptMaps"].CopyTo(array, 0);
				ExTraceGlobals.TaskTracer.Information((long)this.GetHashCode(), "UpdateVDirScriptMaps got ScriptMaps property");
				int num = 0;
				string[] array2 = array;
				int i = 0;
				while (i < array2.Length)
				{
					string text = array2[i];
					if (text.StartsWith(".aspx", StringComparison.OrdinalIgnoreCase))
					{
						ExTraceGlobals.TaskTracer.Information<string>((long)this.GetHashCode(), "UpdateVDirScriptMaps found .aspx mapping: {0}", text);
						if (text.IndexOf(",options", StringComparison.OrdinalIgnoreCase) >= 0)
						{
							ExTraceGlobals.TaskTracer.Information((long)this.GetHashCode(), "Leaving UpdateVDirScriptMaps without updating .aspx mapping.");
							return;
						}
						string text2 = text + ",OPTIONS";
						directoryEntry.Properties["ScriptMaps"][num] = text2;
						ExTraceGlobals.TaskTracer.Information<string>((long)this.GetHashCode(), "UpdateVDirScriptMaps updated .aspx mapping to: {0}", text2);
						break;
					}
					else
					{
						num++;
						i++;
					}
				}
				directoryEntry.CommitChanges();
				IisUtility.CommitMetabaseChanges((base.OwningServer == null) ? null : base.OwningServer.Name);
				ExTraceGlobals.TaskTracer.Information((long)this.GetHashCode(), "UpdateVDirScriptMaps committed mapping edit to vDir object.");
			}
			TaskLogger.LogExit();
		}

		private const string InstallProxySubDirectoryKey = "InstallProxySubDirectory";

		private ArrayList properties;
	}
}
