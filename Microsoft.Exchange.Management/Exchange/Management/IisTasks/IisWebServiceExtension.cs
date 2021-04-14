using System;

namespace Microsoft.Exchange.Management.IisTasks
{
	public struct IisWebServiceExtension
	{
		internal IisWebServiceExtension(string executableName, string relativePath, bool allow, bool uiDeletable)
		{
			this.ExecutableName = executableName;
			this.RelativePath = relativePath;
			this.Allow = allow;
			this.UiDeletable = uiDeletable;
		}

		internal string ExecutableName;

		internal string RelativePath;

		internal bool Allow;

		internal bool UiDeletable;
	}
}
