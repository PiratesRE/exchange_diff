using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	[OwaEventStruct("FVLVF")]
	internal class FolderVirtualListViewFilter
	{
		public int Version
		{
			get
			{
				return this.version;
			}
		}

		public bool IsCurrentVersion
		{
			get
			{
				return this.version == 3;
			}
		}

		public static FolderVirtualListViewFilter ParseFromPropertyValue(object propertyValue)
		{
			string[] array = propertyValue as string[];
			if (array == null || array.Length < 2)
			{
				return null;
			}
			if (string.IsNullOrEmpty(array[0]))
			{
				return null;
			}
			FolderVirtualListViewFilter folderVirtualListViewFilter = new FolderVirtualListViewFilter();
			try
			{
				folderVirtualListViewFilter.SourceFolderId = OwaStoreObjectId.CreateFromString(array[0]);
			}
			catch (OwaInvalidIdFormatException)
			{
				return null;
			}
			int num = 0;
			folderVirtualListViewFilter.version = 0;
			if (!string.IsNullOrEmpty(array[1]))
			{
				string[] array2 = array[1].Split(new char[]
				{
					':'
				});
				if (array2.Length < 1 || !int.TryParse(array2[0], out num))
				{
					num = 0;
				}
				if (array2.Length < 2 || !int.TryParse(array2[1], out folderVirtualListViewFilter.version))
				{
					folderVirtualListViewFilter.version = 0;
				}
			}
			folderVirtualListViewFilter.SendToMe = ((num & 1) != 0);
			folderVirtualListViewFilter.CcToMe = ((num & 2) != 0);
			folderVirtualListViewFilter.IsUnread = ((num & 4) != 0);
			folderVirtualListViewFilter.IsHighImportance = ((num & 16) != 0);
			folderVirtualListViewFilter.HasAttachments = ((num & 32) != 0);
			folderVirtualListViewFilter.IsFlag = ((num & 8) != 0);
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			for (int i = 2; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					if (array[i].StartsWith(";"))
					{
						if (num2 == 3 && string.IsNullOrEmpty(folderVirtualListViewFilter.Categories))
						{
							folderVirtualListViewFilter.Categories = stringBuilder.ToString();
						}
						string value = array[i].Substring(";".Length);
						if ("sTo".Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							num2 = 1;
						}
						else if ("sFrm".Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							num2 = 2;
						}
						else if ("sCat".Equals(value, StringComparison.OrdinalIgnoreCase))
						{
							num2 = 3;
						}
						else
						{
							num2 = 0;
						}
					}
					else
					{
						switch (num2)
						{
						case 1:
							if (string.IsNullOrEmpty(folderVirtualListViewFilter.To))
							{
								folderVirtualListViewFilter.To = array[i];
							}
							break;
						case 2:
							if (string.IsNullOrEmpty(folderVirtualListViewFilter.From))
							{
								folderVirtualListViewFilter.From = array[i];
							}
							break;
						case 3:
							if (!flag)
							{
								stringBuilder.Append(',');
							}
							flag = false;
							stringBuilder.Append(array[i]);
							break;
						}
					}
				}
			}
			if (num2 == 3 && string.IsNullOrEmpty(folderVirtualListViewFilter.Categories))
			{
				folderVirtualListViewFilter.Categories = stringBuilder.ToString();
			}
			return folderVirtualListViewFilter;
		}

		public int GetBooleanFlags()
		{
			int num = 0;
			if (this.SendToMe)
			{
				num |= 1;
			}
			if (this.CcToMe)
			{
				num |= 2;
			}
			if (this.IsUnread)
			{
				num |= 4;
			}
			if (this.IsFlag)
			{
				num |= 8;
			}
			if (this.IsHighImportance)
			{
				num |= 16;
			}
			if (this.HasAttachments)
			{
				num |= 32;
			}
			return num;
		}

		public string[] GetPropertyValueToSave()
		{
			List<string> list = new List<string>();
			list.Add(this.SourceFolderId.ToBase64String());
			list.Add(this.GetBooleanFlags().ToString() + ':' + this.version);
			if (!string.IsNullOrEmpty(this.Categories))
			{
				string[] array = this.Categories.Split(new char[]
				{
					','
				});
				Array.Sort<string>(array);
				if (array.Length > 0)
				{
					list.Add(";sCat");
					foreach (string item in array)
					{
						list.Add(item);
					}
				}
			}
			if (!string.IsNullOrEmpty(this.To))
			{
				list.Add(";sTo");
				list.Add(this.To);
			}
			if (!string.IsNullOrEmpty(this.From))
			{
				list.Add(";sFrm");
				list.Add(this.From);
			}
			return list.ToArray();
		}

		public override int GetHashCode()
		{
			return this.SourceFolderId.GetHashCode() ^ this.SendToMe.GetHashCode() ^ this.CcToMe.GetHashCode() ^ this.IsUnread.GetHashCode() ^ (string.IsNullOrEmpty(this.Categories) ? 0 : this.Categories.GetHashCode()) ^ (string.IsNullOrEmpty(this.To) ? 0 : this.To.GetHashCode()) ^ (string.IsNullOrEmpty(this.From) ? 0 : this.From.GetHashCode()) ^ this.IsFlag.GetHashCode() ^ this.IsHighImportance.GetHashCode() ^ this.HasAttachments.GetHashCode() ^ this.version.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			FolderVirtualListViewFilter folderVirtualListViewFilter = obj as FolderVirtualListViewFilter;
			return folderVirtualListViewFilter != null && this.EqualsIgnoreVersion(folderVirtualListViewFilter) && this.version == folderVirtualListViewFilter.version;
		}

		public bool EqualsIgnoreVersion(FolderVirtualListViewFilter filter)
		{
			return this.SourceFolderId.Equals(filter.SourceFolderId) && this.SendToMe == filter.SendToMe && this.CcToMe == filter.CcToMe && this.IsUnread == filter.IsUnread && FolderVirtualListViewFilter.CheckCategoryEquals(this.Categories, filter.Categories) && FolderVirtualListViewFilter.CheckStringEquals(this.From, filter.From) && FolderVirtualListViewFilter.CheckStringEquals(this.To, filter.To) && this.IsFlag == filter.IsFlag && this.IsHighImportance == filter.IsHighImportance && this.HasAttachments == filter.HasAttachments;
		}

		private static bool CheckStringEquals(string str1, string str2)
		{
			return (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) || (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2) && string.Equals(str1, str2, StringComparison.InvariantCulture));
		}

		private static bool CheckCategoryEquals(string catString1, string catString2)
		{
			if (string.IsNullOrEmpty(catString1) && string.IsNullOrEmpty(catString2))
			{
				return true;
			}
			if (string.IsNullOrEmpty(catString1) || string.IsNullOrEmpty(catString2))
			{
				return false;
			}
			string[] array = catString1.Split(new char[]
			{
				','
			});
			string[] array2 = catString2.Split(new char[]
			{
				','
			});
			if (array.Length != array2.Length)
			{
				return false;
			}
			Array.Sort<string>(array);
			Array.Sort<string>(array2);
			int num = 0;
			while (num < array.Length && num < array2.Length)
			{
				if (array[num] != array2[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		public string[] GetCategories()
		{
			if (string.IsNullOrEmpty(this.Categories))
			{
				return null;
			}
			string[] array = this.Categories.Split(new char[]
			{
				','
			});
			Array.Sort<string>(array);
			return array;
		}

		public string ToDescription()
		{
			LocalizedStrings.GetNonEncoded(538423429);
			LocalizedStrings.GetNonEncoded(742303293);
			List<string> list = new List<string>(10);
			if (this.HasAttachments)
			{
				list.Add(LocalizedStrings.GetNonEncoded(-2070993236));
			}
			if (this.IsHighImportance)
			{
				list.Add(LocalizedStrings.GetNonEncoded(-1382849860));
			}
			if (this.IsFlag)
			{
				list.Add(LocalizedStrings.GetNonEncoded(1398003256));
			}
			if (!string.IsNullOrEmpty(this.To))
			{
				string item = string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(-1733525344), new object[]
				{
					this.To
				});
				list.Add(item);
			}
			if (!string.IsNullOrEmpty(this.From))
			{
				string item2 = string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(1286003943), new object[]
				{
					this.From
				});
				list.Add(item2);
			}
			if (!string.IsNullOrEmpty(this.Categories))
			{
				string text = null;
				string[] array = this.Categories.Split(new char[]
				{
					','
				});
				int num = 0;
				while (num < array.Length && num < 3)
				{
					if (text == null)
					{
						text = array[num];
					}
					else
					{
						text = string.Format(CultureInfo.CurrentCulture, LocalizedStrings.GetNonEncoded(538423429), new object[]
						{
							text,
							array[num]
						});
					}
					num++;
				}
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			if (this.IsUnread)
			{
				list.Add(LocalizedStrings.GetNonEncoded(-1020805457));
			}
			if (this.CcToMe)
			{
				list.Add(LocalizedStrings.GetNonEncoded(954766149));
			}
			if (this.SendToMe)
			{
				list.Add(LocalizedStrings.GetNonEncoded(226051813));
			}
			if (list.Count == 0)
			{
				return string.Empty;
			}
			string text2 = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				string format = (i == 1) ? LocalizedStrings.GetNonEncoded(742303293) : LocalizedStrings.GetNonEncoded(538423429);
				text2 = string.Format(CultureInfo.CurrentCulture, format, new object[]
				{
					list[i],
					text2
				});
			}
			return text2;
		}

		public bool IsValidFilter()
		{
			return this.GetBooleanFlags() != 0 || !string.IsNullOrEmpty(this.To) || !string.IsNullOrEmpty(this.From) || !string.IsNullOrEmpty(this.Categories);
		}

		private static QueryFilter BuildTextFilter(string keyWord, MatchOptions matchOptions, params PropertyDefinition[] searchProperties)
		{
			QueryFilter[] array = new TextFilter[searchProperties.Length];
			for (int i = 0; i < searchProperties.Length; i++)
			{
				array[i] = new TextFilter(searchProperties[i], keyWord, matchOptions, MatchFlags.IgnoreCase);
			}
			if (array.Length <= 1)
			{
				return array[0];
			}
			return new OrFilter(array);
		}

		private QueryFilter GetQueryFilter()
		{
			List<QueryFilter> list = new List<QueryFilter>(10);
			if (this.SendToMe)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.MessageToMe, true));
			}
			if (this.CcToMe)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, MessageItemSchema.MessageCcMe, true));
			}
			if (this.IsUnread)
			{
				list.Add(new BitMaskFilter(MessageItemSchema.Flags, 1UL, false));
			}
			if (!string.IsNullOrEmpty(this.Categories))
			{
				string[] array = this.Categories.Split(new char[]
				{
					','
				});
				List<QueryFilter> list2 = new List<QueryFilter>(array.Length);
				foreach (string propertyValue in array)
				{
					list2.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Categories, propertyValue));
				}
				if (list2.Count > 0)
				{
					list.Add((list2.Count > 1) ? new OrFilter(list2.ToArray()) : list2[0]);
				}
			}
			if (!string.IsNullOrEmpty(this.To))
			{
				list.Add(FolderVirtualListViewFilter.BuildTextFilter(this.To, MatchOptions.SubString, new PropertyDefinition[]
				{
					ItemSchema.DisplayTo
				}));
			}
			if (!string.IsNullOrEmpty(this.From))
			{
				list.Add(FolderVirtualListViewFilter.BuildTextFilter(this.From, MatchOptions.FullString, FolderVirtualListViewFilter.fromProperties));
			}
			if (this.IsFlag)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.FlagStatus, FlagStatus.Flagged));
			}
			if (this.IsHighImportance)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Importance, Importance.High));
			}
			if (this.HasAttachments)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.HasAttachment, true));
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			return new AndFilter(list.ToArray());
		}

		public void UpgradeFilter(SearchFolder folder, PropertyDefinition[] propertiesToLoad)
		{
			if (this.IsCurrentVersion)
			{
				throw new OwaInvalidOperationException("Can't upgrade a filter in current version");
			}
			this.version = 3;
			folder[ViewStateProperties.FilteredViewLabel] = this.GetPropertyValueToSave();
			folder.Save();
			folder.Load(FolderList.FolderTreeQueryProperties);
		}

		public void ApplyFilter(SearchFolder folder, PropertyDefinition[] propertiesToLoad)
		{
			if (!this.IsCurrentVersion)
			{
				throw new OwaInvalidOperationException("Can't apply a filter in different version");
			}
			int num = 0;
			SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(this.GetQueryFilter(), new StoreId[]
			{
				this.SourceFolderId.StoreObjectId
			});
			searchFolderCriteria.DeepTraversal = false;
			SearchPerformanceData searchPerformanceData = new SearchPerformanceData();
			string text = this.ToDescription();
			searchPerformanceData.StartSearch(string.IsNullOrEmpty(text) ? "No Search String" : text);
			IAsyncResult asyncResult = folder.BeginApplyContinuousSearch(searchFolderCriteria, null, null);
			Stopwatch watch = Utilities.StartWatch();
			bool flag = asyncResult.AsyncWaitHandle.WaitOne(5000, false);
			searchPerformanceData.Complete(!flag, true);
			if (flag)
			{
				folder.EndApplyContinuousSearch(asyncResult);
			}
			else
			{
				ExTraceGlobals.MailTracer.TraceDebug((long)this.GetHashCode(), "FolderVirtualListViewEventHandler.GetFilteredView. Search for filtered view timed out.");
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.SearchesTimedOut.Increment();
				}
			}
			OwaContext.Current.SearchPerformanceData = searchPerformanceData;
			Utilities.StopWatch(watch, "FolderVirtualListViewEventHandler.GetFilteredView (Wait for filter to complete)");
			object obj = folder.TryGetProperty(FolderSchema.ExtendedFolderFlags);
			if (!(obj is PropertyError))
			{
				num = (int)obj;
			}
			folder[FolderSchema.ExtendedFolderFlags] = (num | 4194304);
			folder.Save();
			folder.Load(propertiesToLoad);
		}

		private const int MaximumFilterTime = 5;

		public const char CategorySplitChar = ',';

		private const string ParameterNamePrefix = ";";

		private const char SplitCharBetweenFlagAndVersion = ':';

		public const string StructNamespace = "FVLVF";

		public const string FolderId = "fId";

		public const string SendToMeName = "fStm";

		public const string CcToMeName = "fCtm";

		public const string IsUnreadName = "fUR";

		public const string CategoriesName = "sCat";

		public const string ToName = "sTo";

		public const string FromName = "sFrm";

		public const string IsFlagName = "fFlg";

		public const string IsHighImportanceName = "fHI";

		public const string HasAttachmentsName = "fAttch";

		private const int SendToMeFlag = 1;

		private const int CcToMeFlag = 2;

		private const int IsUnreadFlag = 4;

		private const int IsFlagFlag = 8;

		private const int IsHighImportanceFlag = 16;

		private const int HasAttachmentsFlag = 32;

		private const int CurrentFilterConditionVersion = 3;

		private static readonly PropertyDefinition[] fromProperties = new PropertyDefinition[]
		{
			ItemSchema.SentRepresentingDisplayName,
			ItemSchema.SentRepresentingEmailAddress,
			MessageItemSchema.SenderDisplayName,
			MessageItemSchema.SenderEmailAddress
		};

		[OwaEventField("fId")]
		public OwaStoreObjectId SourceFolderId;

		[OwaEventField("fStm", true, false)]
		public bool SendToMe;

		[OwaEventField("fCtm", true, false)]
		public bool CcToMe;

		[OwaEventField("fUR", true, false)]
		public bool IsUnread;

		[OwaEventField("sCat", true, null)]
		public string Categories;

		[OwaEventField("sTo", true, null)]
		public string To;

		[OwaEventField("sFrm", true, null)]
		public string From;

		[OwaEventField("fFlg", true, false)]
		public bool IsFlag;

		[OwaEventField("fHI", true, false)]
		public bool IsHighImportance;

		[OwaEventField("fAttch", true, false)]
		public bool HasAttachments;

		private int version = 3;
	}
}
