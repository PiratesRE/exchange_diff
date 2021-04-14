using System;
using System.Collections;
using System.DirectoryServices;
using System.IO;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	internal class DeleteVirtualDirectory
	{
		public DeleteVirtualDirectory()
		{
			this.Parent = "IIS://localhost/W3SVC/1/Root";
			this.DeleteApplicationPool = false;
		}

		public void Initialize()
		{
			this.checkPoints = new Stack();
			if (this.Parent == null)
			{
				throw new ArgumentNullException(this.Parent, Strings.ErrorParentIISPathNull);
			}
			if (this.Name == null)
			{
				throw new ArgumentNullException(this.Name, Strings.ErrorVirtualDirectoryNameNull);
			}
			if (this.localPath != null && this.localPath.EndsWith("/"))
			{
				this.localPath = this.localPath.Substring(0, this.localPath.Length - 1);
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

		public virtual bool DeleteApplicationPool
		{
			get
			{
				return this.deleteApplicationPool;
			}
			set
			{
				this.deleteApplicationPool = value;
			}
		}

		private void SaveLocalPath()
		{
			DirectoryEntry directoryEntry;
			try
			{
				directoryEntry = IisUtility.FindWebObject(this.Parent, this.Name, "IIsWebVirtualDir");
			}
			catch (WebObjectNotFoundException)
			{
				return;
			}
			this.serverName = IisUtility.GetHostName(this.parent);
			if (WmiWrapper.IsDirectoryExisting(this.serverName, (string)directoryEntry.Properties["Path"].Value))
			{
				this.localPath = (string)directoryEntry.Properties["Path"].Value;
			}
		}

		private void SaveAppPoolName()
		{
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(this.Parent))
			{
				DirectoryEntry directoryEntry2 = IisUtility.FindWebDirObject(this.Parent, this.Name);
				if (!string.Equals((string)directoryEntry.Properties["AppRoot"].Value, (string)directoryEntry2.Properties["AppRoot"].Value, StringComparison.InvariantCultureIgnoreCase) && !string.Equals((string)directoryEntry.Properties["AppPoolId"].Value, (string)directoryEntry2.Properties["AppPoolId"].Value, StringComparison.InvariantCultureIgnoreCase))
				{
					this.applicationPool = (string)directoryEntry2.Properties["AppPoolId"].Value;
				}
			}
		}

		public void Execute()
		{
			this.checkPoints.Clear();
			this.SaveLocalPath();
			this.SaveAppPoolName();
			try
			{
				IisUtility.FindWebDirObject(this.Parent, this.Name);
				IisUtility.DeleteWebDirObject(this.Parent, this.Name);
				this.checkPoints.Push(DeleteVirtualDirectory.CheckPoint.VirtualDirectoryDeleted);
			}
			catch (WebObjectNotFoundException)
			{
			}
			string hostName = IisUtility.GetHostName(this.parent);
			if (this.applicationPool != null && IisUtility.IsSupportedIisVersion(hostName))
			{
				if (!this.DeleteApplicationPool)
				{
					if (!IisUtility.ApplicationPoolIsEmpty(this.applicationPool, hostName))
					{
						return;
					}
				}
				try
				{
					string appPoolRootPath = IisUtility.GetAppPoolRootPath(hostName);
					IisUtility.FindWebObject(appPoolRootPath, this.applicationPool, "IIsApplicationPool");
					IisUtility.DeleteApplicationPool(hostName, this.applicationPool);
					this.checkPoints.Push(DeleteVirtualDirectory.CheckPoint.ApplicationPoolDeleted);
				}
				catch (WebObjectNotFoundException)
				{
				}
			}
		}

		public static void DeleteFromMetabase(string webSiteRoot, string virtualDirectoryName, ICollection childVirtualDirectoryNames)
		{
			if (childVirtualDirectoryNames != null)
			{
				string text = string.Format("{0}/{1}", webSiteRoot, virtualDirectoryName);
				foreach (object obj in childVirtualDirectoryNames)
				{
					string text2 = (string)obj;
					if (IisUtility.WebDirObjectExists(text, text2))
					{
						DeleteVirtualDirectory deleteVirtualDirectory = new DeleteVirtualDirectory();
						deleteVirtualDirectory.Name = text2;
						deleteVirtualDirectory.Parent = text;
						deleteVirtualDirectory.Initialize();
						deleteVirtualDirectory.Execute();
					}
				}
			}
			if (IisUtility.Exists(webSiteRoot, virtualDirectoryName, "IIsWebVirtualDir"))
			{
				DeleteVirtualDirectory deleteVirtualDirectory2 = new DeleteVirtualDirectory();
				deleteVirtualDirectory2.Name = virtualDirectoryName;
				deleteVirtualDirectory2.Parent = webSiteRoot;
				deleteVirtualDirectory2.Initialize();
				deleteVirtualDirectory2.Execute();
			}
		}

		private Stack checkPoints;

		private string serverName;

		private string localPath;

		private string applicationPool;

		private string name;

		private string parent;

		private bool deleteApplicationPool;

		protected enum CheckPoint
		{
			VirtualDirectoryDeleted,
			ApplicationPoolDeleted
		}
	}
}
