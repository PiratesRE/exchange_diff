using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics
{
	public static class ReflectionHelper
	{
		public static TResult TraverseTypeHierarchy<TResult, TParam>(Type type, TParam param, MatchType<TResult, TParam> matchType) where TResult : class
		{
			TResult tresult = default(TResult);
			Type baseType = type;
			while (tresult == null && type != null)
			{
				tresult = matchType(baseType, type, param);
				type = type.GetTypeInfo().BaseType;
			}
			return tresult;
		}

		public static List<TResult> AggregateTypeHierarchy<TResult>(Type type, AggregateType<TResult> aggregateType) where TResult : class
		{
			List<TResult> list = new List<TResult>();
			Type baseType = type;
			while (type != null)
			{
				aggregateType(baseType, type, list);
				type = type.GetTypeInfo().BaseType;
			}
			return list;
		}

		public static FieldInfo MatchStaticField(Type baseType, Type type, string fieldName)
		{
			FieldInfo declaredField = type.GetTypeInfo().GetDeclaredField(fieldName);
			if (declaredField != null && !declaredField.IsPublic && declaredField.IsStatic && (!declaredField.IsPrivate || (declaredField.IsPrivate && baseType == type)))
			{
				return declaredField;
			}
			return null;
		}

		public static FieldInfo MatchInstanceField(Type baseType, Type type, string fieldName)
		{
			FieldInfo declaredField = type.GetTypeInfo().GetDeclaredField(fieldName);
			if (declaredField != null && !declaredField.IsPublic && !declaredField.IsStatic && (!declaredField.IsPrivate || (declaredField.IsPrivate && baseType == type)))
			{
				return declaredField;
			}
			return null;
		}

		public static void AggregateStaticFields(Type baseType, Type type, List<FieldInfo> fields)
		{
			foreach (FieldInfo fieldInfo in type.GetTypeInfo().DeclaredFields)
			{
				if (fieldInfo.IsStatic && (!fieldInfo.IsPrivate || (fieldInfo.IsPrivate && baseType == type)))
				{
					fields.Add(fieldInfo);
				}
			}
		}

		public static bool IsInstanceOfType(object instanceToCheck, Type typeToCheckAgainst)
		{
			bool result = false;
			if (instanceToCheck != null && typeToCheckAgainst != null)
			{
				result = typeToCheckAgainst.GetTypeInfo().IsAssignableFrom(instanceToCheck.GetType().GetTypeInfo());
			}
			return result;
		}

		public static bool HasParameterlessConstructor(Type typeToCheck)
		{
			bool result = false;
			foreach (ConstructorInfo constructorInfo in typeToCheck.GetTypeInfo().DeclaredConstructors)
			{
				if (constructorInfo.GetParameters().Length == 0)
				{
					result = (constructorInfo.IsPublic || constructorInfo.IsStatic);
					break;
				}
			}
			return result;
		}
	}
}
