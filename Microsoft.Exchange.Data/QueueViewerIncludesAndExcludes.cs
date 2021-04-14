using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class QueueViewerIncludesAndExcludes
	{
		private QueueViewerIncludesAndExcludes(int[] filter)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			this.filter = filter;
		}

		public static QueueViewerIncludesAndExcludes Parse(string s)
		{
			if (s == null)
			{
				throw new LocalizedException(DataStrings.IncludeExcludeInvalid(s));
			}
			int[] array = new int[QueueViewerIncludesAndExcludes.MaxFilterCount];
			string[] array2 = s.Split(new char[]
			{
				','
			});
			foreach (string text in array2)
			{
				DeliveryType deliveryType;
				NextHopCategory nextHopCategory;
				if (EnumValidator.TryParse<DeliveryType>(text, EnumParseOptions.IgnoreCase, out deliveryType))
				{
					array[(int)deliveryType] = 1;
				}
				else if (EnumValidator.TryParse<NextHopCategory>(text, EnumParseOptions.IgnoreCase, out nextHopCategory))
				{
					array[(int)(nextHopCategory + QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex)] = 1;
				}
				else if (StringComparer.OrdinalIgnoreCase.Compare("Empty", text.Trim()) == 0)
				{
					array[QueueViewerIncludesAndExcludes.EmptyIndex] = 1;
				}
				else
				{
					if (StringComparer.OrdinalIgnoreCase.Compare("SER", text.Trim()) != 0)
					{
						throw new LocalizedException(DataStrings.IncludeExcludeInvalid(text));
					}
					array[10] = 1;
					array[QueueViewerIncludesAndExcludes.EmptyIndex] = 1;
					array[QueueViewerIncludesAndExcludes.HighRiskIndex] = 1;
				}
			}
			return new QueueViewerIncludesAndExcludes(array);
		}

		public static bool ComposeFilterString(string filter, QueueViewerIncludesAndExcludes excluding, QueueViewerIncludesAndExcludes including, out string combinedFilter, out LocalizedString error)
		{
			error = LocalizedString.Empty;
			combinedFilter = string.Empty;
			if (including == null && excluding == null)
			{
				combinedFilter = filter;
				return true;
			}
			bool includeDeliveryType = false;
			bool includeNextHop = false;
			int[] array = new int[QueueViewerIncludesAndExcludes.MaxFilterCount];
			int[] array2 = (including != null) ? including.filter : new int[QueueViewerIncludesAndExcludes.MaxFilterCount];
			int[] array3 = (excluding != null) ? excluding.filter : new int[QueueViewerIncludesAndExcludes.MaxFilterCount];
			if (filter == null)
			{
				filter = string.Empty;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array3[i] != 0 || array2[i] != 0)
				{
					if (array3[i] == 1 && array2[i] == 1)
					{
						error = DataStrings.IncludeExcludeConflict(QueueViewerIncludesAndExcludes.GetName(i));
						return false;
					}
					if (array3[i] == 1)
					{
						array[i] = array3[i] * -1;
					}
					else
					{
						if (array2[i] != 1)
						{
							throw new InvalidOperationException(string.Format("Invalid filter value found. Exclude: {0}, Include: {1}", array3[i], array2[i]));
						}
						array[i] = array2[i];
						if (QueueViewerIncludesAndExcludes.IsDeliveryType(i))
						{
							includeDeliveryType = true;
						}
						else if (QueueViewerIncludesAndExcludes.IsNextHopCategory(i))
						{
							includeNextHop = true;
						}
					}
				}
			}
			combinedFilter = QueueViewerIncludesAndExcludes.CreateFilterString(filter, array, includeDeliveryType, includeNextHop);
			return true;
		}

		public bool Equals(QueueViewerIncludesAndExcludes other)
		{
			for (int i = 0; i < this.filter.Length; i++)
			{
				if (this.filter[i] != other.filter[i])
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			QueueViewerIncludesAndExcludes queueViewerIncludesAndExcludes = obj as QueueViewerIncludesAndExcludes;
			return queueViewerIncludesAndExcludes != null && this.Equals(queueViewerIncludesAndExcludes);
		}

		public override int GetHashCode()
		{
			return this.filter.GetHashCode();
		}

		public override string ToString()
		{
			if (this.filterString == null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				string arg = string.Empty;
				for (int i = 0; i < QueueViewerIncludesAndExcludes.deliveryTypeNames.Length; i++)
				{
					if (this.filter[i] == 1)
					{
						stringBuilder.AppendFormat("{0} {1} -eq {2}", arg, QueueViewerIncludesAndExcludes.DeliveryTypeProperty, QueueViewerIncludesAndExcludes.GetName(i));
						arg = " -AND ";
					}
				}
				for (int j = QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex; j < QueueViewerIncludesAndExcludes.nextHopCategoryNames.Length + QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex; j++)
				{
					if (this.filter[j] == 1)
					{
						stringBuilder.AppendFormat("{0} {1} -eq {2}", arg, QueueViewerIncludesAndExcludes.NextHopCategoryProperty, QueueViewerIncludesAndExcludes.GetName(j));
						arg = " -AND ";
					}
				}
				if (this.filter[QueueViewerIncludesAndExcludes.EmptyIndex] == 1)
				{
					stringBuilder.AppendFormat("{0} empty", arg);
				}
				if (this.filter[QueueViewerIncludesAndExcludes.HighRiskIndex] == 1)
				{
					stringBuilder.AppendFormat("{0} {1}", arg, "High");
				}
				this.filterString = stringBuilder.ToString();
			}
			return this.filterString;
		}

		private static string CreateFilterString(string filter, int[] combinedIncludeExclude, bool includeDeliveryType, bool includeNextHop)
		{
			StringBuilder stringBuilder = new StringBuilder(filter);
			string arg = (filter.Length > 0) ? " -AND " : string.Empty;
			for (int i = 0; i < QueueViewerIncludesAndExcludes.deliveryTypeNames.Length; i++)
			{
				if (combinedIncludeExclude[i] < 0 || (includeDeliveryType && combinedIncludeExclude[i] == 0))
				{
					stringBuilder.AppendFormat("{0} {1} -ne '{2}'", arg, QueueViewerIncludesAndExcludes.DeliveryTypeProperty, QueueViewerIncludesAndExcludes.GetName(i));
					arg = " -AND ";
				}
			}
			for (int j = QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex; j < QueueViewerIncludesAndExcludes.nextHopCategoryNames.Length + QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex; j++)
			{
				if (combinedIncludeExclude[j] < 0 || (includeNextHop && combinedIncludeExclude[j] == 0))
				{
					stringBuilder.AppendFormat("{0} {1} -ne '{2}'", arg, QueueViewerIncludesAndExcludes.NextHopCategoryProperty, QueueViewerIncludesAndExcludes.GetName(j));
					arg = " -AND ";
				}
			}
			if (combinedIncludeExclude[QueueViewerIncludesAndExcludes.EmptyIndex] < 0)
			{
				stringBuilder.AppendFormat("{0} {1} -gt 0", arg, QueueViewerIncludesAndExcludes.MessageCountProperty);
				arg = " -AND ";
			}
			else if (combinedIncludeExclude[QueueViewerIncludesAndExcludes.EmptyIndex] > 0)
			{
				stringBuilder.AppendFormat("{0} {1} -eq 0", arg, QueueViewerIncludesAndExcludes.MessageCountProperty);
				arg = " -AND ";
			}
			if (combinedIncludeExclude[QueueViewerIncludesAndExcludes.HighRiskIndex] < 0)
			{
				stringBuilder.AppendFormat("{0} {1} -ne '{2}'", arg, QueueViewerIncludesAndExcludes.RiskLevelProperty, "High");
			}
			else if (combinedIncludeExclude[QueueViewerIncludesAndExcludes.HighRiskIndex] > 0)
			{
				stringBuilder.AppendFormat("{0} {1} -eq '{2}'", arg, QueueViewerIncludesAndExcludes.RiskLevelProperty, "High");
			}
			return stringBuilder.ToString();
		}

		private static string GetName(int filter)
		{
			if (filter < QueueViewerIncludesAndExcludes.deliveryTypeNames.Length)
			{
				return QueueViewerIncludesAndExcludes.deliveryTypeNames[filter];
			}
			if (filter < QueueViewerIncludesAndExcludes.nextHopCategoryNames.Length + QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex)
			{
				return QueueViewerIncludesAndExcludes.nextHopCategoryNames[filter - QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex];
			}
			if (filter == QueueViewerIncludesAndExcludes.EmptyIndex)
			{
				return "Empty";
			}
			if (filter == QueueViewerIncludesAndExcludes.HighRiskIndex)
			{
				return "High";
			}
			throw new InvalidOperationException(string.Format("Unexpected filter index found {0}", filter));
		}

		private static bool IsDeliveryType(int filter)
		{
			return filter < QueueViewerIncludesAndExcludes.deliveryTypeNames.Length;
		}

		private static bool IsNextHopCategory(int filter)
		{
			return filter >= QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex && filter < QueueViewerIncludesAndExcludes.nextHopCategoryNames.Length + QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex;
		}

		private const string AndOperand = " -AND ";

		private const string HighRiskLevelValue = "High";

		private const string EmptyFilter = "Empty";

		private const string ShadowEmptyHighRisk = "SER";

		private const int NotSpecified = 0;

		private const int Specified = 1;

		private static readonly string DeliveryTypeProperty = ExtensibleQueueInfoSchema.DeliveryType.Name;

		private static readonly string NextHopCategoryProperty = ExtensibleQueueInfoSchema.NextHopCategory.Name;

		private static readonly string MessageCountProperty = ExtensibleQueueInfoSchema.MessageCount.Name;

		private static readonly string RiskLevelProperty = ExtensibleQueueInfoSchema.RiskLevel.Name;

		private static readonly string[] deliveryTypeNames = Enum.GetNames(typeof(DeliveryType));

		private static readonly string[] nextHopCategoryNames = Enum.GetNames(typeof(NextHopCategory));

		private static readonly int NextHopCategoryBaseIndex = QueueViewerIncludesAndExcludes.deliveryTypeNames.Length;

		private static readonly int EmptyIndex = QueueViewerIncludesAndExcludes.NextHopCategoryBaseIndex + QueueViewerIncludesAndExcludes.nextHopCategoryNames.Length;

		private static readonly int HighRiskIndex = QueueViewerIncludesAndExcludes.EmptyIndex + 1;

		private static readonly int MaxFilterCount = QueueViewerIncludesAndExcludes.HighRiskIndex + 1;

		private int[] filter = new int[QueueViewerIncludesAndExcludes.MaxFilterCount];

		private string filterString;
	}
}
