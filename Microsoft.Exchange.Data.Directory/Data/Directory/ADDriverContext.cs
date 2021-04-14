using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADDriverContext
	{
		internal ADDriverContext(ADServerSettings serverSettings, ContextMode mode)
		{
			this.serverSettings = serverSettings;
			this.mode = mode;
		}

		internal ADServerSettings ServerSettings
		{
			get
			{
				return this.serverSettings;
			}
		}

		internal ContextMode Mode
		{
			get
			{
				return this.mode;
			}
		}

		private ADServerSettings serverSettings;

		private ContextMode mode;
	}
}
