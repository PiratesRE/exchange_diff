using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Threading;

namespace System.Diagnostics
{
	[Serializable]
	internal class StackFrameHelper : IDisposable
	{
		public StackFrameHelper(Thread target)
		{
			this.targetThread = target;
			this.rgMethodBase = null;
			this.rgMethodHandle = null;
			this.rgiMethodToken = null;
			this.rgiOffset = null;
			this.rgiILOffset = null;
			this.rgAssemblyPath = null;
			this.rgLoadedPeAddress = null;
			this.rgiLoadedPeSize = null;
			this.rgInMemoryPdbAddress = null;
			this.rgiInMemoryPdbSize = null;
			this.dynamicMethods = null;
			this.rgFilename = null;
			this.rgiLineNumber = null;
			this.rgiColumnNumber = null;
			this.rgiLastFrameFromForeignExceptionStackTrace = null;
			this.iFrameCount = 0;
		}

		[SecuritySafeCritical]
		internal void InitializeSourceInfo(int iSkip, bool fNeedFileInfo, Exception exception)
		{
			StackTrace.GetStackFramesInternal(this, iSkip, fNeedFileInfo, exception);
			if (!fNeedFileInfo)
			{
				return;
			}
			if (!RuntimeFeature.IsSupported("PortablePdb"))
			{
				return;
			}
			if (StackFrameHelper.t_reentrancy > 0)
			{
				return;
			}
			StackFrameHelper.t_reentrancy++;
			try
			{
				if (!CodeAccessSecurityEngine.QuickCheckForAllDemands())
				{
					new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
					new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
				}
				if (StackFrameHelper.s_getSourceLineInfo == null)
				{
					Type type = Type.GetType("System.Diagnostics.StackTraceSymbols, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false);
					if (type == null)
					{
						return;
					}
					MethodInfo method = type.GetMethod("GetSourceLineInfoWithoutCasAssert", new Type[]
					{
						typeof(string),
						typeof(IntPtr),
						typeof(int),
						typeof(IntPtr),
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(string).MakeByRefType(),
						typeof(int).MakeByRefType(),
						typeof(int).MakeByRefType()
					});
					if (method == null)
					{
						method = type.GetMethod("GetSourceLineInfo", new Type[]
						{
							typeof(string),
							typeof(IntPtr),
							typeof(int),
							typeof(IntPtr),
							typeof(int),
							typeof(int),
							typeof(int),
							typeof(string).MakeByRefType(),
							typeof(int).MakeByRefType(),
							typeof(int).MakeByRefType()
						});
					}
					if (method == null)
					{
						return;
					}
					object target = Activator.CreateInstance(type);
					StackFrameHelper.GetSourceLineInfoDelegate value = (StackFrameHelper.GetSourceLineInfoDelegate)method.CreateDelegate(typeof(StackFrameHelper.GetSourceLineInfoDelegate), target);
					Interlocked.CompareExchange<StackFrameHelper.GetSourceLineInfoDelegate>(ref StackFrameHelper.s_getSourceLineInfo, value, null);
				}
				for (int i = 0; i < this.iFrameCount; i++)
				{
					if (this.rgiMethodToken[i] != 0)
					{
						StackFrameHelper.s_getSourceLineInfo(this.rgAssemblyPath[i], this.rgLoadedPeAddress[i], this.rgiLoadedPeSize[i], this.rgInMemoryPdbAddress[i], this.rgiInMemoryPdbSize[i], this.rgiMethodToken[i], this.rgiILOffset[i], out this.rgFilename[i], out this.rgiLineNumber[i], out this.rgiColumnNumber[i]);
					}
				}
			}
			catch
			{
			}
			finally
			{
				StackFrameHelper.t_reentrancy--;
			}
		}

		void IDisposable.Dispose()
		{
		}

		[SecuritySafeCritical]
		public virtual MethodBase GetMethodBase(int i)
		{
			IntPtr methodHandleValue = this.rgMethodHandle[i];
			if (methodHandleValue.IsNull())
			{
				return null;
			}
			IRuntimeMethodInfo typicalMethodDefinition = RuntimeMethodHandle.GetTypicalMethodDefinition(new RuntimeMethodInfoStub(methodHandleValue, this));
			return RuntimeType.GetMethodBase(typicalMethodDefinition);
		}

		public virtual int GetOffset(int i)
		{
			return this.rgiOffset[i];
		}

		public virtual int GetILOffset(int i)
		{
			return this.rgiILOffset[i];
		}

		public virtual string GetFilename(int i)
		{
			if (this.rgFilename != null)
			{
				return this.rgFilename[i];
			}
			return null;
		}

		public virtual int GetLineNumber(int i)
		{
			if (this.rgiLineNumber != null)
			{
				return this.rgiLineNumber[i];
			}
			return 0;
		}

		public virtual int GetColumnNumber(int i)
		{
			if (this.rgiColumnNumber != null)
			{
				return this.rgiColumnNumber[i];
			}
			return 0;
		}

		public virtual bool IsLastFrameFromForeignExceptionStackTrace(int i)
		{
			return this.rgiLastFrameFromForeignExceptionStackTrace != null && this.rgiLastFrameFromForeignExceptionStackTrace[i];
		}

		public virtual int GetNumberOfFrames()
		{
			return this.iFrameCount;
		}

		public virtual void SetNumberOfFrames(int i)
		{
			this.iFrameCount = i;
		}

		[OnSerializing]
		[SecuritySafeCritical]
		private void OnSerializing(StreamingContext context)
		{
			this.rgMethodBase = ((this.rgMethodHandle == null) ? null : new MethodBase[this.rgMethodHandle.Length]);
			if (this.rgMethodHandle != null)
			{
				for (int i = 0; i < this.rgMethodHandle.Length; i++)
				{
					if (!this.rgMethodHandle[i].IsNull())
					{
						this.rgMethodBase[i] = RuntimeType.GetMethodBase(new RuntimeMethodInfoStub(this.rgMethodHandle[i], this));
					}
				}
			}
		}

		[OnSerialized]
		private void OnSerialized(StreamingContext context)
		{
			this.rgMethodBase = null;
		}

		[OnDeserialized]
		[SecuritySafeCritical]
		private void OnDeserialized(StreamingContext context)
		{
			this.rgMethodHandle = ((this.rgMethodBase == null) ? null : new IntPtr[this.rgMethodBase.Length]);
			if (this.rgMethodBase != null)
			{
				for (int i = 0; i < this.rgMethodBase.Length; i++)
				{
					if (this.rgMethodBase[i] != null)
					{
						this.rgMethodHandle[i] = this.rgMethodBase[i].MethodHandle.Value;
					}
				}
			}
			this.rgMethodBase = null;
		}

		[NonSerialized]
		private Thread targetThread;

		private int[] rgiOffset;

		private int[] rgiILOffset;

		private MethodBase[] rgMethodBase;

		private object dynamicMethods;

		[NonSerialized]
		private IntPtr[] rgMethodHandle;

		private string[] rgAssemblyPath;

		private IntPtr[] rgLoadedPeAddress;

		private int[] rgiLoadedPeSize;

		private IntPtr[] rgInMemoryPdbAddress;

		private int[] rgiInMemoryPdbSize;

		private int[] rgiMethodToken;

		private string[] rgFilename;

		private int[] rgiLineNumber;

		private int[] rgiColumnNumber;

		[OptionalField]
		private bool[] rgiLastFrameFromForeignExceptionStackTrace;

		private int iFrameCount;

		private static StackFrameHelper.GetSourceLineInfoDelegate s_getSourceLineInfo;

		[ThreadStatic]
		private static int t_reentrancy;

		private delegate void GetSourceLineInfoDelegate(string assemblyPath, IntPtr loadedPeAddress, int loadedPeSize, IntPtr inMemoryPdbAddress, int inMemoryPdbSize, int methodToken, int ilOffset, out string sourceFile, out int sourceLine, out int sourceColumn);
	}
}
