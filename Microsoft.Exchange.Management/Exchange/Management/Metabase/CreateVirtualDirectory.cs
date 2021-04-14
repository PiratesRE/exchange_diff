using System;
using System.Collections;
using System.DirectoryServices;
using System.IO;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.Metabase
{
	internal class CreateVirtualDirectory
	{
		public CreateVirtualDirectory()
		{
			this.Parent = "IIS://localhost/W3SVC/1/Root";
		}

		public void Initialize()
		{
			if (this.Parent == null)
			{
				throw new ArgumentNullException(this.Parent, Strings.ErrorParentIISPathNull);
			}
			if (this.Name == null)
			{
				throw new ArgumentNullException(this.Name, Strings.ErrorVirtualDirectoryNameNull);
			}
			if (this.LocalPath != null && this.LocalPath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
			{
				this.LocalPath = null;
			}
			if (this.Parent != null && this.Parent.Trim().EndsWith("/"))
			{
				this.Parent = this.Parent.Substring(0, this.Parent.Length - 1);
			}
			if (this.Name.IndexOfAny(IisUtility.GetReservedUriCharacters()) >= 0 || this.Name.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
			{
				throw new ArgumentException(Strings.ErrorInvalidCharacterInVirtualDirectoryName(this.Name));
			}
		}

		public virtual string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		public string LocalPath
		{
			get
			{
				return this.localPath;
			}
			set
			{
				this.localPath = value;
			}
		}

		public string ApplicationPool
		{
			get
			{
				return this.applicationPool;
			}
			set
			{
				this.applicationPool = value;
			}
		}

		public MetabasePropertyTypes.AppPoolIdentityType AppPoolIdentityType
		{
			get
			{
				return this.appPoolIdentityType;
			}
			set
			{
				this.appPoolIdentityType = value;
			}
		}

		public MetabasePropertyTypes.ManagedPipelineMode AppPoolManagedPipelineMode
		{
			get
			{
				return this.appPoolManagedPipelineMode;
			}
			set
			{
				this.appPoolManagedPipelineMode = value;
			}
		}

		public int AppPoolQueueLength
		{
			get
			{
				return this.appPoolQueueLength;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("application pool cannot be 0 or negative.");
				}
				this.appPoolQueueLength = value;
			}
		}

		public long MaximumMemory { get; set; }

		public long MaximumPrivateMemory { get; set; }

		public ICollection CustomizedVDirProperties
		{
			get
			{
				return this.customizedVDirProperties;
			}
			set
			{
				this.customizedVDirProperties = value;
			}
		}

		public void Execute()
		{
			DirectoryEntry directoryEntry = IisUtility.CreateWebDirObject(this.Parent, this.LocalPath, this.Name);
			if (this.CustomizedVDirProperties != null)
			{
				IisUtility.SetProperties(directoryEntry, this.CustomizedVDirProperties);
			}
			directoryEntry.CommitChanges();
			string hostName = IisUtility.GetHostName(this.Parent);
			IisUtility.CommitMetabaseChanges(hostName);
			if (this.ApplicationPool != null && IisUtility.IsSupportedIisVersion(hostName))
			{
				string appPoolRootPath = IisUtility.GetAppPoolRootPath(hostName);
				if (!IisUtility.Exists(appPoolRootPath, this.ApplicationPool, "IIsApplicationPool"))
				{
					using (DirectoryEntry directoryEntry2 = IisUtility.CreateApplicationPool(hostName, this.ApplicationPool))
					{
						IisUtility.SetProperty(directoryEntry2, "AppPoolIdentityType", (int)this.AppPoolIdentityType, true);
						IisUtility.SetProperty(directoryEntry2, "managedPipelineMode", (int)this.AppPoolManagedPipelineMode, true);
						if (this.AppPoolQueueLength != 0)
						{
							IisUtility.SetProperty(directoryEntry2, "AppPoolQueueLength", this.AppPoolQueueLength, true);
						}
						directoryEntry2.CommitChanges();
						IisUtility.CommitMetabaseChanges(hostName);
						using (ServerManager serverManager = new ServerManager())
						{
							ApplicationPool applicationPool = serverManager.ApplicationPools[this.ApplicationPool];
							applicationPool.ProcessModel.LoadUserProfile = true;
							if (this.MaximumMemory != 0L)
							{
								applicationPool.Recycling.PeriodicRestart.Memory = this.MaximumMemory;
							}
							if (this.MaximumPrivateMemory != 0L)
							{
								applicationPool.Recycling.PeriodicRestart.PrivateMemory = this.MaximumPrivateMemory;
							}
							serverManager.CommitChanges();
						}
					}
				}
				IisUtility.AssignApplicationPool(directoryEntry, this.ApplicationPool);
			}
		}

		private string name;

		private string parent;

		private string localPath;

		private string applicationPool;

		private MetabasePropertyTypes.AppPoolIdentityType appPoolIdentityType = MetabasePropertyTypes.AppPoolIdentityType.NetworkService;

		private MetabasePropertyTypes.ManagedPipelineMode appPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Classic;

		private int appPoolQueueLength;

		private ICollection customizedVDirProperties;
	}
}
