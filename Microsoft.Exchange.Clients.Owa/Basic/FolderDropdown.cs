using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Basic.Controls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal class FolderDropdown
	{
		public FolderDropdown(UserContext userContext)
		{
			this.userContext = userContext;
		}

		public void RenderAllFolderSelectInMailSecondaryNavigation(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			FolderDropdown.FolderFilterOptions folderFilterOptions = FolderDropdown.FolderFilterOptions.SpecialFolders | FolderDropdown.FolderFilterOptions.DeletedItemsFolder | FolderDropdown.FolderFilterOptions.DeletedFolders | FolderDropdown.FolderFilterOptions.UnknownFolderType | FolderDropdown.FolderFilterOptions.FoldersAbleToViewInMailModule;
			FolderDropdown.RenderingOptions renderingOptions = FolderDropdown.RenderingOptions.ItemCount | FolderDropdown.RenderingOptions.Hierarchy | FolderDropdown.RenderingOptions.WholeLinkAsOptionValue | FolderDropdown.RenderingOptions.FolderDisplayNameAsTitle;
			FolderDropdown.RenderFolderList(folderList, null, null, selectedFolderId, new Dictionary<string, string>(2)
			{
				{
					"id",
					"selbrfld"
				},
				{
					"title",
					LocalizedStrings.GetHtmlEncoded(909860845)
				}
			}, folderFilterOptions, renderingOptions, this.userContext, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderMailMove(FolderList allFolderList, StoreObjectId selectedFolderId, string dropdownName, TextWriter writer)
		{
			if (allFolderList == null)
			{
				throw new ArgumentNullException("allFolderList");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			FolderDropdown.FolderFilterOptions folderFilterOptions = FolderDropdown.FolderFilterOptions.SpecialFolders | FolderDropdown.FolderFilterOptions.DeletedItemsFolder | FolderDropdown.FolderFilterOptions.DeletedFolders | FolderDropdown.FolderFilterOptions.UnknownFolderType;
			FolderDropdown.RenderingOptions renderingOptions = FolderDropdown.RenderingOptions.Hierarchy;
			FolderDropdown.RenderFolderList(allFolderList, null, null, selectedFolderId, new Dictionary<string, string>(4)
			{
				{
					"name",
					dropdownName
				},
				{
					"id",
					"selfld"
				},
				{
					"onchange",
					"onChgFldSel()"
				},
				{
					"onkeypress",
					"onKPFldSel(event)"
				}
			}, folderFilterOptions, renderingOptions, this.userContext, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderContactMove(ContactFolderList contactFolderList, StoreObjectId selectedFolderId, string dropdownName, TextWriter writer)
		{
			if (contactFolderList == null)
			{
				throw new ArgumentNullException("contactFolderList");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			FolderDropdown.FolderFilterOptions folderFilterOptions = FolderDropdown.FolderFilterOptions.SpecialFolders;
			FolderDropdown.RenderFolderList(contactFolderList, null, null, selectedFolderId, new Dictionary<string, string>(2)
			{
				{
					"name",
					dropdownName
				},
				{
					"id",
					"selfld"
				}
			}, folderFilterOptions, FolderDropdown.RenderingOptions.None, this.userContext, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderMailFolderToCreateIn(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			string[] folderTypes = new string[]
			{
				"IPF.Note"
			};
			FolderDropdown.RenderFolderList(folderList, null, folderTypes, selectedFolderId, new Dictionary<string, string>(3)
			{
				{
					"name",
					"ftci"
				},
				{
					"id",
					"ftci"
				},
				{
					"class",
					"flddds"
				}
			}, FolderDropdown.FolderFilterOptions.SpecialFolders, FolderDropdown.RenderingOptions.Root | FolderDropdown.RenderingOptions.Hierarchy, this.userContext, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderMailFolderToRename(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			string[] folderTypes = new string[]
			{
				"IPF.Note"
			};
			FolderDropdown.FolderFilterOptions folderFilterOptions = FolderDropdown.FolderFilterOptions.SpecialFolders | FolderDropdown.FolderFilterOptions.DeletedItemsFolder | FolderDropdown.FolderFilterOptions.DeletedFolders;
			this.RenderFolderToRename(folderList, LocalizedStrings.GetHtmlEncoded(1204401984), folderTypes, selectedFolderId, "ftr", folderFilterOptions, FolderDropdown.RenderingOptions.Hierarchy, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderMailFolderToMove(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			string[] folderTypes = new string[]
			{
				"IPF.Note"
			};
			IDictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary.Add("name", "ftm");
			dictionary.Add("id", "ftm");
			dictionary.Add("class", "flddd");
			dictionary.Add("onchange", "onChgFToMv()");
			FolderDropdown.FolderFilterOptions folderFilterOptions = FolderDropdown.FolderFilterOptions.SpecialFolders | FolderDropdown.FolderFilterOptions.DeletedItemsFolder | FolderDropdown.FolderFilterOptions.DeletedFolders;
			FolderDropdown.RenderFolderList(folderList, LocalizedStrings.GetHtmlEncoded(-311510783), folderTypes, selectedFolderId, dictionary, folderFilterOptions, FolderDropdown.RenderingOptions.Hierarchy, this.userContext, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderMailNewLocationForMove(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			string[] folderTypes = new string[]
			{
				"IPF.Note"
			};
			FolderDropdown.RenderFolderList(folderList, null, folderTypes, selectedFolderId, new Dictionary<string, string>(4)
			{
				{
					"name",
					"nlfm"
				},
				{
					"id",
					"nlfm"
				},
				{
					"class",
					"flddd"
				},
				{
					"onchange",
					"onChgNLMv()"
				}
			}, FolderDropdown.FolderFilterOptions.SpecialFolders, FolderDropdown.RenderingOptions.Root | FolderDropdown.RenderingOptions.Hierarchy, this.userContext, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderCalendarFolderToRename(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer, params FolderDropDownFilterDelegate[] filters)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			string[] folderTypes = new string[]
			{
				"IPF.Appointment"
			};
			this.RenderFolderToRename(folderList, LocalizedStrings.GetHtmlEncoded(2017065670), folderTypes, selectedFolderId, "ftr", FolderDropdown.FolderFilterOptions.None, FolderDropdown.RenderingOptions.None, writer, filters);
		}

		public void RenderContactFolderToRename(FolderList folderList, StoreObjectId selectedFolderId, TextWriter writer)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			string[] folderTypes = new string[]
			{
				"IPF.Contact"
			};
			this.RenderFolderToRename(folderList, LocalizedStrings.GetHtmlEncoded(-1471453346), folderTypes, selectedFolderId, "ftr", FolderDropdown.FolderFilterOptions.None, FolderDropdown.RenderingOptions.None, writer, new FolderDropDownFilterDelegate[0]);
		}

		public void RenderFolderToDelete(FolderList folderList, string defaultText, TextWriter writer, params FolderDropDownFilterDelegate[] filters)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			IDictionary<string, string> dictionary = new Dictionary<string, string>(4);
			dictionary.Add("name", "ftd");
			dictionary.Add("id", "ftd");
			dictionary.Add("class", "flddd dddt");
			dictionary.Add("onchange", "onChgDel(this)");
			FolderDropdown.FolderFilterOptions folderFilterOptions = FolderDropdown.FolderFilterOptions.SpecialFolders | FolderDropdown.FolderFilterOptions.DeletedItemsFolder | FolderDropdown.FolderFilterOptions.DeletedFolders;
			FolderDropdown.RenderFolderList(folderList, defaultText, null, null, dictionary, folderFilterOptions, FolderDropdown.RenderingOptions.Hierarchy, this.userContext, writer, filters);
		}

		private static void RenderFolderList(FolderList folderList, string defaultText, string[] folderTypes, StoreObjectId selectedFolderId, IDictionary<string, string> dropdownAttributes, FolderDropdown.FolderFilterOptions folderFilterOptions, FolderDropdown.RenderingOptions renderingOptions, UserContext userContext, TextWriter writer, params FolderDropDownFilterDelegate[] filters)
		{
			if (folderList == null)
			{
				throw new ArgumentNullException("folderList");
			}
			writer.Write("<select");
			if (dropdownAttributes != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in dropdownAttributes)
				{
					writer.Write(" ");
					writer.Write(keyValuePair.Key);
					writer.Write("=\"");
					writer.Write(keyValuePair.Value);
					writer.Write("\"");
				}
			}
			writer.Write(">");
			if (!string.IsNullOrEmpty(defaultText))
			{
				writer.Write("<option value=\"\" class=dddt>");
				writer.Write(defaultText);
				writer.Write("</option>");
			}
			if ((renderingOptions & FolderDropdown.RenderingOptions.Root) != FolderDropdown.RenderingOptions.None)
			{
				writer.Write("<option value=\"");
				Utilities.HtmlEncode(userContext.GetRootFolderId(userContext.MailboxSession).ToBase64String(), writer);
				writer.Write("\"");
				if (Utilities.IsDefaultFolderId(userContext.MailboxSession, selectedFolderId, DefaultFolderType.Root))
				{
					writer.Write(" selected");
				}
				writer.Write(">");
				Utilities.HtmlEncode(userContext.ExchangePrincipal.MailboxInfo.DisplayName, writer);
				writer.Write("</option>");
			}
			for (int i = 0; i < folderList.Count; i++)
			{
				StoreObjectId objectId = ((VersionedId)folderList.GetPropertyValue(i, FolderSchema.Id)).ObjectId;
				if (((folderFilterOptions & FolderDropdown.FolderFilterOptions.SpecialFolders) != FolderDropdown.FolderFilterOptions.None || !Utilities.IsSpecialFolder(objectId, userContext)) && ((folderFilterOptions & FolderDropdown.FolderFilterOptions.DeletedItemsFolder) != FolderDropdown.FolderFilterOptions.None || !Utilities.IsDefaultFolderId(userContext.MailboxSession, objectId, DefaultFolderType.DeletedItems)) && ((folderFilterOptions & FolderDropdown.FolderFilterOptions.DeletedFolders) != FolderDropdown.FolderFilterOptions.None || !folderList.IsFolderDeleted(i)))
				{
					bool flag = false;
					for (int j = 0; j < filters.Length; j++)
					{
						if (!filters[j](folderList, objectId))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						string text = folderList.GetPropertyValue(i, StoreObjectSchema.ContainerClass) as string;
						if (!folderList.IsFolderDeleted(i))
						{
							if (text == null)
							{
								if ((folderFilterOptions & FolderDropdown.FolderFilterOptions.UnknownFolderType) == FolderDropdown.FolderFilterOptions.None)
								{
									goto IL_4A9;
								}
							}
							else if (folderTypes == null)
							{
								if ((folderFilterOptions & FolderDropdown.FolderFilterOptions.FoldersAbleToViewInMailModule) != FolderDropdown.FolderFilterOptions.None)
								{
									if (ObjectClass.IsCalendarFolder(text))
									{
										goto IL_4A9;
									}
									if (ObjectClass.IsContactsFolder(text))
									{
										goto IL_4A9;
									}
								}
							}
							else
							{
								bool flag2 = false;
								for (int k = 0; k < folderTypes.Length; k++)
								{
									if (ObjectClass.IsOfClass(text, folderTypes[k]))
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									goto IL_4A9;
								}
							}
						}
						if (text == null || !Utilities.IsFolderSegmentedOut(text, userContext))
						{
							writer.Write("<option value=\"");
							if ((renderingOptions & FolderDropdown.RenderingOptions.WholeLinkAsOptionValue) != FolderDropdown.RenderingOptions.None)
							{
								Utilities.HtmlEncode(Utilities.UrlEncode(text ?? "IPF.Note"), writer);
								Utilities.HtmlEncode("&", writer);
								writer.Write("id");
								writer.Write("=");
								Utilities.HtmlEncode(Utilities.UrlEncode(objectId.ToBase64String()), writer);
								if (!FolderUtility.IsPrimaryMailFolder(objectId, userContext) && !folderList.IsFolderDeleted(i))
								{
									Utilities.HtmlEncode("&mru=1", writer);
								}
							}
							else
							{
								Utilities.HtmlEncode(objectId.ToBase64String(), writer);
							}
							writer.Write("\"");
							if ((renderingOptions & FolderDropdown.RenderingOptions.FolderDisplayNameAsTitle) != FolderDropdown.RenderingOptions.None)
							{
								writer.Write(" title=\"");
								string text2 = folderList.GetPropertyValue(i, FolderSchema.DisplayName) as string;
								if (text2 != null)
								{
									Utilities.HtmlEncode(text2, writer);
								}
								writer.Write("\"");
							}
							if (Utilities.IsSpecialFolder(objectId, userContext))
							{
								writer.Write(" class=sf ");
							}
							else if (folderList.IsFolderDeleted(i))
							{
								writer.Write(" class=df ");
							}
							if (objectId.Equals(selectedFolderId))
							{
								writer.Write(" selected");
							}
							writer.Write(">");
							int num = 0;
							if ((renderingOptions & FolderDropdown.RenderingOptions.Hierarchy) != FolderDropdown.RenderingOptions.None)
							{
								int num2 = (int)folderList.GetPropertyValue(i, FolderSchema.FolderHierarchyDepth);
								num = (((renderingOptions & FolderDropdown.RenderingOptions.Root) != FolderDropdown.RenderingOptions.None) ? 1 : 0) + num2 - 1;
								if (num * 2 > 24)
								{
									num = 12;
								}
								for (int l = 0; l < num; l++)
								{
									writer.Write(". ");
								}
							}
							int num3 = 24 - num * 2;
							if (num3 > 0)
							{
								Utilities.CropAndRenderText(writer, (string)folderList.GetPropertyValue(i, FolderSchema.DisplayName), num3);
							}
							if ((renderingOptions & FolderDropdown.RenderingOptions.ItemCount) != FolderDropdown.RenderingOptions.None)
							{
								object propertyValue = folderList.GetPropertyValue(i, FolderSchema.ExtendedFolderFlags);
								ContentCountDisplay contentCountDisplay = FolderUtility.GetContentCountDisplay(propertyValue, objectId);
								if (contentCountDisplay == ContentCountDisplay.ItemCount)
								{
									int num4 = (int)folderList.GetPropertyValue(i, FolderSchema.ItemCount);
									if (num4 > 0)
									{
										writer.Write(" [" + num4 + "]");
									}
								}
								else if (contentCountDisplay == ContentCountDisplay.UnreadCount)
								{
									int num5 = (int)folderList.GetPropertyValue(i, FolderSchema.UnreadCount);
									if (num5 > 0)
									{
										writer.Write(" (" + num5 + ")");
									}
								}
							}
							writer.Write("</option>");
						}
					}
				}
				IL_4A9:;
			}
			writer.Write("</select>");
		}

		private void RenderFolderToRename(FolderList folderList, string defaultText, string[] folderTypes, StoreObjectId selectedFolderId, string dropdownName, FolderDropdown.FolderFilterOptions folderFilterOptions, FolderDropdown.RenderingOptions renderingOptions, TextWriter writer, params FolderDropDownFilterDelegate[] filters)
		{
			FolderDropdown.RenderFolderList(folderList, defaultText, folderTypes, selectedFolderId, new Dictionary<string, string>(4)
			{
				{
					"name",
					dropdownName
				},
				{
					"id",
					dropdownName
				},
				{
					"class",
					"flddd"
				},
				{
					"onchange",
					"onChgRn()"
				}
			}, folderFilterOptions, renderingOptions, this.userContext, writer, filters);
		}

		private const string TargetFolderDropDownID = "selfld";

		private const string IdAttribute = "id";

		private const string NameAttribute = "name";

		private const string TitleAttribute = "title";

		private const string ClassAttribute = "class";

		private const string OnChangeAttribute = "onchange";

		private const string OnKeyPressAttribute = "onkeypress";

		private UserContext userContext;

		[Flags]
		private enum FolderFilterOptions
		{
			None = 0,
			SpecialFolders = 1,
			DeletedItemsFolder = 2,
			DeletedFolders = 4,
			UnknownFolderType = 8,
			FoldersAbleToViewInMailModule = 16
		}

		[Flags]
		private enum RenderingOptions
		{
			None = 0,
			Root = 1,
			ItemCount = 2,
			Hierarchy = 4,
			WholeLinkAsOptionValue = 8,
			FolderDisplayNameAsTitle = 16
		}
	}
}
