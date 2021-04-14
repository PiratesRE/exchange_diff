using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace System.Security
{
	[ComVisible(true)]
	[Serializable]
	public sealed class NamedPermissionSet : PermissionSet
	{
		internal NamedPermissionSet()
		{
		}

		public NamedPermissionSet(string name)
		{
			NamedPermissionSet.CheckName(name);
			this.m_name = name;
		}

		public NamedPermissionSet(string name, PermissionState state) : base(state)
		{
			NamedPermissionSet.CheckName(name);
			this.m_name = name;
		}

		public NamedPermissionSet(string name, PermissionSet permSet) : base(permSet)
		{
			NamedPermissionSet.CheckName(name);
			this.m_name = name;
		}

		public NamedPermissionSet(NamedPermissionSet permSet) : base(permSet)
		{
			this.m_name = permSet.m_name;
			this.m_description = permSet.Description;
		}

		internal NamedPermissionSet(SecurityElement permissionSetXml) : base(PermissionState.None)
		{
			this.FromXml(permissionSetXml);
		}

		public string Name
		{
			get
			{
				return this.m_name;
			}
			set
			{
				NamedPermissionSet.CheckName(value);
				this.m_name = value;
			}
		}

		private static void CheckName(string name)
		{
			if (name == null || name.Equals(""))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NPMSInvalidName"));
			}
		}

		public string Description
		{
			get
			{
				if (this.m_descrResource != null)
				{
					this.m_description = Environment.GetResourceString(this.m_descrResource);
					this.m_descrResource = null;
				}
				return this.m_description;
			}
			set
			{
				this.m_description = value;
				this.m_descrResource = null;
			}
		}

		public override PermissionSet Copy()
		{
			return new NamedPermissionSet(this);
		}

		public NamedPermissionSet Copy(string name)
		{
			return new NamedPermissionSet(this)
			{
				Name = name
			};
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = base.ToXml("System.Security.NamedPermissionSet");
			if (this.m_name != null && !this.m_name.Equals(""))
			{
				securityElement.AddAttribute("Name", SecurityElement.Escape(this.m_name));
			}
			if (this.Description != null && !this.Description.Equals(""))
			{
				securityElement.AddAttribute("Description", SecurityElement.Escape(this.Description));
			}
			return securityElement;
		}

		public override void FromXml(SecurityElement et)
		{
			this.FromXml(et, false, false);
		}

		internal override void FromXml(SecurityElement et, bool allowInternalOnly, bool ignoreTypeLoadFailures)
		{
			if (et == null)
			{
				throw new ArgumentNullException("et");
			}
			string text = et.Attribute("Name");
			this.m_name = ((text == null) ? null : text);
			text = et.Attribute("Description");
			this.m_description = ((text == null) ? "" : text);
			this.m_descrResource = null;
			base.FromXml(et, allowInternalOnly, ignoreTypeLoadFailures);
		}

		internal void FromXmlNameOnly(SecurityElement et)
		{
			string text = et.Attribute("Name");
			this.m_name = ((text == null) ? null : text);
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private static object InternalSyncObject
		{
			get
			{
				if (NamedPermissionSet.s_InternalSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange(ref NamedPermissionSet.s_InternalSyncObject, value, null);
				}
				return NamedPermissionSet.s_InternalSyncObject;
			}
		}

		private string m_name;

		private string m_description;

		[OptionalField(VersionAdded = 2)]
		internal string m_descrResource;

		private static object s_InternalSyncObject;
	}
}
