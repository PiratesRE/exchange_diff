using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Exchange.Security
{
	internal static class MetadataStringImporter
	{
		public static uint GetUserStrings(string assemblyPath, HashSet<string> results)
		{
			if (assemblyPath == null)
			{
				throw new ArgumentNullException("assemblyPath");
			}
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			uint num = 0U;
			uint num2 = 0U;
			uint[] array = new uint[256];
			char[] array2 = new char[256];
			object obj = null;
			MetadataStringImporter.IMetaDataImport metaDataImport = null;
			Guid guid = new Guid("7DAC8207-D3AE-4C75-9B67-92801A497D44");
			MetadataStringImporter.IMetaDataDispenserEx metaDataDispenserEx = (MetadataStringImporter.MetaDataDispenserEx)new MetadataStringImporter.CorMetaDataDispenserExClass();
			try
			{
				uint num3 = metaDataDispenserEx.OpenScope(assemblyPath, 16U, ref guid, out obj);
				if (num3 >= 2147483648U)
				{
					return num3;
				}
				metaDataImport = (MetadataStringImporter.IMetaDataImport)obj;
				while ((num3 = metaDataImport.EnumUserStrings(ref num, array, (uint)(array.Length * Marshal.SizeOf(typeof(uint))), out num2)) < 2147483648U && num2 > 0U)
				{
					uint num4 = 0U;
					int num5 = 0;
					while ((long)num5 < (long)((ulong)num2))
					{
						bool flag;
						do
						{
							flag = false;
							num3 = metaDataImport.GetUserString(array[num5], array2, (uint)array2.Length, out num4);
							if (num3 >= 2147483648U)
							{
								goto Block_6;
							}
							if ((ulong)num4 > (ulong)((long)array2.Length))
							{
								array2 = new char[num4 * 2U];
								flag = true;
							}
						}
						while (flag);
						string text = new string(array2, 0, (int)num4);
						text = string.Intern(text);
						if (!results.Contains(text))
						{
							results.Add(text);
						}
						num5++;
						continue;
						Block_6:
						return num3;
					}
				}
				if (num3 >= 2147483648U)
				{
					return num3;
				}
			}
			finally
			{
				if (metaDataImport != null)
				{
					Marshal.ReleaseComObject(metaDataImport);
				}
				if (metaDataDispenserEx != null)
				{
					Marshal.ReleaseComObject(metaDataDispenserEx);
				}
			}
			return 0U;
		}

		private const string MetadataImportIID = "7DAC8207-D3AE-4C75-9B67-92801A497D44";

		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[ComImport]
		private interface IMetaDataDispenserEx
		{
			uint DefineScope(ref Guid rclsid, uint dwCreateFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

			uint OpenScope([MarshalAs(UnmanagedType.LPWStr)] string szScope, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

			uint OpenScopeOnMemory(IntPtr pData, uint cbData, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

			uint SetOption(ref Guid optionid, [MarshalAs(UnmanagedType.Struct)] object value);

			uint GetOption(ref Guid optionid, [MarshalAs(UnmanagedType.Struct)] out object pvalue);

			uint OpenScopeOnITypeInfo([MarshalAs(UnmanagedType.Interface)] ITypeInfo pITI, uint dwOpenFlags, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIUnk);

			uint GetCORSystemDirectory([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] char[] szBuffer, uint cchBuffer, out uint pchBuffer);

			uint FindAssembly([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin, [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin, [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] char[] szName, uint cchName, out uint pcName);

			uint FindAssemblyModule([MarshalAs(UnmanagedType.LPWStr)] string szAppBase, [MarshalAs(UnmanagedType.LPWStr)] string szPrivateBin, [MarshalAs(UnmanagedType.LPWStr)] string szGlobalBin, [MarshalAs(UnmanagedType.LPWStr)] string szAssemblyName, [MarshalAs(UnmanagedType.LPWStr)] string szModuleName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] char[] szName, uint cchName, out uint pcName);
		}

		[Guid("E5CB7A31-7512-11D2-89CE-0080C792E5D8")]
		[ComImport]
		private class CorMetaDataDispenserExClass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern CorMetaDataDispenserExClass();
		}

		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		[CoClass(typeof(MetadataStringImporter.CorMetaDataDispenserExClass))]
		[ComImport]
		private interface MetaDataDispenserEx : MetadataStringImporter.IMetaDataDispenserEx
		{
		}

		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		[Guid("7DAC8207-D3AE-4C75-9B67-92801A497D44")]
		[ComImport]
		private interface IMetaDataImport
		{
			void CloseEnum(uint hEnum);

			uint CountEnum(uint hEnum, out uint count);

			uint ResetEnum(uint hEnum, uint ulPos);

			uint EnumTypeDefs(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rTypeDefs, uint cMax, out uint pcTypeDefs);

			uint EnumInterfaceImpls(ref uint phEnum, uint td, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rImpls, uint cMax, out uint pcImpls);

			uint EnumTypeRefs(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rTypeDefs, uint cMax, out uint pcTypeRefs);

			uint FindTypeDefByName([MarshalAs(UnmanagedType.LPWStr)] string szTypeDef, uint tkEnclosingClass, out uint ptd);

			uint GetScopeProps([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] char[] szName, uint cchName, out uint pchName, ref Guid pmvid);

			uint GetModuleFromScope(out uint pmd);

			uint GetTypeDefProps(uint td, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szTypeDef, uint cchTypeDef, out uint pchTypeDef, out uint pdwTypeDefFlags, out uint ptkExtends);

			uint GetInterfaceImplProps(uint iiImpl, out uint pClass, out uint ptkIface);

			uint GetTypeRefProps(uint tr, out uint ptkResolutionScope, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] char[] szName, uint cchName, out uint pchName);

			uint ResolveTypeRef(uint tr, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppIScope, out uint ptd);

			uint EnumMembers(ref uint phEnum, uint cl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rMembers, uint cMax, out uint pcTokens);

			uint EnumMembersWithName(ref uint phEnum, uint cl, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] uint[] rMembers, uint cMax, out uint pcTokens);

			uint EnumMethods(ref uint phEnum, uint cl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rMethods, uint cMax, out uint pcTokens);

			uint EnumMethodsWithName(ref uint phEnum, uint cl, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] uint[] rMethods, uint cMax, out uint pcTokens);

			uint EnumFields(ref uint phEnum, uint cl, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rFields, uint cMax, out uint pcTokens);

			uint EnumFieldsWithName(ref uint phEnum, uint cl, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)] uint[] rFields, uint cMax, out uint pcTokens);

			uint EnumParams(ref uint phEnum, uint mb, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rParams, uint cMax, out uint pcTokens);

			uint EnumMemberRefs(ref uint phEnum, uint tkParent, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rMemberRefs, uint cMax, out uint pcTokens);

			uint EnumMethodImpls(ref uint phEnum, uint td, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rMethodBody, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rMethodDecl, uint cMax, out uint pcTokens);

			uint EnumPermissionSets(ref uint phEnum, uint tk, uint dwActions, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rPermission, uint cMax, out uint pcTokens);

			uint FindMember(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pvSigBlob, uint cbSigBlob, out uint pmb);

			uint FindMethod(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pvSigBlob, uint cbSigBlob, out uint pmb);

			uint FindField(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pvSigBlob, uint cbSigBlob, out uint pmb);

			uint FindMemberRef(uint td, [MarshalAs(UnmanagedType.LPWStr)] string szName, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] pvSigBlob, int cbSigBlob, out uint pmr);

			uint GetMethodProps(uint mb, out uint pClass, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szMethod, uint cchMethod, out uint pchMethod, out uint pdwAttr, out IntPtr ppvSigBlob, out uint pcbSigBlob, out uint pulCodeRVA, out uint pdwImplFlags);

			uint GetMemberRefProps(uint mr, out uint ptk, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szMember, uint cchMember, out uint pchMember, out IntPtr ppvSigBlob, out uint pbSigBlob);

			uint EnumProperties(ref uint phEnum, uint td, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rProperties, uint cMax, out uint pcProperties);

			uint EnumEvents(ref uint phEnum, uint td, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rEvents, uint cMax, out uint pcEvents);

			uint GetEventProps(uint ev, out uint pClass, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szEvent, uint cchEvent, out uint pchEvent, out uint pdwEventFlags, out uint ptkEventType, out uint pmdAddOn, out uint pmdRemoveOn, out uint pmdFire, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 10)] uint[] rmdOtherMethod, uint cMax, out uint pcOtherMethod);

			uint EnumMethodSemantics(ref uint phEnum, uint mb, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] uint[] rEventProp, uint cMax, out uint pcEventProp);

			uint GetMethodSemantics(uint mb, uint tkEventProp, out uint pdwSemanticsFlags);

			uint GetClassLayout(uint td, out uint pdwPackSize, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] long[] rFieldOffset, uint cMax, out uint pcFieldOffset, out uint pulClassSize);

			uint GetFieldMarshal(uint tk, out IntPtr ppvNativeType, out uint pcbNativeType);

			uint GetRVA(uint tk, out uint pulCodeRVA, out uint pdwImplFlags);

			uint GetPermissionSetProps(uint pm, out uint pdwAction, out IntPtr ppvPermission, out uint pcbPermission);

			uint GetSigFromToken(uint mdSig, out IntPtr ppvSig, out uint pcbSig);

			uint GetModuleRefProps(uint mur, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] char[] szName, uint cchName, out uint pchName);

			uint EnumModuleRefs(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] rModuleRefs, uint cmax, out uint pcModuleRefs);

			uint GetTypeSpecFromToken(uint typespec, out IntPtr ppvSig, out uint pcbSig);

			uint GetNameFromToken(uint tk, out IntPtr pszUtf8NamePtr);

			uint EnumUnresolvedMethods(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] rMethods, uint cMax, out uint pcTokens);

			uint GetUserString(uint stk, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] char[] szString, uint cchString, out uint pchString);

			uint GetPinvokeMap(uint tk, out uint pdwMappingFlags, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szImportName, uint cchImportName, out uint pchImportName, out uint pmrImportDLL);

			uint EnumSignatures(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] rSignatures, uint cmax, out uint pcSignatures);

			uint EnumTypeSpecs(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] rTypeSpecs, uint cmax, out uint pcTypeSpecs);

			uint EnumUserStrings(ref uint phEnum, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] uint[] rStrings, uint cmax, out uint pcStrings);

			uint GetParamForMethodIndex(uint md, uint ulParamSeq, out uint ppd);

			uint EnumCustomAttributes(ref uint phEnum, uint tk, uint tkType, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] rCustomAttributes, uint cMax, out uint pcCustomAttributes);

			uint GetCustomAttributeProps(uint cv, out uint ptkObj, out uint ptkType, out IntPtr ppBlob, out uint pcbSize);

			uint FindTypeRef(uint tkResolutionScope, [MarshalAs(UnmanagedType.LPWStr)] string szName, out uint ptr);

			uint GetMemberProps(uint mb, out uint pClass, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szMember, uint cchMember, out uint pchMember, out uint pdwAttr, out IntPtr ppvSigBlob, out uint pcbSigBlob, out uint pulCodeRVA, out uint pdwImplFlags, out uint pdwCPlusTypeFlag, out IntPtr ppValue, out uint pcchValue);

			uint GetFieldProps(uint mb, out uint pClass, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szField, uint cchField, out uint pchField, out uint pdwAttr, out IntPtr ppvSigBlob, out uint pcbSigBlob, out uint pdwCPlusTypeFlag, out IntPtr ppValue, out uint pcchValue);

			uint GetPropertyProps(uint prop, out uint pClass, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] char[] szProperty, uint cchProperty, out uint pchProperty, out uint pdwPropFlags, out IntPtr ppvSig, out uint pbSig, out uint pdwCPlusTypeFlag, out IntPtr ppDefaultValue, out uint pcchDefaultValue, out uint pmdSetter, out uint pmdGetter, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 13)] uint[] rmdOtherMethod, uint cMax, out uint pcOtherMethod);

			uint GetParamProps(uint tk, out uint pmd, out uint pulSequence, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] char[] szName, uint cchName, out uint pchName, out uint pdwAttr, out uint pdwCPlusTypeFlag, out IntPtr ppValue, out uint pcchValue);

			uint GetCustomAttributeByName(uint tkObj, [MarshalAs(UnmanagedType.LPWStr)] string szName, out IntPtr ppData, out uint pcbData);

			bool IsValidToken(uint tk);

			uint GetNestedClassProps(uint tdNestedClass, out uint ptdEnclosingClass);

			uint GetNativeCallConvFromSig(IntPtr pvSig, uint cbSig, out uint pCallConv);

			uint IsGlobal(uint pd, out uint pbGlobal);
		}
	}
}
