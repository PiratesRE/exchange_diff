using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.Remoting.Metadata
{
	internal class RemotingTypeCachedData : RemotingCachedData
	{
		internal RemotingTypeCachedData(RuntimeType ri)
		{
			this.RI = ri;
		}

		internal override SoapAttribute GetSoapAttributeNoLock()
		{
			object[] customAttributes = this.RI.GetCustomAttributes(typeof(SoapTypeAttribute), true);
			SoapAttribute soapAttribute;
			if (customAttributes != null && customAttributes.Length != 0)
			{
				soapAttribute = (SoapAttribute)customAttributes[0];
			}
			else
			{
				soapAttribute = new SoapTypeAttribute();
			}
			soapAttribute.SetReflectInfo(this.RI);
			return soapAttribute;
		}

		internal MethodBase GetLastCalledMethod(string newMeth)
		{
			RemotingTypeCachedData.LastCalledMethodClass lastMethodCalled = this._lastMethodCalled;
			if (lastMethodCalled == null)
			{
				return null;
			}
			string methodName = lastMethodCalled.methodName;
			MethodBase mb = lastMethodCalled.MB;
			if (mb == null || methodName == null)
			{
				return null;
			}
			if (methodName.Equals(newMeth))
			{
				return mb;
			}
			return null;
		}

		internal void SetLastCalledMethod(string newMethName, MethodBase newMB)
		{
			this._lastMethodCalled = new RemotingTypeCachedData.LastCalledMethodClass
			{
				methodName = newMethName,
				MB = newMB
			};
		}

		internal TypeInfo TypeInfo
		{
			[SecurityCritical]
			get
			{
				if (this._typeInfo == null)
				{
					this._typeInfo = new TypeInfo(this.RI);
				}
				return this._typeInfo;
			}
		}

		internal string QualifiedTypeName
		{
			[SecurityCritical]
			get
			{
				if (this._qualifiedTypeName == null)
				{
					this._qualifiedTypeName = RemotingServices.DetermineDefaultQualifiedTypeName(this.RI);
				}
				return this._qualifiedTypeName;
			}
		}

		internal string AssemblyName
		{
			get
			{
				if (this._assemblyName == null)
				{
					this._assemblyName = this.RI.Module.Assembly.FullName;
				}
				return this._assemblyName;
			}
		}

		internal string SimpleAssemblyName
		{
			[SecurityCritical]
			get
			{
				if (this._simpleAssemblyName == null)
				{
					this._simpleAssemblyName = this.RI.GetRuntimeAssembly().GetSimpleName();
				}
				return this._simpleAssemblyName;
			}
		}

		private RuntimeType RI;

		private RemotingTypeCachedData.LastCalledMethodClass _lastMethodCalled;

		private TypeInfo _typeInfo;

		private string _qualifiedTypeName;

		private string _assemblyName;

		private string _simpleAssemblyName;

		private class LastCalledMethodClass
		{
			public string methodName;

			public MethodBase MB;
		}
	}
}
