using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[StructLayout(LayoutKind.Auto)]
	internal struct CustomAttributeNamedParameter
	{
		public CustomAttributeNamedParameter(string argumentName, CustomAttributeEncoding fieldOrProperty, CustomAttributeType type)
		{
			if (argumentName == null)
			{
				throw new ArgumentNullException("argumentName");
			}
			this.m_argumentName = argumentName;
			this.m_fieldOrProperty = fieldOrProperty;
			this.m_padding = fieldOrProperty;
			this.m_type = type;
			this.m_encodedArgument = default(CustomAttributeEncodedArgument);
		}

		public CustomAttributeEncodedArgument EncodedArgument
		{
			get
			{
				return this.m_encodedArgument;
			}
		}

		private string m_argumentName;

		private CustomAttributeEncoding m_fieldOrProperty;

		private CustomAttributeEncoding m_padding;

		private CustomAttributeType m_type;

		private CustomAttributeEncodedArgument m_encodedArgument;
	}
}
