using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public abstract class ADPresentationObject : ADObject, IConfigurable, ICloneable
	{
		internal ADObject DataObject
		{
			get
			{
				return this.dataObject;
			}
		}

		public ADPresentationObject()
		{
			this.dataObject = null;
		}

		public ADPresentationObject(ADObject dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			this.dataObject = dataObject;
			this.propertyBag = dataObject.propertyBag;
		}

		public override ObjectId Identity
		{
			get
			{
				ObjectId objectId = base.Identity;
				if (objectId is ADObjectId && SuppressingPiiContext.NeedPiiSuppression)
				{
					objectId = (ObjectId)SuppressingPiiProperty.TryRedact(ADObjectSchema.Id, objectId);
				}
				return objectId;
			}
		}

		protected static IEnumerable<PropertyInfo> GetCloneableOnceProperties(ADPresentationObject source)
		{
			return ADPresentationObject.GetPropertiesByAttribute(source, typeof(ProvisionalCloneOnce));
		}

		protected static IEnumerable<PropertyInfo> GetCloneableEnabledStateProperties(ADPresentationObject source)
		{
			return ADPresentationObject.GetPropertiesByAttribute(source, typeof(ProvisionalCloneEnabledState));
		}

		protected static IEnumerable<PropertyInfo> GetCloneableProperties(ADPresentationObject source)
		{
			return ADPresentationObject.GetPropertiesByAttribute(source, typeof(ProvisionalClone));
		}

		private static IEnumerable<PropertyInfo> GetPropertiesByAttribute(ADPresentationObject source, Type type)
		{
			PropertyInfo[] properties = source.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			List<PropertyInfo> list = new List<PropertyInfo>();
			foreach (PropertyInfo propertyInfo in properties)
			{
				object[] customAttributes = propertyInfo.GetCustomAttributes(type, true);
				if (customAttributes.Length != 0)
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.MailboxPlans.CloneLimitedSetOfMailboxPlanProperties.Enabled)
					{
						ProvisionalCloneBase provisionalCloneBase = customAttributes[0] as ProvisionalCloneBase;
						if (provisionalCloneBase != null && provisionalCloneBase.CloneSet == CloneSet.CloneLimitedSet)
						{
							goto IL_A4;
						}
					}
					if (propertyInfo.PropertyType == typeof(MultiValuedProperty<Capability>))
					{
						if (propertyInfo.CanRead)
						{
							list.Add(propertyInfo);
						}
					}
					else if (propertyInfo.CanRead && propertyInfo.CanWrite)
					{
						list.Add(propertyInfo);
					}
				}
				IL_A4:;
			}
			return list;
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.dataObject == null)
			{
				throw new NotSupportedException("Can't call ValidateRead on presentation object when DataObject is null");
			}
			ValidationError[] array = this.dataObject.ValidateRead();
			if (array != null && 0 < array.Length)
			{
				errors.AddRange(array);
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			if (this.dataObject == null)
			{
				throw new NotSupportedException("Can't call ValidateWrite on presentation object when DataObject is null");
			}
			ValidationError[] array = this.dataObject.ValidateWrite();
			if (array != null && 0 < array.Length)
			{
				errors.AddRange(array);
			}
		}

		public override bool IsValid
		{
			get
			{
				if (this.dataObject == null)
				{
					throw new NotSupportedException("Can't call IsValid on presentation object when DataObject is null");
				}
				return this.dataObject.IsValid;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				if (this.dataObject == null)
				{
					throw new NotSupportedException("Can't call MostDerivedObjectClass on presentation object when DataObject is null");
				}
				return this.dataObject.MostDerivedObjectClass;
			}
		}

		internal sealed override ADObjectSchema Schema
		{
			get
			{
				return this.PresentationSchema;
			}
		}

		internal abstract ADPresentationSchema PresentationSchema { get; }

		internal override ObjectSchema DeserializationSchema
		{
			get
			{
				if (this.dataObject == null)
				{
					throw new NotSupportedException("Can't call DeserializationSchema on presentation object when DataObject is null");
				}
				return this.dataObject.ObjectSchema;
			}
		}

		internal override bool ExchangeVersionUpgradeSupported
		{
			get
			{
				return this.dataObject != null && this.dataObject.ExchangeVersionUpgradeSupported;
			}
		}

		private static IEnumerable<PropertyInfo> GetEnabledPropertiesFromPropertyInfoList(ADPresentationObject source)
		{
			return ADPresentationObject.GetEnabledPropertiesFromPropertyInfoList(source, null);
		}

		private static IEnumerable<PropertyInfo> GetEnabledPropertiesFromPropertyInfoList(ADPresentationObject source1, ADPresentationObject source2)
		{
			List<PropertyInfo> list = new List<PropertyInfo>();
			if (source1.CloneableEnabledStateProperties != null)
			{
				foreach (PropertyInfo propertyInfo in source1.CloneableEnabledStateProperties)
				{
					if ((bool)propertyInfo.GetValue(source1, null) || (source2 != null && (bool)propertyInfo.GetValue(source2, null)))
					{
						list.Add(propertyInfo);
					}
				}
			}
			return list;
		}

		internal void ApplyCloneableProperties(ADPresentationObject source)
		{
			IEnumerable<PropertyInfo> enabledPropertiesFromPropertyInfoList = ADPresentationObject.GetEnabledPropertiesFromPropertyInfoList(source);
			IEnumerable<PropertyInfo>[] array = new IEnumerable<PropertyInfo>[]
			{
				this.CloneableOnceProperties,
				this.CloneableProperties,
				enabledPropertiesFromPropertyInfoList
			};
			foreach (IEnumerable<PropertyInfo> enumerable in array)
			{
				foreach (PropertyInfo propertyInfo in enumerable)
				{
					object value = propertyInfo.GetValue(source, null);
					object value2 = propertyInfo.GetValue(this, null);
					if (value != null && value2 != value)
					{
						MultiValuedPropertyBase multiValuedPropertyBase = value as MultiValuedPropertyBase;
						if (multiValuedPropertyBase == null || multiValuedPropertyBase.Count != 0)
						{
							if (propertyInfo.PropertyType == typeof(MultiValuedProperty<Capability>))
							{
								MultiValuedProperty<Capability> multiValuedProperty = (MultiValuedProperty<Capability>)value;
								CapabilityHelper.SetSKUCapability((multiValuedProperty.Count == 0) ? null : new Capability?(multiValuedProperty[0]), (MultiValuedProperty<Capability>)value2);
							}
							else
							{
								propertyInfo.SetValue(this, value, null);
							}
						}
					}
				}
			}
		}

		internal static void ApplyPresentationObjectDelta(ADPresentationObject oldPlan, ADPresentationObject newPlan, ADPresentationObject target, ApplyMailboxPlanFlags flags)
		{
			IEnumerable<PropertyInfo> enabledPropertiesFromPropertyInfoList = ADPresentationObject.GetEnabledPropertiesFromPropertyInfoList(newPlan, oldPlan);
			IEnumerable<PropertyInfo>[] array;
			if (oldPlan == null)
			{
				array = new IEnumerable<PropertyInfo>[]
				{
					target.CloneableOnceProperties,
					target.CloneableProperties,
					enabledPropertiesFromPropertyInfoList
				};
			}
			else
			{
				array = new IEnumerable<PropertyInfo>[]
				{
					target.CloneableProperties,
					enabledPropertiesFromPropertyInfoList
				};
			}
			bool flag = oldPlan == null && newPlan != null && flags.HasFlag(ApplyMailboxPlanFlags.PreservePreviousExplicitlySetValues);
			foreach (IEnumerable<PropertyInfo> enumerable in array)
			{
				foreach (PropertyInfo propertyInfo in enumerable)
				{
					object obj = null;
					object value = propertyInfo.GetValue(newPlan, null);
					MultiValuedPropertyBase multiValuedPropertyBase = value as MultiValuedPropertyBase;
					if (oldPlan != null)
					{
						obj = propertyInfo.GetValue(oldPlan, null);
					}
					if (propertyInfo.PropertyType == typeof(MultiValuedProperty<Capability>))
					{
						if (!object.Equals(obj, value))
						{
							MultiValuedProperty<Capability> sourceCapabilities = MultiValuedProperty<Capability>.Empty;
							if (value != null)
							{
								sourceCapabilities = (MultiValuedProperty<Capability>)value;
							}
							CapabilityHelper.SetSKUCapabilities(newPlan.Name, sourceCapabilities, (MultiValuedProperty<Capability>)propertyInfo.GetValue(target, null));
						}
					}
					else if (obj != null || value != null)
					{
						if (obj == null || value == null)
						{
							if (flag)
							{
								object value2 = propertyInfo.GetValue(target, null);
								ADPropertyDefinition adpropertyDefinition = newPlan.Schema[propertyInfo.Name] as ADPropertyDefinition;
								if (adpropertyDefinition != null && ((adpropertyDefinition.DefaultValue == null && value2 != null) || (adpropertyDefinition.DefaultValue != null && !adpropertyDefinition.DefaultValue.Equals(value2))))
								{
									continue;
								}
							}
							propertyInfo.SetValue(target, value, null);
						}
						else if (!obj.Equals(value))
						{
							if (multiValuedPropertyBase == null)
							{
								propertyInfo.SetValue(target, value, null);
							}
							else
							{
								bool flag2 = false;
								MultiValuedPropertyBase multiValuedPropertyBase2 = obj as MultiValuedPropertyBase;
								if (multiValuedPropertyBase2.Count != multiValuedPropertyBase.Count)
								{
									flag2 = true;
								}
								else
								{
									foreach (object obj2 in ((IEnumerable)multiValuedPropertyBase2))
									{
										bool flag3 = false;
										foreach (object obj3 in ((IEnumerable)multiValuedPropertyBase))
										{
											if (obj2.Equals(obj3))
											{
												flag3 = true;
												break;
											}
										}
										if (!flag3)
										{
											flag2 = true;
											break;
										}
									}
								}
								if (flag2)
								{
									propertyInfo.SetValue(target, value, null);
								}
							}
						}
					}
				}
			}
		}

		protected virtual IEnumerable<PropertyInfo> CloneableProperties
		{
			get
			{
				return ADPresentationObject.GetCloneableProperties(this);
			}
		}

		protected virtual IEnumerable<PropertyInfo> CloneableOnceProperties
		{
			get
			{
				return null;
			}
		}

		protected virtual IEnumerable<PropertyInfo> CloneableEnabledStateProperties
		{
			get
			{
				return null;
			}
		}

		private ADObject dataObject;
	}
}
