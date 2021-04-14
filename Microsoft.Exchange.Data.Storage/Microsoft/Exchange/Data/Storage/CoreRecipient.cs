using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CoreRecipient : IValidatable
	{
		internal CoreRecipient(RecipientTable recipientTable, int rowId, CoreRecipient.SetDefaultPropertiesDelegate setDefaultPropertiesDelegate, Participant participant) : this(recipientTable)
		{
			setDefaultPropertiesDelegate(this);
			this.Participant = participant;
			this.PropertyBag[InternalSchema.RecipientBaseParticipant] = participant;
			this.propertyBag[InternalSchema.RowId] = rowId;
			this.EndInitialization();
			this.OnAddRecipient();
		}

		internal CoreRecipient(RecipientTable recipientTable, IList<NativeStorePropertyDefinition> propertyDefinitions, object[] propValues) : this(recipientTable)
		{
			this.propertyBag.PreLoadStoreProperty<NativeStorePropertyDefinition>(propertyDefinitions, propValues);
			this.Participant = this.PropertyBag.GetValueOrDefault<Participant>(InternalSchema.RecipientBaseParticipant);
			this.EndInitialization();
			this.propertyBag.ClearChangeInfo();
		}

		internal CoreRecipient(RecipientTable recipientTable, int rowId) : this(recipientTable)
		{
			this.propertyBag[InternalSchema.RowId] = rowId;
			this.Participant = this.PropertyBag.GetValueOrDefault<Participant>(InternalSchema.RecipientBaseParticipant);
			this.EndInitialization();
			this.OnAddRecipient();
		}

		internal CoreRecipient(CoreRecipient sourceCoreRecipient, int rowId, IRecipientChangeTracker destinationRecipientChangeTracker, ExTimeZone destinationTimeZone)
		{
			this.participant = sourceCoreRecipient.participant;
			this.recipientChangeTracker = destinationRecipientChangeTracker;
			this.propertyBag = new CoreRecipient.CoreRecipientPropertyBag(sourceCoreRecipient.propertyBag, this);
			this.propertyBag.ExTimeZone = destinationTimeZone;
			this.propertyBag[InternalSchema.RowId] = rowId;
			this.EndInitialization();
			this.OnAddRecipient();
		}

		private CoreRecipient(RecipientTable recipientTable)
		{
			this.recipientChangeTracker = recipientTable.RecipientChangeTracker;
			this.propertyBag = new CoreRecipient.CoreRecipientPropertyBag(this);
			this.propertyBag.ExTimeZone = recipientTable.ExTimeZone;
		}

		public ICorePropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public int RowId
		{
			get
			{
				return this.PropertyBag.GetValueOrDefault<int>(InternalSchema.RowId, -1);
			}
		}

		bool IValidatable.ValidateAllProperties
		{
			get
			{
				return true;
			}
		}

		Schema IValidatable.Schema
		{
			get
			{
				return RecipientSchema.Instance;
			}
		}

		internal RecipientId Id
		{
			get
			{
				if (this.id == null)
				{
					this.id = new RecipientId(CoreRecipient.unicodeEncoding.GetBytes(this.GetStringId()));
				}
				return this.id;
			}
		}

		internal Participant Participant
		{
			get
			{
				return this.participant;
			}
			private set
			{
				this.participant = value;
			}
		}

		internal RecipientItemType RecipientItemType
		{
			get
			{
				return MapiUtil.MapiRecipientTypeToRecipientItemType(this.MapiRecipientType);
			}
			set
			{
				EnumValidator.ThrowIfInvalid<RecipientItemType>(value, "value");
				this.MapiRecipientType = MapiUtil.RecipientItemTypeToMapiRecipientType(value, this.Submitted);
			}
		}

		internal bool Submitted
		{
			get
			{
				RecipientType mapiRecipientType = this.MapiRecipientType;
				return mapiRecipientType != RecipientType.Unknown && (this.MapiRecipientType & RecipientType.Submitted) != RecipientType.Orig;
			}
			set
			{
				if (value)
				{
					this.MapiRecipientType |= RecipientType.Submitted;
					return;
				}
				this.MapiRecipientType &= (RecipientType)2147483647;
			}
		}

		private RecipientType MapiRecipientType
		{
			get
			{
				int? valueAsNullable = this.PropertyBag.GetValueAsNullable<int>(InternalSchema.RecipientType);
				RecipientType? recipientType = (valueAsNullable != null) ? new RecipientType?((RecipientType)valueAsNullable.GetValueOrDefault()) : null;
				if (recipientType == null)
				{
					return RecipientType.Unknown;
				}
				return recipientType.GetValueOrDefault();
			}
			set
			{
				this.PropertyBag[InternalSchema.RecipientType] = (int)value;
			}
		}

		public bool TryValidateRecipient()
		{
			if (this.Participant != null)
			{
				throw new InvalidOperationException("Cannot update a participant that has already been initialized.");
			}
			this.Participant = this.PropertyBag.GetValueOrDefault<Participant>(InternalSchema.RecipientBaseParticipant);
			return this.Participant != null;
		}

		void IValidatable.Validate(ValidationContext context, IList<StoreObjectValidationError> validationErrors)
		{
			Validation.ValidateProperties(context, this, this.propertyBag, validationErrors);
		}

		internal MemoryPropertyBag GetMemoryPropertyBag()
		{
			return this.propertyBag;
		}

		internal void SetUnchanged()
		{
			this.recipientState = CoreRecipient.RecipientState.Unchanged;
		}

		internal void InternalUpdateParticipant(Participant newParticipant)
		{
			this.CheckCanUpdateParticipant(newParticipant);
			this.Participant = newParticipant;
			this.PropertyBag[InternalSchema.RecipientBaseParticipant] = this.Participant;
		}

		internal void SetRowId(int rowId)
		{
			CoreRecipient.RecipientState recipientState = this.recipientState;
			this.recipientState = CoreRecipient.RecipientState.Uninitialized;
			try
			{
				this.PropertyBag.SetProperty(InternalSchema.RowId, rowId);
			}
			finally
			{
				this.recipientState = recipientState;
			}
		}

		internal void GetCharsetDetectionData(StringBuilder stringBuilder)
		{
			foreach (StorePropertyDefinition propertyDefinition in RecipientSchema.Instance.DetectCodepageProperties)
			{
				string text = this.PropertyBag.TryGetProperty(propertyDefinition) as string;
				if (text != null)
				{
					stringBuilder.AppendLine(text);
				}
			}
		}

		internal void OnRemoveRecipient()
		{
			switch (this.recipientState)
			{
			case CoreRecipient.RecipientState.Unchanged:
				this.recipientChangeTracker.RemoveUnchangedRecipient(this);
				break;
			case CoreRecipient.RecipientState.Added:
				this.recipientChangeTracker.RemoveAddedRecipient(this);
				break;
			case CoreRecipient.RecipientState.Modified:
				this.recipientChangeTracker.RemoveModifiedRecipient(this);
				break;
			}
			this.recipientState = CoreRecipient.RecipientState.Removed;
		}

		private void OnAddRecipient()
		{
			this.recipientState = CoreRecipient.RecipientState.Added;
			bool flag = false;
			this.recipientChangeTracker.AddRecipient(this, out flag);
			if (flag)
			{
				this.recipientState = CoreRecipient.RecipientState.Modified;
			}
		}

		private void OnModifyRecipient()
		{
			switch (this.recipientState)
			{
			case CoreRecipient.RecipientState.Uninitialized:
			case CoreRecipient.RecipientState.Added:
			case CoreRecipient.RecipientState.Modified:
				break;
			case CoreRecipient.RecipientState.Unchanged:
				this.recipientChangeTracker.OnModifyRecipient(this);
				this.recipientState = CoreRecipient.RecipientState.Modified;
				break;
			case CoreRecipient.RecipientState.Removed:
				throw new InvalidOperationException(ServerStrings.ExCannotModifyRemovedRecipient);
			default:
				return;
			}
		}

		private void EndInitialization()
		{
			this.recipientState = CoreRecipient.RecipientState.Unchanged;
		}

		private string GetStringId()
		{
			return string.Concat(new object[]
			{
				this.MapiRecipientType,
				":",
				this.Participant.RoutingType,
				":",
				this.Participant.EmailAddress
			});
		}

		private void CheckCanUpdateParticipant(Participant newParticipant)
		{
			if (!newParticipant.AreAddressesEqual(this.Participant))
			{
				throw new InvalidOperationException("Participant on a RecipientBase can be updated only to the one with the same address");
			}
		}

		internal const int InvalidRowId = -1;

		private static readonly UnicodeEncoding unicodeEncoding = new UnicodeEncoding();

		private readonly CoreRecipient.CoreRecipientPropertyBag propertyBag;

		private readonly IRecipientChangeTracker recipientChangeTracker;

		private RecipientId id;

		private Participant participant;

		private CoreRecipient.RecipientState recipientState;

		internal enum RecipientState : byte
		{
			Uninitialized,
			Unchanged,
			Added,
			Modified,
			Removed
		}

		internal delegate void SetDefaultPropertiesDelegate(CoreRecipient coreRecipient);

		internal class CoreRecipientPropertyBag : MemoryPropertyBag, ICorePropertyBag, ILocationIdentifierSetter
		{
			internal CoreRecipientPropertyBag(CoreRecipient coreRecipient)
			{
				this.coreRecipient = coreRecipient;
				base.SetAllPropertiesLoaded();
			}

			internal CoreRecipientPropertyBag(CoreRecipient.CoreRecipientPropertyBag propertyBag, CoreRecipient coreRecipient) : base(propertyBag)
			{
				this.coreRecipient = coreRecipient;
			}

			T ICorePropertyBag.GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
			{
				return base.GetValueOrDefault<T>(propertyDefinition, default(T));
			}

			T ICorePropertyBag.GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
			{
				return base.GetValueOrDefault<T>(propertyDefinition, defaultValue);
			}

			T? ICorePropertyBag.GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition)
			{
				return base.GetValueAsNullable<T>(propertyDefinition);
			}

			public Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
			{
				throw new NotSupportedException("Streams are not supported on Recipients.");
			}

			public void Reload()
			{
				throw new NotSupportedException("Reload is not supported.");
			}

			protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.CheckCanModify(propertyDefinition);
				base.SetValidatedStoreProperty(propertyDefinition, propertyValue);
				this.coreRecipient.OnModifyRecipient();
			}

			protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
			{
				this.CheckCanModify(propertyDefinition);
				base.DeleteStoreProperty(propertyDefinition);
				this.coreRecipient.OnModifyRecipient();
			}

			private void CheckCanModify(StorePropertyDefinition propertyDefinition)
			{
				if (!(propertyDefinition is PropertyTagPropertyDefinition) || (this.coreRecipient.recipientState != CoreRecipient.RecipientState.Uninitialized && propertyDefinition == InternalSchema.RowId))
				{
					throw PropertyError.ToException(new PropertyError[]
					{
						new PropertyError(propertyDefinition, PropertyErrorCode.NotSupported)
					});
				}
			}

			private readonly CoreRecipient coreRecipient;
		}
	}
}
