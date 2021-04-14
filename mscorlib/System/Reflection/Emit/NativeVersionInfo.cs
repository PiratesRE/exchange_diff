using System;

namespace System.Reflection.Emit
{
	internal class NativeVersionInfo
	{
		internal NativeVersionInfo()
		{
			this.m_strDescription = null;
			this.m_strCompany = null;
			this.m_strTitle = null;
			this.m_strCopyright = null;
			this.m_strTrademark = null;
			this.m_strProduct = null;
			this.m_strProductVersion = null;
			this.m_strFileVersion = null;
			this.m_lcid = -1;
		}

		internal string m_strDescription;

		internal string m_strCompany;

		internal string m_strTitle;

		internal string m_strCopyright;

		internal string m_strTrademark;

		internal string m_strProduct;

		internal string m_strProductVersion;

		internal string m_strFileVersion;

		internal int m_lcid;
	}
}
