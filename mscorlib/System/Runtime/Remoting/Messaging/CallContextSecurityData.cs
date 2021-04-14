using System;
using System.Security.Principal;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class CallContextSecurityData : ICloneable
	{
		internal IPrincipal Principal
		{
			get
			{
				return this._principal;
			}
			set
			{
				this._principal = value;
			}
		}

		internal bool HasInfo
		{
			get
			{
				return this._principal != null;
			}
		}

		public object Clone()
		{
			return new CallContextSecurityData
			{
				_principal = this._principal
			};
		}

		private IPrincipal _principal;
	}
}
