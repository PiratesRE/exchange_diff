using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class GacInstalled : EvidenceBase, IIdentityPermissionFactory
	{
		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new GacIdentityPermission();
		}

		public override bool Equals(object o)
		{
			return o is GacInstalled;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override EvidenceBase Clone()
		{
			return new GacInstalled();
		}

		public object Copy()
		{
			return this.Clone();
		}

		internal SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement(base.GetType().FullName);
			securityElement.AddAttribute("version", "1");
			return securityElement;
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}
	}
}
