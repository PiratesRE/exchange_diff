using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PropertyKeywordHelper
	{
		public static HashSet<PropertyKeyword> AllPropertyKeywords { get; private set; } = new HashSet<PropertyKeyword>((PropertyKeyword[])Enum.GetValues(typeof(PropertyKeyword)));

		public static HashSet<PropertyKeyword> CiPropertyKeywords { get; private set; }

		public static HashSet<PropertyKeyword> BasicPropertyKeywords { get; private set; }

		static PropertyKeywordHelper()
		{
			IEnumerable<PropertyKeyword> collection = from x in typeof(PropertyKeyword).GetTypeInfo().DeclaredFields
			where x.IsPublic && x.GetCustomAttributes(typeof(CIKeyword), false).Count<object>() > 0
			select (PropertyKeyword)x.GetValue(null);
			PropertyKeywordHelper.CiPropertyKeywords = new HashSet<PropertyKeyword>(collection);
			IEnumerable<PropertyKeyword> collection2 = from x in typeof(PropertyKeyword).GetTypeInfo().DeclaredFields
			where x.IsPublic && x.GetCustomAttributes(typeof(BasicKeyword), false).Count<object>() > 0
			select (PropertyKeyword)x.GetValue(null);
			PropertyKeywordHelper.BasicPropertyKeywords = new HashSet<PropertyKeyword>(collection2);
		}
	}
}
