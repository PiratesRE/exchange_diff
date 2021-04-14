using System;

namespace Microsoft.Exchange.Services.OData
{
	internal static class TypeExtensions
	{
		public static string MakeODataCollectionTypeName(this Type type)
		{
			return string.Format("Collection({0})", type.IsEnum ? type.FullName : type.FullName.Replace("System.", "Edm."));
		}
	}
}
