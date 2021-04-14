using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.Remoting.Contexts
{
	[SecurityCritical]
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
	public class ContextProperty
	{
		public virtual string Name
		{
			get
			{
				return this._name;
			}
		}

		public virtual object Property
		{
			get
			{
				return this._property;
			}
		}

		internal ContextProperty(string name, object prop)
		{
			this._name = name;
			this._property = prop;
		}

		internal string _name;

		internal object _property;
	}
}
