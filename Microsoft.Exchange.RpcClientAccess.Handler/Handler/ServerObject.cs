using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ServerObject : BaseObject, IServerObject
	{
		protected ServerObject()
		{
			this.logon = null;
		}

		protected ServerObject(Logon logon)
		{
			Util.ThrowOnNullArgument(logon, "logon");
			this.logon = logon;
		}

		public Logon LogonObject
		{
			get
			{
				if (this.logon == null)
				{
					this.logon = (Logon)this;
				}
				return this.logon;
			}
		}

		internal virtual void OnAccess()
		{
		}

		public virtual Encoding String8Encoding
		{
			get
			{
				return this.LogonObject.LogonString8Encoding;
			}
		}

		private Logon logon;
	}
}
