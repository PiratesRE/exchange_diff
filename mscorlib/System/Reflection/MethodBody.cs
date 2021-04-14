using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public class MethodBody
	{
		protected MethodBody()
		{
		}

		public virtual int LocalSignatureMetadataToken
		{
			get
			{
				return this.m_localSignatureMetadataToken;
			}
		}

		public virtual IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				return Array.AsReadOnly<LocalVariableInfo>(this.m_localVariables);
			}
		}

		public virtual int MaxStackSize
		{
			get
			{
				return this.m_maxStackSize;
			}
		}

		public virtual bool InitLocals
		{
			get
			{
				return this.m_initLocals;
			}
		}

		public virtual byte[] GetILAsByteArray()
		{
			return this.m_IL;
		}

		public virtual IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				return Array.AsReadOnly<ExceptionHandlingClause>(this.m_exceptionHandlingClauses);
			}
		}

		private byte[] m_IL;

		private ExceptionHandlingClause[] m_exceptionHandlingClauses;

		private LocalVariableInfo[] m_localVariables;

		internal MethodBase m_methodBase;

		private int m_localSignatureMetadataToken;

		private int m_maxStackSize;

		private bool m_initLocals;
	}
}
