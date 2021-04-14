using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class WacDiscoveryResultFailure : WacDiscoveryResultBase
	{
		public WacDiscoveryResultFailure(WacDiscoveryFailureException ex)
		{
			this.wacDiscoveryFailedException = ex;
		}

		public override string[] WacViewableFileTypes
		{
			get
			{
				throw this.wacDiscoveryFailedException;
			}
		}

		public override string[] WacEditableFileTypes
		{
			get
			{
				throw this.wacDiscoveryFailedException;
			}
		}

		public override string GetWacViewableFileTypesDisplayText()
		{
			throw this.wacDiscoveryFailedException;
		}

		public override void AddViewMapping(string fileExtension, string path)
		{
			throw this.wacDiscoveryFailedException;
		}

		public override void AddEditMapping(string fileExtension, string path)
		{
			throw this.wacDiscoveryFailedException;
		}

		public override bool TryGetViewUrlForFileExtension(string extension, string cultureName, out string url)
		{
			throw this.wacDiscoveryFailedException;
		}

		public override bool TryGetEditUrlForFileExtension(string extension, string cultureName, out string url)
		{
			throw this.wacDiscoveryFailedException;
		}

		public override void MarkInitializationComplete()
		{
			throw this.wacDiscoveryFailedException;
		}

		private WacDiscoveryFailureException wacDiscoveryFailedException;
	}
}
