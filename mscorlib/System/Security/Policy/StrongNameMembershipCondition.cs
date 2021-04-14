using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Util;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class StrongNameMembershipCondition : IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable, IConstantMembershipCondition, IReportMatchMembershipCondition
	{
		internal StrongNameMembershipCondition()
		{
		}

		public StrongNameMembershipCondition(StrongNamePublicKeyBlob blob, string name, Version version)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (name != null && name.Equals(""))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyStrongName"));
			}
			this.m_publicKeyBlob = blob;
			this.m_name = name;
			this.m_version = version;
		}

		public StrongNamePublicKeyBlob PublicKey
		{
			get
			{
				if (this.m_publicKeyBlob == null && this.m_element != null)
				{
					this.ParseKeyBlob();
				}
				return this.m_publicKeyBlob;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("PublicKey");
				}
				this.m_publicKeyBlob = value;
			}
		}

		public string Name
		{
			get
			{
				if (this.m_name == null && this.m_element != null)
				{
					this.ParseName();
				}
				return this.m_name;
			}
			set
			{
				if (value == null)
				{
					if (this.m_publicKeyBlob == null && this.m_element != null)
					{
						this.ParseKeyBlob();
					}
					if (this.m_version == null && this.m_element != null)
					{
						this.ParseVersion();
					}
					this.m_element = null;
				}
				else if (value.Length == 0)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_EmptyName"));
				}
				this.m_name = value;
			}
		}

		public Version Version
		{
			get
			{
				if (this.m_version == null && this.m_element != null)
				{
					this.ParseVersion();
				}
				return this.m_version;
			}
			set
			{
				if (value == null)
				{
					if (this.m_name == null && this.m_element != null)
					{
						this.ParseName();
					}
					if (this.m_publicKeyBlob == null && this.m_element != null)
					{
						this.ParseKeyBlob();
					}
					this.m_element = null;
				}
				this.m_version = value;
			}
		}

		public bool Check(Evidence evidence)
		{
			object obj = null;
			return ((IReportMatchMembershipCondition)this).Check(evidence, out obj);
		}

		bool IReportMatchMembershipCondition.Check(Evidence evidence, out object usedEvidence)
		{
			usedEvidence = null;
			if (evidence == null)
			{
				return false;
			}
			StrongName delayEvaluatedHostEvidence = evidence.GetDelayEvaluatedHostEvidence<StrongName>();
			if (delayEvaluatedHostEvidence != null)
			{
				bool flag = this.PublicKey != null && this.PublicKey.Equals(delayEvaluatedHostEvidence.PublicKey);
				bool flag2 = this.Name == null || (delayEvaluatedHostEvidence.Name != null && StrongName.CompareNames(delayEvaluatedHostEvidence.Name, this.Name));
				bool flag3 = this.Version == null || (delayEvaluatedHostEvidence.Version != null && delayEvaluatedHostEvidence.Version.CompareTo(this.Version) == 0);
				if (flag && flag2 && flag3)
				{
					usedEvidence = delayEvaluatedHostEvidence;
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new StrongNameMembershipCondition(this.PublicKey, this.Name, this.Version);
		}

		public SecurityElement ToXml()
		{
			return this.ToXml(null);
		}

		public void FromXml(SecurityElement e)
		{
			this.FromXml(e, null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = new SecurityElement("IMembershipCondition");
			XMLUtil.AddClassAttribute(securityElement, base.GetType(), "System.Security.Policy.StrongNameMembershipCondition");
			securityElement.AddAttribute("version", "1");
			if (this.PublicKey != null)
			{
				securityElement.AddAttribute("PublicKeyBlob", Hex.EncodeHexString(this.PublicKey.PublicKey));
			}
			if (this.Name != null)
			{
				securityElement.AddAttribute("Name", this.Name);
			}
			if (this.Version != null)
			{
				securityElement.AddAttribute("AssemblyVersion", this.Version.ToString());
			}
			return securityElement;
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (!e.Tag.Equals("IMembershipCondition"))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MembershipConditionElement"));
			}
			lock (this)
			{
				this.m_name = null;
				this.m_publicKeyBlob = null;
				this.m_version = null;
				this.m_element = e;
			}
		}

		private void ParseName()
		{
			lock (this)
			{
				if (this.m_element != null)
				{
					string text = this.m_element.Attribute("Name");
					this.m_name = ((text == null) ? null : text);
					if (this.m_version != null && this.m_name != null && this.m_publicKeyBlob != null)
					{
						this.m_element = null;
					}
				}
			}
		}

		private void ParseKeyBlob()
		{
			lock (this)
			{
				if (this.m_element != null)
				{
					string text = this.m_element.Attribute("PublicKeyBlob");
					StrongNamePublicKeyBlob strongNamePublicKeyBlob = new StrongNamePublicKeyBlob();
					if (text == null)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_BlobCannotBeNull"));
					}
					strongNamePublicKeyBlob.PublicKey = Hex.DecodeHexString(text);
					this.m_publicKeyBlob = strongNamePublicKeyBlob;
					if (this.m_version != null && this.m_name != null && this.m_publicKeyBlob != null)
					{
						this.m_element = null;
					}
				}
			}
		}

		private void ParseVersion()
		{
			lock (this)
			{
				if (this.m_element != null)
				{
					string text = this.m_element.Attribute("AssemblyVersion");
					this.m_version = ((text == null) ? null : new Version(text));
					if (this.m_version != null && this.m_name != null && this.m_publicKeyBlob != null)
					{
						this.m_element = null;
					}
				}
			}
		}

		public override string ToString()
		{
			string arg = "";
			string arg2 = "";
			if (this.Name != null)
			{
				arg = " " + string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("StrongName_Name"), this.Name);
			}
			if (this.Version != null)
			{
				arg2 = " " + string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("StrongName_Version"), this.Version);
			}
			return string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("StrongName_ToString"), Hex.EncodeHexString(this.PublicKey.PublicKey), arg, arg2);
		}

		public override bool Equals(object o)
		{
			StrongNameMembershipCondition strongNameMembershipCondition = o as StrongNameMembershipCondition;
			if (strongNameMembershipCondition != null)
			{
				if (this.m_publicKeyBlob == null && this.m_element != null)
				{
					this.ParseKeyBlob();
				}
				if (strongNameMembershipCondition.m_publicKeyBlob == null && strongNameMembershipCondition.m_element != null)
				{
					strongNameMembershipCondition.ParseKeyBlob();
				}
				if (object.Equals(this.m_publicKeyBlob, strongNameMembershipCondition.m_publicKeyBlob))
				{
					if (this.m_name == null && this.m_element != null)
					{
						this.ParseName();
					}
					if (strongNameMembershipCondition.m_name == null && strongNameMembershipCondition.m_element != null)
					{
						strongNameMembershipCondition.ParseName();
					}
					if (object.Equals(this.m_name, strongNameMembershipCondition.m_name))
					{
						if (this.m_version == null && this.m_element != null)
						{
							this.ParseVersion();
						}
						if (strongNameMembershipCondition.m_version == null && strongNameMembershipCondition.m_element != null)
						{
							strongNameMembershipCondition.ParseVersion();
						}
						if (object.Equals(this.m_version, strongNameMembershipCondition.m_version))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			if (this.m_publicKeyBlob == null && this.m_element != null)
			{
				this.ParseKeyBlob();
			}
			if (this.m_publicKeyBlob != null)
			{
				return this.m_publicKeyBlob.GetHashCode();
			}
			if (this.m_name == null && this.m_element != null)
			{
				this.ParseName();
			}
			if (this.m_version == null && this.m_element != null)
			{
				this.ParseVersion();
			}
			if (this.m_name != null || this.m_version != null)
			{
				return ((this.m_name == null) ? 0 : this.m_name.GetHashCode()) + ((this.m_version == null) ? 0 : this.m_version.GetHashCode());
			}
			return typeof(StrongNameMembershipCondition).GetHashCode();
		}

		private StrongNamePublicKeyBlob m_publicKeyBlob;

		private string m_name;

		private Version m_version;

		private SecurityElement m_element;

		private const string s_tagName = "Name";

		private const string s_tagVersion = "AssemblyVersion";

		private const string s_tagPublicKeyBlob = "PublicKeyBlob";
	}
}
