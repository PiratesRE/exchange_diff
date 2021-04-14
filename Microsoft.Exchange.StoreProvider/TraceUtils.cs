using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TraceUtils : ComponentTrace<MapiNetTags>
	{
		public static string MakeString(object someObject)
		{
			if (someObject == null)
			{
				return "null";
			}
			if (someObject is string)
			{
				return "\"" + someObject + "\"";
			}
			return someObject.ToString();
		}

		public static string MakeHash(object someObject)
		{
			if (someObject != null)
			{
				return someObject.GetHashCode().ToString() + "(hash)";
			}
			return "null";
		}

		public static string DumpArray(Array someArray)
		{
			if (someArray != null)
			{
				return "[..., array_len=" + someArray.Length.ToString() + "]";
			}
			return "null";
		}

		public static string DumpCollection<T>(ICollection<T> collection)
		{
			if (collection != null)
			{
				return "[..., collection_len=" + collection.Count.ToString() + "]";
			}
			return "null";
		}

		public static string DumpEntryId(byte[] entryId)
		{
			if (entryId == null)
			{
				return "null";
			}
			return string.Format("[len={0}, data={1}]", entryId.GetLength(0), TraceUtils.DumpBytes(entryId));
		}

		public static string DumpBytes(byte[] bytes)
		{
			if (bytes == null)
			{
				return "null";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in bytes)
			{
				stringBuilder.Append(b.ToString("X2"));
			}
			return stringBuilder.ToString();
		}

		public static string DumpMvString(string[] strings)
		{
			if (strings == null)
			{
				return "null";
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string str in strings)
			{
				stringBuilder.Append(str + "; ");
			}
			return stringBuilder.ToString();
		}

		public static string DumpPropTag(PropTag ptag)
		{
			if (ptag < PropTag.Null)
			{
				return string.Format("0x{0:x}(NamedProp)", (int)ptag);
			}
			if (EnumValidator<PropTag>.IsValidValue(ptag))
			{
				return string.Format("0x{0:x}({1})", (int)ptag, ptag);
			}
			return string.Format("0x{0:x}", (int)ptag);
		}

		public static string DumpPropTagsArray(ICollection<PropTag> ptagsArray)
		{
			return TraceUtils.DumpCollection<PropTag>(ptagsArray);
		}

		public static string DumpNamedPropsArray(ICollection<NamedProp> namedPropsArray)
		{
			return TraceUtils.DumpCollection<NamedProp>(namedPropsArray);
		}

		public static string DumpPropVal(PropValue propVal)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("[Tag:{0}, Value:", TraceUtils.DumpPropTag(propVal.PropTag));
			if (propVal.RawValue == null)
			{
				stringBuilder.Append("null");
			}
			else if (propVal.IsError())
			{
				stringBuilder.AppendFormat("{0}(error)", propVal.GetErrorValue());
			}
			else if (propVal.Value is short)
			{
				stringBuilder.AppendFormat("{0}(short)", propVal.Value.ToString());
			}
			else if (propVal.Value is int)
			{
				stringBuilder.AppendFormat("{0}(int)", propVal.Value.ToString());
			}
			else if (propVal.Value is float)
			{
				stringBuilder.AppendFormat("{0}(float)", propVal.Value.ToString());
			}
			else if (propVal.Value is double)
			{
				stringBuilder.AppendFormat("{0}(double)", propVal.Value.ToString());
			}
			else if (propVal.Value is bool)
			{
				stringBuilder.AppendFormat("{0}(bool)", propVal.Value.ToString());
			}
			else if (propVal.Value is long)
			{
				stringBuilder.AppendFormat("{0}(long)", propVal.Value.ToString());
			}
			else if (propVal.Value is string)
			{
				stringBuilder.AppendFormat("\"{0}\"(string)", propVal.Value);
			}
			else if (propVal.Value is Guid)
			{
				stringBuilder.AppendFormat("{0}(Guid)", propVal.Value.ToString());
			}
			else if (propVal.Value is byte[])
			{
				stringBuilder.AppendFormat("cb = {0}, lpb = {1}", propVal.GetBytes().GetLength(0), TraceUtils.DumpBytes(propVal.GetBytes()));
			}
			else if (propVal.Value is string[])
			{
				stringBuilder.AppendFormat("MvString: cValues = {0}. Values = {1}", propVal.GetStringArray().GetLength(0), TraceUtils.DumpMvString(propVal.GetStringArray()));
			}
			else if (propVal.Value != null)
			{
				stringBuilder.AppendFormat("({0})", propVal.Value.ToString());
			}
			else
			{
				stringBuilder.AppendFormat("({0})", propVal.RawValue.GetType().ToString());
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public static string DumpPropValsArray(ICollection<PropValue> propValsArray)
		{
			return TraceUtils.DumpCollection<PropValue>(propValsArray);
		}

		public static string DumpPropProblemsArray(PropProblem[] propProblemsArray)
		{
			return TraceUtils.DumpArray(propProblemsArray);
		}

		public static string DumpPropValsMatrix(PropValue[][] propValsMatrix)
		{
			return TraceUtils.DumpArray(propValsMatrix);
		}
	}
}
