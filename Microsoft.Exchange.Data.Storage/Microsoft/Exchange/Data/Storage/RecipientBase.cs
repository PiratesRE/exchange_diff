using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RecipientBase : IRecipientBase, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag
	{
		internal RecipientBase(CoreRecipient coreRecipient)
		{
			this.coreRecipient = coreRecipient;
			this.propertyBag = new RecipientBase.RecipientBasePropertyBag(this.coreRecipient.GetMemoryPropertyBag());
		}

		public RecipientId Id
		{
			get
			{
				return this.coreRecipient.Id;
			}
		}

		public bool? IsDistributionList()
		{
			return this.Participant.GetValueAsNullable<bool>(ParticipantSchema.IsDistributionList);
		}

		public Participant Participant
		{
			get
			{
				return this.coreRecipient.Participant;
			}
		}

		public Schema Schema
		{
			get
			{
				return RecipientSchema.Instance;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.PropertyBag.IsDirty;
			}
		}

		public bool IsPropertyDirty(PropertyDefinition propertyDefinition)
		{
			return this.PropertyBag.IsPropertyDirty(propertyDefinition);
		}

		public void Load()
		{
			this.Load(null);
		}

		public void Load(ICollection<PropertyDefinition> propertyDefinitions)
		{
			this.PropertyBag.Load(propertyDefinitions);
		}

		public Stream OpenPropertyStream(PropertyDefinition propertyDefinition, PropertyOpenMode openMode)
		{
			throw new NotSupportedException();
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return this.PropertyBag[propertyDefinition];
			}
			set
			{
				this.PropertyBag[propertyDefinition] = value;
			}
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			return this.PropertyBag.GetProperties<PropertyDefinition>(propertyDefinitionArray);
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			if (propertyDefinitionArray == null || propertyValuesArray == null)
			{
				throw new ArgumentException(ServerStrings.PropertyDefinitionsValuesNotMatch);
			}
			if (propertyDefinitionArray.Count != propertyValuesArray.Length)
			{
				throw new ArgumentException(ServerStrings.PropertyDefinitionsValuesNotMatch);
			}
			int num = 0;
			foreach (PropertyDefinition propertyDefinition in propertyDefinitionArray)
			{
				this[propertyDefinition] = propertyValuesArray[num++];
			}
		}

		public void Delete(PropertyDefinition propertyDefinition)
		{
			this.PropertyBag.Delete(propertyDefinition);
		}

		public object TryGetProperty(PropertyDefinition property)
		{
			return this.PropertyBag.TryGetProperty(property);
		}

		public T GetValueOrDefault<T>(PropertyDefinition propertyDefinition, T defaultValue)
		{
			return this.PropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		public void SetOrDeleteProperty(PropertyDefinition propertyDefinition, object propertyValue)
		{
			this.PropertyBag.SetOrDeleteProperty(propertyDefinition, propertyValue);
		}

		private void SetProperty(PropertyDefinition propertyDefinition, object value)
		{
			this.PropertyBag.SetProperty(propertyDefinition, value);
		}

		internal T? GetValueAsNullable<T>(StorePropertyDefinition propertyDefinition) where T : struct
		{
			return this.PropertyBag.GetValueAsNullable<T>(propertyDefinition);
		}

		internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition)
		{
			return this.GetValueOrDefault<T>(propertyDefinition, default(T));
		}

		internal T GetValueOrDefault<T>(StorePropertyDefinition propertyDefinition, T defaultValue)
		{
			return this.PropertyBag.GetValueOrDefault<T>(propertyDefinition, defaultValue);
		}

		internal RecipientFlags RecipientFlags
		{
			get
			{
				return this.GetValueOrDefault<RecipientFlags>(InternalSchema.RecipientFlags, RecipientFlags.Sendable);
			}
			set
			{
				EnumValidator.AssertValid<RecipientFlags>(value);
				this[InternalSchema.RecipientFlags] = (int)value;
			}
		}

		public bool Submitted
		{
			get
			{
				return this.coreRecipient.Submitted;
			}
			set
			{
				this.coreRecipient.Submitted = value;
			}
		}

		public RecipientItemType RecipientItemType
		{
			get
			{
				return this.coreRecipient.RecipientItemType;
			}
			set
			{
				this.coreRecipient.RecipientItemType = value;
			}
		}

		internal CoreRecipient CoreRecipient
		{
			get
			{
				return this.coreRecipient;
			}
		}

		internal bool HasFlags(RecipientFlags flags)
		{
			return (this.RecipientFlags & flags) == flags;
		}

		public bool? IsGroupMailbox()
		{
			if (!(this.Participant == null))
			{
				return this.Participant.GetValueAsNullable<bool>(ParticipantSchema.IsGroupMailbox);
			}
			return null;
		}

		public string SmtpAddress()
		{
			if (!(this.Participant == null))
			{
				return this.Participant.GetValueOrDefault<string>(ParticipantSchema.SmtpAddress, string.Empty);
			}
			return string.Empty;
		}

		private PropertyBag PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		protected static void SetDefaultRecipientBaseProperties(CoreRecipient coreRecipient)
		{
			coreRecipient.PropertyBag[InternalSchema.RecipientFlags] = 1;
		}

		private readonly CoreRecipient coreRecipient;

		private readonly RecipientBase.RecipientBasePropertyBag propertyBag;

		internal static readonly IList<PropertyDefinition> ImmutableProperties = new ReadOnlyCollection<PropertyDefinition>(new PropertyDefinition[]
		{
			InternalSchema.DisplayName,
			InternalSchema.EmailAddress,
			InternalSchema.AddrType,
			InternalSchema.EntryId
		});

		private sealed class RecipientBasePropertyBag : ProxyPropertyBag
		{
			internal RecipientBasePropertyBag(PropertyBag propertyBag) : base(propertyBag)
			{
			}

			protected override void SetValidatedStoreProperty(StorePropertyDefinition propertyDefinition, object propertyValue)
			{
				this.CheckCanModify(propertyDefinition);
				base.SetValidatedStoreProperty(propertyDefinition, propertyValue);
			}

			protected override void DeleteStoreProperty(StorePropertyDefinition propertyDefinition)
			{
				this.CheckCanModify(propertyDefinition);
				base.DeleteStoreProperty(propertyDefinition);
			}

			private void CheckCanModify(StorePropertyDefinition propertyDefinition)
			{
				if (RecipientBase.ImmutableProperties.Contains(propertyDefinition))
				{
					throw PropertyError.ToException(new PropertyError[]
					{
						new PropertyError(propertyDefinition, PropertyErrorCode.NotSupported)
					});
				}
			}
		}
	}
}
