using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct IteratorCacher<TDelegate> where TDelegate : class
	{
		internal IteratorCacher(TDelegate sample, Func<TDelegate, IEnumerator<FastTransferStateMachine?>> factory)
		{
			this.iteratorInstance = factory(sample);
			IteratorCacher<TDelegate>.EnsureInitializerDelegate(((Delegate)((object)sample)).GetMethodInfo(), this.iteratorInstance.GetType());
		}

		internal object GetInstance()
		{
			Util.DisposeIfPresent(this.iteratorInstance as IDisposable);
			return this.iteratorInstance;
		}

		internal TDelegate GetInitializer()
		{
			return IteratorCacher<TDelegate>.delegateInstance;
		}

		private static void EnsureInitializerDelegate(MethodInfo method, Type displayClassType)
		{
			if (IteratorCacher<TDelegate>.delegateInstance != null)
			{
				return;
			}
			ConstructorInfo[] array = displayClassType.GetTypeInfo().DeclaredConstructors.ToArray<ConstructorInfo>();
			ParameterInfo[] parameters = method.GetParameters();
			List<Type> list = new List<Type>();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				list.Add(parameterInfo.ParameterType);
			}
			DynamicMethod dynamicMethod = new DynamicMethod(displayClassType.Name, typeof(IEnumerator<FastTransferStateMachine?>), list.ToArray(), typeof(FastTransferStateMachineFactory).GetTypeInfo().Module, true);
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.DeclareLocal(displayClassType);
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Castclass, displayClassType);
			ilgenerator.Emit(OpCodes.Stloc_0);
			ilgenerator.Emit(OpCodes.Ldloc_0);
			ilgenerator.Emit(OpCodes.Ldc_I4_0);
			ilgenerator.Emit(OpCodes.Call, array[0]);
			for (int j = 1; j < parameters.Length; j++)
			{
				FieldInfo declaredField = displayClassType.GetTypeInfo().GetDeclaredField(parameters[j].Name);
				if (declaredField == null)
				{
					throw new ArgumentException(string.Format("Cannot find field for parameter: {0}", parameters[j].Name));
				}
				ilgenerator.Emit(OpCodes.Ldloc_0);
				if (j < IteratorCacher<TDelegate>.Ldarg_N.Length)
				{
					ilgenerator.Emit(IteratorCacher<TDelegate>.Ldarg_N[j]);
				}
				else
				{
					ilgenerator.Emit(OpCodes.Ldarg_S, (byte)j);
				}
				ilgenerator.Emit(OpCodes.Stfld, declaredField);
			}
			ilgenerator.Emit(OpCodes.Ldloc_0);
			ilgenerator.Emit(OpCodes.Ret);
			IteratorCacher<TDelegate>.delegateInstance = (TDelegate)((object)dynamicMethod.CreateDelegate(typeof(TDelegate)));
		}

		private static readonly OpCode[] Ldarg_N = new OpCode[]
		{
			OpCodes.Ldarg_0,
			OpCodes.Ldarg_1,
			OpCodes.Ldarg_2,
			OpCodes.Ldarg_3
		};

		private static TDelegate delegateInstance;

		private readonly object iteratorInstance;
	}
}
