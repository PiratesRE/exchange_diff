using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class NewWebAppVirtualDirectory<T> : NewExchangeVirtualDirectory<T> where T : ExchangeWebAppVirtualDirectory, new()
	{
		internal override MetabasePropertyTypes.AppPoolIdentityType AppPoolIdentityType
		{
			get
			{
				return MetabasePropertyTypes.AppPoolIdentityType.LocalSystem;
			}
		}

		protected string AppRootValue
		{
			get
			{
				return base.RetrieveVDirAppRootValue(base.GetAbsolutePath(IisUtility.AbsolutePathType.WebSiteRoot), base.Name);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			T dataObject = this.DataObject;
			dataObject.WebSite = base.WebSiteName;
		}

		protected override void WriteResultMetabaseFixup(ExchangeVirtualDirectory targetDataObject)
		{
			ExchangeWebAppVirtualDirectory target = (ExchangeWebAppVirtualDirectory)targetDataObject;
			WebAppVirtualDirectoryHelper.CopyMetabaseProperties(target, this.DataObject);
		}

		internal new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
		}

		internal void AddBinVDir(ListDictionary childVDirs)
		{
			this.AddChildVDir(childVDirs, "bin", new MetabaseProperty[]
			{
				new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.NoAccess)
			});
		}

		internal void AddChildVDir(ListDictionary childVDirs, string childFolder, IList<MetabaseProperty> vdirProperties)
		{
			TaskLogger.LogEnter(new object[]
			{
				childFolder
			});
			string path = System.IO.Path.Combine(base.Path, childFolder);
			if (!Directory.Exists(path))
			{
				this.WriteWarning(Strings.ErrorChildFolderNotExistsOnServer(path, base.OwningServer.Name));
			}
			childVDirs.Add(childFolder, vdirProperties);
			TaskLogger.LogExit();
		}
	}
}
