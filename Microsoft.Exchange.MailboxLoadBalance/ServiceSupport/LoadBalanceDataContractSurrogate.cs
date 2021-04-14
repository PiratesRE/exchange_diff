using System;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceDataContractSurrogate : IDataContractSurrogate
	{
		public object GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
		{
			return null;
		}

		public object GetCustomDataToExport(Type clrType, Type dataContractType)
		{
			return null;
		}

		public Type GetDataContractType(Type type)
		{
			if (typeof(ByteQuantifiedSize).IsAssignableFrom(type))
			{
				return typeof(ByteQuantifiedSizeSurrogate);
			}
			return type;
		}

		public object GetDeserializedObject(object obj, Type targetType)
		{
			ByteQuantifiedSizeSurrogate byteQuantifiedSizeSurrogate = obj as ByteQuantifiedSizeSurrogate;
			if (byteQuantifiedSizeSurrogate != null)
			{
				return byteQuantifiedSizeSurrogate.ToByteQuantifiedSize();
			}
			return obj;
		}

		public void GetKnownCustomDataTypes(Collection<Type> customDataTypes)
		{
		}

		public object GetObjectToSerialize(object obj, Type targetType)
		{
			if (obj is ByteQuantifiedSize)
			{
				return new ByteQuantifiedSizeSurrogate((ByteQuantifiedSize)obj);
			}
			return obj;
		}

		public Type GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
		{
			if (typeName.Equals(typeof(ByteQuantifiedSizeSurrogate).Name))
			{
				return typeof(ByteQuantifiedSize);
			}
			return null;
		}

		public CodeTypeDeclaration ProcessImportedType(CodeTypeDeclaration typeDeclaration, CodeCompileUnit compileUnit)
		{
			return typeDeclaration;
		}
	}
}
