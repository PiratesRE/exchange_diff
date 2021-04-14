using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class PublicFolderMailboxHierarchyInfo : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PublicFolderMailboxHierarchyInfo.schema;
			}
		}

		private PublicFolderMailboxHierarchyInfo() : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			this.propertyBag[SimpleProviderObjectSchema.Identity] = new PublicFolderHierarchyInfoId();
		}

		public int TotalFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.TotalFolderCount];
			}
		}

		public int MaxFolderChildCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.MaxFolderChildCount];
			}
		}

		public int HierarchyDepth
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.HierarchyDepth];
			}
		}

		public int MailPublicFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.MailPublicFolderCount];
			}
		}

		public int CalendarFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.CalendarFolderCount];
			}
		}

		public int ContactFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.ContactFolderCount];
			}
		}

		public int InfoPathFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.InfoPathFolderCount];
			}
		}

		public int JournalFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.JournalFolderCount];
			}
		}

		public int StickyNoteFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.StickyNoteFolderCount];
			}
		}

		public int TaskFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.TaskFolderCount];
			}
		}

		public int NoteFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.NoteFolderCount];
			}
		}

		public int OtherFolderCount
		{
			get
			{
				return (int)this[PublicFolderHierarchyInfoSchema.OtherFolderCount];
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static PublicFolderMailboxHierarchyInfo LoadInfo(PublicFolderSession session, Action<LocalizedString, LocalizedString, int> writeProgress)
		{
			int num = 1;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 1;
			using (Folder folder = Folder.Bind(session, session.GetIpmSubtreeFolderId(), PublicFolderMailboxHierarchyInfo.PublicFoldersProperties))
			{
				num2 = folder.GetValueOrDefault<int>(FolderSchema.ChildCount, 0);
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, PublicFolderMailboxHierarchyInfo.PublicFoldersProperties))
				{
					int estimatedRowCount = queryResult.EstimatedRowCount;
					bool flag = false;
					int num13 = 0;
					int num14 = 0;
					for (;;)
					{
						object[][] rows = queryResult.GetRows(10000);
						if (rows == null || rows.Length == 0)
						{
							break;
						}
						if (num14 == 0 && estimatedRowCount > rows.Length && writeProgress != null)
						{
							flag = true;
						}
						foreach (object[] array2 in rows)
						{
							num14++;
							if (flag)
							{
								if (num14 > estimatedRowCount)
								{
									estimatedRowCount = queryResult.EstimatedRowCount;
								}
								int num15 = num14 * 100 / estimatedRowCount;
								if (num15 != num13 && num15 <= 100)
								{
									writeProgress(ClientStrings.PublicFolderMailboxHierarchyInfo, ClientStrings.PublicFolderMailboxInfoFolderEnumeration(num14, estimatedRowCount), num15);
									num13 = num15;
								}
							}
							object obj = array2[0];
							int num16 = (obj is int) ? ((int)obj) : 0;
							object obj2 = array2[1];
							bool flag2 = obj2 is bool && (bool)obj2;
							object obj3 = array2[2];
							int num17 = (obj3 is int) ? ((int)obj3) : 0;
							string containerClass = array2[3] as string;
							num++;
							if (num4 < num16)
							{
								num4 = num16;
							}
							if (flag2)
							{
								num3++;
							}
							if (num2 < num17)
							{
								num2 = num17;
							}
							if (ObjectClass.IsCalendarFolder(containerClass))
							{
								num5++;
							}
							else if (ObjectClass.IsContactsFolder(containerClass))
							{
								num6++;
							}
							else if (ObjectClass.IsInfoPathFormFolder(containerClass))
							{
								num7++;
							}
							else if (ObjectClass.IsJournalFolder(containerClass))
							{
								num8++;
							}
							else if (ObjectClass.IsMessageFolder(containerClass))
							{
								num11++;
							}
							else if (ObjectClass.IsNotesFolder(containerClass))
							{
								num9++;
							}
							else if (ObjectClass.IsTaskFolder(containerClass))
							{
								num10++;
							}
							else
							{
								num12++;
							}
						}
					}
				}
			}
			PublicFolderMailboxHierarchyInfo publicFolderMailboxHierarchyInfo = new PublicFolderMailboxHierarchyInfo();
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.TotalFolderCount] = num;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.MailPublicFolderCount] = num3;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.MaxFolderChildCount] = num2;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.HierarchyDepth] = num4;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.CalendarFolderCount] = num5;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.ContactFolderCount] = num6;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.InfoPathFolderCount] = num7;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.JournalFolderCount] = num8;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.NoteFolderCount] = num11;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.StickyNoteFolderCount] = num9;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.TaskFolderCount] = num10;
			publicFolderMailboxHierarchyInfo[PublicFolderHierarchyInfoSchema.OtherFolderCount] = num12;
			publicFolderMailboxHierarchyInfo.propertyBag.ResetChangeTracking();
			return publicFolderMailboxHierarchyInfo;
		}

		private const int FolderHierarchyDepthPropertyIndex = 0;

		private const int MailEnabledPropertyIndex = 1;

		private const int ChildCountPropertyIndex = 2;

		private const int ContainerClassPropertyIndex = 3;

		private static readonly PublicFolderHierarchyInfoSchema schema = ObjectSchema.GetInstance<PublicFolderHierarchyInfoSchema>();

		private static readonly PropertyDefinition[] PublicFoldersProperties = new PropertyDefinition[]
		{
			FolderSchema.FolderHierarchyDepth,
			FolderSchema.MailEnabled,
			FolderSchema.ChildCount,
			InternalSchema.ContainerClass
		};
	}
}
