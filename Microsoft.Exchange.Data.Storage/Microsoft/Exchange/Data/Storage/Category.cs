using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Category
	{
		private Category(MemoryPropertyBag propertyBagToAssume)
		{
			this.propertyBag = propertyBagToAssume;
			this.categoryLastTimeUsed = new Category.CategoryLastTimeUsed(this);
		}

		private Category() : this(new MemoryPropertyBag())
		{
		}

		private Category(Category copyFrom) : this(new MemoryPropertyBag(copyFrom.propertyBag))
		{
		}

		public bool AllowRenameOnFirstUse
		{
			get
			{
				this.CheckAbandoned("AllowRenameOnFirstUse::get");
				return (bool)this.propertyBag[CategorySchema.AllowRenameOnFirstUse];
			}
		}

		public int Color
		{
			get
			{
				this.CheckAbandoned("Color::get");
				return (int)this.propertyBag[CategorySchema.Color];
			}
			set
			{
				this.CheckAbandoned("Color::set");
				this.propertyBag[CategorySchema.Color] = value;
				this.UpdateLastTimeUsed();
			}
		}

		public Guid Guid
		{
			get
			{
				this.CheckAbandoned("Guid::get");
				return (Guid)this.propertyBag[CategorySchema.Guid];
			}
		}

		public int KeyboardShortcut
		{
			get
			{
				this.CheckAbandoned("KeyboardShortcut::get");
				return (int)this.propertyBag[CategorySchema.KeyboardShortcut];
			}
			set
			{
				this.CheckAbandoned("KeyboardShortcut::set");
				this.propertyBag[CategorySchema.KeyboardShortcut] = value;
				this.UpdateLastTimeUsed();
			}
		}

		public string Name
		{
			get
			{
				this.CheckAbandoned("Name::get");
				return (string)this.propertyBag[CategorySchema.Name];
			}
		}

		internal Category.CategoryLastTimeUsed LastTimeUsed
		{
			get
			{
				this.CheckAbandoned("LastTimeUsed::get");
				return this.categoryLastTimeUsed;
			}
		}

		internal MemoryPropertyBag CategoryPropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public static Category Create(string name, int color, bool renameOnFirstUse)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			Category category = new Category();
			category.propertyBag[CategorySchema.Name] = name;
			category.propertyBag[CategorySchema.Color] = color;
			category.propertyBag[CategorySchema.AllowRenameOnFirstUse] = renameOnFirstUse;
			category.propertyBag[CategorySchema.Guid] = Guid.NewGuid();
			category.propertyBag.SetAllPropertiesLoaded();
			category.LastTimeUsed.SetForAllModules(ExDateTime.GetNow(ExTimeZone.UtcTimeZone));
			category.ValidateAndFillInDefaults();
			return category;
		}

		internal static Category Load(IEnumerable<PropValue> propValues)
		{
			Category category = new Category();
			IDirectPropertyBag directPropertyBag = category.propertyBag;
			foreach (PropValue propValue in propValues)
			{
				directPropertyBag.SetValue(propValue.Property, propValue.Value);
			}
			category.propertyBag.SetAllPropertiesLoaded();
			category.propertyBag.ClearChangeInfo();
			category.ValidateAndFillInDefaults();
			return category;
		}

		internal static Category Resolve(Category client, Category server, Category original)
		{
			if (client != null && server != null)
			{
				return Category.Merge(client, server, original);
			}
			if (client != null)
			{
				if (original == null || client.LastTimeUsed[OutlookModule.None] > original.LastTimeUsed[OutlookModule.None])
				{
					return client.Clone();
				}
			}
			else if (server != null && original == null)
			{
				return server.Clone();
			}
			return null;
		}

		internal void AssignMasterCategoryList(MasterCategoryList masterCategoryList)
		{
			this.CheckAbandoned("AssignMasterCategoryList");
			if (this.masterCategoryList == null)
			{
				this.masterCategoryList = masterCategoryList;
				return;
			}
			throw new InvalidOperationException("Category cannot be added to more than one MasterCategoryList");
		}

		internal void Abandon()
		{
			this.isAbandoned = true;
		}

		internal Category Clone()
		{
			return new Category(this);
		}

		internal void Detach()
		{
			this.CheckAbandoned("Detach");
			if (this.masterCategoryList != null)
			{
				this.masterCategoryList = null;
				return;
			}
			throw new InvalidOperationException();
		}

		internal object TryGetProperty(PropertyDefinition propertyDefinition)
		{
			return this.propertyBag.TryGetProperty(propertyDefinition);
		}

		internal void UpdateLastTimeUsed(ExDateTime newLastTimeUsed, OutlookModule? restrictToModule)
		{
			if (restrictToModule != null)
			{
				this.categoryLastTimeUsed[restrictToModule.Value] = newLastTimeUsed;
				this.categoryLastTimeUsed[OutlookModule.None] = newLastTimeUsed;
				return;
			}
			this.categoryLastTimeUsed.SetForAllModules(newLastTimeUsed);
		}

		private static Category Merge(Category client, Category server, Category original)
		{
			return Category.Load(MasterCategoryList.ResolveProperties(client.propertyBag, server.propertyBag, (original != null) ? original.propertyBag : null, AcrProfile.CategoryProfile));
		}

		private void CheckAbandoned(string methodName)
		{
			if (this.isAbandoned)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new InvalidOperationException("This Category object was invalidated by the last call to MasterCategoryList.Save()");
			}
		}

		private void UpdateLastTimeUsed()
		{
			this.UpdateLastTimeUsed(ExDateTime.GetNow(ExTimeZone.UtcTimeZone), new OutlookModule?(OutlookModule.None));
		}

		private void ValidateAndFillInDefaults()
		{
			foreach (PropertyDefinition propertyDefinition in CategorySchema.Instance.AllProperties)
			{
				XmlAttributePropertyDefinition xmlAttributePropertyDefinition = propertyDefinition as XmlAttributePropertyDefinition;
				if (xmlAttributePropertyDefinition != null)
				{
					object obj = this.propertyBag.TryGetProperty(xmlAttributePropertyDefinition);
					PropertyValidationError[] array = null;
					if (PropertyError.IsPropertyNotFound(obj) || (array = xmlAttributePropertyDefinition.Validate(null, obj)).Length > 0)
					{
						if (xmlAttributePropertyDefinition.HasDefaultValue)
						{
							this.propertyBag[xmlAttributePropertyDefinition] = xmlAttributePropertyDefinition.DefaultValue;
						}
						else if (array != null && array.Length > 0)
						{
							throw new PropertyValidationException(array[0].Description, xmlAttributePropertyDefinition, array);
						}
					}
				}
			}
			List<StoreObjectValidationError> list = new List<StoreObjectValidationError>();
			ValidationContext context = new ValidationContext(null);
			foreach (StoreObjectConstraint storeObjectConstraint in CategorySchema.Instance.Constraints)
			{
				StoreObjectValidationError storeObjectValidationError = storeObjectConstraint.Validate(context, this.propertyBag);
				if (storeObjectValidationError != null)
				{
					list.Add(storeObjectValidationError);
				}
			}
			if (list.Count > 0)
			{
				throw new ObjectValidationException(list[0].Description, list.ToArray());
			}
		}

		internal static readonly char[] ProhibitedCharacters = new char[]
		{
			','
		};

		internal static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;

		internal static readonly StringComparison NameComparison = StringComparison.OrdinalIgnoreCase;

		private static readonly string[] forbiddenNameFragments = new string[]
		{
			","
		};

		private readonly Category.CategoryLastTimeUsed categoryLastTimeUsed;

		private readonly MemoryPropertyBag propertyBag;

		private bool isAbandoned;

		private MasterCategoryList masterCategoryList;

		internal sealed class CategoryLastTimeUsed
		{
			internal CategoryLastTimeUsed(Category category)
			{
				this.category = category;
			}

			internal ExDateTime this[OutlookModule module]
			{
				get
				{
					this.category.CheckAbandoned("CategoryLastTimeUsed::this::get");
					return this.category.propertyBag.GetValueOrDefault<ExDateTime>(Category.CategoryLastTimeUsed.outlookModuleToLtuPropDef[module], ExDateTime.MinValue);
				}
				set
				{
					this.category.CheckAbandoned("CategoryLastTimeUsed::this::set");
					if (value.TimeZone != ExTimeZone.UtcTimeZone)
					{
						throw new ArgumentException("CategoryLastTimeUsed operates only on UTC date/times");
					}
					ExDateTime exDateTime = this[module];
					this.category.propertyBag[Category.CategoryLastTimeUsed.outlookModuleToLtuPropDef[module]] = ((value > exDateTime) ? value : exDateTime);
				}
			}

			internal void SetForAllModules(ExDateTime value)
			{
				foreach (PropertyDefinition propertyDefinition in Category.CategoryLastTimeUsed.outlookModuleToLtuPropDef.Values)
				{
					this.category.propertyBag[propertyDefinition] = value;
				}
			}

			private static readonly Dictionary<OutlookModule, StorePropertyDefinition> outlookModuleToLtuPropDef = Util.AddElements<Dictionary<OutlookModule, StorePropertyDefinition>, KeyValuePair<OutlookModule, StorePropertyDefinition>>(new Dictionary<OutlookModule, StorePropertyDefinition>(), new KeyValuePair<OutlookModule, StorePropertyDefinition>[]
			{
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.Calendar, CategorySchema.LastTimeUsedCalendar),
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.Contacts, CategorySchema.LastTimeUsedContacts),
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.Journal, CategorySchema.LastTimeUsedJournal),
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.Mail, CategorySchema.LastTimeUsedMail),
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.Notes, CategorySchema.LastTimeUsedNotes),
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.Tasks, CategorySchema.LastTimeUsedTasks),
				Util.Pair<OutlookModule, StorePropertyDefinition>(OutlookModule.None, CategorySchema.LastTimeUsed)
			});

			private readonly Category category;
		}
	}
}
