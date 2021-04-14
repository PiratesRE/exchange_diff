using System;
using System.Collections;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class SetExchangeVirtualDirectory<T> : SetVirtualDirectory<T> where T : ExchangeVirtualDirectory, new()
	{
		[Parameter(Mandatory = false)]
		public ExtendedProtectionTokenCheckingMode ExtendedProtectionTokenChecking
		{
			get
			{
				return (ExtendedProtectionTokenCheckingMode)(base.Fields["ExtendedProtectionTokenChecking"] ?? ExtendedProtectionTokenCheckingMode.None);
			}
			set
			{
				base.Fields["ExtendedProtectionTokenChecking"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<ExtendedProtectionFlag> ExtendedProtectionFlags
		{
			get
			{
				return ExchangeVirtualDirectory.ExtendedProtectionFlagsToMultiValuedProperty((ExtendedProtectionFlag)base.Fields["ExtendedProtectionFlags"]);
			}
			set
			{
				base.Fields["ExtendedProtectionFlags"] = (int)ExchangeVirtualDirectory.ExtendedProtectionMultiValuedPropertyToFlags(value);
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ExtendedProtectionSPNList
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ExtendedProtectionSPNList"];
			}
			set
			{
				base.Fields["ExtendedProtectionSPNList"] = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			T dataObject = this.DataObject;
			string hostName = IisUtility.GetHostName(dataObject.MetabasePath);
			try
			{
				if (!new IisVersionValidCondition(hostName).Verify())
				{
					Exception exception = new ArgumentException(Strings.ErrorIisVersionIsInvalid(hostName), "Server");
					ErrorCategory category = ErrorCategory.InvalidArgument;
					T dataObject2 = this.DataObject;
					base.WriteError(exception, category, dataObject2.Identity);
					return;
				}
			}
			catch (IOException innerException)
			{
				Exception exception2 = new ArgumentException(Strings.ErrorCannotDetermineIisVersion(hostName), "Server", innerException);
				ErrorCategory category2 = ErrorCategory.InvalidArgument;
				T dataObject3 = this.DataObject;
				base.WriteError(exception2, category2, dataObject3.Identity);
			}
			catch (InvalidOperationException innerException2)
			{
				Exception exception3 = new ArgumentException(Strings.ErrorCannotDetermineIisVersion(hostName), "Server", innerException2);
				ErrorCategory category3 = ErrorCategory.InvalidArgument;
				T dataObject4 = this.DataObject;
				base.WriteError(exception3, category3, dataObject4.Identity);
			}
			ExtendedProtection.Validate(this, this.DataObject);
			base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.DataObject, true, new DataAccessTask<T>.ADObjectOutOfScopeString(Strings.ErrorCannotSetVirtualDirectoryOutOfWriteScope));
			TaskLogger.LogExit();
		}

		internal void CommitMetabaseValues(ExchangeVirtualDirectory dataObject, ArrayList MetabasePropertiesToChange)
		{
			if (MetabasePropertiesToChange != null)
			{
				string metabasePath = dataObject.MetabasePath;
				Task.TaskErrorLoggingReThrowDelegate writeError = new Task.TaskErrorLoggingReThrowDelegate(this.WriteError);
				T dataObject2 = this.DataObject;
				using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(metabasePath, writeError, dataObject2.Identity))
				{
					IisUtility.SetProperties(directoryEntry, MetabasePropertiesToChange);
					directoryEntry.CommitChanges();
					IisUtility.CommitMetabaseChanges((dataObject.Server == null) ? null : dataObject.Server.Name);
				}
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ExchangeVirtualDirectory exchangeVirtualDirectory = (ExchangeVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields.Contains("ExtendedProtectionTokenChecking"))
			{
				exchangeVirtualDirectory.ExtendedProtectionTokenChecking = (ExtendedProtectionTokenCheckingMode)base.Fields["ExtendedProtectionTokenChecking"];
			}
			if (base.Fields.Contains("ExtendedProtectionSPNList"))
			{
				exchangeVirtualDirectory.ExtendedProtectionSPNList = (MultiValuedProperty<string>)base.Fields["ExtendedProtectionSPNList"];
			}
			if (base.Fields.Contains("ExtendedProtectionFlags"))
			{
				ExtendedProtectionFlag flags = (ExtendedProtectionFlag)base.Fields["ExtendedProtectionFlags"];
				exchangeVirtualDirectory.ExtendedProtectionFlags = ExchangeVirtualDirectory.ExtendedProtectionFlagsToMultiValuedProperty(flags);
			}
			return exchangeVirtualDirectory;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			ExtendedProtection.CommitToMetabase(this.DataObject, this);
			TaskLogger.LogExit();
		}
	}
}
