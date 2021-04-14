using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public abstract class NavigationHost : OwaPage
	{
		protected NavigationModule NavigationModule
		{
			get
			{
				return this.navigationModule;
			}
		}

		protected List<OwaSubPage> ChildSubPages
		{
			get
			{
				if (this.childSubPages == null)
				{
					this.childSubPages = new List<OwaSubPage>();
				}
				return this.childSubPages;
			}
		}

		protected virtual IEnumerable<string> ExternalScriptFiles
		{
			get
			{
				return new string[]
				{
					"startpage.js"
				};
			}
		}

		public IEnumerable<string> ExternalScriptFilesIncludeChildSubPages
		{
			get
			{
				foreach (string scriptFile in this.ExternalScriptFiles)
				{
					yield return scriptFile;
				}
				foreach (OwaSubPage owaSubPage in this.ChildSubPages)
				{
					foreach (string scriptFile2 in owaSubPage.ExternalScriptFilesIncludeChildSubPages)
					{
						yield return scriptFile2;
					}
				}
				yield break;
			}
		}

		protected abstract NavigationModule SelectNavagationModule();

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.lastModuleContainerId = base.UserContext.InboxFolderId.ToBase64String();
			this.navigationModule = this.SelectNavagationModule();
			this.lastModuleName = this.navigationModule.ToString();
			switch (this.navigationModule)
			{
			case NavigationModule.Mail:
				this.lastModuleApplicationElement = "Folder";
				this.lastModuleContentClass = "IPF.Note";
				this.lastModuleContainerId = base.UserContext.InboxFolderId.ToBase64String();
				this.lastMailFolderId = Utilities.GetQueryStringParameter(base.Request, "id", false);
				goto IL_342;
			case NavigationModule.Calendar:
				if (!base.UserContext.IsFeatureEnabled(Feature.Calendar))
				{
					throw new OwaSegmentationException("Calendar is disabled");
				}
				this.lastModuleApplicationElement = "Folder";
				this.lastModuleContentClass = "IPF.Appointment";
				this.lastModuleContainerId = base.UserContext.CalendarFolderId.ToBase64String();
				goto IL_342;
			case NavigationModule.Contacts:
				if (!base.UserContext.IsFeatureEnabled(Feature.Contacts))
				{
					throw new OwaSegmentationException("Contacts feature is disabled");
				}
				this.lastModuleApplicationElement = "Folder";
				this.lastModuleContentClass = "IPF.Contact";
				this.lastModuleContainerId = base.UserContext.ContactsFolderId.ToBase64String();
				goto IL_342;
			case NavigationModule.Tasks:
				if (!base.UserContext.IsFeatureEnabled(Feature.Tasks))
				{
					throw new OwaSegmentationException("Tasks are disabled");
				}
				this.lastModuleContentClass = "IPF.Task";
				this.lastModuleContainerId = base.UserContext.FlaggedItemsAndTasksFolderId.ToBase64String();
				goto IL_342;
			case NavigationModule.AddressBook:
			{
				if (!(this is AddressBook))
				{
					throw new OwaInvalidRequestException("Invalid navigation module value.");
				}
				if (base.UserContext.IsFeatureEnabled(Feature.GlobalAddressList))
				{
					this.lastModuleApplicationElement = "AddressList";
					this.recipientBlockType = ((AddressBook)this).RecipientBlockType;
					if (((AddressBook)this).IsRoomPicker && base.UserContext.AllRoomsAddressBookInfo != null && !base.UserContext.AllRoomsAddressBookInfo.IsEmpty)
					{
						this.lastModuleContentClass = "Rooms";
						this.lastModuleContainerId = base.UserContext.AllRoomsAddressBookInfo.ToBase64String();
					}
					else
					{
						this.lastModuleContentClass = "Recipients";
						this.lastModuleContainerId = base.UserContext.GlobalAddressListInfo.ToBase64String();
					}
					this.lastModuleName = this.lastModuleContentClass;
					goto IL_342;
				}
				this.lastModuleApplicationElement = "Folder";
				this.lastModuleContentClass = "IPF.Contact";
				this.lastModuleContainerId = base.UserContext.ContactsFolderId.ToBase64String();
				bool isPicker = ((AddressBook)this).IsPicker;
				if (isPicker)
				{
					this.lastModuleState = "AddressBookPicker";
					goto IL_342;
				}
				this.lastModuleState = "AddressBookBrowse";
				goto IL_342;
			}
			case NavigationModule.Documents:
				throw new OwaInvalidRequestException("Invalid navigation module value.");
			case NavigationModule.PublicFolders:
				if (!base.UserContext.IsFeatureEnabled(Feature.PublicFolders))
				{
					throw new OwaSegmentationException("Public Folders are disabled");
				}
				this.lastModuleApplicationElement = "Folder";
				this.lastModuleContentClass = "IPF.Note";
				this.lastModuleContainerId = (base.UserContext.TryGetPublicFolderRootIdString() ?? string.Empty);
				this.lastMailFolderId = this.lastModuleContainerId;
				goto IL_342;
			}
			this.lastModuleContentClass = "IPF.Note";
			this.lastModuleContainerId = base.UserContext.InboxFolderId.ToBase64String();
			this.lastModuleName = NavigationModule.Mail.ToString();
			IL_342:
			if (this.viewPlaceHolder != null)
			{
				this.InitializeView(this.viewPlaceHolder);
			}
		}

		protected void RenderSecondaryNavigation(TextWriter output)
		{
			this.RenderSecondaryNavigation(output, base.UserContext.IsFeatureEnabled(Feature.Contacts));
		}

		protected void RenderSecondaryNavigation(TextWriter output, bool showContacts)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			output.Write("<div id=\"{0}\" class=\"secNvPaneCont\">", this.lastModuleName);
			switch (this.navigationModule)
			{
			case NavigationModule.Mail:
				NavigationHost.RenderMailSecondaryNavigation(output, base.UserContext);
				goto IL_229;
			case NavigationModule.Calendar:
			{
				PropertyDefinition[] propsToReturn = new PropertyDefinition[]
				{
					ViewStateProperties.CalendarViewType,
					ViewStateProperties.DailyViewDays
				};
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(base.UserContext.MailboxSession, DefaultFolderType.Calendar, propsToReturn))
				{
					DailyView.RenderSecondaryNavigation(output, calendarFolder, base.UserContext);
					goto IL_229;
				}
				break;
			}
			case NavigationModule.Contacts:
				break;
			case NavigationModule.Tasks:
				TaskView.RenderSecondaryNavigation(output, base.UserContext);
				goto IL_229;
			case NavigationModule.Options:
				goto IL_21D;
			case NavigationModule.AddressBook:
				this.recipientBlockType = ((AddressBook)this).RecipientBlockType;
				if (base.UserContext.IsFeatureEnabled(Feature.GlobalAddressList))
				{
					bool isRoomPicker = ((AddressBook)this).IsRoomPicker && DirectoryAssistance.IsRoomsAddressListAvailable(base.UserContext);
					output.Write("<div class=\"abNavPane\" style=\"height:");
					output.Write(showContacts ? "30" : "100");
					output.Write("%;top:0px;\"><div id=\"divMdNmAD\">");
					output.Write(LocalizedStrings.GetHtmlEncoded(346766088));
					output.Write("</div><div id=\"divSecNvAD\">");
					DirectoryView.RenderSecondaryNavigation(output, base.UserContext, isRoomPicker);
					output.Write("</div></div>");
				}
				if (showContacts)
				{
					output.Write("<div class=\"abNavPane\" style=\"height:");
					output.Write(base.UserContext.IsFeatureEnabled(Feature.GlobalAddressList) ? "70" : "100");
					output.Write("%;bottom:0px;\"><div id=\"divMdNmC\">");
					output.Write(LocalizedStrings.GetHtmlEncoded(-1165546057));
					output.Write("</div><div id=\"divSecNvC\"");
					bool isPicker = ((AddressBook)this).IsPicker;
					if (isPicker)
					{
						output.Write(" class=\"noFltrsCntRg\"");
					}
					output.Write(">");
					ContactView.RenderSecondaryNavigation(output, base.UserContext, isPicker);
					output.Write("</div></div>");
					goto IL_229;
				}
				goto IL_229;
			case NavigationModule.Documents:
				DocumentLibraryUtilities.RenderSecondaryNavigation(output, base.UserContext);
				goto IL_229;
			case NavigationModule.PublicFolders:
				NavigationHost.RenderPublicFolderSecondaryNavigation(output, base.UserContext);
				goto IL_229;
			default:
				goto IL_21D;
			}
			ContactView.RenderSecondaryNavigation(output, base.UserContext, false);
			goto IL_229;
			IL_21D:
			NavigationHost.RenderMailSecondaryNavigation(output, base.UserContext);
			IL_229:
			output.Write("</div>");
		}

		internal static void RenderPublicFolderSecondaryNavigation(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write("<div id=\"divPFFlt\"></div>");
			PublicFolderTree publicFolderTree = PublicFolderTree.CreatePublicFolderRootTree(userContext);
			ContextMenu contextMenu = new PublicFolderTreeContextMenu(userContext);
			output.Write("<div id=\"divPFTrR\">");
			Infobar infobar = new Infobar("divErrPF", "infobar");
			infobar.Render(output);
			NavigationHost.RenderTreeRegionDivStart(output, null);
			NavigationHost.RenderTreeDivStart(output, "publictree");
			publicFolderTree.ErrDiv = "divErrPF";
			publicFolderTree.Render(output);
			NavigationHost.RenderTreeDivEnd(output);
			NavigationHost.RenderTreeRegionDivEnd(output);
			contextMenu.Render(output);
			output.Write("</div>");
		}

		internal static void RenderMailSecondaryNavigation(TextWriter output, UserContext userContext)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			output.Write("<div id=\"divMTrR\">");
			Infobar infobar = new Infobar("divErrMail", "infobar");
			infobar.Render(output);
			NavigationHost.RenderTreeRegionDivStart(output, null);
			NavigationTree navigationTree;
			MailboxFolderTree mailboxFolderTree;
			MailboxFolderTree[] array;
			bool expandBuddyList;
			NavigationTree.CreateFavoriteAndMailboxTreeAndGetBuddyListStatus(userContext, out navigationTree, out mailboxFolderTree, out array, out expandBuddyList);
			NavigationHost.RenderTreeDivStart(output, "favTr");
			navigationTree.ErrDiv = "divErrMail";
			navigationTree.Render(output);
			NavigationHost.RenderTreeDivEnd(output);
			NavigationHost.RenderTreeDivStart(output, "mailtree");
			mailboxFolderTree.ErrDiv = "divErrMail";
			mailboxFolderTree.Render(output);
			NavigationHost.RenderTreeDivEnd(output);
			if (!userContext.IsExplicitLogon)
			{
				if (userContext.HasArchive)
				{
					NavigationHost.RenderTreeDivStart(output, "archivetree", "othTr");
					MailboxFolderTree mailboxFolderTree2 = MailboxFolderTree.CreateStartPageDummyArchiveMailboxFolderTree(userContext);
					mailboxFolderTree2.ErrDiv = "divErrMail";
					mailboxFolderTree2.Render(output);
					NavigationHost.RenderTreeDivEnd(output);
				}
				foreach (OtherMailboxConfigEntry entry in OtherMailboxConfiguration.GetOtherMailboxes(userContext))
				{
					NavigationHost.RenderOtherMailboxFolderTree(output, userContext, entry, false);
				}
			}
			if (userContext.IsInstantMessageEnabled())
			{
				NavigationHost.RenderTreeDivStart(output, "buddytree");
				NavigationHost.RenderBuddyListTreeControl(output, userContext, expandBuddyList);
				NavigationHost.RenderTreeDivEnd(output);
			}
			NavigationHost.RenderTreeRegionDivEnd(output);
			output.Write("</div>");
			ContextMenu contextMenu = new FolderTreeContextMenu(userContext);
			contextMenu.Render(output);
			if (userContext.IsInstantMessageEnabled())
			{
				ContextMenu contextMenu2 = new BuddyTreeContextMenu(userContext);
				contextMenu2.Render(output);
			}
		}

		internal static void RenderOtherMailboxFolderTree(TextWriter writer, UserContext userContext, OtherMailboxConfigEntry entry, bool isExpanded)
		{
			MailboxFolderTree mailboxFolderTree = MailboxFolderTree.CreateOtherMailboxFolderTree(userContext, entry, isExpanded);
			if (mailboxFolderTree != null)
			{
				NavigationHost.RenderTreeDivStart(writer, "t" + Convert.ToBase64String(Encoding.UTF8.GetBytes(mailboxFolderTree.RootNode.FolderId.MailboxOwnerLegacyDN)), "othTr");
				mailboxFolderTree.ErrDiv = "divErrMail";
				mailboxFolderTree.Render(writer);
				NavigationHost.RenderTreeDivEnd(writer);
			}
		}

		public static void RenderTreeRegionDivStart(TextWriter output, string treeRegionId)
		{
			output.Write("<div");
			if (!string.IsNullOrEmpty(treeRegionId))
			{
				output.Write(" id=\"");
				output.Write(treeRegionId);
				output.Write("\"");
			}
			output.Write(" class=\"trRgO\">");
			output.Write("<div class=\"trRgI\">");
		}

		public static void RenderTreeDivStart(TextWriter output, string treeId)
		{
			NavigationHost.RenderTreeDivStart(output, treeId, null);
		}

		public static void RenderTreeDivStart(TextWriter output, string treeId, string treeClassName)
		{
			output.Write("<div id=\"");
			Utilities.HtmlEncode(treeId, output);
			output.Write("\"");
			if (!string.IsNullOrEmpty(treeClassName))
			{
				output.Write(" class=\"");
				output.Write(treeClassName);
				output.Write("\"");
			}
			output.Write(">");
		}

		public static void RenderTreeRegionDivEnd(TextWriter output)
		{
			output.Write("</div></div>");
		}

		public static void RenderTreeDivEnd(TextWriter output)
		{
			output.Write("</div>");
		}

		protected void InitializeView(PlaceHolder placeHolder)
		{
			if (placeHolder == null)
			{
				throw new ArgumentNullException("placeHolder");
			}
			OwaQueryStringParameters owaQueryStringParameters = new OwaQueryStringParameters();
			owaQueryStringParameters.SetApplicationElement(this.lastModuleApplicationElement);
			owaQueryStringParameters.ItemClass = this.lastModuleContentClass;
			if (this.lastModuleState != null)
			{
				owaQueryStringParameters.State = this.lastModuleState;
			}
			if (this.lastModuleMappingAction != null)
			{
				owaQueryStringParameters.Action = this.lastModuleMappingAction;
			}
			if (this.lastMailFolderId != null)
			{
				owaQueryStringParameters.Id = this.lastMailFolderId;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(owaQueryStringParameters.QueryString);
			if (this.recipientBlockType == RecipientBlockType.DL)
			{
				stringBuilder.Append("&dl=1");
			}
			FormValue formValue = RequestDispatcherUtilities.DoFormsRegistryLookup(base.SessionContext, owaQueryStringParameters.ApplicationElement, owaQueryStringParameters.ItemClass, owaQueryStringParameters.Action, owaQueryStringParameters.State);
			if (formValue != null && RequestDispatcher.DoesSubPageSupportSingleDocument(formValue.Value as string))
			{
				OwaSubPage owaSubPage = (OwaSubPage)this.Page.LoadControl(Path.GetFileName(formValue.Value as string));
				Utilities.PutOwaSubPageIntoPlaceHolder(placeHolder, "b" + Utilities.HtmlEncode(this.lastModuleContainerId), owaSubPage, owaQueryStringParameters, "class=\"mainView\"", false);
				this.ChildSubPages.Add(owaSubPage);
				return;
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<iframe allowtransparency id=\"");
			stringBuilder2.Append("b");
			Utilities.HtmlEncode(this.lastModuleContainerId, stringBuilder2);
			stringBuilder2.Append("\" src=\"");
			stringBuilder2.Append(stringBuilder.ToString());
			stringBuilder2.Append("\" class=\"mainView\" _cF=\"1\" frameborder=\"0\"></iframe>");
			placeHolder.Controls.Add(new LiteralControl(stringBuilder2.ToString()));
		}

		internal static void RenderBuddyListTreeControl(TextWriter output, UserContext userContext, bool expandBuddyList)
		{
			SimpleTreeNode simpleTreeNode = new SimpleTreeNode(userContext, "bddyRt");
			simpleTreeNode.SetSelectable(false);
			simpleTreeNode.ClientNodeType = "buddyRootNode";
			SimpleTreeNode simpleTreeNode2 = simpleTreeNode;
			simpleTreeNode2.HighlightClassName += " trNdGpHdHl";
			SimpleTreeNode simpleTreeNode3 = simpleTreeNode;
			simpleTreeNode3.NodeClassName += " trNdGpHd";
			simpleTreeNode.NeedSync = false;
			simpleTreeNode.IsExpanded = expandBuddyList;
			simpleTreeNode.SetContent(LocalizedStrings.GetNonEncoded(2047178654));
			new SimpleTree(userContext, simpleTreeNode, "divTrBddy")
			{
				ErrDiv = "divErrMail"
			}.Render(output);
		}

		private static string GetNavigationTreeId(NavigationNodeGroupSection groupSection)
		{
			switch (groupSection)
			{
			case NavigationNodeGroupSection.First:
				return "favTr";
			case NavigationNodeGroupSection.Calendar:
				return "calTr";
			case NavigationNodeGroupSection.Contacts:
				return "cntTr";
			case NavigationNodeGroupSection.Tasks:
				return "tskTr";
			}
			throw new ArgumentException("Navigation tree is only available in calendar, contact and task.");
		}

		private static string GetNavigationTreeRegionId(NavigationNodeGroupSection groupSection)
		{
			switch (groupSection)
			{
			case NavigationNodeGroupSection.Calendar:
				return "divCalTrR";
			case NavigationNodeGroupSection.Contacts:
				return "divCntTrR";
			case NavigationNodeGroupSection.Tasks:
				return "divTskTrR";
			default:
				throw new ArgumentException("Navigation tree is only available in calendar, contact and task.");
			}
		}

		private static string GetNavigationTreeErrDivId(NavigationNodeGroupSection groupSection)
		{
			switch (groupSection)
			{
			case NavigationNodeGroupSection.First:
				return "divErrMail";
			case NavigationNodeGroupSection.Calendar:
				return "divErrCal";
			case NavigationNodeGroupSection.Contacts:
				return "divErrCnt";
			case NavigationNodeGroupSection.Tasks:
				return "divErrTsk";
			}
			throw new ArgumentException("Navigation tree is only available in calendar, contact and task.");
		}

		internal static void RenderNavigationTreeControl(TextWriter writer, UserContext userContext, NavigationNodeGroupSection groupSection, OwaStoreObjectId newFolderId)
		{
			if (groupSection == NavigationNodeGroupSection.First)
			{
				throw new ArgumentException("Should not use this function to render favorites tree.");
			}
			string navigationTreeErrDivId = NavigationHost.GetNavigationTreeErrDivId(groupSection);
			string navigationTreeId = NavigationHost.GetNavigationTreeId(groupSection);
			string navigationTreeRegionId = NavigationHost.GetNavigationTreeRegionId(groupSection);
			writer.Write("<div id=\"");
			writer.Write(navigationTreeRegionId);
			writer.Write("\">");
			Infobar infobar = new Infobar(navigationTreeErrDivId, "infobar");
			infobar.Render(writer);
			NavigationHost.RenderTreeRegionDivStart(writer, null);
			NavigationTree navigationTree = NavigationTree.CreateNavigationTree(userContext, groupSection);
			if (newFolderId != null)
			{
				navigationTree.RootNode.SelectSpecifiedFolder(newFolderId);
			}
			NavigationHost.RenderTreeDivStart(writer, navigationTreeId);
			navigationTree.ErrDiv = navigationTreeErrDivId;
			navigationTree.Render(writer);
			NavigationHost.RenderTreeDivEnd(writer);
			NavigationHost.RenderTreeRegionDivEnd(writer);
			writer.Write("</div>");
		}

		internal static void RenderNavigationTreeControl(TextWriter writer, UserContext userContext, NavigationModule navigationModule)
		{
			NavigationNodeGroupSection groupSection;
			switch (navigationModule)
			{
			case NavigationModule.Calendar:
				groupSection = NavigationNodeGroupSection.Calendar;
				break;
			case NavigationModule.Contacts:
				groupSection = NavigationNodeGroupSection.Contacts;
				break;
			case NavigationModule.Tasks:
				groupSection = NavigationNodeGroupSection.Tasks;
				break;
			default:
				throw new ArgumentException("Navigation tree is only available in calendar, contact and task.");
			}
			NavigationHost.RenderNavigationTreeControl(writer, userContext, groupSection, null);
		}

		internal static void RenderFavoritesAndNavigationTrees(TextWriter writer, UserContext userContext, OwaStoreObjectId newFolderId, params NavigationNodeGroupSection[] groupSections)
		{
			NavigationTree[] array = NavigationTree.CreateFavoriteAndNavigationTrees(userContext, groupSections);
			writer.Write("<div id=ntn>");
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					if (newFolderId != null)
					{
						array[i].RootNode.SelectSpecifiedFolder(newFolderId);
					}
					NavigationHost.RenderTreeDivStart(writer, NavigationHost.GetNavigationTreeId(groupSections[i]));
					array[i].ErrDiv = NavigationHost.GetNavigationTreeErrDivId(groupSections[i]);
					array[i].ErrHideId = array[i].ErrDiv + "Tr";
					array[i].Render(writer);
					NavigationHost.RenderTreeDivEnd(writer);
				}
			}
			writer.Write("</div>");
		}

		private const string MailErrDivId = "divErrMail";

		private const string PublicFolderErrorDivId = "divErrPF";

		private const string Folder = "Folder";

		private const string AddressList = "AddressList";

		private const string DocumentsFolder = "IPF.DocumentLibrary";

		private const string RecipientAddressList = "Recipients";

		private const string RoomsAddressList = "Rooms";

		private const string FolderIdPrefix = "b";

		private const string BuddyTreeRootId = "bddyRt";

		protected NavigationModule navigationModule;

		private RecipientBlockType recipientBlockType;

		protected PlaceHolder viewPlaceHolder;

		protected string lastModuleContainerId = string.Empty;

		protected string lastModuleContentClass = "IPF.Note";

		protected string lastModuleName;

		protected string lastModuleApplicationElement = "Folder";

		protected string lastModuleMappingAction = string.Empty;

		protected string lastModuleMappingState = string.Empty;

		protected string lastMailFolderId;

		protected string lastModuleState;

		private List<OwaSubPage> childSubPages;
	}
}
