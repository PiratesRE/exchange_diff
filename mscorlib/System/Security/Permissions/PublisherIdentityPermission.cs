using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Util;

namespace System.Security.Permissions
{
	[ComVisible(true)]
	[Serializable]
	public sealed class PublisherIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		public PublisherIdentityPermission(PermissionState state)
		{
			if (state == PermissionState.Unrestricted)
			{
				this.m_unrestricted = true;
				return;
			}
			if (state == PermissionState.None)
			{
				this.m_unrestricted = false;
				return;
			}
			throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPermissionState"));
		}

		public PublisherIdentityPermission(X509Certificate certificate)
		{
			this.Certificate = certificate;
		}

		public X509Certificate Certificate
		{
			get
			{
				if (this.m_certs == null || this.m_certs.Length < 1)
				{
					return null;
				}
				if (this.m_certs.Length > 1)
				{
					throw new NotSupportedException(Environment.GetResourceString("NotSupported_AmbiguousIdentity"));
				}
				if (this.m_certs[0] == null)
				{
					return null;
				}
				return new X509Certificate(this.m_certs[0]);
			}
			set
			{
				PublisherIdentityPermission.CheckCertificate(value);
				this.m_unrestricted = false;
				this.m_certs = new X509Certificate[1];
				this.m_certs[0] = new X509Certificate(value);
			}
		}

		private static void CheckCertificate(X509Certificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (certificate.GetRawCertData() == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_UninitializedCertificate"));
			}
		}

		public override IPermission Copy()
		{
			PublisherIdentityPermission publisherIdentityPermission = new PublisherIdentityPermission(PermissionState.None);
			publisherIdentityPermission.m_unrestricted = this.m_unrestricted;
			if (this.m_certs != null)
			{
				publisherIdentityPermission.m_certs = new X509Certificate[this.m_certs.Length];
				for (int i = 0; i < this.m_certs.Length; i++)
				{
					publisherIdentityPermission.m_certs[i] = ((this.m_certs[i] == null) ? null : new X509Certificate(this.m_certs[i]));
				}
			}
			return publisherIdentityPermission;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			if (target == null)
			{
				return !this.m_unrestricted && (this.m_certs == null || this.m_certs.Length == 0);
			}
			PublisherIdentityPermission publisherIdentityPermission = target as PublisherIdentityPermission;
			if (publisherIdentityPermission == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_WrongType", new object[]
				{
					base.GetType().FullName
				}));
			}
			if (publisherIdentityPermission.m_unrestricted)
			{
				return true;
			}
			if (this.m_unrestricted)
			{
				return false;
			}
			if (this.m_certs != null)
			{
				foreach (X509Certificate x509Certificate in this.m_certs)
				{
					bool flag = false;
					if (publisherIdentityPermission.m_certs != null)
					{
						foreach (X509Certificate other in publisherIdentityPermission.m_certs)
						{
							if (x509Certificate.Equals(other))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override IPermission Intersect(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			PublisherIdentityPermission publisherIdentityPermission = target as PublisherIdentityPermission;
			if (publisherIdentityPermission == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_WrongType", new object[]
				{
					base.GetType().FullName
				}));
			}
			if (this.m_unrestricted && publisherIdentityPermission.m_unrestricted)
			{
				return new PublisherIdentityPermission(PermissionState.None)
				{
					m_unrestricted = true
				};
			}
			if (this.m_unrestricted)
			{
				return publisherIdentityPermission.Copy();
			}
			if (publisherIdentityPermission.m_unrestricted)
			{
				return this.Copy();
			}
			if (this.m_certs == null || publisherIdentityPermission.m_certs == null || this.m_certs.Length == 0 || publisherIdentityPermission.m_certs.Length == 0)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			foreach (X509Certificate x509Certificate in this.m_certs)
			{
				foreach (X509Certificate other in publisherIdentityPermission.m_certs)
				{
					if (x509Certificate.Equals(other))
					{
						arrayList.Add(new X509Certificate(x509Certificate));
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return null;
			}
			return new PublisherIdentityPermission(PermissionState.None)
			{
				m_certs = (X509Certificate[])arrayList.ToArray(typeof(X509Certificate))
			};
		}

		public override IPermission Union(IPermission target)
		{
			if (target == null)
			{
				if ((this.m_certs == null || this.m_certs.Length == 0) && !this.m_unrestricted)
				{
					return null;
				}
				return this.Copy();
			}
			else
			{
				PublisherIdentityPermission publisherIdentityPermission = target as PublisherIdentityPermission;
				if (publisherIdentityPermission == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_WrongType", new object[]
					{
						base.GetType().FullName
					}));
				}
				if (this.m_unrestricted || publisherIdentityPermission.m_unrestricted)
				{
					return new PublisherIdentityPermission(PermissionState.None)
					{
						m_unrestricted = true
					};
				}
				if (this.m_certs == null || this.m_certs.Length == 0)
				{
					if (publisherIdentityPermission.m_certs == null || publisherIdentityPermission.m_certs.Length == 0)
					{
						return null;
					}
					return publisherIdentityPermission.Copy();
				}
				else
				{
					if (publisherIdentityPermission.m_certs == null || publisherIdentityPermission.m_certs.Length == 0)
					{
						return this.Copy();
					}
					ArrayList arrayList = new ArrayList();
					foreach (X509Certificate value in this.m_certs)
					{
						arrayList.Add(value);
					}
					foreach (X509Certificate x509Certificate in publisherIdentityPermission.m_certs)
					{
						bool flag = false;
						foreach (object obj in arrayList)
						{
							X509Certificate other = (X509Certificate)obj;
							if (x509Certificate.Equals(other))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							arrayList.Add(x509Certificate);
						}
					}
					return new PublisherIdentityPermission(PermissionState.None)
					{
						m_certs = (X509Certificate[])arrayList.ToArray(typeof(X509Certificate))
					};
				}
			}
		}

		public override void FromXml(SecurityElement esd)
		{
			this.m_unrestricted = false;
			this.m_certs = null;
			CodeAccessPermission.ValidateElement(esd, this);
			string text = esd.Attribute("Unrestricted");
			if (text != null && string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_unrestricted = true;
				return;
			}
			string text2 = esd.Attribute("X509v3Certificate");
			ArrayList arrayList = new ArrayList();
			if (text2 != null)
			{
				arrayList.Add(new X509Certificate(Hex.DecodeHexString(text2)));
			}
			ArrayList children = esd.Children;
			if (children != null)
			{
				foreach (object obj in children)
				{
					SecurityElement securityElement = (SecurityElement)obj;
					text2 = securityElement.Attribute("X509v3Certificate");
					if (text2 != null)
					{
						arrayList.Add(new X509Certificate(Hex.DecodeHexString(text2)));
					}
				}
			}
			if (arrayList.Count != 0)
			{
				this.m_certs = (X509Certificate[])arrayList.ToArray(typeof(X509Certificate));
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = CodeAccessPermission.CreatePermissionElement(this, "System.Security.Permissions.PublisherIdentityPermission");
			if (this.m_unrestricted)
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else if (this.m_certs != null)
			{
				if (this.m_certs.Length == 1)
				{
					securityElement.AddAttribute("X509v3Certificate", this.m_certs[0].GetRawCertDataString());
				}
				else
				{
					for (int i = 0; i < this.m_certs.Length; i++)
					{
						SecurityElement securityElement2 = new SecurityElement("Cert");
						securityElement2.AddAttribute("X509v3Certificate", this.m_certs[i].GetRawCertDataString());
						securityElement.AddChild(securityElement2);
					}
				}
			}
			return securityElement;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return PublisherIdentityPermission.GetTokenIndex();
		}

		internal static int GetTokenIndex()
		{
			return 10;
		}

		private bool m_unrestricted;

		private X509Certificate[] m_certs;
	}
}
