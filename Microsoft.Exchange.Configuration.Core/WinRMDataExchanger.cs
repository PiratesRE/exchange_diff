using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Exchange.Configuration.Core
{
	internal abstract class WinRMDataExchanger : IDisposable
	{
		public static string PipeName
		{
			get
			{
				if (WinRMDataExchanger.pipeName == null)
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						WinRMDataExchanger.pipeName = "M.E.C.Core.WinRMDataCommunicator.NamedPipe." + currentProcess.Id;
					}
				}
				return WinRMDataExchanger.pipeName;
			}
		}

		public string Identity
		{
			get
			{
				return this[WinRMDataExchanger.ItemKeyIdentity];
			}
			protected set
			{
				this[WinRMDataExchanger.ItemKeyIdentity] = value;
			}
		}

		public string SessionId
		{
			get
			{
				return this[WinRMDataExchanger.ItemKeySessionId];
			}
			set
			{
				this[WinRMDataExchanger.ItemKeySessionId] = value;
			}
		}

		public string RequestId
		{
			get
			{
				return this[WinRMDataExchanger.ItemKeyRequestId];
			}
			set
			{
				this[WinRMDataExchanger.ItemKeyRequestId] = value;
			}
		}

		public UserToken UserToken
		{
			get
			{
				string text = this["X-EX-UserToken"];
				if (text == null)
				{
					return null;
				}
				return UserToken.Deserialize(text);
			}
			set
			{
				this["X-EX-UserToken"] = ((value == null) ? null : value.Serialize());
			}
		}

		protected Dictionary<string, string> Items
		{
			get
			{
				return this.items;
			}
		}

		public string this[string key]
		{
			get
			{
				if (!this.items.ContainsKey(key))
				{
					throw new WinRMDataKeyNotFoundException(this.Identity, key);
				}
				return this.items[key];
			}
			set
			{
				this.items[key] = value;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		internal static readonly string ItemKeyIdentity = "Item-Identity";

		internal static readonly string ItemKeySessionId = "Item-SessionId";

		internal static readonly string ItemKeyRequestId = "Item-RequestId";

		private static string pipeName;

		private Dictionary<string, string> items = new Dictionary<string, string>();
	}
}
