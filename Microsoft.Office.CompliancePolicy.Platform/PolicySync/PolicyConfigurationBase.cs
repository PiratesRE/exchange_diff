using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	[DataContract]
	public abstract class PolicyConfigurationBase
	{
		public PolicyConfigurationBase(ConfigurationObjectType objectType)
		{
			this.ObjectType = objectType;
		}

		public PolicyConfigurationBase(ConfigurationObjectType objectType, Guid tenantId, Guid objectId) : this(objectType)
		{
			this.TenantId = tenantId;
			this.ObjectId = objectId;
		}

		[DataMember]
		public ConfigurationObjectType ObjectType { get; set; }

		[DataMember]
		public Guid TenantId { get; set; }

		[DataMember]
		public Guid ObjectId { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ChangeType ChangeType { get; set; }

		[DataMember]
		public Workload Workload { get; set; }

		[DataMember]
		public DateTime? WhenCreatedUTC { get; set; }

		[DataMember]
		public DateTime? WhenChangedUTC { get; set; }

		[DataMember]
		public PolicyVersion Version { get; set; }

		protected static IDictionary<string, string> BasePropertyNameMapping
		{
			get
			{
				return PolicyConfigurationBase.basePropertyNameMapping;
			}
		}

		protected virtual IDictionary<string, string> PropertyNameMapping
		{
			get
			{
				return PolicyConfigurationBase.basePropertyNameMapping;
			}
		}

		public virtual void MergeInto(PolicyConfigBase originalObject, bool isObjectSync, PolicyConfigProvider policyConfigProvider = null)
		{
			ArgumentValidator.ThrowIfNull("originalObject", originalObject);
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public;
			foreach (PropertyInfo propertyInfo in base.GetType().GetProperties(bindingAttr))
			{
				string name = propertyInfo.Name;
				if (this.PropertyNameMapping.ContainsKey(name))
				{
					string name2 = this.PropertyNameMapping[name];
					MethodInfo setMethod = originalObject.GetType().GetProperty(name2, bindingAttr).GetSetMethod();
					if (propertyInfo.PropertyType.Name == typeof(IncrementalAttribute<>).Name)
					{
						object obj;
						if (Utility.GetIncrementalProperty(this, name, out obj))
						{
							setMethod.Invoke(originalObject, new object[]
							{
								obj
							});
						}
					}
					else
					{
						object value = propertyInfo.GetValue(this);
						setMethod.Invoke(originalObject, new object[]
						{
							value
						});
					}
				}
			}
		}

		private static IDictionary<string, string> basePropertyNameMapping = new Dictionary<string, string>
		{
			{
				"Workload",
				PolicyConfigBaseSchema.Workload
			},
			{
				"Name",
				PolicyConfigBaseSchema.Name
			},
			{
				"Version",
				PolicyConfigBaseSchema.Version
			}
		};
	}
}
