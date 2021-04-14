using System;

namespace System.Runtime.Remoting
{
	internal class RemoteAppEntry
	{
		internal RemoteAppEntry(string appName, string appURI)
		{
			this._remoteAppName = appName;
			this._remoteAppURI = appURI;
		}

		internal string GetAppURI()
		{
			return this._remoteAppURI;
		}

		private string _remoteAppName;

		private string _remoteAppURI;
	}
}
