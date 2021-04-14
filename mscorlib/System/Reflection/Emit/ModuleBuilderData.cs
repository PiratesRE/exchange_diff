using System;
using System.IO;
using System.Security;

namespace System.Reflection.Emit
{
	[Serializable]
	internal class ModuleBuilderData
	{
		[SecurityCritical]
		internal ModuleBuilderData(ModuleBuilder module, string strModuleName, string strFileName, int tkFile)
		{
			this.m_globalTypeBuilder = new TypeBuilder(module);
			this.m_module = module;
			this.m_tkFile = tkFile;
			this.InitNames(strModuleName, strFileName);
		}

		[SecurityCritical]
		private void InitNames(string strModuleName, string strFileName)
		{
			this.m_strModuleName = strModuleName;
			if (strFileName == null)
			{
				this.m_strFileName = strModuleName;
				return;
			}
			string extension = Path.GetExtension(strFileName);
			if (extension == null || extension == string.Empty)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NoModuleFileExtension", new object[]
				{
					strFileName
				}));
			}
			this.m_strFileName = strFileName;
		}

		[SecurityCritical]
		internal virtual void ModifyModuleName(string strModuleName)
		{
			this.InitNames(strModuleName, null);
		}

		internal int FileToken
		{
			get
			{
				return this.m_tkFile;
			}
			set
			{
				this.m_tkFile = value;
			}
		}

		internal string m_strModuleName;

		internal string m_strFileName;

		internal bool m_fGlobalBeenCreated;

		internal bool m_fHasGlobal;

		[NonSerialized]
		internal TypeBuilder m_globalTypeBuilder;

		[NonSerialized]
		internal ModuleBuilder m_module;

		private int m_tkFile;

		internal bool m_isSaved;

		[NonSerialized]
		internal ResWriterData m_embeddedRes;

		internal const string MULTI_BYTE_VALUE_CLASS = "$ArrayType$";

		internal string m_strResourceFileName;

		internal byte[] m_resourceBytes;
	}
}
