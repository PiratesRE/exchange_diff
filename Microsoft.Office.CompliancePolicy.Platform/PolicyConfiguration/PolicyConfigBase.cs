using System;
using System.Collections;
using System.Collections.Specialized;

namespace Microsoft.Office.CompliancePolicy.PolicyConfiguration
{
	public abstract class PolicyConfigBase
	{
		public virtual Guid Identity
		{
			get
			{
				object obj = this[PolicyConfigBaseSchema.Identity];
				if (obj != null)
				{
					return (Guid)obj;
				}
				return Guid.Empty;
			}
			set
			{
				this[PolicyConfigBaseSchema.Identity] = value;
			}
		}

		public virtual string Name
		{
			get
			{
				return (string)this[PolicyConfigBaseSchema.Name];
			}
			set
			{
				this[PolicyConfigBaseSchema.Name] = value;
			}
		}

		public virtual Workload Workload
		{
			get
			{
				object obj = this[PolicyConfigBaseSchema.Workload];
				if (obj != null)
				{
					return (Workload)obj;
				}
				return Workload.None;
			}
			set
			{
				this[PolicyConfigBaseSchema.Workload] = value;
			}
		}

		public virtual DateTime? WhenCreatedUTC
		{
			get
			{
				return (DateTime?)this[PolicyConfigBaseSchema.WhenCreatedUTC];
			}
			set
			{
				this[PolicyConfigBaseSchema.WhenCreatedUTC] = value;
			}
		}

		public virtual DateTime? WhenChangedUTC
		{
			get
			{
				return (DateTime?)this[PolicyConfigBaseSchema.WhenChangedUTC];
			}
			set
			{
				this[PolicyConfigBaseSchema.WhenChangedUTC] = value;
			}
		}

		public ChangeType ObjectState
		{
			get
			{
				object obj = this[PolicyConfigBaseSchema.ObjectState];
				if (obj != null)
				{
					return (ChangeType)obj;
				}
				return ChangeType.Add;
			}
			private set
			{
				this[PolicyConfigBaseSchema.ObjectState] = value;
			}
		}

		public PolicyVersion Version
		{
			get
			{
				return (PolicyVersion)this[PolicyConfigBaseSchema.Version];
			}
			set
			{
				this[PolicyConfigBaseSchema.Version] = value;
			}
		}

		internal object RawObject { get; set; }

		protected object this[string key]
		{
			get
			{
				PolicyConfigBase.ChangeTrackingField changeTrackingField = (PolicyConfigBase.ChangeTrackingField)this.changeTrackingFields[key];
				if (changeTrackingField == null)
				{
					return null;
				}
				return changeTrackingField.Data;
			}
			set
			{
				PolicyConfigBase.ChangeTrackingField changeTrackingField = (PolicyConfigBase.ChangeTrackingField)this.changeTrackingFields[key];
				if (changeTrackingField == null)
				{
					changeTrackingField = new PolicyConfigBase.ChangeTrackingField();
					this.changeTrackingFields[key] = changeTrackingField;
				}
				changeTrackingField.Data = value;
				if (!PolicyConfigBaseSchema.ObjectState.Equals(key) && this.ObjectState == ChangeType.None)
				{
					this.ObjectState = ChangeType.Update;
				}
			}
		}

		public virtual void ResetChangeTracking()
		{
			this.ObjectState = ChangeType.None;
			foreach (object obj in this.changeTrackingFields.Values)
			{
				((PolicyConfigBase.ChangeTrackingField)obj).ResetChangeTracking();
			}
		}

		public virtual void MarkAsDeleted()
		{
			this.ObjectState = ChangeType.Delete;
		}

		public virtual void MarkAsUpdated()
		{
			this.ObjectState = ChangeType.Update;
		}

		public bool IsModified(string key)
		{
			ArgumentValidator.ThrowIfNull("key", key);
			PolicyConfigBase.ChangeTrackingField changeTrackingField = (PolicyConfigBase.ChangeTrackingField)this.changeTrackingFields[key];
			return changeTrackingField != null && changeTrackingField.IsModified;
		}

		public virtual void Validate()
		{
			if (string.IsNullOrEmpty(this.Name))
			{
				throw new ArgumentException("Name can't be null or empty");
			}
		}

		internal PolicyConfigBase Clone()
		{
			PolicyConfigBase policyConfigBase = (PolicyConfigBase)base.MemberwiseClone();
			policyConfigBase.changeTrackingFields = new HybridDictionary();
			foreach (object obj in this.changeTrackingFields)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
				policyConfigBase.changeTrackingFields[dictionaryEntry.Key] = ((PolicyConfigBase.ChangeTrackingField)this.changeTrackingFields[dictionaryEntry.Key]).Clone();
			}
			return policyConfigBase;
		}

		private HybridDictionary changeTrackingFields = new HybridDictionary();

		private class ChangeTrackingField
		{
			public bool IsModified { get; private set; }

			public object Data
			{
				get
				{
					return this.data;
				}
				set
				{
					this.data = value;
					this.IsModified = true;
				}
			}

			public void ResetChangeTracking()
			{
				this.IsModified = false;
			}

			public PolicyConfigBase.ChangeTrackingField Clone()
			{
				return (PolicyConfigBase.ChangeTrackingField)base.MemberwiseClone();
			}

			private object data;
		}
	}
}
