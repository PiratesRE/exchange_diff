using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.PSDirectInvoke
{
	internal class PSLocalSessionState : ISessionState
	{
		public PSLocalSessionState()
		{
			this.Variables = new PSLocalVariableDictionary();
		}

		public string CurrentPath
		{
			get
			{
				return Environment.CurrentDirectory;
			}
		}

		public string CurrentPathProviderName
		{
			get
			{
				return "FileSystem";
			}
		}

		public IVariableDictionary Variables { get; private set; }
	}
}
