using System;
using System.Security;

namespace System.Globalization
{
	[Serializable]
	internal class CodePageDataItem
	{
		[SecurityCritical]
		internal unsafe CodePageDataItem(int dataIndex)
		{
			this.m_dataIndex = dataIndex;
			this.m_uiFamilyCodePage = (int)EncodingTable.codePageDataPtr[dataIndex].uiFamilyCodePage;
			this.m_flags = EncodingTable.codePageDataPtr[dataIndex].flags;
		}

		[SecurityCritical]
		internal unsafe static string CreateString(sbyte* pStrings, uint index)
		{
			if (*pStrings == 124)
			{
				int num = 1;
				int num2 = 1;
				for (;;)
				{
					sbyte b = pStrings[num2];
					if (b == 124 || b == 0)
					{
						if (index == 0U)
						{
							break;
						}
						index -= 1U;
						num = num2 + 1;
						if (b == 0)
						{
							goto IL_37;
						}
					}
					num2++;
				}
				return new string(pStrings, num, num2 - num);
				IL_37:
				throw new ArgumentException("pStrings");
			}
			return new string(pStrings);
		}

		public unsafe string WebName
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_webName == null)
				{
					this.m_webName = CodePageDataItem.CreateString(EncodingTable.codePageDataPtr[this.m_dataIndex].Names, 0U);
				}
				return this.m_webName;
			}
		}

		public virtual int UIFamilyCodePage
		{
			get
			{
				return this.m_uiFamilyCodePage;
			}
		}

		public unsafe string HeaderName
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_headerName == null)
				{
					this.m_headerName = CodePageDataItem.CreateString(EncodingTable.codePageDataPtr[this.m_dataIndex].Names, 1U);
				}
				return this.m_headerName;
			}
		}

		public unsafe string BodyName
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_bodyName == null)
				{
					this.m_bodyName = CodePageDataItem.CreateString(EncodingTable.codePageDataPtr[this.m_dataIndex].Names, 2U);
				}
				return this.m_bodyName;
			}
		}

		public uint Flags
		{
			get
			{
				return this.m_flags;
			}
		}

		internal int m_dataIndex;

		internal int m_uiFamilyCodePage;

		internal string m_webName;

		internal string m_headerName;

		internal string m_bodyName;

		internal uint m_flags;
	}
}
