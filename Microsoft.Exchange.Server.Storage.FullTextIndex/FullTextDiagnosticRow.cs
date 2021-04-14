using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Ceres.InteractionEngine.Services.Exchange;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.FullTextIndex
{
	public class FullTextDiagnosticRow
	{
		internal FullTextDiagnosticRow()
		{
		}

		public static ICollection<string> FastColumns
		{
			get
			{
				return FullTextDiagnosticRow.ColumnSetters.Keys;
			}
		}

		[Queryable(Visibility = Visibility.Public)]
		public int MailboxNumber { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		public int DocumentId { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("mailboxguid")]
		public string MailboxGuid { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("compositeitemid")]
		[Queryable(Visibility = Visibility.Public)]
		public string CompositItemId { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("documentid")]
		public long? FastIndexId { get; private set; }

		[Queryable(Visibility = Visibility.Redacted)]
		[FullTextDiagnosticRow.FastPropertyAttribute("subject")]
		public string Subject { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("to")]
		[Queryable(Visibility = Visibility.Redacted)]
		public string To { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("recipients")]
		[Queryable(Visibility = Visibility.Redacted)]
		public string Recipients { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("folderid")]
		public string FolderId { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("from")]
		[Queryable(Visibility = Visibility.Redacted)]
		public string From { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("importance")]
		[Queryable(Visibility = Visibility.Public)]
		public long? Importance { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("itemclass")]
		public string ItemClass { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("watermark")]
		[Queryable(Visibility = Visibility.Public)]
		public long? Watermark { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("errorcode")]
		[Queryable(Visibility = Visibility.Public)]
		public long? ErrorCode { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("errormessage")]
		public string ErrorMessage { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("attemptcount")]
		[Queryable(Visibility = Visibility.Public)]
		public long? AttemptCount { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("lastattempttime")]
		public DateTime? LastAttemptTime { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("ispartiallyprocessed")]
		[Queryable(Visibility = Visibility.Public)]
		public bool? IsPartiallyProcessed { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("conversationid")]
		[Queryable(Visibility = Visibility.Public)]
		public long? ConversationId { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("isread")]
		public bool? IsRead { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("hasirm")]
		public bool? HasIrm { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("iconindex")]
		[Queryable(Visibility = Visibility.Public)]
		public long? IconIndex { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("hasattachment")]
		public string HasAttachment { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("mid")]
		public long? Mid { get; private set; }

		[Queryable(Visibility = Visibility.Private)]
		[FullTextDiagnosticRow.FastPropertyAttribute("bodypreview")]
		public string BodyPreview { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("refinablereceived")]
		public DateTime? RefinableReceived { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("refinablefrom")]
		[Queryable(Visibility = Visibility.Redacted)]
		public string RefinableFrom { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("workingsetsource")]
		[Queryable(Visibility = Visibility.Public)]
		public long? WorkingSetSource { get; private set; }

		[FullTextDiagnosticRow.FastPropertyAttribute("workingsetsourcepartition")]
		[Queryable(Visibility = Visibility.Public)]
		public string WorkingSetSourcePartition { get; private set; }

		[Queryable(Visibility = Visibility.Public)]
		[FullTextDiagnosticRow.FastPropertyAttribute("workingsetid")]
		public string WorkingSetId { get; private set; }

		internal static Dictionary<string, PropertyInfo> ColumnSetters
		{
			get
			{
				if (FullTextDiagnosticRow.columnSetters == null)
				{
					PropertyInfo[] properties = typeof(FullTextDiagnosticRow).GetProperties(BindingFlags.Instance | BindingFlags.Public);
					Dictionary<string, PropertyInfo> dictionary = new Dictionary<string, PropertyInfo>(properties.Length);
					foreach (PropertyInfo propertyInfo in properties)
					{
						foreach (Attribute attribute in propertyInfo.GetCustomAttributes(false))
						{
							FullTextDiagnosticRow.FastPropertyAttribute fastPropertyAttribute = attribute as FullTextDiagnosticRow.FastPropertyAttribute;
							if (fastPropertyAttribute != null)
							{
								dictionary.Add(fastPropertyAttribute.FastPropertyName, propertyInfo);
							}
						}
					}
					FullTextDiagnosticRow.columnSetters = dictionary;
				}
				return FullTextDiagnosticRow.columnSetters;
			}
		}

		public static IEnumerable<FullTextDiagnosticRow> Parse(IEnumerable<SearchResultItem[]> pagedResults)
		{
			foreach (SearchResultItem[] page in pagedResults)
			{
				foreach (SearchResultItem item in page)
				{
					yield return FullTextDiagnosticRow.Parse(item);
				}
			}
			yield break;
		}

		internal void SetRowValue(string propertyName, object propertyValue)
		{
			IEnumerable enumerable = propertyValue as IEnumerable;
			object obj = (enumerable != null && propertyValue.GetType().IsGenericType) ? FullTextDiagnosticRow.GetEnumerableValue(enumerable) : propertyValue;
			PropertyInfo propertyInfo;
			if (FullTextDiagnosticRow.ColumnSetters.TryGetValue(propertyName, out propertyInfo))
			{
				if (obj != null)
				{
					Type type = obj.GetType();
					if (type != propertyInfo.PropertyType)
					{
						if (!(Nullable.GetUnderlyingType(propertyInfo.PropertyType) == type))
						{
							throw new FullTextIndexException((LID)2524327229U, ErrorCodeValue.FullTextIndexCallFailed, string.Format("Fast property '{0}' type '{1}' is inconsistent with value type '{2}'!", propertyName, obj.GetType(), propertyInfo.PropertyType));
						}
						obj = Convert.ChangeType(obj, Nullable.GetUnderlyingType(propertyInfo.PropertyType));
					}
				}
				propertyInfo.SetValue(this, obj, null);
			}
		}

		private static FullTextDiagnosticRow Parse(SearchResultItem item)
		{
			long num = (long)item.Fields[1].Value;
			SearchResultItem searchResultItem = item.Fields[2].Value as SearchResultItem;
			if (searchResultItem == null)
			{
				throw new ArgumentNullException("otherFields");
			}
			FullTextDiagnosticRow fullTextDiagnosticRow = new FullTextDiagnosticRow();
			fullTextDiagnosticRow.MailboxNumber = IndexId.GetMailboxNumber(num);
			fullTextDiagnosticRow.DocumentId = IndexId.GetDocumentId(num);
			foreach (IFieldHolder fieldHolder in searchResultItem.Fields)
			{
				fullTextDiagnosticRow.SetRowValue(fieldHolder.Name, fieldHolder.Value);
			}
			return fullTextDiagnosticRow;
		}

		private static string GetEnumerableValue(IEnumerable enumerableValue)
		{
			StringBuilder stringBuilder = new StringBuilder(20);
			foreach (object arg in enumerableValue)
			{
				stringBuilder.AppendFormat("{0};", arg);
			}
			int length = (stringBuilder.Length > 0) ? (stringBuilder.Length - 1) : 0;
			return stringBuilder.ToString(0, length);
		}

		private const int FastSearchResultsDocumentIdFieldIndex = 1;

		private const int FastSearchResultsOtherFieldIndex = 2;

		private static Dictionary<string, PropertyInfo> columnSetters;

		[AttributeUsage(AttributeTargets.Property, Inherited = false)]
		private class FastPropertyAttribute : Attribute
		{
			public FastPropertyAttribute(string fastPropertyName)
			{
				this.FastPropertyName = fastPropertyName;
			}

			public string FastPropertyName { get; private set; }
		}
	}
}
