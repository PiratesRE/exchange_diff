using System;
using System.Reflection;
using System.Runtime.Remoting.Metadata;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	internal class ArgMapper
	{
		[SecurityCritical]
		internal ArgMapper(IMethodMessage mm, bool fOut)
		{
			this._mm = mm;
			MethodBase methodBase = this._mm.MethodBase;
			this._methodCachedData = InternalRemotingServices.GetReflectionCachedData(methodBase);
			if (fOut)
			{
				this._map = this._methodCachedData.MarshalResponseArgMap;
				return;
			}
			this._map = this._methodCachedData.MarshalRequestArgMap;
		}

		[SecurityCritical]
		internal ArgMapper(MethodBase mb, bool fOut)
		{
			this._methodCachedData = InternalRemotingServices.GetReflectionCachedData(mb);
			if (fOut)
			{
				this._map = this._methodCachedData.MarshalResponseArgMap;
				return;
			}
			this._map = this._methodCachedData.MarshalRequestArgMap;
		}

		internal int[] Map
		{
			get
			{
				return this._map;
			}
		}

		internal int ArgCount
		{
			get
			{
				if (this._map == null)
				{
					return 0;
				}
				return this._map.Length;
			}
		}

		[SecurityCritical]
		internal object GetArg(int argNum)
		{
			if (this._map == null || argNum < 0 || argNum >= this._map.Length)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_InternalState"));
			}
			return this._mm.GetArg(this._map[argNum]);
		}

		[SecurityCritical]
		internal string GetArgName(int argNum)
		{
			if (this._map == null || argNum < 0 || argNum >= this._map.Length)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_InternalState"));
			}
			return this._mm.GetArgName(this._map[argNum]);
		}

		internal object[] Args
		{
			[SecurityCritical]
			get
			{
				if (this._map == null)
				{
					return null;
				}
				object[] array = new object[this._map.Length];
				for (int i = 0; i < this._map.Length; i++)
				{
					array[i] = this._mm.GetArg(this._map[i]);
				}
				return array;
			}
		}

		internal Type[] ArgTypes
		{
			get
			{
				Type[] array = null;
				if (this._map != null)
				{
					ParameterInfo[] parameters = this._methodCachedData.Parameters;
					array = new Type[this._map.Length];
					for (int i = 0; i < this._map.Length; i++)
					{
						array[i] = parameters[this._map[i]].ParameterType;
					}
				}
				return array;
			}
		}

		internal string[] ArgNames
		{
			get
			{
				string[] array = null;
				if (this._map != null)
				{
					ParameterInfo[] parameters = this._methodCachedData.Parameters;
					array = new string[this._map.Length];
					for (int i = 0; i < this._map.Length; i++)
					{
						array[i] = parameters[this._map[i]].Name;
					}
				}
				return array;
			}
		}

		internal static void GetParameterMaps(ParameterInfo[] parameters, out int[] inRefArgMap, out int[] outRefArgMap, out int[] outOnlyArgMap, out int[] nonRefOutArgMap, out int[] marshalRequestMap, out int[] marshalResponseMap)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int[] array = new int[parameters.Length];
			int[] array2 = new int[parameters.Length];
			int i = 0;
			foreach (ParameterInfo parameterInfo in parameters)
			{
				bool isIn = parameterInfo.IsIn;
				bool isOut = parameterInfo.IsOut;
				bool isByRef = parameterInfo.ParameterType.IsByRef;
				if (!isByRef)
				{
					num++;
					if (isOut)
					{
						num4++;
					}
				}
				else if (isOut)
				{
					num2++;
					num3++;
				}
				else
				{
					num++;
					num2++;
				}
				bool flag;
				bool flag2;
				if (isByRef)
				{
					if (isIn == isOut)
					{
						flag = true;
						flag2 = true;
					}
					else
					{
						flag = isIn;
						flag2 = isOut;
					}
				}
				else
				{
					flag = true;
					flag2 = isOut;
				}
				if (flag)
				{
					array[num5++] = i;
				}
				if (flag2)
				{
					array2[num6++] = i;
				}
				i++;
			}
			inRefArgMap = new int[num];
			outRefArgMap = new int[num2];
			outOnlyArgMap = new int[num3];
			nonRefOutArgMap = new int[num4];
			num = 0;
			num2 = 0;
			num3 = 0;
			num4 = 0;
			for (i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo2 = parameters[i];
				bool isOut2 = parameterInfo2.IsOut;
				if (!parameterInfo2.ParameterType.IsByRef)
				{
					inRefArgMap[num++] = i;
					if (isOut2)
					{
						nonRefOutArgMap[num4++] = i;
					}
				}
				else if (isOut2)
				{
					outRefArgMap[num2++] = i;
					outOnlyArgMap[num3++] = i;
				}
				else
				{
					inRefArgMap[num++] = i;
					outRefArgMap[num2++] = i;
				}
			}
			marshalRequestMap = new int[num5];
			Array.Copy(array, marshalRequestMap, num5);
			marshalResponseMap = new int[num6];
			Array.Copy(array2, marshalResponseMap, num6);
		}

		internal static object[] ExpandAsyncEndArgsToSyncArgs(RemotingMethodCachedData syncMethod, object[] asyncEndArgs)
		{
			object[] array = new object[syncMethod.Parameters.Length];
			int[] outRefArgMap = syncMethod.OutRefArgMap;
			for (int i = 0; i < outRefArgMap.Length; i++)
			{
				array[outRefArgMap[i]] = asyncEndArgs[i];
			}
			return array;
		}

		private int[] _map;

		private IMethodMessage _mm;

		private RemotingMethodCachedData _methodCachedData;
	}
}
