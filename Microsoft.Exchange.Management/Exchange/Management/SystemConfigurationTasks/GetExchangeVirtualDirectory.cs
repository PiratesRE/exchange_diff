using System;
using System.Collections;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetExchangeVirtualDirectory<T> : GetVirtualDirectory<T> where T : ExchangeVirtualDirectory, new()
	{
		[Parameter]
		public SwitchParameter ADPropertiesOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ADPropertiesOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ADPropertiesOnly"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ShowMailboxVirtualDirectories
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowMailboxVirtualDirectories"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowMailboxVirtualDirectories"] = value;
			}
		}

		[Parameter]
		public SwitchParameter ShowBackEndVirtualDirectories
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowBackEndVirtualDirectories"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowBackEndVirtualDirectories"] = value;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ExchangeVirtualDirectory exchangeVirtualDirectory = (ExchangeVirtualDirectory)dataObject;
			if (!this.FilterBackendVdir(exchangeVirtualDirectory))
			{
				exchangeVirtualDirectory.ADPropertiesOnly = this.ADPropertiesOnly;
				if (!exchangeVirtualDirectory.IsReadOnly && !this.ADPropertiesOnly)
				{
					bool flag = true;
					try
					{
						using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(exchangeVirtualDirectory.MetabasePath, new Task.TaskErrorLoggingReThrowDelegate(this.WriteError), dataObject.Identity, false))
						{
							if (directoryEntry != null)
							{
								this.MetabaseProperties = IisUtility.GetProperties(directoryEntry);
								this.ProcessMetabaseProperties(exchangeVirtualDirectory);
								flag = false;
							}
						}
					}
					catch (IISNotInstalledException exception)
					{
						this.WriteError(exception, ErrorCategory.ReadError, null, false);
					}
					catch (IISNotReachableException exception2)
					{
						this.WriteError(exception2, ErrorCategory.ReadError, null, false);
					}
					catch (IISGeneralCOMException ex)
					{
						if (ex.Code == -2147023174)
						{
							this.WriteError(new IISNotReachableException(IisUtility.GetHostName(exchangeVirtualDirectory.MetabasePath), ex.Message), ErrorCategory.ResourceUnavailable, null, false);
						}
						else
						{
							if (ex.Code != -2147024893)
							{
								throw;
							}
							if (!this.CanIgnoreMissingMetabaseEntry())
							{
								this.WriteError(ex, ErrorCategory.ReadError, null, false);
							}
							else
							{
								this.WriteWarning(this.GetMissingMetabaseEntryWarning(exchangeVirtualDirectory));
								flag = false;
							}
						}
					}
					catch (UnauthorizedAccessException exception3)
					{
						this.WriteError(exception3, ErrorCategory.PermissionDenied, null, false);
					}
					if (flag)
					{
						return;
					}
				}
				base.WriteResult(dataObject);
			}
		}

		protected virtual void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			dataObject.Path = (string)IisUtility.GetIisPropertyValue("Path", this.MetabaseProperties);
			ExtendedProtection.LoadFromMetabase(dataObject, this);
		}

		protected virtual bool CanIgnoreMissingMetabaseEntry()
		{
			return false;
		}

		protected virtual LocalizedString GetMissingMetabaseEntryWarning(ExchangeVirtualDirectory vdir)
		{
			return Strings.ErrorObjectNotFound(vdir.MetabasePath);
		}

		protected bool FilterBackendVdir(ExchangeVirtualDirectory vdir)
		{
			if (this.ShowMailboxVirtualDirectories || this.ShowBackEndVirtualDirectories)
			{
				return false;
			}
			if (ExchangeServiceVDirHelper.IsBackEndVirtualDirectory(vdir))
			{
				return true;
			}
			Server server = (Server)base.GetDataObject<Server>(new ServerIdParameter(vdir.Server), base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(vdir.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(vdir.Server.ToString())));
			return server.IsE15OrLater && !server.IsCafeServer && server.IsMailboxServer;
		}

		private ICollection MetabaseProperties;
	}
}
