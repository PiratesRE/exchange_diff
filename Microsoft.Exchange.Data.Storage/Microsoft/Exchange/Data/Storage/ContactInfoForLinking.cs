using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class ContactInfoForLinking : IContactLinkingMatchProperties
	{
		protected ContactInfoForLinking(bool linked, PersonId personId, HashSet<PersonId> linkRejectHistory, Guid? galLinkID, byte[] addressBookEntryId, GALLinkState galLinkState, string[] smtpAddressCache, bool userApprovedLink)
		{
			Util.ThrowOnNullArgument(linkRejectHistory, "linkRejectHistory");
			Util.ThrowOnNullArgument(smtpAddressCache, "smtpAddressCache");
			if (personId == null)
			{
				this.personId = PersonId.CreateNew();
				this.isDirty = true;
			}
			else
			{
				this.personId = personId;
			}
			this.linked = linked;
			this.linkRejectHistory = linkRejectHistory;
			this.galLinkID = galLinkID;
			this.addressBookEntryId = addressBookEntryId;
			this.galLinkState = galLinkState;
			this.smtpAddressCache = smtpAddressCache;
			this.userApprovedLink = userApprovedLink;
		}

		protected ContactInfoForLinking(PropertyBagAdaptor propertyBag) : this(propertyBag.GetValueOrDefault<bool>(ContactSchema.Linked, false), propertyBag.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null), ContactInfoForLinking.GetPropertyAsHashSet<PersonId>(propertyBag, ContactSchema.LinkRejectHistory, new HashSet<PersonId>()), propertyBag.GetValueOrDefault<Guid?>(ContactSchema.GALLinkID, null), propertyBag.GetValueOrDefault<byte[]>(ContactSchema.AddressBookEntryId, null), propertyBag.GetValueOrDefault<GALLinkState>(ContactSchema.GALLinkState, GALLinkState.NotLinked), propertyBag.GetValueOrDefault<string[]>(ContactSchema.SmtpAddressCache, Array<string>.Empty), propertyBag.GetValueOrDefault<bool>(ContactSchema.UserApprovedLink, false))
		{
			this.ItemId = propertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
			this.EmailAddresses = ContactInfoForLinking.GetEmailAddresses(propertyBag);
			this.GivenName = propertyBag.GetValueOrDefault<string>(ContactSchema.GivenName, string.Empty);
			this.Surname = propertyBag.GetValueOrDefault<string>(ContactSchema.Surname, string.Empty);
			this.DisplayName = propertyBag.GetValueOrDefault<string>(StoreObjectSchema.DisplayName, string.Empty);
			this.PartnerNetworkId = propertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
			this.PartnerNetworkUserId = propertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkUserId, string.Empty);
			this.IMAddress = ContactInfoForLinking.CanonicalizeEmailAddress(propertyBag.GetValueOrDefault<string>(ContactSchema.IMAddress, string.Empty));
			this.IsDL = ObjectClass.IsOfClass(propertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty), "IPM.DistList");
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.ItemId == null;
			}
		}

		public HashSet<PersonId> LinkRejectHistory
		{
			get
			{
				return this.linkRejectHistory;
			}
			set
			{
				if (!this.linkRejectHistory.SetEquals(value))
				{
					this.linkRejectHistory = value;
					this.isDirty = true;
				}
			}
		}

		public PersonId PersonId
		{
			get
			{
				return this.personId;
			}
			set
			{
				if (!this.PersonId.Equals(value))
				{
					this.personId = value;
					this.isDirty = true;
				}
			}
		}

		public bool Linked
		{
			get
			{
				return this.linked;
			}
			set
			{
				if (this.linked != value)
				{
					this.linked = value;
					this.isDirty = true;
				}
			}
		}

		public Guid? GALLinkID
		{
			get
			{
				return this.galLinkID;
			}
			private set
			{
				if (!this.galLinkID.Equals(value))
				{
					this.galLinkID = value;
					this.isDirty = true;
				}
			}
		}

		public byte[] AddressBookEntryId
		{
			get
			{
				return this.addressBookEntryId;
			}
			private set
			{
				if (!ArrayComparer<byte>.Comparer.Equals(this.addressBookEntryId, value))
				{
					this.addressBookEntryId = value;
					this.isDirty = true;
				}
			}
		}

		public GALLinkState GALLinkState
		{
			get
			{
				return this.galLinkState;
			}
			private set
			{
				if (this.galLinkState != value)
				{
					this.galLinkState = value;
					this.isDirty = true;
				}
			}
		}

		public bool UserApprovedLink
		{
			get
			{
				return this.userApprovedLink;
			}
			set
			{
				if (this.userApprovedLink != value)
				{
					this.userApprovedLink = value;
					this.isDirty = true;
				}
			}
		}

		public string[] SmtpAddressCache
		{
			get
			{
				return this.smtpAddressCache;
			}
			private set
			{
				if (!ContactInfoForLinking.SmtpAddressCacheComparer.Equals(this.smtpAddressCache, value))
				{
					this.smtpAddressCache = value;
					this.isDirty = true;
				}
			}
		}

		public VersionedId ItemId { get; protected set; }

		public HashSet<string> EmailAddresses { get; protected set; }

		public string GivenName { get; protected set; }

		public string Surname { get; protected set; }

		public string DisplayName { get; protected set; }

		public string PartnerNetworkId { get; protected set; }

		public string PartnerNetworkUserId { get; protected set; }

		public string IMAddress { get; protected set; }

		public bool IsDL { get; protected set; }

		public void UpdateGALLink(GALLinkState galLinkState, Guid? galLinkId, byte[] addressBookEntryId, string[] smtpAddressCache)
		{
			this.GALLinkState = galLinkState;
			this.GALLinkID = galLinkId;
			this.AddressBookEntryId = addressBookEntryId;
			this.SmtpAddressCache = smtpAddressCache;
		}

		public void UpdateGALLinkFrom(ContactInfoForLinking otherContact)
		{
			ArgumentValidator.ThrowIfNull("otherContact", otherContact);
			this.UpdateGALLink(otherContact.GALLinkState, otherContact.GALLinkID, otherContact.AddressBookEntryId, otherContact.SmtpAddressCache);
		}

		public void SetGALLink(ContactInfoForLinkingFromDirectory directoryContact)
		{
			ArgumentValidator.ThrowIfNull("directoryContact", directoryContact);
			this.UpdateGALLink(GALLinkState.Linked, new Guid?(directoryContact.GALLinkID), directoryContact.AddressBookEntryId, directoryContact.SmtpAddressCache);
			this.Linked = true;
		}

		public void ClearGALLink(GALLinkState galLinkState)
		{
			this.UpdateGALLink(galLinkState, null, null, Array<string>.Empty);
		}

		public void Commit(IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker)
		{
			if (this.isDirty)
			{
				this.UpdateContact(logger, performanceTracker);
				this.isDirty = false;
				logger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.ContactUpdate>
				{
					{
						ContactLinkingLogSchema.ContactUpdate.ItemId,
						this.ItemId
					},
					{
						ContactLinkingLogSchema.ContactUpdate.PersonId,
						this.PersonId
					},
					{
						ContactLinkingLogSchema.ContactUpdate.Linked,
						this.linked
					},
					{
						ContactLinkingLogSchema.ContactUpdate.LinkRejectHistory,
						this.linkRejectHistory
					},
					{
						ContactLinkingLogSchema.ContactUpdate.GALLinkState,
						this.galLinkState
					},
					{
						ContactLinkingLogSchema.ContactUpdate.GALLinkID,
						this.galLinkID
					},
					{
						ContactLinkingLogSchema.ContactUpdate.AddressBookEntryId,
						this.addressBookEntryId
					},
					{
						ContactLinkingLogSchema.ContactUpdate.SmtpAddressCache,
						this.smtpAddressCache
					},
					{
						ContactLinkingLogSchema.ContactUpdate.UserApprovedLink,
						this.userApprovedLink
					}
				});
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(300);
			ContactInfoForLinking.AddToString(stringBuilder, "Type", base.GetType().Name);
			ContactInfoForLinking.AddToString(stringBuilder, "DisplayName", this.DisplayName);
			ContactInfoForLinking.AddToString(stringBuilder, "GivenName", this.GivenName);
			ContactInfoForLinking.AddToString(stringBuilder, "Surname", this.Surname);
			ContactInfoForLinking.AddToString(stringBuilder, "EmailAddresses", this.EmailAddresses);
			ContactInfoForLinking.AddToString(stringBuilder, "PartnerNetworkId", this.PartnerNetworkId);
			ContactInfoForLinking.AddToString(stringBuilder, "PartnerNetworkUserId", this.PartnerNetworkUserId);
			ContactInfoForLinking.AddToString(stringBuilder, "IMAddress", this.IMAddress);
			ContactInfoForLinking.AddToString(stringBuilder, "ItemId", this.ItemId);
			ContactInfoForLinking.AddToString(stringBuilder, "PersonId", this.personId);
			ContactInfoForLinking.AddToString(stringBuilder, "Linked", this.linked);
			ContactInfoForLinking.AddToString(stringBuilder, "LinkRejectHistory", this.linkRejectHistory);
			ContactInfoForLinking.AddToString(stringBuilder, "GALLinkState", this.galLinkState);
			ContactInfoForLinking.AddToString(stringBuilder, "GALLinkID", this.galLinkID);
			ContactInfoForLinking.AddToString(stringBuilder, "AddressBookEntryId", this.addressBookEntryId);
			ContactInfoForLinking.AddToString(stringBuilder, "SmtpAddressCache", this.smtpAddressCache);
			ContactInfoForLinking.AddToString(stringBuilder, "UserApprovedLink", this.userApprovedLink);
			ContactInfoForLinking.AddToString(stringBuilder, "IsDirty", this.isDirty);
			return stringBuilder.ToString();
		}

		internal static string CanonicalizeEmailAddress(string emailAddress)
		{
			return emailAddress.Trim().ToLowerInvariant();
		}

		protected abstract void UpdateContact(IExtensibleLogger logger, IContactLinkingPerformanceTracker performanceTracker);

		protected void SetLinkingProperties(PropertyBagAdaptor propertyBag)
		{
			propertyBag.SetValue(ContactSchema.Linked, this.Linked);
			propertyBag.SetValue(ContactSchema.PersonId, this.PersonId);
			if (this.LinkRejectHistory.Count > 0)
			{
				propertyBag.SetValue(ContactSchema.LinkRejectHistory, ContactInfoForLinking.ToArray<PersonId>(this.LinkRejectHistory));
			}
			else
			{
				propertyBag.DeleteValue(ContactSchema.LinkRejectHistory);
			}
			if (this.GALLinkID != null)
			{
				propertyBag.SetValue(ContactSchema.GALLinkID, this.GALLinkID.Value);
			}
			else
			{
				propertyBag.DeleteValue(ContactSchema.GALLinkID);
			}
			if (this.addressBookEntryId != null)
			{
				propertyBag.SetValue(ContactSchema.AddressBookEntryId, this.addressBookEntryId);
			}
			else
			{
				propertyBag.DeleteValue(ContactSchema.AddressBookEntryId);
			}
			if (this.SmtpAddressCache != null)
			{
				propertyBag.SetValue(ContactSchema.SmtpAddressCache, this.SmtpAddressCache);
			}
			else
			{
				propertyBag.DeleteValue(ContactSchema.SmtpAddressCache);
			}
			propertyBag.SetValue(ContactSchema.GALLinkState, this.GALLinkState);
			propertyBag.SetValue(ContactSchema.UserApprovedLink, this.UserApprovedLink);
		}

		protected void RetryOnTransientException(IExtensibleLogger logger, string description, Action action)
		{
			int num = 2;
			try
			{
				IL_02:
				action();
			}
			catch (StorageTransientException ex)
			{
				ContactInfoForLinking.Tracer.TraceError((long)this.GetHashCode(), "ContactInfoForLinking.RetryOnTransientException: failed {0} with id = {1}; given-name: {2}; person-id: {3}.  Exception: {4}", new object[]
				{
					description,
					this.ItemId,
					this.GivenName,
					this.PersonId,
					ex
				});
				if (num > 0)
				{
					num--;
					goto IL_02;
				}
				logger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.Error>
				{
					{
						ContactLinkingLogSchema.Error.Context,
						description
					},
					{
						ContactLinkingLogSchema.Error.Exception,
						ex
					}
				});
				throw;
			}
			catch (StoragePermanentException ex2)
			{
				ContactInfoForLinking.Tracer.TraceError((long)this.GetHashCode(), "ContactInfoForLinking.RetryOnTransientException: failed {0} contact with id = {1}; given-name: {2}; person-id: {3}.  Exception: {4}", new object[]
				{
					description,
					this.ItemId,
					this.GivenName,
					this.PersonId,
					ex2
				});
				logger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.Error>
				{
					{
						ContactLinkingLogSchema.Error.Context,
						description
					},
					{
						ContactLinkingLogSchema.Error.Exception,
						ex2
					}
				});
				throw;
			}
		}

		protected void RetryOnTransientExceptionCatchObjectNotFoundException(IExtensibleLogger logger, string description, Action action)
		{
			this.RetryOnTransientException(logger, description, delegate
			{
				try
				{
					action();
				}
				catch (ObjectNotFoundException ex)
				{
					ContactInfoForLinking.Tracer.TraceError((long)this.GetHashCode(), "Failed to perform {0}. ItemId: {1}; given-name: {2}; person-id: {3}. Exception: {4}", new object[]
					{
						description,
						this.ItemId,
						this.GivenName,
						this.PersonId,
						ex
					});
					logger.LogEvent(new SchemaBasedLogEvent<ContactLinkingLogSchema.Error>
					{
						{
							ContactLinkingLogSchema.Error.Context,
							description
						},
						{
							ContactLinkingLogSchema.Error.Exception,
							ex
						}
					});
				}
			});
		}

		private static void AddToString(StringBuilder text, string description, object value)
		{
			string valueString = ContactLinkingStrings.GetValueString(value);
			if (valueString != null)
			{
				if (text.Length > 0)
				{
					text.Append(", ");
				}
				text.Append(description);
				text.Append("=");
				text.Append(valueString);
			}
		}

		private static HashSet<string> GetEmailAddresses(PropertyBagAdaptor propertyBag)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (StorePropertyDefinition propertyDefinition in ContactSchema.EmailAddressProperties)
			{
				string text = ContactInfoForLinking.CanonicalizeEmailAddress(propertyBag.GetValueOrDefault<string>(propertyDefinition, string.Empty));
				if (!string.IsNullOrEmpty(text))
				{
					hashSet.Add(text);
				}
			}
			return hashSet;
		}

		private static HashSet<T> GetPropertyAsHashSet<T>(PropertyBagAdaptor propertyBag, StorePropertyDefinition property, HashSet<T> defaultValue)
		{
			T[] valueOrDefault = propertyBag.GetValueOrDefault<T[]>(property, null);
			if (valueOrDefault == null || valueOrDefault.Length == 0)
			{
				return defaultValue;
			}
			return new HashSet<T>(valueOrDefault);
		}

		private static T[] ToArray<T>(HashSet<T> set)
		{
			T[] array = new T[set.Count];
			set.CopyTo(array);
			return array;
		}

		[Conditional("DEBUG")]
		private static void ValidateGALLinkProperties(GALLinkState galLinkState, Guid? galLinkId, byte[] addressBookEntryId, string[] smtpAddressCache)
		{
			switch (galLinkState)
			{
			default:
				return;
			}
		}

		private const int MaximumRetry = 2;

		internal static readonly StorePropertyDefinition[] Properties = new StorePropertyDefinition[]
		{
			ItemSchema.Id,
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email2EmailAddress,
			ContactSchema.Email3EmailAddress,
			ContactSchema.GivenName,
			ContactSchema.Surname,
			StoreObjectSchema.DisplayName,
			ContactSchema.PartnerNetworkId,
			ContactSchema.PartnerNetworkUserId,
			ContactProtectedPropertiesSchema.ProtectedEmailAddress,
			ContactSchema.Linked,
			ContactSchema.LinkRejectHistory,
			ContactSchema.IMAddress,
			ContactSchema.GALLinkID,
			ContactSchema.AddressBookEntryId,
			ContactSchema.SmtpAddressCache,
			ContactSchema.GALLinkState,
			ContactSchema.PersonId,
			ContactSchema.UserApprovedLink,
			StoreObjectSchema.ItemClass
		};

		protected static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private bool linked;

		private PersonId personId;

		private HashSet<PersonId> linkRejectHistory;

		private Guid? galLinkID;

		private byte[] addressBookEntryId;

		private GALLinkState galLinkState;

		private string[] smtpAddressCache;

		private bool isDirty;

		private bool userApprovedLink;

		internal static class SmtpAddressCacheComparer
		{
			public static bool Equals(string[] left, string[] right)
			{
				if (object.ReferenceEquals(left, right))
				{
					return true;
				}
				if (left == null || right == null)
				{
					return false;
				}
				if (left.Length != right.Length)
				{
					return false;
				}
				for (int i = 0; i < left.Length; i++)
				{
					if (!StringComparer.Ordinal.Equals(left[i], right[i]))
					{
						return false;
					}
				}
				return true;
			}
		}
	}
}
