using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class PublicFolderMailboxDumpsterInfo : ConfigurableObject
	{
		private PublicFolderMailboxDumpsterInfo() : base(new SimplePropertyBag(SimpleProviderObjectSchema.Identity, SimpleProviderObjectSchema.ObjectState, SimpleProviderObjectSchema.ExchangeVersion))
		{
			this.propertyBag[SimpleProviderObjectSchema.Identity] = new PublicFolderDumpsterInfoId();
		}

		public string DumpsterHolderEntryId
		{
			get
			{
				return (string)this[PublicFolderDumpsterInfoSchema.DumpsterHolderEntryId];
			}
		}

		public int CountTotalFolders
		{
			get
			{
				return (int)this[PublicFolderDumpsterInfoSchema.CountTotalFolders];
			}
		}

		public bool HasDumpsterExtended
		{
			get
			{
				return (bool)this[PublicFolderDumpsterInfoSchema.HasDumpsterExtended];
			}
		}

		public int CountLegacyDumpsters
		{
			get
			{
				return (int)this[PublicFolderDumpsterInfoSchema.CountLegacyDumpsters];
			}
		}

		public int CountContainerLevel1
		{
			get
			{
				return (int)this[PublicFolderDumpsterInfoSchema.CountContainerLevel1];
			}
		}

		public int CountContainerLevel2
		{
			get
			{
				return (int)this[PublicFolderDumpsterInfoSchema.CountContainerLevel2];
			}
		}

		public int CountDumpsters
		{
			get
			{
				return (int)this[PublicFolderDumpsterInfoSchema.CountDumpsters];
			}
		}

		public int CountDeletedFolders
		{
			get
			{
				return (int)this[PublicFolderDumpsterInfoSchema.CountDeletedFolders];
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return PublicFolderMailboxDumpsterInfo.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal static PublicFolderMailboxDumpsterInfo LoadInfo(PublicFolderSession session, Action<LocalizedString, LocalizedString, int> writeProgress)
		{
			string value = null;
			int num = 0;
			bool flag = false;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			using (Folder folder = Folder.Bind(session, session.GetDumpsterRootFolderId(), PublicFolderMailboxDumpsterInfo.DumpsterPropertiesToLoad))
			{
				byte[] valueOrDefault = folder.GetValueOrDefault<byte[]>(FolderSchema.PublicFolderDumpsterHolderEntryId, null);
				if (valueOrDefault != null)
				{
					value = session.IdConverter.CreateFolderId(session.IdConverter.GetIdFromLongTermId(valueOrDefault)).ToHexEntryId();
				}
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.DeepTraversal, null, null, PublicFolderMailboxDumpsterInfo.DumpsterSubfoldersProperties))
				{
					int estimatedRowCount = queryResult.EstimatedRowCount;
					bool flag2 = false;
					int num7 = 0;
					for (;;)
					{
						object[][] rows = queryResult.GetRows(10000);
						if (rows == null || rows.Length == 0)
						{
							break;
						}
						if (num == 0 && estimatedRowCount > rows.Length && writeProgress != null)
						{
							flag2 = true;
						}
						foreach (object[] array2 in rows)
						{
							num++;
							if (flag2)
							{
								if (num > estimatedRowCount)
								{
									estimatedRowCount = queryResult.EstimatedRowCount;
								}
								int num8 = num * 100 / estimatedRowCount;
								if (num8 != num7 && num8 <= 100)
								{
									writeProgress(ClientStrings.PublicFolderMailboxDumpsterInfo, ClientStrings.PublicFolderMailboxInfoFolderEnumeration(num, estimatedRowCount), num8);
									num7 = num8;
								}
							}
							string text = array2[1] as string;
							if (!string.IsNullOrEmpty(text))
							{
								bool flag3 = StringComparer.OrdinalIgnoreCase.Equals("\\NON_IPM_SUBTREE\\DUMPSTER_ROOT\\DUMPSTER_EXTEND", text);
								if (flag3)
								{
									flag = true;
								}
								else if (!text.StartsWith("\\NON_IPM_SUBTREE\\DUMPSTER_ROOT\\DUMPSTER_EXTEND\\", StringComparison.OrdinalIgnoreCase))
								{
									num2++;
								}
								else
								{
									object obj = array2[0];
									if (obj is int)
									{
										int num9 = (int)obj;
										if (num9 == 2)
										{
											num3++;
										}
										else if (num9 == 3)
										{
											num4++;
										}
										else if (num9 == 4)
										{
											num5++;
										}
										else if (num9 > 4)
										{
											num6++;
										}
									}
								}
							}
						}
					}
				}
			}
			PublicFolderMailboxDumpsterInfo publicFolderMailboxDumpsterInfo = new PublicFolderMailboxDumpsterInfo();
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.DumpsterHolderEntryId] = value;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.CountTotalFolders] = num;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.HasDumpsterExtended] = flag;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.CountLegacyDumpsters] = num2;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.CountContainerLevel1] = num3;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.CountContainerLevel2] = num4;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.CountDumpsters] = num5;
			publicFolderMailboxDumpsterInfo[PublicFolderDumpsterInfoSchema.CountDeletedFolders] = num6;
			publicFolderMailboxDumpsterInfo.propertyBag.ResetChangeTracking();
			return publicFolderMailboxDumpsterInfo;
		}

		private const int FolderHierarchyDepthPropertyIndex = 0;

		private const int FolderPathNamePropertyIndex = 1;

		private const string DumpsterExtendedFolderPath = "\\NON_IPM_SUBTREE\\DUMPSTER_ROOT\\DUMPSTER_EXTEND";

		private const string UnderDumpsterExtendedFolderPath = "\\NON_IPM_SUBTREE\\DUMPSTER_ROOT\\DUMPSTER_EXTEND\\";

		private static readonly PublicFolderDumpsterInfoSchema schema = ObjectSchema.GetInstance<PublicFolderDumpsterInfoSchema>();

		private static readonly PropertyDefinition[] DumpsterPropertiesToLoad = new PropertyDefinition[]
		{
			FolderSchema.PublicFolderDumpsterHolderEntryId
		};

		private static readonly PropertyDefinition[] DumpsterSubfoldersProperties = new PropertyDefinition[]
		{
			FolderSchema.FolderHierarchyDepth,
			FolderSchema.FolderPathName
		};
	}
}
