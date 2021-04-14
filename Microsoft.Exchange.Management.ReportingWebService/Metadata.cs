using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Services.Providers;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class Metadata
	{
		public Metadata(IPrincipal user, Dictionary<string, IEntity>.ValueCollection entities, Dictionary<string, ResourceType> complexTypeResourceTypes)
		{
			this.user = user;
			this.ResourceTypes = new Dictionary<string, ResourceType>(entities.Count + complexTypeResourceTypes.Count);
			this.ResourceSets = new Dictionary<string, ResourceSet>(entities.Count);
			foreach (KeyValuePair<string, ResourceType> keyValuePair in complexTypeResourceTypes)
			{
				if (!this.ResourceTypes.ContainsKey(keyValuePair.Value.FullName))
				{
					keyValuePair.Value.SetReadOnly();
					this.ResourceTypes.Add(keyValuePair.Value.FullName, keyValuePair.Value);
				}
			}
			this.CreateEntitySchema(entities, complexTypeResourceTypes);
		}

		public Dictionary<string, ResourceType> ResourceTypes { get; private set; }

		public Dictionary<string, ResourceSet> ResourceSets { get; private set; }

		private void CreateEntitySchema(Dictionary<string, IEntity>.ValueCollection entities, Dictionary<string, ResourceType> complexTypeResourceTypes)
		{
			foreach (IEntity entity in entities)
			{
				string text = string.Format("{0}\\{1}", entity.TaskInvocationInfo.SnapinName, entity.TaskInvocationInfo.CmdletName);
				if (this.user.IsInRole(text))
				{
					bool flag = true;
					if (entity.TaskInvocationInfo.Parameters != null)
					{
						foreach (string param in entity.TaskInvocationInfo.Parameters.Keys)
						{
							if (!this.IsParamAllowedByRbac(text, param))
							{
								flag = false;
								break;
							}
						}
					}
					if (flag)
					{
						ResourceType resourceType = this.GetResourceType(entity, complexTypeResourceTypes);
						ResourceSet resourceSet = this.GetResourceSet(entity, resourceType);
						this.ResourceSets.Add(resourceSet.Name, resourceSet);
					}
				}
			}
		}

		private ResourceSet GetResourceSet(IEntity entity, ResourceType resourceType)
		{
			if (!Metadata.GlobalResourceSets.ContainsKey(entity.Name))
			{
				ResourceSet value = this.CreateResourceSet(entity, resourceType);
				Metadata.GlobalResourceSets.TryAdd(entity.Name, value);
			}
			return Metadata.GlobalResourceSets[entity.Name];
		}

		private ResourceSet CreateResourceSet(IEntity entity, ResourceType resourceType)
		{
			ResourceSet resourceSet = new ResourceSet(entity.Name, resourceType);
			resourceSet.SetReadOnly();
			return resourceSet;
		}

		private ResourceType GetResourceType(IEntity entity, Dictionary<string, ResourceType> complexTypeResourceTypes)
		{
			ResourceType resourceType = new ResourceType(entity.ClrType, 0, null, "TenantReporting", entity.ClrType.Name, false);
			if (this.ResourceTypes.ContainsKey(resourceType.FullName))
			{
				resourceType = this.ResourceTypes[resourceType.FullName];
			}
			else
			{
				if (!Metadata.GlobalResourceTypes.ContainsKey(resourceType.FullName))
				{
					this.BuildResourceType(entity, resourceType, complexTypeResourceTypes);
					Metadata.GlobalResourceTypes.TryAdd(resourceType.FullName, resourceType);
				}
				resourceType = Metadata.GlobalResourceTypes[resourceType.FullName];
				this.ResourceTypes.Add(resourceType.FullName, resourceType);
			}
			return resourceType;
		}

		private void BuildResourceType(IEntity entity, ResourceType resourceType, Dictionary<string, ResourceType> complexTypeResourceTypes)
		{
			if (ResourceType.GetPrimitiveResourceType(entity.ClrType) == null)
			{
				foreach (PropertyInfo propertyInfo in entity.ClrType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy))
				{
					if (entity.ReportPropertyCmdletParamsMap == null || !entity.ReportPropertyCmdletParamsMap.ContainsKey(propertyInfo.Name) || this.IsEntityPropertyVisibleForCurrentUser(entity, propertyInfo.Name))
					{
						Type type = propertyInfo.PropertyType;
						Type type2;
						if (ReportingSchema.IsNullableType(type, out type2))
						{
							type = type2;
						}
						ResourcePropertyKind resourcePropertyKind = 1;
						ResourceType resourceType2 = ResourceType.GetPrimitiveResourceType(type);
						if (resourceType2 == null)
						{
							if (type.IsEnum || type.IsValueType)
							{
								throw new NotSupportedException("struct and enum are not supported. For struct, try to change it to class. For enum, try to change it to integer or string.");
							}
							if (type.Equals(entity.ClrType))
							{
								resourceType2 = resourceType;
							}
							else
							{
								resourceType2 = complexTypeResourceTypes[type.FullName];
							}
							resourcePropertyKind = 4;
						}
						resourceType.AddProperty(new ResourceProperty(propertyInfo.Name, resourcePropertyKind | (entity.KeyMembers.Contains(propertyInfo.Name) ? 2 : 0), resourceType2));
					}
				}
			}
			resourceType.SetReadOnly();
		}

		private bool IsEntityPropertyVisibleForCurrentUser(IEntity entity, string property)
		{
			foreach (string param in entity.ReportPropertyCmdletParamsMap[property])
			{
				if (!this.IsParamAllowedByRbac(entity.TaskInvocationInfo.CmdletName, param))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsParamAllowedByRbac(string cmdlet, string param)
		{
			return this.user.IsInRole(string.Format("{0}?{1}", cmdlet, param));
		}

		private static readonly ConcurrentDictionary<string, ResourceType> GlobalResourceTypes = new ConcurrentDictionary<string, ResourceType>();

		private static readonly ConcurrentDictionary<string, ResourceSet> GlobalResourceSets = new ConcurrentDictionary<string, ResourceSet>();

		private IPrincipal user;
	}
}
