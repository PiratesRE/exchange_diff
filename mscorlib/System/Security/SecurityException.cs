using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security.Util;
using System.Text;
using System.Threading;

namespace System.Security
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class SecurityException : SystemException
	{
		[SecuritySafeCritical]
		internal static string GetResString(string sResourceName)
		{
			PermissionSet.s_fullTrust.Assert();
			return Environment.GetResourceString(sResourceName);
		}

		[SecurityCritical]
		internal static Exception MakeSecurityException(AssemblyName asmName, Evidence asmEvidence, PermissionSet granted, PermissionSet refused, RuntimeMethodHandleInternal rmh, SecurityAction action, object demand, IPermission permThatFailed)
		{
			HostProtectionPermission hostProtectionPermission = permThatFailed as HostProtectionPermission;
			if (hostProtectionPermission != null)
			{
				return new HostProtectionException(SecurityException.GetResString("HostProtection_HostProtection"), HostProtectionPermission.protectedResources, hostProtectionPermission.Resources);
			}
			string message = "";
			MethodInfo method = null;
			try
			{
				if (granted == null && refused == null && demand == null)
				{
					message = SecurityException.GetResString("Security_NoAPTCA");
				}
				else if (demand != null && demand is IPermission)
				{
					message = string.Format(CultureInfo.InvariantCulture, SecurityException.GetResString("Security_Generic"), demand.GetType().AssemblyQualifiedName);
				}
				else if (permThatFailed != null)
				{
					message = string.Format(CultureInfo.InvariantCulture, SecurityException.GetResString("Security_Generic"), permThatFailed.GetType().AssemblyQualifiedName);
				}
				else
				{
					message = SecurityException.GetResString("Security_GenericNoType");
				}
				method = SecurityRuntime.GetMethodInfo(rmh);
			}
			catch (Exception ex)
			{
				if (ex is ThreadAbortException)
				{
					throw;
				}
			}
			return new SecurityException(message, asmName, granted, refused, method, action, demand, permThatFailed, asmEvidence);
		}

		private static byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			byte[] result;
			try
			{
				binaryFormatter.Serialize(memoryStream, obj);
				byte[] array = memoryStream.ToArray();
				result = array;
			}
			catch (NotSupportedException)
			{
				result = null;
			}
			return result;
		}

		private static object ByteArrayToObject(byte[] array)
		{
			if (array == null || array.Length == 0)
			{
				return null;
			}
			MemoryStream serializationStream = new MemoryStream(array);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			return binaryFormatter.Deserialize(serializationStream);
		}

		[__DynamicallyInvokable]
		public SecurityException() : base(SecurityException.GetResString("Arg_SecurityException"))
		{
			base.SetErrorCode(-2146233078);
		}

		[__DynamicallyInvokable]
		public SecurityException(string message) : base(message)
		{
			base.SetErrorCode(-2146233078);
		}

		[SecuritySafeCritical]
		public SecurityException(string message, Type type) : base(message)
		{
			PermissionSet.s_fullTrust.Assert();
			base.SetErrorCode(-2146233078);
			this.m_typeOfPermissionThatFailed = type;
		}

		[SecuritySafeCritical]
		public SecurityException(string message, Type type, string state) : base(message)
		{
			PermissionSet.s_fullTrust.Assert();
			base.SetErrorCode(-2146233078);
			this.m_typeOfPermissionThatFailed = type;
			this.m_demanded = state;
		}

		[__DynamicallyInvokable]
		public SecurityException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233078);
		}

		[SecurityCritical]
		internal SecurityException(PermissionSet grantedSetObj, PermissionSet refusedSetObj) : base(SecurityException.GetResString("Arg_SecurityException"))
		{
			PermissionSet.s_fullTrust.Assert();
			base.SetErrorCode(-2146233078);
			if (grantedSetObj != null)
			{
				this.m_granted = grantedSetObj.ToXml().ToString();
			}
			if (refusedSetObj != null)
			{
				this.m_refused = refusedSetObj.ToXml().ToString();
			}
		}

		[SecurityCritical]
		internal SecurityException(string message, PermissionSet grantedSetObj, PermissionSet refusedSetObj) : base(message)
		{
			PermissionSet.s_fullTrust.Assert();
			base.SetErrorCode(-2146233078);
			if (grantedSetObj != null)
			{
				this.m_granted = grantedSetObj.ToXml().ToString();
			}
			if (refusedSetObj != null)
			{
				this.m_refused = refusedSetObj.ToXml().ToString();
			}
		}

		[SecuritySafeCritical]
		protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			try
			{
				this.m_action = (SecurityAction)info.GetValue("Action", typeof(SecurityAction));
				this.m_permissionThatFailed = (string)info.GetValueNoThrow("FirstPermissionThatFailed", typeof(string));
				this.m_demanded = (string)info.GetValueNoThrow("Demanded", typeof(string));
				this.m_granted = (string)info.GetValueNoThrow("GrantedSet", typeof(string));
				this.m_refused = (string)info.GetValueNoThrow("RefusedSet", typeof(string));
				this.m_denied = (string)info.GetValueNoThrow("Denied", typeof(string));
				this.m_permitOnly = (string)info.GetValueNoThrow("PermitOnly", typeof(string));
				this.m_assemblyName = (AssemblyName)info.GetValueNoThrow("Assembly", typeof(AssemblyName));
				this.m_serializedMethodInfo = (byte[])info.GetValueNoThrow("Method", typeof(byte[]));
				this.m_strMethodInfo = (string)info.GetValueNoThrow("Method_String", typeof(string));
				this.m_zone = (SecurityZone)info.GetValue("Zone", typeof(SecurityZone));
				this.m_url = (string)info.GetValueNoThrow("Url", typeof(string));
			}
			catch
			{
				this.m_action = (SecurityAction)0;
				this.m_permissionThatFailed = "";
				this.m_demanded = "";
				this.m_granted = "";
				this.m_refused = "";
				this.m_denied = "";
				this.m_permitOnly = "";
				this.m_assemblyName = null;
				this.m_serializedMethodInfo = null;
				this.m_strMethodInfo = null;
				this.m_zone = SecurityZone.NoZone;
				this.m_url = "";
			}
		}

		[SecuritySafeCritical]
		public SecurityException(string message, AssemblyName assemblyName, PermissionSet grant, PermissionSet refused, MethodInfo method, SecurityAction action, object demanded, IPermission permThatFailed, Evidence evidence) : base(message)
		{
			PermissionSet.s_fullTrust.Assert();
			base.SetErrorCode(-2146233078);
			this.Action = action;
			if (permThatFailed != null)
			{
				this.m_typeOfPermissionThatFailed = permThatFailed.GetType();
			}
			this.FirstPermissionThatFailed = permThatFailed;
			this.Demanded = demanded;
			this.m_granted = ((grant == null) ? "" : grant.ToXml().ToString());
			this.m_refused = ((refused == null) ? "" : refused.ToXml().ToString());
			this.m_denied = "";
			this.m_permitOnly = "";
			this.m_assemblyName = assemblyName;
			this.Method = method;
			this.m_url = "";
			this.m_zone = SecurityZone.NoZone;
			if (evidence != null)
			{
				Url hostEvidence = evidence.GetHostEvidence<Url>();
				if (hostEvidence != null)
				{
					this.m_url = hostEvidence.GetURLString().ToString();
				}
				Zone hostEvidence2 = evidence.GetHostEvidence<Zone>();
				if (hostEvidence2 != null)
				{
					this.m_zone = hostEvidence2.SecurityZone;
				}
			}
			this.m_debugString = this.ToString(true, false);
		}

		[SecuritySafeCritical]
		public SecurityException(string message, object deny, object permitOnly, MethodInfo method, object demanded, IPermission permThatFailed) : base(message)
		{
			PermissionSet.s_fullTrust.Assert();
			base.SetErrorCode(-2146233078);
			this.Action = SecurityAction.Demand;
			if (permThatFailed != null)
			{
				this.m_typeOfPermissionThatFailed = permThatFailed.GetType();
			}
			this.FirstPermissionThatFailed = permThatFailed;
			this.Demanded = demanded;
			this.m_granted = "";
			this.m_refused = "";
			this.DenySetInstance = deny;
			this.PermitOnlySetInstance = permitOnly;
			this.m_assemblyName = null;
			this.Method = method;
			this.m_zone = SecurityZone.NoZone;
			this.m_url = "";
			this.m_debugString = this.ToString(true, false);
		}

		[ComVisible(false)]
		public SecurityAction Action
		{
			get
			{
				return this.m_action;
			}
			set
			{
				this.m_action = value;
			}
		}

		public Type PermissionType
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_typeOfPermissionThatFailed == null)
				{
					object obj = XMLUtil.XmlStringToSecurityObject(this.m_permissionThatFailed);
					if (obj == null)
					{
						obj = XMLUtil.XmlStringToSecurityObject(this.m_demanded);
					}
					if (obj != null)
					{
						this.m_typeOfPermissionThatFailed = obj.GetType();
					}
				}
				return this.m_typeOfPermissionThatFailed;
			}
			set
			{
				this.m_typeOfPermissionThatFailed = value;
			}
		}

		public IPermission FirstPermissionThatFailed
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return (IPermission)XMLUtil.XmlStringToSecurityObject(this.m_permissionThatFailed);
			}
			set
			{
				this.m_permissionThatFailed = XMLUtil.SecurityObjectToXmlString(value);
			}
		}

		public string PermissionState
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return this.m_demanded;
			}
			set
			{
				this.m_demanded = value;
			}
		}

		[ComVisible(false)]
		public object Demanded
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return XMLUtil.XmlStringToSecurityObject(this.m_demanded);
			}
			set
			{
				this.m_demanded = XMLUtil.SecurityObjectToXmlString(value);
			}
		}

		public string GrantedSet
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return this.m_granted;
			}
			set
			{
				this.m_granted = value;
			}
		}

		public string RefusedSet
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return this.m_refused;
			}
			set
			{
				this.m_refused = value;
			}
		}

		[ComVisible(false)]
		public object DenySetInstance
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return XMLUtil.XmlStringToSecurityObject(this.m_denied);
			}
			set
			{
				this.m_denied = XMLUtil.SecurityObjectToXmlString(value);
			}
		}

		[ComVisible(false)]
		public object PermitOnlySetInstance
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return XMLUtil.XmlStringToSecurityObject(this.m_permitOnly);
			}
			set
			{
				this.m_permitOnly = XMLUtil.SecurityObjectToXmlString(value);
			}
		}

		[ComVisible(false)]
		public AssemblyName FailedAssemblyInfo
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return this.m_assemblyName;
			}
			set
			{
				this.m_assemblyName = value;
			}
		}

		private MethodInfo getMethod()
		{
			return (MethodInfo)SecurityException.ByteArrayToObject(this.m_serializedMethodInfo);
		}

		[ComVisible(false)]
		public MethodInfo Method
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return this.getMethod();
			}
			set
			{
				RuntimeMethodInfo runtimeMethodInfo = value as RuntimeMethodInfo;
				this.m_serializedMethodInfo = SecurityException.ObjectToByteArray(runtimeMethodInfo);
				if (runtimeMethodInfo != null)
				{
					this.m_strMethodInfo = runtimeMethodInfo.ToString();
				}
			}
		}

		public SecurityZone Zone
		{
			get
			{
				return this.m_zone;
			}
			set
			{
				this.m_zone = value;
			}
		}

		public string Url
		{
			[SecuritySafeCritical]
			[SecurityPermission(SecurityAction.Demand, Flags = (SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy))]
			get
			{
				return this.m_url;
			}
			set
			{
				this.m_url = value;
			}
		}

		private void ToStringHelper(StringBuilder sb, string resourceString, object attr)
		{
			if (attr == null)
			{
				return;
			}
			string text = attr as string;
			if (text == null)
			{
				text = attr.ToString();
			}
			if (text.Length == 0)
			{
				return;
			}
			sb.Append(Environment.NewLine);
			sb.Append(SecurityException.GetResString(resourceString));
			sb.Append(Environment.NewLine);
			sb.Append(text);
		}

		[SecurityCritical]
		private string ToString(bool includeSensitiveInfo, bool includeBaseInfo)
		{
			PermissionSet.s_fullTrust.Assert();
			StringBuilder stringBuilder = new StringBuilder();
			if (includeBaseInfo)
			{
				stringBuilder.Append(base.ToString());
			}
			if (this.Action > (SecurityAction)0)
			{
				this.ToStringHelper(stringBuilder, "Security_Action", this.Action);
			}
			this.ToStringHelper(stringBuilder, "Security_TypeFirstPermThatFailed", this.PermissionType);
			if (includeSensitiveInfo)
			{
				this.ToStringHelper(stringBuilder, "Security_FirstPermThatFailed", this.m_permissionThatFailed);
				this.ToStringHelper(stringBuilder, "Security_Demanded", this.m_demanded);
				this.ToStringHelper(stringBuilder, "Security_GrantedSet", this.m_granted);
				this.ToStringHelper(stringBuilder, "Security_RefusedSet", this.m_refused);
				this.ToStringHelper(stringBuilder, "Security_Denied", this.m_denied);
				this.ToStringHelper(stringBuilder, "Security_PermitOnly", this.m_permitOnly);
				this.ToStringHelper(stringBuilder, "Security_Assembly", this.m_assemblyName);
				this.ToStringHelper(stringBuilder, "Security_Method", this.m_strMethodInfo);
			}
			if (this.m_zone != SecurityZone.NoZone)
			{
				this.ToStringHelper(stringBuilder, "Security_Zone", this.m_zone);
			}
			if (includeSensitiveInfo)
			{
				this.ToStringHelper(stringBuilder, "Security_Url", this.m_url);
			}
			return stringBuilder.ToString();
		}

		[SecurityCritical]
		private bool CanAccessSensitiveInfo()
		{
			bool result = false;
			try
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy).Demand();
				result = true;
			}
			catch (SecurityException)
			{
			}
			return result;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.ToString(this.CanAccessSensitiveInfo(), true);
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("Action", this.m_action, typeof(SecurityAction));
			info.AddValue("FirstPermissionThatFailed", this.m_permissionThatFailed, typeof(string));
			info.AddValue("Demanded", this.m_demanded, typeof(string));
			info.AddValue("GrantedSet", this.m_granted, typeof(string));
			info.AddValue("RefusedSet", this.m_refused, typeof(string));
			info.AddValue("Denied", this.m_denied, typeof(string));
			info.AddValue("PermitOnly", this.m_permitOnly, typeof(string));
			info.AddValue("Assembly", this.m_assemblyName, typeof(AssemblyName));
			info.AddValue("Method", this.m_serializedMethodInfo, typeof(byte[]));
			info.AddValue("Method_String", this.m_strMethodInfo, typeof(string));
			info.AddValue("Zone", this.m_zone, typeof(SecurityZone));
			info.AddValue("Url", this.m_url, typeof(string));
		}

		private string m_debugString;

		private SecurityAction m_action;

		[NonSerialized]
		private Type m_typeOfPermissionThatFailed;

		private string m_permissionThatFailed;

		private string m_demanded;

		private string m_granted;

		private string m_refused;

		private string m_denied;

		private string m_permitOnly;

		private AssemblyName m_assemblyName;

		private byte[] m_serializedMethodInfo;

		private string m_strMethodInfo;

		private SecurityZone m_zone;

		private string m_url;

		private const string ActionName = "Action";

		private const string FirstPermissionThatFailedName = "FirstPermissionThatFailed";

		private const string DemandedName = "Demanded";

		private const string GrantedSetName = "GrantedSet";

		private const string RefusedSetName = "RefusedSet";

		private const string DeniedName = "Denied";

		private const string PermitOnlyName = "PermitOnly";

		private const string Assembly_Name = "Assembly";

		private const string MethodName_Serialized = "Method";

		private const string MethodName_String = "Method_String";

		private const string ZoneName = "Zone";

		private const string UrlName = "Url";
	}
}
