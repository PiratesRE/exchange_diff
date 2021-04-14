using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal sealed class TypeInformation
	{
		internal string FullTypeName
		{
			get
			{
				return this.fullTypeName;
			}
		}

		internal string AssemblyString
		{
			get
			{
				return this.assemblyString;
			}
		}

		internal bool HasTypeForwardedFrom
		{
			get
			{
				return this.hasTypeForwardedFrom;
			}
		}

		internal TypeInformation(string fullTypeName, string assemblyString, bool hasTypeForwardedFrom)
		{
			this.fullTypeName = fullTypeName;
			this.assemblyString = assemblyString;
			this.hasTypeForwardedFrom = hasTypeForwardedFrom;
		}

		private string fullTypeName;

		private string assemblyString;

		private bool hasTypeForwardedFrom;
	}
}
