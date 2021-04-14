using System;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal sealed class EditContactHelper : DisposeTrackableBase
	{
		public EditContactHelper(UserContext userContext, HttpRequest httpRequest, bool isSaving)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			this.userContext = userContext;
			this.httpRequest = httpRequest;
			this.GetContact(isSaving);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing && this.contact != null)
			{
				this.contact.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EditContactHelper>(this);
		}

		private void GetContact(bool isSaving)
		{
			this.contact = null;
			this.isExistingItem = false;
			string text = this.GetQueryStringParameter("id") ?? this.GetFormParameterStringValue("hidid");
			if (isSaving)
			{
				string formParameterStringValue = this.GetFormParameterStringValue("hidchk");
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(formParameterStringValue))
				{
					VersionedId storeId = Utilities.CreateItemId(this.userContext.MailboxSession, text, formParameterStringValue);
					this.contact = Utilities.GetItem<Contact>(this.userContext, storeId, new PropertyDefinition[0]);
					this.folderId = this.contact.ParentId;
					this.isExistingItem = true;
					return;
				}
				this.MakeFolderId();
				this.contact = Contact.Create(this.userContext.MailboxSession, this.folderId);
				if (Globals.ArePerfCountersEnabled)
				{
					OwaSingleCounters.ItemsCreated.Increment();
					return;
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(text))
				{
					this.contact = Utilities.GetItem<Contact>(this.userContext, Utilities.CreateStoreObjectId(this.userContext.MailboxSession, text), ContactUtilities.PrefetchProperties);
					this.folderId = this.contact.ParentId;
					return;
				}
				this.MakeFolderId();
			}
		}

		private void MakeFolderId()
		{
			string text = this.GetFormParameterStringValue("hidfldid") ?? this.GetQueryStringParameter("fId");
			if (!string.IsNullOrEmpty(text))
			{
				this.folderId = Utilities.CreateStoreObjectId(this.userContext.MailboxSession, text);
				return;
			}
			this.folderId = this.userContext.ContactsFolderId;
		}

		public bool IsPropertiesChanged
		{
			get
			{
				return this.isPropertiesChanged;
			}
		}

		public Contact Contact
		{
			get
			{
				return this.contact;
			}
		}

		public bool IsExistingItem
		{
			get
			{
				return this.isExistingItem;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public string GetStringValue(ContactPropertyInfo propertyInfo)
		{
			return this.GetStringValue(propertyInfo, string.Empty);
		}

		public string GetStringValue(ContactPropertyInfo propertyInfo, string defaultValue)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			string formParameterStringValue = this.GetFormParameterStringValue(propertyInfo.Id);
			if (this.Contact != null)
			{
				string contactPropertyStringValue = this.GetContactPropertyStringValue(propertyInfo);
				if (formParameterStringValue != null && !string.Equals(formParameterStringValue, contactPropertyStringValue))
				{
					this.isPropertiesChanged = true;
				}
				return (formParameterStringValue ?? contactPropertyStringValue) ?? defaultValue;
			}
			if (formParameterStringValue != null && formParameterStringValue != defaultValue)
			{
				this.isPropertiesChanged = true;
			}
			return formParameterStringValue ?? defaultValue;
		}

		public string GetNotes()
		{
			string formParameterStringValue = this.GetFormParameterStringValue("notes");
			if (this.Contact != null)
			{
				string itemBody = ItemUtility.GetItemBody(this.Contact, BodyFormat.TextPlain);
				if (formParameterStringValue != null && !string.Equals(formParameterStringValue, itemBody))
				{
					this.isPropertiesChanged = true;
				}
				return (formParameterStringValue ?? itemBody) ?? string.Empty;
			}
			if (!string.IsNullOrEmpty(formParameterStringValue))
			{
				this.isPropertiesChanged = true;
			}
			return formParameterStringValue ?? string.Empty;
		}

		public int GetIntValue(ContactPropertyInfo propertyInfo, int defaultValue)
		{
			if (propertyInfo == null)
			{
				throw new ArgumentNullException("propertyInfo");
			}
			int? formParameterIntValue = this.GetFormParameterIntValue(propertyInfo);
			if (this.Contact != null)
			{
				int? contactPropertyIntValue = this.GetContactPropertyIntValue(propertyInfo);
				if (formParameterIntValue != null && formParameterIntValue != contactPropertyIntValue)
				{
					this.isPropertiesChanged = true;
				}
				int? num = formParameterIntValue ?? contactPropertyIntValue;
				if (num == null)
				{
					return defaultValue;
				}
				return num.GetValueOrDefault();
			}
			else
			{
				if (formParameterIntValue != null && formParameterIntValue != defaultValue)
				{
					this.isPropertiesChanged = true;
				}
				int? num2 = formParameterIntValue;
				if (num2 == null)
				{
					return defaultValue;
				}
				return num2.GetValueOrDefault();
			}
		}

		private string GetContactPropertyStringValue(ContactPropertyInfo property)
		{
			string result = string.Empty;
			if (this.Contact == null)
			{
				return result;
			}
			string result2 = null;
			string result3 = null;
			if (property == ContactUtilities.Email1EmailAddress || property == ContactUtilities.Email2EmailAddress || property == ContactUtilities.Email3EmailAddress)
			{
				EmailAddressIndex emailPropertyIndex = ContactUtilities.GetEmailPropertyIndex(property);
				ContactUtilities.GetContactEmailAddress(this.Contact, emailPropertyIndex, out result2, out result3);
				return result2;
			}
			if (property == ContactUtilities.Email1DisplayName || property == ContactUtilities.Email2DisplayName || property == ContactUtilities.Email3DisplayName)
			{
				EmailAddressIndex emailPropertyIndex2 = ContactUtilities.GetEmailPropertyIndex(property);
				ContactUtilities.GetContactEmailAddress(this.Contact, emailPropertyIndex2, out result2, out result3);
				return result3;
			}
			string text = this.Contact.TryGetProperty(property.PropertyDefinition) as string;
			if (text != null)
			{
				result = text;
			}
			return result;
		}

		private int? GetContactPropertyIntValue(ContactPropertyInfo propertyInfo)
		{
			if (propertyInfo == ContactUtilities.FileAsId)
			{
				return new int?((int)ContactUtilities.GetFileAs(this.Contact));
			}
			object obj = this.Contact.TryGetProperty(propertyInfo.PropertyDefinition);
			if (obj != null && obj is int)
			{
				return new int?((int)obj);
			}
			return null;
		}

		private int? GetFormParameterIntValue(ContactPropertyInfo property)
		{
			return this.GetFormParameterIntValue(property.Id);
		}

		public string GetFormParameterStringValue(string name)
		{
			if (!Utilities.IsPostRequest(this.httpRequest))
			{
				return null;
			}
			return Utilities.GetFormParameter(this.httpRequest, name, false);
		}

		public string GetQueryStringParameter(string name)
		{
			return Utilities.GetQueryStringParameter(this.httpRequest, name, false);
		}

		private int? GetFormParameterIntValue(string name)
		{
			if (!Utilities.IsPostRequest(this.httpRequest))
			{
				return null;
			}
			int value;
			if (int.TryParse(this.GetFormParameterStringValue(name), out value))
			{
				return new int?(value);
			}
			return null;
		}

		public SaveResult SaveContact()
		{
			foreach (ContactPropertyInfo contactPropertyInfo in ContactUtilities.AllContactProperties)
			{
				if (contactPropertyInfo == ContactUtilities.FileAsId || contactPropertyInfo == ContactUtilities.PostalAddressId || contactPropertyInfo == ContactUtilities.DefaultPhoneNumber)
				{
					this.SetInt(contactPropertyInfo);
				}
				else if (contactPropertyInfo == ContactUtilities.WorkAddressStreet || contactPropertyInfo == ContactUtilities.HomeStreet || contactPropertyInfo == ContactUtilities.OtherStreet)
				{
					this.SetText(contactPropertyInfo, 256);
				}
				else
				{
					this.SetText(contactPropertyInfo);
				}
			}
			this.SetEmail(ContactUtilities.Email1EmailAddress);
			this.SetEmail(ContactUtilities.Email2EmailAddress);
			this.SetEmail(ContactUtilities.Email3EmailAddress);
			this.SetNotes();
			AttachmentUtility.PromoteInlineAttachments(this.contact);
			ConflictResolutionResult conflictResolutionResult = this.Contact.Save(SaveMode.ResolveConflicts);
			if (conflictResolutionResult.SaveStatus != SaveResult.IrresolvableConflict && this.IsExistingItem && Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.ItemsUpdated.Increment();
			}
			return conflictResolutionResult.SaveStatus;
		}

		private void SetText(ContactPropertyInfo propertyInfo)
		{
			string text = this.GetFormParameterStringValue(propertyInfo.Id);
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(text))
			{
				text = string.Empty;
			}
			this.Contact[propertyInfo.PropertyDefinition] = text;
		}

		private void SetText(ContactPropertyInfo propertyInfo, int maxLength)
		{
			string text = this.GetFormParameterStringValue(propertyInfo.Id);
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(text))
			{
				text = string.Empty;
			}
			if (text.Length > maxLength)
			{
				text = text.Substring(0, maxLength);
			}
			this.Contact[propertyInfo.PropertyDefinition] = text;
		}

		private void SetInt(ContactPropertyInfo propertyInfo)
		{
			int num;
			if (int.TryParse(this.GetFormParameterStringValue(propertyInfo.Id), out num))
			{
				this.Contact[propertyInfo.PropertyDefinition] = num;
			}
		}

		private void SetEmail(ContactPropertyInfo propertyInfo)
		{
			ContactPropertyInfo emailDisplayAsProperty = ContactUtilities.GetEmailDisplayAsProperty(propertyInfo);
			string text = this.GetFormParameterStringValue(propertyInfo.Id);
			string text2 = this.GetFormParameterStringValue(emailDisplayAsProperty.Id);
			EmailAddressIndex emailPropertyIndex = ContactUtilities.GetEmailPropertyIndex(propertyInfo);
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(text))
			{
				text = null;
			}
			if (Utilities.WhiteSpaceOnlyOrNullEmpty(text2))
			{
				text2 = null;
			}
			ContactUtilities.SetContactEmailAddress(this.Contact, emailPropertyIndex, text, text2);
		}

		private void SetNotes()
		{
			string formParameterStringValue = this.GetFormParameterStringValue("notes");
			if (formParameterStringValue != null)
			{
				string itemBody = ItemUtility.GetItemBody(this.contact, BodyFormat.TextPlain);
				if (formParameterStringValue.Trim() != itemBody.Trim())
				{
					ItemUtility.SetItemBody(this.contact, BodyFormat.TextPlain, formParameterStringValue);
				}
			}
		}

		public const string NotesFormParameter = "notes";

		public const string ContactIdFormParameter = "hidid";

		public const string ContactIdQueryParameter = "id";

		public const string FolderIdQueryParameter = "fId";

		public const string FolderIdFormParameter = "hidfldid";

		public const string ChangeKeyFormParameter = "hidchk";

		public const int MaxAddressLength = 256;

		private readonly UserContext userContext;

		private readonly HttpRequest httpRequest;

		private StoreObjectId folderId;

		private Contact contact;

		private bool isExistingItem;

		private bool isPropertiesChanged;
	}
}
