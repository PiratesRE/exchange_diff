﻿using System;
using System.Reflection;

namespace System.Runtime.InteropServices.TCEAdapterGen
{
	internal class EventItfInfo
	{
		public EventItfInfo(string strEventItfName, string strSrcItfName, string strEventProviderName, RuntimeAssembly asmImport, RuntimeAssembly asmSrcItf)
		{
			this.m_strEventItfName = strEventItfName;
			this.m_strSrcItfName = strSrcItfName;
			this.m_strEventProviderName = strEventProviderName;
			this.m_asmImport = asmImport;
			this.m_asmSrcItf = asmSrcItf;
		}

		public Type GetEventItfType()
		{
			Type type = this.m_asmImport.GetType(this.m_strEventItfName, true, false);
			if (type != null && !type.IsVisible)
			{
				type = null;
			}
			return type;
		}

		public Type GetSrcItfType()
		{
			Type type = this.m_asmSrcItf.GetType(this.m_strSrcItfName, true, false);
			if (type != null && !type.IsVisible)
			{
				type = null;
			}
			return type;
		}

		public string GetEventProviderName()
		{
			return this.m_strEventProviderName;
		}

		private string m_strEventItfName;

		private string m_strSrcItfName;

		private string m_strEventProviderName;

		private RuntimeAssembly m_asmImport;

		private RuntimeAssembly m_asmSrcItf;
	}
}
