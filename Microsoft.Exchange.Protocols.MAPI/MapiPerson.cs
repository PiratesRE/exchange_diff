using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiPerson : MapiPropBagBase
	{
		internal MapiPerson() : base(MapiObjectType.Person)
		{
		}

		protected override PropertyBag StorePropertyBag
		{
			get
			{
				return this.StorePerson;
			}
		}

		internal Recipient StorePerson
		{
			get
			{
				base.ThrowIfNotValid(null);
				return this.storePerson;
			}
		}

		internal bool IsDeleted
		{
			get
			{
				base.ThrowIfNotValid(null);
				return this.storePerson == null;
			}
		}

		public static StorePropTag[] GetRecipientPropListStandard()
		{
			return MapiPerson.recipientPropListStandard;
		}

		public int GetRowId()
		{
			base.ThrowIfNotValid(null);
			return this.rowId;
		}

		public void Delete()
		{
			base.ThrowIfNotValid(null);
			this.parentMapiObject.GetRecipients().GetRecipientCollection().Remove(this.storePerson);
			this.storePerson = null;
		}

		internal static int GetRecipientPropListStandardCount(MapiMessage messageBase)
		{
			if (messageBase == null || !messageBase.IsValid)
			{
				throw new ExExceptionInvalidParameter((LID)61752U, "Null or invalid 'messageBase' parameter in 'GetRecipientPropListStandardCount' method.");
			}
			return MapiPerson.recipientPropListStandard.Length;
		}

		internal void Configure(MapiMessage message, int rowId, Recipient storePerson)
		{
			if (base.IsDisposed)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on a Dispose'd MapiPerson!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)37176U, "Configure cannot be invoked after Dispose.");
			}
			if (base.IsValid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "Configure called on already configured MapiPerson!  Throwing ExExceptionInvalidObject!");
				throw new ExExceptionInvalidObject((LID)53560U, "Object has already been Configure'd");
			}
			this.parentMapiObject = message;
			if (this.parentMapiObject == null || !this.parentMapiObject.IsValid)
			{
				ExTraceGlobals.GeneralTracer.TraceError(0L, "messageBase is null or invalid, throwing ExExceptionInvalidParameter");
				throw new ExExceptionInvalidParameter((LID)41272U, "Expected valid messageBase");
			}
			this.rowId = rowId;
			this.storePerson = storePerson;
			base.Logon = this.parentMapiObject.Logon;
			base.IsValid = true;
		}

		protected override bool TryGetPropertyImp(MapiContext context, ushort propId, out StorePropTag actualPropTag, out object propValue)
		{
			if (propId == 12288)
			{
				propValue = this.GetRowId();
				actualPropTag = PropTag.Recipient.RowId;
				return true;
			}
			return base.TryGetPropertyImp(context, propId, out actualPropTag, out propValue);
		}

		protected override object GetPropertyValueImp(MapiContext context, StorePropTag propTag)
		{
			ushort propId = propTag.PropId;
			object result;
			if (propId == 12288)
			{
				result = ((propTag != PropTag.Recipient.RowId) ? null : this.GetRowId());
			}
			else
			{
				result = base.GetPropertyValueImp(context, propTag);
			}
			return result;
		}

		public int GetRecipientType()
		{
			base.ThrowIfNotValid(null);
			return (int)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.RecipientType);
		}

		internal void SetRecipientType(int recipientType)
		{
			base.ThrowIfNotValid(null);
			this.StorePerson.SetProperty(base.CurrentOperationContext, PropTag.Recipient.RecipientType, recipientType);
		}

		internal string GetDisplayName()
		{
			base.ThrowIfNotValid(null);
			return (string)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.DisplayName);
		}

		internal string GetTransmitableDisplayName()
		{
			base.ThrowIfNotValid(null);
			return (string)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.TransmitableDisplayName);
		}

		internal string GetSimpleDisplayName()
		{
			base.ThrowIfNotValid(null);
			return (string)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.SimpleDisplayName);
		}

		internal byte[] GetEntryId()
		{
			base.ThrowIfNotValid(null);
			return (byte[])this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.EntryId);
		}

		internal string GetAddrType()
		{
			base.ThrowIfNotValid(null);
			return (string)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.AddressType);
		}

		internal string GetEmailAddress()
		{
			base.ThrowIfNotValid(null);
			return (string)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.EmailAddress);
		}

		internal bool GetResponsibility()
		{
			base.ThrowIfNotValid(null);
			return (bool)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.Responsibility);
		}

		internal void SetResponsibility(bool value)
		{
			base.ThrowIfNotValid(null);
			this.StorePerson.SetProperty(base.CurrentOperationContext, PropTag.Recipient.Responsibility, value);
		}

		internal bool GetSendRichInfo()
		{
			base.ThrowIfNotValid(null);
			return (bool)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.SendRichInfo);
		}

		internal int GetDisplayType()
		{
			base.ThrowIfNotValid(null);
			return (int)this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.DisplayType);
		}

		internal byte[] GetSearchKey()
		{
			base.ThrowIfNotValid(null);
			return (byte[])this.StorePerson.GetPropertyValue(base.CurrentOperationContext, PropTag.Recipient.SearchKey);
		}

		internal void SaveChanges(MapiContext context)
		{
			base.ThrowIfNotValid(null);
			if (this.IsDeleted)
			{
				return;
			}
			if (string.Compare("SMTP", this.GetAddrType(), StringComparison.OrdinalIgnoreCase) == 0)
			{
				string value = this.StorePropertyBag.GetPropertyValue(context, PropTag.Recipient.SmtpAddress) as string;
				if (string.IsNullOrEmpty(value))
				{
					this.StorePerson.SetProperty(context, PropTag.Recipient.SmtpAddress, this.GetEmailAddress());
				}
			}
			base.CommitDirtyStreams(context);
		}

		internal override void CheckRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, bool allRights, AccessCheckOperation operation, LID lid)
		{
		}

		internal override void CheckPropertyRights(MapiContext context, FolderSecurity.ExchangeSecurityDescriptorFolderRights requestedRights, AccessCheckOperation operation, LID lid)
		{
		}

		internal void CollectExtraProperties(MapiContext context, HashSet<StorePropTag> extraProperties)
		{
			this.StorePropertyBag.EnumerateProperties(context, delegate(StorePropTag propTag, object propValue)
			{
				extraProperties.Add(propTag);
				return true;
			}, false);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiPerson>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			this.storePerson = null;
			this.rowId = -1;
			this.parentMapiObject = null;
			base.InternalDispose(calledFromDispose);
		}

		private static StorePropTag[] recipientPropListStandard = new StorePropTag[]
		{
			PropTag.Recipient.DisplayName,
			PropTag.Recipient.AddressType,
			PropTag.Recipient.EmailAddress,
			PropTag.Recipient.RowId,
			PropTag.Recipient.InstanceKey,
			PropTag.Recipient.RecipientType,
			PropTag.Recipient.EntryId,
			PropTag.Recipient.SearchKey,
			PropTag.Recipient.TransmitableDisplayName,
			PropTag.Recipient.Responsibility,
			PropTag.Recipient.SendRichInfo
		};

		private MapiMessage parentMapiObject;

		private Recipient storePerson;

		private int rowId;
	}
}
