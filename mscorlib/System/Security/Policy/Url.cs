using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Util;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Url : EvidenceBase, IIdentityPermissionFactory
	{
		internal Url(string name, bool parsed)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.m_url = new URLString(name, parsed);
		}

		public Url(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.m_url = new URLString(name);
		}

		private Url(Url url)
		{
			this.m_url = url.m_url;
		}

		public string Value
		{
			get
			{
				return this.m_url.ToString();
			}
		}

		internal URLString GetURLString()
		{
			return this.m_url;
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new UrlIdentityPermission(this.m_url);
		}

		public override bool Equals(object o)
		{
			Url url = o as Url;
			return url != null && url.m_url.Equals(this.m_url);
		}

		public override int GetHashCode()
		{
			return this.m_url.GetHashCode();
		}

		public override EvidenceBase Clone()
		{
			return new Url(this);
		}

		public object Copy()
		{
			return this.Clone();
		}

		internal SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Url");
			securityElement.AddAttribute("version", "1");
			if (this.m_url != null)
			{
				securityElement.AddChild(new SecurityElement("Url", this.m_url.ToString()));
			}
			return securityElement;
		}

		public override string ToString()
		{
			return this.ToXml().ToString();
		}

		internal object Normalize()
		{
			return this.m_url.NormalizeUrl();
		}

		private URLString m_url;
	}
}
