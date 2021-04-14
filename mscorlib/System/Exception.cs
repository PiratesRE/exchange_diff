using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Exception))]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class Exception : ISerializable, _Exception
	{
		private void Init()
		{
			this._message = null;
			this._stackTrace = null;
			this._dynamicMethods = null;
			this.HResult = -2146233088;
			this._xcode = -532462766;
			this._xptrs = (IntPtr)0;
			this._watsonBuckets = null;
			this._ipForWatsonBuckets = UIntPtr.Zero;
			this._safeSerializationManager = new SafeSerializationManager();
		}

		[__DynamicallyInvokable]
		public Exception()
		{
			this.Init();
		}

		[__DynamicallyInvokable]
		public Exception(string message)
		{
			this.Init();
			this._message = message;
		}

		[__DynamicallyInvokable]
		public Exception(string message, Exception innerException)
		{
			this.Init();
			this._message = message;
			this._innerException = innerException;
		}

		[SecuritySafeCritical]
		protected Exception(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this._className = info.GetString("ClassName");
			this._message = info.GetString("Message");
			this._data = (IDictionary)info.GetValueNoThrow("Data", typeof(IDictionary));
			this._innerException = (Exception)info.GetValue("InnerException", typeof(Exception));
			this._helpURL = info.GetString("HelpURL");
			this._stackTraceString = info.GetString("StackTraceString");
			this._remoteStackTraceString = info.GetString("RemoteStackTraceString");
			this._remoteStackIndex = info.GetInt32("RemoteStackIndex");
			this._exceptionMethodString = (string)info.GetValue("ExceptionMethod", typeof(string));
			this.HResult = info.GetInt32("HResult");
			this._source = info.GetString("Source");
			this._watsonBuckets = info.GetValueNoThrow("WatsonBuckets", typeof(byte[]));
			this._safeSerializationManager = (info.GetValueNoThrow("SafeSerializationManager", typeof(SafeSerializationManager)) as SafeSerializationManager);
			if (this._className == null || this.HResult == 0)
			{
				throw new SerializationException(Environment.GetResourceString("Serialization_InsufficientState"));
			}
			if (context.State == StreamingContextStates.CrossAppDomain)
			{
				this._remoteStackTraceString += this._stackTraceString;
				this._stackTraceString = null;
			}
		}

		[__DynamicallyInvokable]
		public virtual string Message
		{
			[__DynamicallyInvokable]
			get
			{
				if (this._message == null)
				{
					if (this._className == null)
					{
						this._className = this.GetClassName();
					}
					return Environment.GetResourceString("Exception_WasThrown", new object[]
					{
						this._className
					});
				}
				return this._message;
			}
		}

		[__DynamicallyInvokable]
		public virtual IDictionary Data
		{
			[SecuritySafeCritical]
			[__DynamicallyInvokable]
			get
			{
				if (this._data == null)
				{
					if (Exception.IsImmutableAgileException(this))
					{
						this._data = new EmptyReadOnlyDictionaryInternal();
					}
					else
					{
						this._data = new ListDictionaryInternal();
					}
				}
				return this._data;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsImmutableAgileException(Exception e);

		[FriendAccessAllowed]
		internal void AddExceptionDataForRestrictedErrorInfo(string restrictedError, string restrictedErrorReference, string restrictedCapabilitySid, object restrictedErrorObject, bool hasrestrictedLanguageErrorObject = false)
		{
			IDictionary data = this.Data;
			if (data != null)
			{
				data.Add("RestrictedDescription", restrictedError);
				data.Add("RestrictedErrorReference", restrictedErrorReference);
				data.Add("RestrictedCapabilitySid", restrictedCapabilitySid);
				data.Add("__RestrictedErrorObject", (restrictedErrorObject == null) ? null : new Exception.__RestrictedErrorObject(restrictedErrorObject));
				data.Add("__HasRestrictedLanguageErrorObject", hasrestrictedLanguageErrorObject);
			}
		}

		internal bool TryGetRestrictedLanguageErrorObject(out object restrictedErrorObject)
		{
			restrictedErrorObject = null;
			if (this.Data != null && this.Data.Contains("__HasRestrictedLanguageErrorObject"))
			{
				if (this.Data.Contains("__RestrictedErrorObject"))
				{
					Exception.__RestrictedErrorObject _RestrictedErrorObject = this.Data["__RestrictedErrorObject"] as Exception.__RestrictedErrorObject;
					if (_RestrictedErrorObject != null)
					{
						restrictedErrorObject = _RestrictedErrorObject.RealErrorObject;
					}
				}
				return (bool)this.Data["__HasRestrictedLanguageErrorObject"];
			}
			return false;
		}

		private string GetClassName()
		{
			if (this._className == null)
			{
				this._className = this.GetType().ToString();
			}
			return this._className;
		}

		[__DynamicallyInvokable]
		public virtual Exception GetBaseException()
		{
			Exception innerException = this.InnerException;
			Exception result = this;
			while (innerException != null)
			{
				result = innerException;
				innerException = innerException.InnerException;
			}
			return result;
		}

		[__DynamicallyInvokable]
		public Exception InnerException
		{
			[__DynamicallyInvokable]
			get
			{
				return this._innerException;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IRuntimeMethodInfo GetMethodFromStackTrace(object stackTrace);

		[SecuritySafeCritical]
		private MethodBase GetExceptionMethodFromStackTrace()
		{
			IRuntimeMethodInfo methodFromStackTrace = Exception.GetMethodFromStackTrace(this._stackTrace);
			if (methodFromStackTrace == null)
			{
				return null;
			}
			return RuntimeType.GetMethodBase(methodFromStackTrace);
		}

		public MethodBase TargetSite
		{
			[SecuritySafeCritical]
			get
			{
				return this.GetTargetSiteInternal();
			}
		}

		[SecurityCritical]
		private MethodBase GetTargetSiteInternal()
		{
			if (this._exceptionMethod != null)
			{
				return this._exceptionMethod;
			}
			if (this._stackTrace == null)
			{
				return null;
			}
			if (this._exceptionMethodString != null)
			{
				this._exceptionMethod = this.GetExceptionMethodFromString();
			}
			else
			{
				this._exceptionMethod = this.GetExceptionMethodFromStackTrace();
			}
			return this._exceptionMethod;
		}

		[__DynamicallyInvokable]
		public virtual string StackTrace
		{
			[__DynamicallyInvokable]
			get
			{
				return this.GetStackTrace(true);
			}
		}

		private string GetStackTrace(bool needFileInfo)
		{
			string text = this._stackTraceString;
			string text2 = this._remoteStackTraceString;
			if (!needFileInfo)
			{
				text = this.StripFileInfo(text, false);
				text2 = this.StripFileInfo(text2, true);
			}
			if (text != null)
			{
				return text2 + text;
			}
			if (this._stackTrace == null)
			{
				return text2;
			}
			string stackTrace = Environment.GetStackTrace(this, needFileInfo);
			return text2 + stackTrace;
		}

		[FriendAccessAllowed]
		internal void SetErrorCode(int hr)
		{
			this.HResult = hr;
		}

		[__DynamicallyInvokable]
		public virtual string HelpLink
		{
			[__DynamicallyInvokable]
			get
			{
				return this._helpURL;
			}
			[__DynamicallyInvokable]
			set
			{
				this._helpURL = value;
			}
		}

		[__DynamicallyInvokable]
		public virtual string Source
		{
			[__DynamicallyInvokable]
			get
			{
				if (this._source == null)
				{
					StackTrace stackTrace = new StackTrace(this, true);
					if (stackTrace.FrameCount > 0)
					{
						StackFrame frame = stackTrace.GetFrame(0);
						MethodBase method = frame.GetMethod();
						Module module = method.Module;
						RuntimeModule runtimeModule = module as RuntimeModule;
						if (runtimeModule == null)
						{
							ModuleBuilder moduleBuilder = module as ModuleBuilder;
							if (!(moduleBuilder != null))
							{
								throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeReflectionObject"));
							}
							runtimeModule = moduleBuilder.InternalModule;
						}
						this._source = runtimeModule.GetRuntimeAssembly().GetSimpleName();
					}
				}
				return this._source;
			}
			[__DynamicallyInvokable]
			set
			{
				this._source = value;
			}
		}

		[__DynamicallyInvokable]
		public override string ToString()
		{
			return this.ToString(true, true);
		}

		private string ToString(bool needFileLineInfo, bool needMessage)
		{
			string text = needMessage ? this.Message : null;
			string text2;
			if (text == null || text.Length <= 0)
			{
				text2 = this.GetClassName();
			}
			else
			{
				text2 = this.GetClassName() + ": " + text;
			}
			if (this._innerException != null)
			{
				text2 = string.Concat(new string[]
				{
					text2,
					" ---> ",
					this._innerException.ToString(needFileLineInfo, needMessage),
					Environment.NewLine,
					"   ",
					Environment.GetResourceString("Exception_EndOfInnerExceptionStack")
				});
			}
			string stackTrace = this.GetStackTrace(needFileLineInfo);
			if (stackTrace != null)
			{
				text2 = text2 + Environment.NewLine + stackTrace;
			}
			return text2;
		}

		[SecurityCritical]
		private string GetExceptionMethodString()
		{
			MethodBase targetSiteInternal = this.GetTargetSiteInternal();
			if (targetSiteInternal == null)
			{
				return null;
			}
			if (targetSiteInternal is DynamicMethod.RTDynamicMethod)
			{
				return null;
			}
			char value = '\n';
			StringBuilder stringBuilder = new StringBuilder();
			if (targetSiteInternal is ConstructorInfo)
			{
				RuntimeConstructorInfo runtimeConstructorInfo = (RuntimeConstructorInfo)targetSiteInternal;
				Type reflectedType = runtimeConstructorInfo.ReflectedType;
				stringBuilder.Append(1);
				stringBuilder.Append(value);
				stringBuilder.Append(runtimeConstructorInfo.Name);
				if (reflectedType != null)
				{
					stringBuilder.Append(value);
					stringBuilder.Append(reflectedType.Assembly.FullName);
					stringBuilder.Append(value);
					stringBuilder.Append(reflectedType.FullName);
				}
				stringBuilder.Append(value);
				stringBuilder.Append(runtimeConstructorInfo.ToString());
			}
			else
			{
				RuntimeMethodInfo runtimeMethodInfo = (RuntimeMethodInfo)targetSiteInternal;
				Type declaringType = runtimeMethodInfo.DeclaringType;
				stringBuilder.Append(8);
				stringBuilder.Append(value);
				stringBuilder.Append(runtimeMethodInfo.Name);
				stringBuilder.Append(value);
				stringBuilder.Append(runtimeMethodInfo.Module.Assembly.FullName);
				stringBuilder.Append(value);
				if (declaringType != null)
				{
					stringBuilder.Append(declaringType.FullName);
					stringBuilder.Append(value);
				}
				stringBuilder.Append(runtimeMethodInfo.ToString());
			}
			return stringBuilder.ToString();
		}

		[SecurityCritical]
		private MethodBase GetExceptionMethodFromString()
		{
			string[] array = this._exceptionMethodString.Split(new char[]
			{
				'\0',
				'\n'
			});
			if (array.Length != 5)
			{
				throw new SerializationException();
			}
			SerializationInfo serializationInfo = new SerializationInfo(typeof(MemberInfoSerializationHolder), new FormatterConverter());
			serializationInfo.AddValue("MemberType", int.Parse(array[0], CultureInfo.InvariantCulture), typeof(int));
			serializationInfo.AddValue("Name", array[1], typeof(string));
			serializationInfo.AddValue("AssemblyName", array[2], typeof(string));
			serializationInfo.AddValue("ClassName", array[3]);
			serializationInfo.AddValue("Signature", array[4]);
			StreamingContext context = new StreamingContext(StreamingContextStates.All);
			MethodBase result;
			try
			{
				result = (MethodBase)new MemberInfoSerializationHolder(serializationInfo, context).GetRealObject(context);
			}
			catch (SerializationException)
			{
				result = null;
			}
			return result;
		}

		protected event EventHandler<SafeSerializationEventArgs> SerializeObjectState
		{
			add
			{
				this._safeSerializationManager.SerializeObjectState += value;
			}
			remove
			{
				this._safeSerializationManager.SerializeObjectState -= value;
			}
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			string text = this._stackTraceString;
			if (this._stackTrace != null)
			{
				if (text == null)
				{
					text = Environment.GetStackTrace(this, true);
				}
				if (this._exceptionMethod == null)
				{
					this._exceptionMethod = this.GetExceptionMethodFromStackTrace();
				}
			}
			if (this._source == null)
			{
				this._source = this.Source;
			}
			info.AddValue("ClassName", this.GetClassName(), typeof(string));
			info.AddValue("Message", this._message, typeof(string));
			info.AddValue("Data", this._data, typeof(IDictionary));
			info.AddValue("InnerException", this._innerException, typeof(Exception));
			info.AddValue("HelpURL", this._helpURL, typeof(string));
			info.AddValue("StackTraceString", text, typeof(string));
			info.AddValue("RemoteStackTraceString", this._remoteStackTraceString, typeof(string));
			info.AddValue("RemoteStackIndex", this._remoteStackIndex, typeof(int));
			info.AddValue("ExceptionMethod", this.GetExceptionMethodString(), typeof(string));
			info.AddValue("HResult", this.HResult);
			info.AddValue("Source", this._source, typeof(string));
			info.AddValue("WatsonBuckets", this._watsonBuckets, typeof(byte[]));
			if (this._safeSerializationManager != null && this._safeSerializationManager.IsActive)
			{
				info.AddValue("SafeSerializationManager", this._safeSerializationManager, typeof(SafeSerializationManager));
				this._safeSerializationManager.CompleteSerialization(this, info, context);
			}
		}

		internal Exception PrepForRemoting()
		{
			string remoteStackTraceString;
			if (this._remoteStackIndex == 0)
			{
				remoteStackTraceString = string.Concat(new object[]
				{
					Environment.NewLine,
					"Server stack trace: ",
					Environment.NewLine,
					this.StackTrace,
					Environment.NewLine,
					Environment.NewLine,
					"Exception rethrown at [",
					this._remoteStackIndex,
					"]: ",
					Environment.NewLine
				});
			}
			else
			{
				remoteStackTraceString = string.Concat(new object[]
				{
					this.StackTrace,
					Environment.NewLine,
					Environment.NewLine,
					"Exception rethrown at [",
					this._remoteStackIndex,
					"]: ",
					Environment.NewLine
				});
			}
			this._remoteStackTraceString = remoteStackTraceString;
			this._remoteStackIndex++;
			return this;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this._stackTrace = null;
			this._ipForWatsonBuckets = UIntPtr.Zero;
			if (this._safeSerializationManager == null)
			{
				this._safeSerializationManager = new SafeSerializationManager();
				return;
			}
			this._safeSerializationManager.CompleteDeserialization(this);
		}

		internal void InternalPreserveStackTrace()
		{
			string stackTrace;
			if (AppDomain.IsAppXModel())
			{
				stackTrace = this.GetStackTrace(true);
				string source = this.Source;
			}
			else
			{
				stackTrace = this.StackTrace;
			}
			if (stackTrace != null && stackTrace.Length > 0)
			{
				this._remoteStackTraceString = stackTrace + Environment.NewLine;
			}
			this._stackTrace = null;
			this._stackTraceString = null;
		}

		internal UIntPtr IPForWatsonBuckets
		{
			get
			{
				return this._ipForWatsonBuckets;
			}
		}

		internal object WatsonBuckets
		{
			get
			{
				return this._watsonBuckets;
			}
		}

		internal string RemoteStackTrace
		{
			get
			{
				return this._remoteStackTraceString;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PrepareForForeignExceptionRaise();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetStackTracesDeepCopy(Exception exception, out object currentStackTrace, out object dynamicMethodArray);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SaveStackTracesFromDeepCopy(Exception exception, object currentStackTrace, object dynamicMethodArray);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object CopyStackTrace(object currentStackTrace);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object CopyDynamicMethods(object currentDynamicMethods);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string StripFileInfo(string stackTrace, bool isRemoteStackTrace);

		[SecuritySafeCritical]
		internal object DeepCopyStackTrace(object currentStackTrace)
		{
			if (currentStackTrace != null)
			{
				return Exception.CopyStackTrace(currentStackTrace);
			}
			return null;
		}

		[SecuritySafeCritical]
		internal object DeepCopyDynamicMethods(object currentDynamicMethods)
		{
			if (currentDynamicMethods != null)
			{
				return Exception.CopyDynamicMethods(currentDynamicMethods);
			}
			return null;
		}

		[SecuritySafeCritical]
		internal void GetStackTracesDeepCopy(out object currentStackTrace, out object dynamicMethodArray)
		{
			Exception.GetStackTracesDeepCopy(this, out currentStackTrace, out dynamicMethodArray);
		}

		[SecuritySafeCritical]
		internal void RestoreExceptionDispatchInfo(ExceptionDispatchInfo exceptionDispatchInfo)
		{
			bool flag = !Exception.IsImmutableAgileException(this);
			if (flag)
			{
				try
				{
				}
				finally
				{
					object currentStackTrace = (exceptionDispatchInfo.BinaryStackTraceArray == null) ? null : this.DeepCopyStackTrace(exceptionDispatchInfo.BinaryStackTraceArray);
					object dynamicMethodArray = (exceptionDispatchInfo.DynamicMethodArray == null) ? null : this.DeepCopyDynamicMethods(exceptionDispatchInfo.DynamicMethodArray);
					object obj = Exception.s_EDILock;
					lock (obj)
					{
						this._watsonBuckets = exceptionDispatchInfo.WatsonBuckets;
						this._ipForWatsonBuckets = exceptionDispatchInfo.IPForWatsonBuckets;
						this._remoteStackTraceString = exceptionDispatchInfo.RemoteStackTrace;
						Exception.SaveStackTracesFromDeepCopy(this, currentStackTrace, dynamicMethodArray);
					}
					this._stackTraceString = null;
					Exception.PrepareForForeignExceptionRaise();
				}
			}
		}

		[__DynamicallyInvokable]
		public int HResult
		{
			[__DynamicallyInvokable]
			get
			{
				return this._HResult;
			}
			[__DynamicallyInvokable]
			protected set
			{
				this._HResult = value;
			}
		}

		[SecurityCritical]
		internal virtual string InternalToString()
		{
			try
			{
				SecurityPermission securityPermission = new SecurityPermission(SecurityPermissionFlag.ControlEvidence | SecurityPermissionFlag.ControlPolicy);
				securityPermission.Assert();
			}
			catch
			{
			}
			bool needFileLineInfo = true;
			return this.ToString(needFileLineInfo, true);
		}

		[__DynamicallyInvokable]
		public new Type GetType()
		{
			return base.GetType();
		}

		internal bool IsTransient
		{
			[SecuritySafeCritical]
			get
			{
				return Exception.nIsTransient(this._HResult);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool nIsTransient(int hr);

		[SecuritySafeCritical]
		internal static string GetMessageFromNativeResources(Exception.ExceptionMessageKind kind)
		{
			string result = null;
			Exception.GetMessageFromNativeResources(kind, JitHelpers.GetStringHandleOnStack(ref result));
			return result;
		}

		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private static extern void GetMessageFromNativeResources(Exception.ExceptionMessageKind kind, StringHandleOnStack retMesg);

		[OptionalField]
		private static object s_EDILock = new object();

		private string _className;

		private MethodBase _exceptionMethod;

		private string _exceptionMethodString;

		internal string _message;

		private IDictionary _data;

		private Exception _innerException;

		private string _helpURL;

		private object _stackTrace;

		[OptionalField]
		private object _watsonBuckets;

		private string _stackTraceString;

		private string _remoteStackTraceString;

		private int _remoteStackIndex;

		private object _dynamicMethods;

		internal int _HResult;

		private string _source;

		private IntPtr _xptrs;

		private int _xcode;

		[OptionalField]
		private UIntPtr _ipForWatsonBuckets;

		[OptionalField(VersionAdded = 4)]
		private SafeSerializationManager _safeSerializationManager;

		private const int _COMPlusExceptionCode = -532462766;

		[Serializable]
		internal class __RestrictedErrorObject
		{
			internal __RestrictedErrorObject(object errorObject)
			{
				this._realErrorObject = errorObject;
			}

			public object RealErrorObject
			{
				get
				{
					return this._realErrorObject;
				}
			}

			[NonSerialized]
			private object _realErrorObject;
		}

		internal enum ExceptionMessageKind
		{
			ThreadAbort = 1,
			ThreadInterrupted,
			OutOfMemory
		}
	}
}
