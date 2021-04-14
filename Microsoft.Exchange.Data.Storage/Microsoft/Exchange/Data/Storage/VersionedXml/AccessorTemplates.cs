using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class AccessorTemplates
	{
		internal static List<T> ListPropertyGetter<T>(ref List<T> field)
		{
			if (field == null)
			{
				field = new List<T>();
			}
			return field;
		}

		internal static void ListPropertySetter<T>(ref List<T> field, List<T> val)
		{
			if (val == null)
			{
				if (field != null)
				{
					field.Clear();
					return;
				}
			}
			else
			{
				if (field == null)
				{
					field = new List<T>(val);
					return;
				}
				field.Clear();
				field.AddRange(val);
			}
		}

		internal static T DefaultConstructionPropertyGetter<T>(ref T field) where T : new()
		{
			if (field == null)
			{
				field = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
			}
			return field;
		}
	}
}
