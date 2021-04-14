using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Runtime.Remoting.Messaging
{
	[SecurityCritical]
	[CLSCompliant(false)]
	[ComVisible(true)]
	[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.Infrastructure)]
	[Serializable]
	public class MethodResponse : IMethodReturnMessage, IMethodMessage, IMessage, ISerializable, ISerializationRootObject, IInternalMessage
	{
		[SecurityCritical]
		public MethodResponse(Header[] h1, IMethodCallMessage mcm)
		{
			if (mcm == null)
			{
				throw new ArgumentNullException("mcm");
			}
			Message message = mcm as Message;
			if (message != null)
			{
				this.MI = message.GetMethodBase();
			}
			else
			{
				this.MI = mcm.MethodBase;
			}
			if (this.MI == null)
			{
				throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_MethodMissing"), mcm.MethodName, mcm.TypeName));
			}
			this._methodCache = InternalRemotingServices.GetReflectionCachedData(this.MI);
			this.argCount = this._methodCache.Parameters.Length;
			this.fSoap = true;
			this.FillHeaders(h1);
		}

		[SecurityCritical]
		internal MethodResponse(IMethodCallMessage msg, SmuggledMethodReturnMessage smuggledMrm, ArrayList deserializedArgs)
		{
			this.MI = msg.MethodBase;
			this._methodCache = InternalRemotingServices.GetReflectionCachedData(this.MI);
			this.methodName = msg.MethodName;
			this.uri = msg.Uri;
			this.typeName = msg.TypeName;
			if (this._methodCache.IsOverloaded())
			{
				this.methodSignature = (Type[])msg.MethodSignature;
			}
			this.retVal = smuggledMrm.GetReturnValue(deserializedArgs);
			this.outArgs = smuggledMrm.GetArgs(deserializedArgs);
			this.fault = smuggledMrm.GetException(deserializedArgs);
			this.callContext = smuggledMrm.GetCallContext(deserializedArgs);
			if (smuggledMrm.MessagePropertyCount > 0)
			{
				smuggledMrm.PopulateMessageProperties(this.Properties, deserializedArgs);
			}
			this.argCount = this._methodCache.Parameters.Length;
			this.fSoap = false;
		}

		[SecurityCritical]
		internal MethodResponse(IMethodCallMessage msg, object handlerObject, BinaryMethodReturnMessage smuggledMrm)
		{
			if (msg != null)
			{
				this.MI = msg.MethodBase;
				this._methodCache = InternalRemotingServices.GetReflectionCachedData(this.MI);
				this.methodName = msg.MethodName;
				this.uri = msg.Uri;
				this.typeName = msg.TypeName;
				if (this._methodCache.IsOverloaded())
				{
					this.methodSignature = (Type[])msg.MethodSignature;
				}
				this.argCount = this._methodCache.Parameters.Length;
			}
			this.retVal = smuggledMrm.ReturnValue;
			this.outArgs = smuggledMrm.Args;
			this.fault = smuggledMrm.Exception;
			this.callContext = smuggledMrm.LogicalCallContext;
			if (smuggledMrm.HasProperties)
			{
				smuggledMrm.PopulateMessageProperties(this.Properties);
			}
			this.fSoap = false;
		}

		[SecurityCritical]
		internal MethodResponse(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.SetObjectData(info, context);
		}

		[SecurityCritical]
		public virtual object HeaderHandler(Header[] h)
		{
			SerializationMonkey serializationMonkey = (SerializationMonkey)FormatterServices.GetUninitializedObject(typeof(SerializationMonkey));
			Header[] array;
			if (h != null && h.Length != 0 && h[0].Name == "__methodName")
			{
				if (h.Length > 1)
				{
					array = new Header[h.Length - 1];
					Array.Copy(h, 1, array, 0, h.Length - 1);
				}
				else
				{
					array = null;
				}
			}
			else
			{
				array = h;
			}
			Type type = null;
			MethodInfo methodInfo = this.MI as MethodInfo;
			if (methodInfo != null)
			{
				type = methodInfo.ReturnType;
			}
			ParameterInfo[] parameters = this._methodCache.Parameters;
			int num = this._methodCache.MarshalResponseArgMap.Length;
			if (!(type == null) && !(type == typeof(void)))
			{
				num++;
			}
			Type[] array2 = new Type[num];
			string[] array3 = new string[num];
			int num2 = 0;
			if (!(type == null) && !(type == typeof(void)))
			{
				array2[num2++] = type;
			}
			foreach (int num3 in this._methodCache.MarshalResponseArgMap)
			{
				array3[num2] = parameters[num3].Name;
				if (parameters[num3].ParameterType.IsByRef)
				{
					array2[num2++] = parameters[num3].ParameterType.GetElementType();
				}
				else
				{
					array2[num2++] = parameters[num3].ParameterType;
				}
			}
			((IFieldInfo)serializationMonkey).FieldTypes = array2;
			((IFieldInfo)serializationMonkey).FieldNames = array3;
			this.FillHeaders(array, true);
			serializationMonkey._obj = this;
			return serializationMonkey;
		}

		[SecurityCritical]
		public void RootSetObjectData(SerializationInfo info, StreamingContext ctx)
		{
			this.SetObjectData(info, ctx);
		}

		[SecurityCritical]
		internal void SetObjectData(SerializationInfo info, StreamingContext ctx)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			if (this.fSoap)
			{
				this.SetObjectFromSoapData(info);
				return;
			}
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			bool flag = false;
			bool flag2 = false;
			while (enumerator.MoveNext())
			{
				if (enumerator.Name.Equals("__return"))
				{
					flag = true;
					break;
				}
				if (enumerator.Name.Equals("__fault"))
				{
					flag2 = true;
					this.fault = (Exception)enumerator.Value;
					break;
				}
				this.FillHeader(enumerator.Name, enumerator.Value);
			}
			if (flag2 && flag)
			{
				throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadSerialization"));
			}
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
		}

		internal void SetObjectFromSoapData(SerializationInfo info)
		{
			Hashtable keyToNamespaceTable = (Hashtable)info.GetValue("__keyToNamespaceTable", typeof(Hashtable));
			ArrayList arrayList = (ArrayList)info.GetValue("__paramNameList", typeof(ArrayList));
			SoapFault soapFault = (SoapFault)info.GetValue("__fault", typeof(SoapFault));
			if (soapFault != null)
			{
				ServerFault serverFault = soapFault.Detail as ServerFault;
				if (serverFault != null)
				{
					if (serverFault.Exception != null)
					{
						this.fault = serverFault.Exception;
						return;
					}
					Type type = Type.GetType(serverFault.ExceptionType, false, false);
					if (type == null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append("\nException Type: ");
						stringBuilder.Append(serverFault.ExceptionType);
						stringBuilder.Append("\n");
						stringBuilder.Append("Exception Message: ");
						stringBuilder.Append(serverFault.ExceptionMessage);
						stringBuilder.Append("\n");
						stringBuilder.Append(serverFault.StackTrace);
						this.fault = new ServerException(stringBuilder.ToString());
						return;
					}
					object[] args = new object[]
					{
						serverFault.ExceptionMessage
					};
					this.fault = (Exception)Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance, null, args, null, null);
					return;
				}
				else
				{
					if (soapFault.Detail != null && soapFault.Detail.GetType() == typeof(string) && ((string)soapFault.Detail).Length != 0)
					{
						this.fault = new ServerException((string)soapFault.Detail);
						return;
					}
					this.fault = new ServerException(soapFault.FaultString);
					return;
				}
			}
			else
			{
				MethodInfo methodInfo = this.MI as MethodInfo;
				int num = 0;
				if (methodInfo != null)
				{
					Type returnType = methodInfo.ReturnType;
					if (returnType != typeof(void))
					{
						num++;
						object value = info.GetValue((string)arrayList[0], typeof(object));
						if (value is string)
						{
							this.retVal = Message.SoapCoerceArg(value, returnType, keyToNamespaceTable);
						}
						else
						{
							this.retVal = value;
						}
					}
				}
				ParameterInfo[] parameters = this._methodCache.Parameters;
				object obj = (this.InternalProperties == null) ? null : this.InternalProperties["__UnorderedParams"];
				if (obj != null && obj is bool && (bool)obj)
				{
					for (int i = num; i < arrayList.Count; i++)
					{
						string text = (string)arrayList[i];
						int num2 = -1;
						for (int j = 0; j < parameters.Length; j++)
						{
							if (text.Equals(parameters[j].Name))
							{
								num2 = parameters[j].Position;
							}
						}
						if (num2 == -1)
						{
							if (!text.StartsWith("__param", StringComparison.Ordinal))
							{
								throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadSerialization"));
							}
							num2 = int.Parse(text.Substring(7), CultureInfo.InvariantCulture);
						}
						if (num2 >= this.argCount)
						{
							throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadSerialization"));
						}
						if (this.outArgs == null)
						{
							this.outArgs = new object[this.argCount];
						}
						this.outArgs[num2] = Message.SoapCoerceArg(info.GetValue(text, typeof(object)), parameters[num2].ParameterType, keyToNamespaceTable);
					}
					return;
				}
				if (this.argMapper == null)
				{
					this.argMapper = new ArgMapper(this, true);
				}
				for (int k = num; k < arrayList.Count; k++)
				{
					string name = (string)arrayList[k];
					if (this.outArgs == null)
					{
						this.outArgs = new object[this.argCount];
					}
					int num3 = this.argMapper.Map[k - num];
					this.outArgs[num3] = Message.SoapCoerceArg(info.GetValue(name, typeof(object)), parameters[num3].ParameterType, keyToNamespaceTable);
				}
				return;
			}
		}

		[SecurityCritical]
		internal LogicalCallContext GetLogicalCallContext()
		{
			if (this.callContext == null)
			{
				this.callContext = new LogicalCallContext();
			}
			return this.callContext;
		}

		internal LogicalCallContext SetLogicalCallContext(LogicalCallContext ctx)
		{
			LogicalCallContext result = this.callContext;
			this.callContext = ctx;
			return result;
		}

		public string Uri
		{
			[SecurityCritical]
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		public string MethodName
		{
			[SecurityCritical]
			get
			{
				return this.methodName;
			}
		}

		public string TypeName
		{
			[SecurityCritical]
			get
			{
				return this.typeName;
			}
		}

		public object MethodSignature
		{
			[SecurityCritical]
			get
			{
				return this.methodSignature;
			}
		}

		public MethodBase MethodBase
		{
			[SecurityCritical]
			get
			{
				return this.MI;
			}
		}

		public bool HasVarArgs
		{
			[SecurityCritical]
			get
			{
				return false;
			}
		}

		public int ArgCount
		{
			[SecurityCritical]
			get
			{
				if (this.outArgs == null)
				{
					return 0;
				}
				return this.outArgs.Length;
			}
		}

		[SecurityCritical]
		public object GetArg(int argNum)
		{
			return this.outArgs[argNum];
		}

		[SecurityCritical]
		public string GetArgName(int index)
		{
			if (!(this.MI != null))
			{
				return "__param" + index;
			}
			RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(this.MI);
			ParameterInfo[] parameters = reflectionCachedData.Parameters;
			if (index < 0 || index >= parameters.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return reflectionCachedData.Parameters[index].Name;
		}

		public object[] Args
		{
			[SecurityCritical]
			get
			{
				return this.outArgs;
			}
		}

		public int OutArgCount
		{
			[SecurityCritical]
			get
			{
				if (this.argMapper == null)
				{
					this.argMapper = new ArgMapper(this, true);
				}
				return this.argMapper.ArgCount;
			}
		}

		[SecurityCritical]
		public object GetOutArg(int argNum)
		{
			if (this.argMapper == null)
			{
				this.argMapper = new ArgMapper(this, true);
			}
			return this.argMapper.GetArg(argNum);
		}

		[SecurityCritical]
		public string GetOutArgName(int index)
		{
			if (this.argMapper == null)
			{
				this.argMapper = new ArgMapper(this, true);
			}
			return this.argMapper.GetArgName(index);
		}

		public object[] OutArgs
		{
			[SecurityCritical]
			get
			{
				if (this.argMapper == null)
				{
					this.argMapper = new ArgMapper(this, true);
				}
				return this.argMapper.Args;
			}
		}

		public Exception Exception
		{
			[SecurityCritical]
			get
			{
				return this.fault;
			}
		}

		public object ReturnValue
		{
			[SecurityCritical]
			get
			{
				return this.retVal;
			}
		}

		public virtual IDictionary Properties
		{
			[SecurityCritical]
			get
			{
				IDictionary externalProperties;
				lock (this)
				{
					if (this.InternalProperties == null)
					{
						this.InternalProperties = new Hashtable();
					}
					if (this.ExternalProperties == null)
					{
						this.ExternalProperties = new MRMDictionary(this, this.InternalProperties);
					}
					externalProperties = this.ExternalProperties;
				}
				return externalProperties;
			}
		}

		public LogicalCallContext LogicalCallContext
		{
			[SecurityCritical]
			get
			{
				return this.GetLogicalCallContext();
			}
		}

		[SecurityCritical]
		internal void FillHeaders(Header[] h)
		{
			this.FillHeaders(h, false);
		}

		[SecurityCritical]
		private void FillHeaders(Header[] h, bool bFromHeaderHandler)
		{
			if (h == null)
			{
				return;
			}
			if (bFromHeaderHandler && this.fSoap)
			{
				foreach (Header header in h)
				{
					if (header.HeaderNamespace == "http://schemas.microsoft.com/clr/soap/messageProperties")
					{
						this.FillHeader(header.Name, header.Value);
					}
					else
					{
						string propertyKeyForHeader = LogicalCallContext.GetPropertyKeyForHeader(header);
						this.FillHeader(propertyKeyForHeader, header);
					}
				}
				return;
			}
			for (int j = 0; j < h.Length; j++)
			{
				this.FillHeader(h[j].Name, h[j].Value);
			}
		}

		[SecurityCritical]
		internal void FillHeader(string name, object value)
		{
			if (name.Equals("__MethodName"))
			{
				this.methodName = (string)value;
				return;
			}
			if (name.Equals("__Uri"))
			{
				this.uri = (string)value;
				return;
			}
			if (name.Equals("__MethodSignature"))
			{
				this.methodSignature = (Type[])value;
				return;
			}
			if (name.Equals("__TypeName"))
			{
				this.typeName = (string)value;
				return;
			}
			if (name.Equals("__OutArgs"))
			{
				this.outArgs = (object[])value;
				return;
			}
			if (name.Equals("__CallContext"))
			{
				if (value is string)
				{
					this.callContext = new LogicalCallContext();
					this.callContext.RemotingData.LogicalCallID = (string)value;
					return;
				}
				this.callContext = (LogicalCallContext)value;
				return;
			}
			else
			{
				if (name.Equals("__Return"))
				{
					this.retVal = value;
					return;
				}
				if (this.InternalProperties == null)
				{
					this.InternalProperties = new Hashtable();
				}
				this.InternalProperties[name] = value;
				return;
			}
		}

		ServerIdentity IInternalMessage.ServerIdentityObject
		{
			[SecurityCritical]
			get
			{
				return null;
			}
			[SecurityCritical]
			set
			{
			}
		}

		Identity IInternalMessage.IdentityObject
		{
			[SecurityCritical]
			get
			{
				return null;
			}
			[SecurityCritical]
			set
			{
			}
		}

		[SecurityCritical]
		void IInternalMessage.SetURI(string val)
		{
			this.uri = val;
		}

		[SecurityCritical]
		void IInternalMessage.SetCallContext(LogicalCallContext newCallContext)
		{
			this.callContext = newCallContext;
		}

		[SecurityCritical]
		bool IInternalMessage.HasProperties()
		{
			return this.ExternalProperties != null || this.InternalProperties != null;
		}

		private MethodBase MI;

		private string methodName;

		private Type[] methodSignature;

		private string uri;

		private string typeName;

		private object retVal;

		private Exception fault;

		private object[] outArgs;

		private LogicalCallContext callContext;

		protected IDictionary InternalProperties;

		protected IDictionary ExternalProperties;

		private int argCount;

		private bool fSoap;

		private ArgMapper argMapper;

		private RemotingMethodCachedData _methodCache;
	}
}
