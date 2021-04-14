using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public abstract class WacDiscoveryResultBase
	{
		public abstract string[] WacViewableFileTypes { get; }

		public abstract string[] WacEditableFileTypes { get; }

		public abstract string GetWacViewableFileTypesDisplayText();

		public abstract void AddViewMapping(string fileExtension, string path);

		public abstract void AddEditMapping(string fileExtension, string path);

		public abstract bool TryGetViewUrlForFileExtension(string extension, string cultureName, out string url);

		public abstract bool TryGetEditUrlForFileExtension(string extension, string cultureName, out string url);

		public abstract void MarkInitializationComplete();
	}
}
