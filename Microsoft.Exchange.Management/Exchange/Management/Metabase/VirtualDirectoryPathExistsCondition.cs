using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.Management.Metabase
{
	[Serializable]
	internal sealed class VirtualDirectoryPathExistsCondition : Condition
	{
		public VirtualDirectoryPathExistsCondition(string serverName, string path)
		{
			this.ServerName = serverName;
			this.Path = path;
		}

		public string ServerName
		{
			get
			{
				return this.serverName;
			}
			set
			{
				this.serverName = value;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		public override bool Verify()
		{
			return (this.Path.Length >= "\\\\.\\".Length && this.Path.StartsWith("\\\\.\\")) || WmiWrapper.IsDirectoryExisting(this.ServerName, this.Path);
		}

		private const string uncPrefix = "\\\\.\\";

		private string serverName;

		private string path;
	}
}
