using System;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	[Serializable]
	public abstract class ConfigObject : IConfigurable
	{
		public ConfigObject(PropertyBag propertyBag)
		{
			ExTraceGlobals.ConfigObjectTracer.Information<string>((long)this.GetHashCode(), "ConfigObject::ConfigObject - initializing ConfigObject with{0} property bag.", (propertyBag == null) ? "out" : "");
			if (propertyBag == null)
			{
				this.fields = new PropertyBag();
				this.isNew = true;
				this.Fields["Identity"] = Guid.NewGuid().ToString();
				this.InitializeDefaults();
				this.Fields.ResetChangeTracking();
				return;
			}
			this.fields = propertyBag;
			this.isNew = false;
		}

		public ConfigObject() : this(null)
		{
		}

		public virtual string Identity
		{
			get
			{
				return (string)this.Fields["Identity"];
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Identity cannot be null or empty");
				}
				this.Fields["Identity"] = value;
			}
		}

		public abstract string Name { get; set; }

		public DataSourceInfo DataSourceInfo
		{
			get
			{
				return this.dataSourceInfo;
			}
		}

		public bool IsDeleted
		{
			get
			{
				return this.isDeleted;
			}
		}

		public bool IsNew
		{
			get
			{
				return this.isNew;
			}
		}

		public virtual PropertyBag Fields
		{
			get
			{
				return this.fields;
			}
			set
			{
				this.fields = value;
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				return new ConfigObjectId(this.Identity);
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				if (this.IsNew)
				{
					return ObjectState.New;
				}
				if (this.IsDeleted)
				{
					return ObjectState.Deleted;
				}
				if (this.fields != null && this.fields.FieldDictionary != null)
				{
					foreach (object obj in this.fields.FieldDictionary.Values)
					{
						Field field = (Field)obj;
						if (field.IsChanged)
						{
							return ObjectState.Changed;
						}
					}
					return ObjectState.Unchanged;
				}
				return ObjectState.Unchanged;
			}
		}

		public virtual void InitializeDefaults()
		{
		}

		public virtual void Validate()
		{
			this.isValid = true;
		}

		public void SetDataSourceInfo(DataSourceInfo dsi)
		{
			this.dataSourceInfo = dsi;
		}

		public void CopyChangesFrom(ConfigObject changedObject)
		{
			if (changedObject == null)
			{
				throw new ArgumentNullException("changedObject");
			}
			if (changedObject.Fields == null)
			{
				throw new ArgumentNullException("changedObject.Fields");
			}
			foreach (object key in changedObject.Fields.Keys)
			{
				if (changedObject.Fields.IsModified(key))
				{
					this.Fields[key] = changedObject.Fields[key];
				}
			}
		}

		public void ResetChangeTracking()
		{
			this.Fields.ResetChangeTracking();
		}

		ValidationError[] IConfigurable.Validate()
		{
			this.Validate();
			return new ValidationError[0];
		}

		void IConfigurable.CopyChangesFrom(IConfigurable changedObject)
		{
			this.CopyChangesFrom((ConfigObject)changedObject);
		}

		internal void SetIsDeleted(bool isDeleted)
		{
			this.isDeleted = isDeleted;
		}

		internal void SetIsNew(bool isNew)
		{
			this.isNew = isNew;
		}

		internal void RestoreFrom(PropertyBag oldPropertyBag)
		{
			foreach (object key in this.Fields.Keys)
			{
				if (this.Fields.IsModified(key))
				{
					this.Fields[key] = oldPropertyBag[key];
				}
			}
		}

		private bool isDeleted;

		private bool isNew;

		private bool isValid;

		private PropertyBag fields;

		private DataSourceInfo dataSourceInfo;
	}
}
