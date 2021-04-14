using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class WebPartPreFormAction : IPreFormAction
	{
		private static bool IsSupportedModule(string module)
		{
			return string.Equals(module, "inbox", StringComparison.OrdinalIgnoreCase) || string.Equals(module, "calendar", StringComparison.OrdinalIgnoreCase) || string.Equals(module, "contacts", StringComparison.OrdinalIgnoreCase) || string.Equals(module, "tasks", StringComparison.OrdinalIgnoreCase) || string.Compare(module, "publicfolders", true) == 0;
		}

		private Dictionary<string, string> LoadWebPartParameters()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["cmd"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "cmd", false);
			dictionary["id"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "id", false);
			dictionary["fPath"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "fPath", false);
			dictionary["module"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "module", false);
			dictionary["view"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "view", false);
			dictionary["d"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "d", false);
			dictionary["m"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "m", false);
			dictionary["y"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "y", false);
			dictionary["part"] = Utilities.GetQueryStringParameter(this.owaContext.HttpContext.Request, "part", false);
			return dictionary;
		}

		public PreFormActionResponse Execute(OwaContext owaContext, out ApplicationElement applicationElement, out string type, out string state, out string action)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			this.userContext = owaContext.UserContext;
			this.owaContext = owaContext;
			applicationElement = ApplicationElement.Folder;
			type = "IPF.Note";
			action = null;
			state = null;
			PreFormActionResponse preFormActionResponse = null;
			using (Folder folder = this.ProcessWebPartRequest())
			{
				if (folder != null)
				{
					preFormActionResponse = new PreFormActionResponse();
					preFormActionResponse.AddParameter("id", OwaStoreObjectId.CreateFromStoreObject(folder).ToBase64String());
					if (!string.IsNullOrEmpty(this.webPartParameters["view"]))
					{
						preFormActionResponse.AddParameter("view", this.webPartParameters["view"]);
					}
					if (this.isoDateString != null)
					{
						preFormActionResponse.AddParameter("d", this.isoDateString);
					}
					type = folder.ClassName;
					int num = 0;
					if (int.TryParse(this.webPartParameters["part"], out num) && num == 1)
					{
						applicationElement = ApplicationElement.WebPartFolder;
					}
					preFormActionResponse.ApplicationElement = applicationElement;
					preFormActionResponse.Action = action;
					preFormActionResponse.Type = type;
					preFormActionResponse.State = state;
				}
			}
			return preFormActionResponse;
		}

		private Folder ProcessWebPartRequest()
		{
			this.webPartParameters = this.LoadWebPartParameters();
			Folder result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				Folder folder = null;
				string text = this.webPartParameters["fPath"];
				bool flag = string.Compare(this.webPartParameters["module"], "publicfolders", true) == 0;
				if (this.owaContext.IsExplicitLogon && flag)
				{
					this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1044509156), OwaEventHandlerErrorCode.WebPartExplicitLogonPublicFolderViaOwaBasicError);
					result = null;
				}
				else if (flag && !this.userContext.IsFeatureEnabled(Feature.PublicFolders))
				{
					this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1242252459), OwaEventHandlerErrorCode.WebPartSegmentationError);
					result = null;
				}
				else
				{
					if (!string.IsNullOrEmpty(this.webPartParameters["id"]))
					{
						OwaStoreObjectId folderId = this.GetFolderId(this.webPartParameters["id"], flag);
						if (folderId == null || !Folder.IsFolderId(folderId.StoreObjectId))
						{
							this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1323257605), OwaEventHandlerErrorCode.WebPartUnsupportedItemError);
						}
						else
						{
							folder = this.GetFolderFromId(folderId);
							if (folder == null)
							{
								this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1725230116), OwaEventHandlerErrorCode.WebPartFolderPathError);
							}
						}
					}
					else if (!string.IsNullOrEmpty(text))
					{
						folder = this.GetFolderFromPath(text, flag);
						if (folder == null)
						{
							this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1725230116), OwaEventHandlerErrorCode.WebPartFolderPathError);
						}
					}
					else if (!string.IsNullOrEmpty(this.webPartParameters["module"]))
					{
						if (!WebPartPreFormAction.IsSupportedModule(this.webPartParameters["module"]))
						{
							this.owaContext.ErrorInformation = new ErrorInformation(string.Format(Strings.WebPartInvalidParameterError, "module"), OwaEventHandlerErrorCode.WebPartInvalidParameterError);
						}
						else
						{
							folder = this.GetFolderFromModule(this.webPartParameters["module"]);
							if (folder == null)
							{
								this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1725230116), OwaEventHandlerErrorCode.WebPartFolderPathError);
							}
						}
					}
					else
					{
						StoreObjectId storeObjectId = this.userContext.TryGetMyDefaultFolderId(DefaultFolderType.Inbox);
						if (storeObjectId == null)
						{
							this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1925343075), OwaEventHandlerErrorCode.WebPartContentsPermissionsError);
						}
						else
						{
							folder = this.GetFolderFromId(OwaStoreObjectId.CreateFromMailboxFolderId(storeObjectId));
							if (folder == null)
							{
								this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1925343075), OwaEventHandlerErrorCode.WebPartContentsPermissionsError);
							}
						}
					}
					if (folder == null)
					{
						result = null;
					}
					else
					{
						disposeGuard.Add<Folder>(folder);
						if (!this.CheckFolderIsSupported(folder))
						{
							result = null;
						}
						else if (!string.IsNullOrEmpty(this.webPartParameters["view"]) && !this.IsViewValid(folder.Id.ObjectId.ObjectType))
						{
							this.owaContext.ErrorInformation = new ErrorInformation(string.Format(Strings.WebPartInvalidParameterError, "view"), OwaEventHandlerErrorCode.WebPartInvalidParameterError);
							result = null;
						}
						else
						{
							if (ObjectClass.IsCalendarFolder(folder.ClassName) && (!string.IsNullOrEmpty(this.webPartParameters["d"]) || !string.IsNullOrEmpty(this.webPartParameters["m"]) || !string.IsNullOrEmpty(this.webPartParameters["y"])))
							{
								this.isoDateString = WebPartUtilities.ValidateDate(this.webPartParameters["d"], this.webPartParameters["m"], this.webPartParameters["y"]);
								if (this.isoDateString == null)
								{
									this.owaContext.ErrorInformation = new ErrorInformation(string.Format(LocalizedStrings.GetNonEncoded(-1413211833), "m, d, y"), OwaEventHandlerErrorCode.WebPartInvalidParameterError);
									return null;
								}
							}
							if (this.IsBasicClient())
							{
								this.webPartParameters["part"] = "1";
							}
							disposeGuard.Success();
							result = folder;
						}
					}
				}
			}
			return result;
		}

		private bool IsViewValid(StoreObjectType objectType)
		{
			bool result = false;
			Dictionary<string, WebPartListView> dictionary = WebPartUtilities.Views[objectType];
			WebPartListView webPartListView = null;
			if (dictionary.TryGetValue(this.webPartParameters["view"], out webPartListView))
			{
				result = true;
				if (this.IsBasicClient() && webPartListView.IsPremiumOnly)
				{
					result = false;
				}
			}
			return result;
		}

		private bool IsBasicClient()
		{
			return this.userContext.IsBasicExperience;
		}

		private bool CheckFolderIsSupported(Folder folder)
		{
			string text = folder.ClassName;
			if (string.IsNullOrEmpty(text))
			{
				text = "IPF.Note";
			}
			if (Utilities.GetFolderProperty<EffectiveRights>(folder, StoreObjectSchema.EffectiveRights, EffectiveRights.None) == EffectiveRights.None)
			{
				this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1925343075), OwaEventHandlerErrorCode.WebPartContentsPermissionsError);
				return false;
			}
			if (this.IsBasicClient() && Utilities.IsPublic(folder))
			{
				this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(807691393), OwaEventHandlerErrorCode.WebPartAccessPublicFolderViaOwaBasicError);
				return false;
			}
			if (ObjectClass.IsNotesFolder(text) || ObjectClass.IsJournalFolder(text))
			{
				this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(1063985686), OwaEventHandlerErrorCode.WebPartUnsupportedFolderTypeError);
				return false;
			}
			if (this.IsBasicClient() && ObjectClass.IsTaskFolder(text))
			{
				this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1065897770), OwaEventHandlerErrorCode.WebPartTaskFolderError);
				return false;
			}
			if (this.IsBasicClient() && ObjectClass.IsCalendarFolder(text))
			{
				this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-765983365), OwaEventHandlerErrorCode.WebPartCalendarFolderError);
				return false;
			}
			if ((folder.Id.ObjectId.ObjectType == StoreObjectType.SearchFolder && !this.userContext.IsFeatureEnabled(Feature.SearchFolders)) || (Utilities.IsDefaultFolder(folder, DefaultFolderType.JunkEmail) && !this.userContext.IsFeatureEnabled(Feature.JunkEMail)) || this.IsFeatureSegmentedOut(text))
			{
				this.owaContext.ErrorInformation = new ErrorInformation(LocalizedStrings.GetNonEncoded(-1242252459), OwaEventHandlerErrorCode.WebPartSegmentationError);
				return false;
			}
			return true;
		}

		private bool IsFeatureSegmentedOut(string className)
		{
			return (ObjectClass.IsCalendarFolder(className) && !this.userContext.IsFeatureEnabled(Feature.Calendar)) || (ObjectClass.IsTaskFolder(className) && !this.userContext.IsFeatureEnabled(Feature.Tasks)) || (ObjectClass.IsContactsFolder(className) && !this.userContext.IsFeatureEnabled(Feature.Contacts));
		}

		private OwaStoreObjectId GetFolderId(string folderId, bool isPublicFolder)
		{
			OwaStoreObjectId result = null;
			try
			{
				StoreObjectId folderStoreObjectId = Utilities.CreateStoreObjectId(folderId);
				result = OwaStoreObjectId.CreateFromFolderId(folderStoreObjectId, isPublicFolder ? OwaStoreObjectIdType.PublicStoreFolder : OwaStoreObjectIdType.MailBoxObject);
			}
			catch (OwaInvalidIdFormatException)
			{
				ExTraceGlobals.WebPartRequestTracer.TraceDebug<string>(0L, "Web part request invalid store object id: {0}", folderId);
			}
			return result;
		}

		private Folder GetFolderFromModule(string module)
		{
			StoreObjectId storeObjectId = null;
			OwaStoreObjectIdType objectIdType = OwaStoreObjectIdType.MailBoxObject;
			if (string.Equals(module, "inbox", StringComparison.OrdinalIgnoreCase))
			{
				storeObjectId = this.userContext.TryGetMyDefaultFolderId(DefaultFolderType.Inbox);
			}
			else if (string.Equals(module, "calendar", StringComparison.OrdinalIgnoreCase))
			{
				storeObjectId = this.userContext.TryGetMyDefaultFolderId(DefaultFolderType.Calendar);
			}
			else if (string.Equals(module, "contacts", StringComparison.OrdinalIgnoreCase))
			{
				storeObjectId = this.userContext.TryGetMyDefaultFolderId(DefaultFolderType.Contacts);
			}
			else if (string.Equals(module, "tasks", StringComparison.OrdinalIgnoreCase))
			{
				storeObjectId = this.userContext.TryGetMyDefaultFolderId(DefaultFolderType.Tasks);
			}
			else if (string.Compare(module, "publicfolders", StringComparison.OrdinalIgnoreCase) == 0)
			{
				storeObjectId = this.userContext.PublicFolderRootId;
				objectIdType = OwaStoreObjectIdType.PublicStoreFolder;
			}
			if (storeObjectId != null)
			{
				return this.GetFolderFromId(OwaStoreObjectId.CreateFromFolderId(storeObjectId, objectIdType));
			}
			return null;
		}

		private Folder GetFolderFromId(OwaStoreObjectId owaStoreObjectId)
		{
			Folder result = null;
			try
			{
				result = Utilities.GetFolder<Folder>(this.userContext, owaStoreObjectId, this.folderViewProperties);
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.WebPartRequestTracer.TraceDebug<string>(0L, "Web part request could not bind to folder {0}.  User may not have sufficient folder permissions", owaStoreObjectId.ToBase64String());
			}
			return result;
		}

		private Folder GetFolderFromPath(string path, bool isPublicFolder)
		{
			string[] array = path.Split(new char[]
			{
				'/',
				'\\'
			}, StringSplitOptions.RemoveEmptyEntries);
			CultureInfo userCulture = Culture.GetUserCulture();
			StoreObjectId storeObjectId = null;
			StoreSession storeSession = null;
			OwaStoreObjectIdType objectIdType = OwaStoreObjectIdType.MailBoxObject;
			try
			{
				if (isPublicFolder)
				{
					storeSession = PublicFolderSession.OpenAsAdmin(this.userContext.ExchangePrincipal.MailboxInfo.OrganizationId, this.userContext.ExchangePrincipal, Guid.Empty, null, userCulture, "Client=OWA;Action=WebPart + Admin + GetFolderFromPath", null);
					storeObjectId = this.userContext.PublicFolderRootId;
					objectIdType = OwaStoreObjectIdType.PublicStoreFolder;
				}
				else
				{
					storeSession = MailboxSession.OpenAsAdmin(this.owaContext.ExchangePrincipal, userCulture, "Client=OWA;Action=WebPart + Admin + GetFolderFromPath");
					GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(storeSession, this.owaContext.HttpContext.Request);
					storeObjectId = this.userContext.GetRootFolderId(this.userContext.MailboxSession);
				}
				for (int i = 0; i < array.Length; i++)
				{
					object[][] folderIdByDisplayName = this.GetFolderIdByDisplayName(array[i], storeObjectId, storeSession);
					if (folderIdByDisplayName == null || folderIdByDisplayName.Length == 0)
					{
						return null;
					}
					storeObjectId = ((VersionedId)folderIdByDisplayName[0][0]).ObjectId;
				}
			}
			finally
			{
				if (storeSession != null)
				{
					storeSession.Dispose();
					storeSession = null;
				}
			}
			if (storeObjectId == null)
			{
				return null;
			}
			return this.GetFolderFromId(OwaStoreObjectId.CreateFromFolderId(storeObjectId, objectIdType));
		}

		private object[][] GetFolderIdByDisplayName(string displayName, StoreObjectId parentFolderId, StoreSession session)
		{
			object[][] result = null;
			using (Folder folder = Folder.Bind(session, parentFolderId))
			{
				if (!Utilities.GetFolderProperty<bool>(folder, FolderSchema.HasChildren, false))
				{
					return null;
				}
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, this.folderViewProperties))
				{
					ComparisonFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, FolderSchema.DisplayName, displayName);
					queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter);
					result = queryResult.GetRows(1);
				}
			}
			return result;
		}

		public const string Command = "cmd";

		public const string CommandValue = "contents";

		public const string FolderId = "id";

		public const string FolderPath = "fPath";

		public const string Module = "module";

		public const string View = "view";

		public const string Day = "d";

		public const string Month = "m";

		public const string Year = "y";

		public const string SmallWebPart = "part";

		public const string Inbox = "inbox";

		public const string Calendar = "calendar";

		public const string Tasks = "tasks";

		public const string Contacts = "contacts";

		public const string PublicFolders = "publicfolders";

		private Dictionary<string, string> webPartParameters;

		private PropertyDefinition[] folderViewProperties = new PropertyDefinition[]
		{
			FolderSchema.Id,
			FolderSchema.DisplayName,
			FolderSchema.HasChildren,
			StoreObjectSchema.EffectiveRights
		};

		private string isoDateString;

		private UserContext userContext;

		private OwaContext owaContext;
	}
}
