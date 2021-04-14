using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.DirSync;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal abstract class SyncObject : IPropertyBag, IReadOnlyPropertyBag, ICloneable
	{
		public SyncObject(SyncDirection syncDirection)
		{
			this.propertyBag = new ADPropertyBag();
			this.propertyBag.SetObjectVersion(ExchangeObjectVersion.Exchange2010);
			this.SyncDirection = syncDirection;
		}

		public abstract SyncObjectSchema Schema { get; }

		public bool IsDeleted
		{
			get
			{
				return (bool)this[SyncObjectSchema.Deleted];
			}
			set
			{
				this[SyncObjectSchema.Deleted] = value;
			}
		}

		public string ContextId
		{
			get
			{
				return (string)this[SyncObjectSchema.ContextId];
			}
		}

		public bool HasBaseProperties
		{
			get
			{
				return this.hasBaseProperties;
			}
		}

		public bool HasLinkedProperties
		{
			get
			{
				return this.hasLinkedProperties;
			}
		}

		public Guid BatchId { get; private set; }

		public string ObjectId
		{
			get
			{
				return (string)this[SyncObjectSchema.ObjectId];
			}
		}

		public ADObjectId Id
		{
			get
			{
				return (ADObjectId)this[ADObjectSchema.Id];
			}
		}

		public bool All
		{
			get
			{
				return (bool)this[SyncObjectSchema.All];
			}
			private set
			{
				this[SyncObjectSchema.All] = value;
			}
		}

		public MultiValuedProperty<ValidationError> PropertyValidationErrors
		{
			get
			{
				return (MultiValuedProperty<ValidationError>)this[SyncObjectSchema.PropertyValidationErrors];
			}
			private set
			{
				this[SyncObjectSchema.PropertyValidationErrors] = value;
			}
		}

		public SyncState SyncState { get; set; }

		public SyncDirection SyncDirection { get; private set; }

		public ServiceInstanceId FaultInServiceInstance
		{
			get
			{
				return (ServiceInstanceId)this[SyncObjectSchema.FaultInServiceInstance];
			}
		}

		internal abstract DirectoryObjectClass ObjectClass { get; }

		private static XmlReaderSettings SyncObjectXmlSettings
		{
			get
			{
				if (SyncObject.syncObjectXmlSettings == null)
				{
					XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
					xmlReaderSettings.ValidationType = ValidationType.Schema;
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "Annotations.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "DirectoryChange.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "DirectorySync.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "DirectorySync2.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "DirectorySyncMetadata.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "Serialization.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "Serialization.Arrays.xsd"));
					xmlSchemaSet.Add(SyncObject.LoadSchema(executingAssembly, "System.xsd"));
					xmlReaderSettings.Schemas.Add(xmlSchemaSet);
					xmlReaderSettings.Schemas.Compile();
					SyncObject.syncObjectXmlSettings = xmlReaderSettings;
				}
				return SyncObject.syncObjectXmlSettings;
			}
		}

		public object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				SyncPropertyDefinition syncPropertyDefinition = propertyDefinition as SyncPropertyDefinition;
				if (syncPropertyDefinition == null || !syncPropertyDefinition.IsForwardSync || (syncPropertyDefinition.Flags & SyncPropertyDefinitionFlags.Ignore) != (SyncPropertyDefinitionFlags)0)
				{
					return this.propertyBag[propertyDefinition];
				}
				if (this.IsPropertyPresent(syncPropertyDefinition))
				{
					return SyncPropertyFactory.Create(syncPropertyDefinition.Type, this.propertyBag[syncPropertyDefinition], syncPropertyDefinition.IsMultivalued);
				}
				return SyncPropertyFactory.CreateDefault(syncPropertyDefinition.Type, syncPropertyDefinition.IsMultivalued);
			}
			set
			{
				ISyncProperty syncProperty = value as ISyncProperty;
				this.propertyBag[propertyDefinition] = ((syncProperty == null) ? value : syncProperty.GetValue());
			}
		}

		public static SyncObject Create(DirectoryObject directoryObject, IList<DirectoryLink> directoryLinks, Guid batchId)
		{
			bool flag = directoryObject != null;
			bool flag2 = directoryLinks != null && directoryLinks.Count != 0;
			if (!flag && !flag2)
			{
				throw new ArgumentException("directoryObject");
			}
			DirectoryObjectClass objectClass;
			if (directoryObject != null)
			{
				objectClass = SyncObject.GetObjectClass(directoryObject);
			}
			else
			{
				objectClass = directoryLinks[0].GetSourceClass();
			}
			SyncObject syncObject = SyncObject.CreateBlankObjectByClass(objectClass, SyncDirection.Forward);
			syncObject.BatchId = batchId;
			if (directoryObject != null)
			{
				syncObject.PopulatePropertyBag<string>(SyncObjectSchema.ContextId, directoryObject.ContextId);
				syncObject.PopulatePropertyBag<bool>(SyncObjectSchema.Deleted, directoryObject.Deleted);
				syncObject.PopulatePropertyBag<string>(SyncObjectSchema.ObjectId, directoryObject.ObjectId);
				syncObject.PopulatePropertyBag<bool>(SyncObjectSchema.All, directoryObject.All);
				SyncObject.FwdSyncDataConverter fwdSyncDataConverter = new SyncObject.FwdSyncDataConverter(new Action<SyncPropertyDefinition, DirectoryProperty>(syncObject.PopulatePropertyBag), syncObject.propertyBag);
				directoryObject.ForEachProperty(fwdSyncDataConverter);
				flag = (fwdSyncDataConverter.BasePropertiesModified || directoryObject.Deleted);
			}
			else
			{
				syncObject.PopulatePropertyBag<string>(SyncObjectSchema.ContextId, directoryLinks[0].ContextId);
				syncObject.PopulatePropertyBag<bool>(SyncObjectSchema.Deleted, directoryLinks[0].Deleted);
				syncObject.PopulatePropertyBag<string>(SyncObjectSchema.ObjectId, directoryLinks[0].SourceId);
			}
			syncObject.hasBaseProperties = flag;
			syncObject.hasLinkedProperties = flag2;
			if (directoryLinks != null && directoryLinks.Count > 0)
			{
				foreach (DirectoryLink directoryLink in directoryLinks)
				{
					syncObject.AddLinkValue(directoryLink.GetType().Name, directoryLink.Deleted, directoryLink.GetTargetClass(), directoryLink.TargetId);
				}
			}
			return syncObject;
		}

		public static DirectoryObjectClass GetObjectClass(DirectoryObject directoryObject)
		{
			if (directoryObject is User)
			{
				return DirectoryObjectClass.User;
			}
			if (directoryObject is Group)
			{
				return DirectoryObjectClass.Group;
			}
			if (directoryObject is Contact)
			{
				return DirectoryObjectClass.Contact;
			}
			if (directoryObject is Company)
			{
				return DirectoryObjectClass.Company;
			}
			if (directoryObject is ForeignPrincipal)
			{
				return DirectoryObjectClass.ForeignPrincipal;
			}
			if (directoryObject is SubscribedPlan)
			{
				return DirectoryObjectClass.SubscribedPlan;
			}
			if (directoryObject is Account)
			{
				return DirectoryObjectClass.Account;
			}
			throw new NotSupportedException(string.Format("Not supported object type {0}.", directoryObject.GetType().Name));
		}

		public virtual bool IsValid(bool isFullSyncObject)
		{
			return true;
		}

		public List<DirectoryLink> GetDirectoryLinks()
		{
			List<DirectoryLink> list = new List<DirectoryLink>();
			IDictionary<SyncPropertyDefinition, object> changedProperties = this.GetChangedProperties(SyncSchema.Instance.AllBackSyncLinkedProperties);
			foreach (SyncPropertyDefinition syncPropertyDefinition in changedProperties.Keys)
			{
				if (!(syncPropertyDefinition.ExternalType == typeof(object)))
				{
					MultiValuedProperty<SyncLink> multiValuedProperty = null;
					if (syncPropertyDefinition.IsMultivalued)
					{
						multiValuedProperty = (MultiValuedProperty<SyncLink>)changedProperties[syncPropertyDefinition];
					}
					else if (changedProperties[syncPropertyDefinition] != null)
					{
						multiValuedProperty = new MultiValuedProperty<SyncLink>((SyncLink)changedProperties[syncPropertyDefinition]);
					}
					if (multiValuedProperty != null)
					{
						ICollection<DirectoryLink> directoryLinks = SyncObject.GetDirectoryLinks(this, multiValuedProperty, syncPropertyDefinition);
						list.AddRange(directoryLinks);
					}
				}
			}
			return list;
		}

		public void RemoveProperty(SyncPropertyDefinition propertyDefinition)
		{
			this.propertyBag.Remove(propertyDefinition);
		}

		public object[] GetProperties(ICollection<PropertyDefinition> propertyDefinitionArray)
		{
			throw new NotImplementedException();
		}

		public void SetProperties(ICollection<PropertyDefinition> propertyDefinitionArray, object[] propertyValuesArray)
		{
			this.propertyBag.SetProperties(propertyDefinitionArray, propertyValuesArray);
		}

		public void PopulatePropertyBag(SyncPropertyDefinition propertyDefinition, DirectoryProperty values)
		{
			if (!propertyDefinition.IsForwardSync)
			{
				return;
			}
			if (values != null)
			{
				List<ValidationError> list = new List<ValidationError>();
				object obj = SyncValueConvertor.GetValuesFromDirectoryProperty(propertyDefinition, values, list);
				foreach (ValidationError item in list)
				{
					this.PropertyValidationErrors.Add(item);
				}
				if (propertyDefinition.Equals(SyncRecipientSchema.EmailAddresses) && obj != null)
				{
					ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)obj;
					ProxyAddressCollection proxyAddressCollection2 = new ProxyAddressCollection();
					foreach (ProxyAddress proxyAddress in proxyAddressCollection)
					{
						if (!proxyAddressCollection2.Contains(proxyAddress))
						{
							if (!(proxyAddress is InvalidProxyAddress))
							{
								proxyAddressCollection2.Add(proxyAddress);
							}
							else
							{
								Globals.LogEvent(DirectoryEventLogConstants.Tuple_SyncObjectInvalidProxyAddressStripped, this.ObjectId, new object[]
								{
									proxyAddress.ToString(),
									this.ObjectId,
									this.ContextId
								});
							}
						}
					}
					obj = proxyAddressCollection2;
				}
				if (propertyDefinition.Equals(SyncUserSchema.WindowsLiveID) && obj != null)
				{
					string text = obj.ToString();
					string text2 = this.ObjectId.Replace("-", string.Empty);
					if (text.StartsWith(text2, StringComparison.OrdinalIgnoreCase))
					{
						obj = new SmtpAddress(text.Substring(text2.Length));
					}
				}
				if (obj != null || list.Count == 0)
				{
					this.propertyBag[propertyDefinition] = obj;
				}
			}
		}

		public void PopulatePropertyBag<T>(SyncPropertyDefinition propertyDefinition, T value)
		{
			this.propertyBag[propertyDefinition] = value;
		}

		public void PopulateForwardSyncDataFromPropertyBag(ADRawEntry adRawEntry)
		{
			foreach (ADPropertyDefinition propertyDefinition in SyncObject.ForwardSyncProperties)
			{
				this[propertyDefinition] = adRawEntry[propertyDefinition];
			}
		}

		public virtual object Clone()
		{
			SyncObject syncObject = SyncObject.CreateBlankObjectByClass(this.ObjectClass, this.SyncDirection);
			syncObject.propertyBag = (ADPropertyBag)this.propertyBag.Clone();
			syncObject.hasBaseProperties = this.hasBaseProperties;
			syncObject.hasLinkedProperties = this.hasLinkedProperties;
			return syncObject;
		}

		public virtual void CopyForwardChangeFrom(SyncObject sourceObject)
		{
			this.CopyChangeFrom(sourceObject, null);
			this.All = (this.All || sourceObject.All);
		}

		public void CopyChangeFrom(SyncObject sourceObject, SyncPropertyDefinition[] properties)
		{
			IDictionary<SyncPropertyDefinition, object> changedProperties = sourceObject.GetChangedProperties();
			using (IEnumerator<KeyValuePair<SyncPropertyDefinition, object>> enumerator = changedProperties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<SyncPropertyDefinition, object> property = enumerator.Current;
					if (properties != null && properties.Length != 0)
					{
						if (!Array.Exists<SyncPropertyDefinition>(properties, delegate(SyncPropertyDefinition x)
						{
							KeyValuePair<SyncPropertyDefinition, object> property7 = property;
							return x.Equals(property7.Key);
						}))
						{
							continue;
						}
					}
					KeyValuePair<SyncPropertyDefinition, object> property8 = property;
					if (!property8.Key.IsCalculated)
					{
						goto IL_127;
					}
					KeyValuePair<SyncPropertyDefinition, object> property2 = property;
					if (property2.Key.IsReadOnly)
					{
						KeyValuePair<SyncPropertyDefinition, object> property3 = property;
						using (ReadOnlyCollection<ProviderPropertyDefinition>.Enumerator enumerator2 = property3.Key.SupportingProperties.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								ProviderPropertyDefinition providerPropertyDefinition = enumerator2.Current;
								SyncPropertyDefinition key = providerPropertyDefinition as SyncPropertyDefinition;
								if (key != null && properties != null && properties.Length != 0 && !Array.Exists<SyncPropertyDefinition>(properties, (SyncPropertyDefinition x) => x.Equals(key)) && changedProperties.ContainsKey(key))
								{
									this[key] = changedProperties[key];
								}
							}
							goto IL_14D;
						}
						goto IL_127;
					}
					goto IL_127;
					IL_14D:
					KeyValuePair<SyncPropertyDefinition, object> property4 = property;
					if (property4.Key.IsSyncLink)
					{
						this.hasLinkedProperties = true;
						continue;
					}
					this.hasBaseProperties = true;
					continue;
					IL_127:
					KeyValuePair<SyncPropertyDefinition, object> property5 = property;
					PropertyDefinition key2 = property5.Key;
					KeyValuePair<SyncPropertyDefinition, object> property6 = property;
					this[key2] = property6.Value;
					goto IL_14D;
				}
			}
		}

		public IDictionary<SyncPropertyDefinition, object> GetChangedProperties()
		{
			ICollection<SyncPropertyDefinition> properties;
			switch (this.SyncDirection)
			{
			case SyncDirection.Forward:
				properties = this.Schema.AllForwardSyncProperties;
				break;
			case SyncDirection.Back:
				properties = this.Schema.AllBackSyncProperties;
				break;
			default:
				throw new ArgumentOutOfRangeException("syncDirection");
			}
			return this.GetChangedProperties(properties);
		}

		internal static SyncObject Create(ADPropertyBag propertyBag)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)propertyBag[ADObjectSchema.ObjectClass];
			DirectoryObjectClass directoryObjectClass;
			SyncObject syncObject;
			if (multiValuedProperty.Contains(ADOrganizationalUnit.MostDerivedClass))
			{
				directoryObjectClass = DirectoryObjectClass.Company;
				syncObject = new SyncCompany(SyncDirection.Back);
			}
			else
			{
				directoryObjectClass = SyncRecipient.GetRecipientType(propertyBag);
				DirectoryObjectClass directoryObjectClass2 = directoryObjectClass;
				if (directoryObjectClass2 != DirectoryObjectClass.Contact)
				{
					if (directoryObjectClass2 != DirectoryObjectClass.Group)
					{
						if (directoryObjectClass2 != DirectoryObjectClass.User)
						{
							throw new InvalidOperationException("Unexpected object type");
						}
						syncObject = new SyncUser(SyncDirection.Back);
					}
					else
					{
						syncObject = new SyncGroup(SyncDirection.Back);
					}
				}
				else
				{
					syncObject = new SyncContact(SyncDirection.Back);
				}
			}
			syncObject.propertyBag = propertyBag;
			DirectoryObject directoryObject = syncObject.CreateDirectoryObject();
			SyncObject.BasePropertyModificationChecker basePropertyModificationChecker = new SyncObject.BasePropertyModificationChecker(directoryObjectClass, propertyBag);
			directoryObject.ForEachProperty(basePropertyModificationChecker);
			syncObject.hasBaseProperties = basePropertyModificationChecker.BasePropertiesModified;
			syncObject.hasLinkedProperties = (syncObject.GetChangedProperties(SyncSchema.Instance.AllBackSyncLinkedProperties).Count > 0);
			syncObject.hasLinkedProperties |= (syncObject.GetChangedProperties(SyncSchema.Instance.AllBackSyncShadowLinkedProperties).Count > 0);
			return syncObject;
		}

		internal static string SerializeResponse(object response)
		{
			return SyncObject.SerializeResponse(response, false);
		}

		internal static string SerializeResponse(object response, bool validateWithSchema)
		{
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add("timest", "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01");
			XmlSerializer xmlSerializer = new XmlSerializer(response.GetType(), "http://schemas.microsoft.com/online/directoryservices/sync/2008/11");
			StringBuilder stringBuilder = new StringBuilder();
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = true,
				IndentChars = "  "
			}))
			{
				xmlSerializerNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
				xmlSerializer.Serialize(xmlWriter, response, xmlSerializerNamespaces);
			}
			stringBuilder.Replace("_X", "&#x005f;&#x0058;");
			string text = stringBuilder.ToString();
			if (validateWithSchema)
			{
				using (XmlReader xmlReader = XmlReader.Create(new StringReader(text), SyncObject.SyncObjectXmlSettings))
				{
					while (xmlReader.Read())
					{
					}
				}
			}
			return text;
		}

		internal static GetChangesResponse CreateGetChangesResponse(IEnumerable<SyncObject> objects, bool moreData, byte[] cookie, ServiceInstanceId currentServiceInstanceId)
		{
			DirectoryChanges directoryChanges = new DirectoryChanges();
			List<DirectoryObject> list = new List<DirectoryObject>();
			List<DirectoryLink> list2 = new List<DirectoryLink>();
			SyncObject.AddObjectAndLinksWithChanges(objects, list, list2, currentServiceInstanceId);
			directoryChanges.Links = list2.ToArray();
			directoryChanges.Objects = list.ToArray();
			directoryChanges.Contexts = new DirectoryContext[0];
			directoryChanges.More = moreData;
			directoryChanges.NextCookie = cookie;
			return new GetChangesResponse(directoryChanges);
		}

		internal static GetDirectoryObjectsResponse CreateGetDirectoryObjectsResponse(IEnumerable<SyncObject> objects, bool moreData, byte[] pageToken, DirectoryObjectError[] errors, ServiceInstanceId currentServiceInstanceId)
		{
			DirectoryObjectsAndLinks directoryObjectsAndLinks = new DirectoryObjectsAndLinks();
			List<DirectoryObject> list = new List<DirectoryObject>();
			List<DirectoryLink> list2 = new List<DirectoryLink>();
			SyncObject.AddObjectAndLinksWithChanges(objects, list, list2, currentServiceInstanceId);
			SyncObject.RemoveDuplicates(list, list2);
			directoryObjectsAndLinks.Links = list2.ToArray();
			directoryObjectsAndLinks.Objects = list.ToArray();
			directoryObjectsAndLinks.More = moreData;
			directoryObjectsAndLinks.NextPageToken = pageToken;
			directoryObjectsAndLinks.Errors = (errors ?? new DirectoryObjectError[0]);
			return new GetDirectoryObjectsResponse(directoryObjectsAndLinks);
		}

		internal bool IsPropertyPresent(ADPropertyDefinition property)
		{
			if (this.propertyBag.Contains(property))
			{
				return true;
			}
			if (property.IsCalculated)
			{
				foreach (ProviderPropertyDefinition providerPropertyDefinition in property.SupportingProperties)
				{
					ADPropertyDefinition key = (ADPropertyDefinition)providerPropertyDefinition;
					if (this.propertyBag.Contains(key))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal DirectoryObject ToDirectoryObject()
		{
			DirectoryObject directoryObject = this.CreateDirectoryObject();
			directoryObject.ContextId = this.ContextId;
			directoryObject.ObjectId = this.ObjectId;
			directoryObject.Deleted = this.IsDeleted;
			if (!this.IsDeleted)
			{
				directoryObject.ForEachProperty(new SyncObject.BackSyncDataConverter(this.ObjectClass, this.propertyBag));
			}
			return directoryObject;
		}

		protected static SyncObject CreateBlankObjectByClass(DirectoryObjectClass objectClass, SyncDirection syncDirection)
		{
			switch (objectClass)
			{
			case DirectoryObjectClass.Account:
				return new SyncAccount(syncDirection);
			case DirectoryObjectClass.Company:
				return new SyncCompany(syncDirection);
			case DirectoryObjectClass.Contact:
				return new SyncContact(syncDirection);
			case DirectoryObjectClass.ForeignPrincipal:
				return new SyncForeignPrincipal(syncDirection);
			case DirectoryObjectClass.Group:
				return new SyncGroup(syncDirection);
			case DirectoryObjectClass.SubscribedPlan:
				return new SyncSubscribedPlan(syncDirection);
			case DirectoryObjectClass.User:
				return new SyncUser(syncDirection);
			}
			throw new NotImplementedException(objectClass.ToString());
		}

		[Conditional("DEBUG")]
		protected static void AssertGetterIsInvokedOnlyIfPropertyIsPresent(IPropertyBag propertyBag, ADPropertyDefinition propertyDefinition)
		{
			if (!(propertyBag is ADPropertyBag))
			{
				throw new DataValidationException(new PropertyValidationError(new LocalizedString("Unknown type"), propertyDefinition, propertyBag));
			}
		}

		protected abstract DirectoryObject CreateDirectoryObject();

		private static void AddObjectAndLinksWithChanges(IEnumerable<SyncObject> objects, List<DirectoryObject> directoryObjects, List<DirectoryLink> directoryLinks, ServiceInstanceId currentServiceInstanceId)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (SyncObject syncObject in objects)
			{
				DirectoryObject directoryObject = null;
				if (syncObject.FaultInServiceInstance != null)
				{
					DirectoryObject directoryObject2 = syncObject.CreateDirectoryObject();
					if (directoryObject2 is IValidationErrorSupport)
					{
						directoryObject2.ContextId = syncObject.ContextId;
						directoryObject2.ObjectId = syncObject.ObjectId;
						((IValidationErrorSupport)directoryObject2).ValidationError = SyncObject.CreateFaultinValidationError(currentServiceInstanceId, syncObject.FaultInServiceInstance);
						directoryObject = directoryObject2;
						if (!hashSet.Contains(directoryObject2.ObjectId))
						{
							directoryObjects.Add(directoryObject);
							hashSet.Add(directoryObject2.ObjectId);
						}
					}
				}
				if (directoryObject == null)
				{
					if (syncObject.hasBaseProperties || syncObject.IsDeleted)
					{
						directoryObjects.Add(syncObject.ToDirectoryObject());
					}
					if (syncObject.HasLinkedProperties)
					{
						directoryLinks.AddRange(SyncObject.GetDirectoryLinks(syncObject));
					}
				}
			}
		}

		private static void RemoveDuplicates(List<DirectoryObject> directoryObjects, List<DirectoryLink> directoryLinks)
		{
			if (directoryObjects.Count == 0 || !directoryObjects[directoryObjects.Count - 1].Deleted)
			{
				return;
			}
			HashSet<string> deletedIds = new HashSet<string>();
			int num = 0;
			for (int i = directoryObjects.Count - 1; i >= 0; i--)
			{
				if (directoryObjects[i].Deleted)
				{
					deletedIds.Add(directoryObjects[i].ObjectId);
				}
				else if (deletedIds.Contains(directoryObjects[i].ObjectId))
				{
					directoryObjects.RemoveAt(i);
					num++;
				}
			}
			int num2 = directoryLinks.RemoveAll((DirectoryLink dirLink) => deletedIds.Contains(dirLink.SourceId));
			if (num > 0 || num2 > 0)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug<int, int, int>(0L, "SyncObject.RemoveDuplicates: - Removed {0} duplicate objects and {1} links from the response after analyzing {2} deleted objects", num, num2, deletedIds.Count);
			}
		}

		private static IEnumerable<DirectoryLink> GetDirectoryLinks(SyncObject syncObject)
		{
			List<DirectoryLink> result = new List<DirectoryLink>();
			SyncObject.AddNonShadowLinks(syncObject, result);
			SyncObject.AddShadowLinks(syncObject, result);
			return result;
		}

		private static void AddShadowLinks(SyncObject syncObject, List<DirectoryLink> result)
		{
			if (syncObject.ObjectClass == DirectoryObjectClass.User)
			{
				IDictionary<SyncPropertyDefinition, object> changedProperties = syncObject.GetChangedProperties(SyncSchema.Instance.AllBackSyncShadowLinkedProperties);
				foreach (SyncPropertyDefinition syncPropertyDefinition in changedProperties.Keys)
				{
					MultiValuedProperty<SyncLink> multiValuedProperty = (MultiValuedProperty<SyncLink>)syncObject.propertyBag[syncPropertyDefinition];
					if (multiValuedProperty != null)
					{
						ICollection<DirectoryLink> directoryLinks = SyncObject.GetDirectoryLinks(syncObject, multiValuedProperty, syncPropertyDefinition);
						if (directoryLinks.Count > 0)
						{
							DateTime timestamp = (DateTime)(syncObject[ADRecipientSchema.LastExchangeChangedTime] ?? DateTime.MinValue);
							foreach (DirectoryLink directoryLink in directoryLinks)
							{
								Manager manager = (Manager)directoryLink;
								manager.Timestamp = timestamp;
								manager.TimestampSpecified = true;
							}
						}
						result.AddRange(directoryLinks);
					}
				}
			}
		}

		private static void AddNonShadowLinks(SyncObject syncObject, List<DirectoryLink> result)
		{
			IDictionary<SyncPropertyDefinition, object> changedProperties = syncObject.GetChangedProperties(SyncSchema.Instance.AllBackSyncLinkedProperties);
			foreach (SyncPropertyDefinition syncPropertyDefinition in changedProperties.Keys)
			{
				if (!(syncPropertyDefinition.ExternalType == typeof(object)) && !SyncObject.IsMultimasteredLink(syncObject, syncPropertyDefinition) && ((SyncConfiguration.EnableSyncingBackCloudLinks() && syncPropertyDefinition.IsCloud) || !SyncObject.IsDirsyncedObject(syncObject.propertyBag)))
				{
					MultiValuedProperty<SyncLink> multiValuedProperty = (MultiValuedProperty<SyncLink>)syncObject.propertyBag[syncPropertyDefinition];
					if (multiValuedProperty != null)
					{
						ICollection<DirectoryLink> directoryLinks = SyncObject.GetDirectoryLinks(syncObject, multiValuedProperty, syncPropertyDefinition);
						result.AddRange(directoryLinks);
					}
				}
			}
		}

		private static bool IsDirsyncedObject(ADPropertyBag propertyBag)
		{
			return SyncObject.IsBackSyncRecipientDirSynced(propertyBag) && SyncObject.IsBackSyncOrganizationDirSyncRunning(propertyBag);
		}

		private static bool IsBackSyncRecipientDirSynced(ADPropertyBag propertyBag)
		{
			bool isDirSynced = (bool)propertyBag[ADRecipientSchema.IsDirSynced];
			return ADObject.IsRecipientDirSynced(isDirSynced);
		}

		private static bool IsBackSyncOrganizationDirSyncRunning(ADPropertyBag propertyBag)
		{
			bool isDirSyncRunning = (bool)propertyBag[OrganizationSchema.IsDirSyncRunning];
			MultiValuedProperty<string> dirSyncStatus = (MultiValuedProperty<string>)propertyBag[OrganizationSchema.DirSyncStatus];
			return ExchangeConfigurationUnit.IsOrganizationDirSyncRunning(isDirSyncRunning, dirSyncStatus, new List<DirSyncState>
			{
				DirSyncState.Disabled,
				DirSyncState.PendingEnabled
			});
		}

		private static bool IsMultimasteredLink(SyncObject syncObject, SyncPropertyDefinition propertyDefinition)
		{
			return propertyDefinition == SyncUserSchema.Manager && syncObject.ObjectClass == DirectoryObjectClass.User;
		}

		private static ICollection<DirectoryLink> GetDirectoryLinks(SyncObject syncObject, IEnumerable<SyncLink> links, SyncPropertyDefinition propertyDefinition)
		{
			List<DirectoryLink> list = new List<DirectoryLink>();
			foreach (SyncLink syncLink in links)
			{
				DirectoryLink directoryLink = (DirectoryLink)Activator.CreateInstance(propertyDefinition.ExternalType);
				directoryLink.Deleted = (syncLink.State == LinkState.Removed);
				directoryLink.ContextId = syncObject.ContextId;
				directoryLink.SourceId = syncObject.ObjectId;
				directoryLink.TargetId = syncLink.TargetId;
				directoryLink.SetSourceClass(syncObject.ObjectClass);
				directoryLink.SetTargetClass(syncLink.TargetObjectClass);
				list.Add(directoryLink);
			}
			return list;
		}

		private static XmlSchema LoadSchema(Assembly assembly, string schemaName)
		{
			XmlSchema result;
			using (Stream manifestResourceStream = assembly.GetManifestResourceStream(schemaName))
			{
				result = SafeXmlSchema.Read(manifestResourceStream, null);
			}
			return result;
		}

		private static DirectoryPropertyXmlValidationError CreateFaultinValidationError(ServiceInstanceId sourceServiceInstanceId, ServiceInstanceId targetServiceInstanceId)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Migrated");
			XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("ServiceInstance");
			xmlAttribute.Value = targetServiceInstanceId.InstanceId;
			xmlElement.Attributes.Append(xmlAttribute);
			XmlValueValidationError xmlValueValidationError = new XmlValueValidationError();
			xmlValueValidationError.ErrorInfo = new ValidationErrorValue();
			xmlValueValidationError.ErrorInfo.ServiceInstance = sourceServiceInstanceId.InstanceId;
			xmlValueValidationError.ErrorInfo.Resolved = false;
			xmlValueValidationError.ErrorInfo.Timestamp = DateTime.UtcNow;
			xmlValueValidationError.ErrorInfo.ErrorDetail = xmlElement;
			return new DirectoryPropertyXmlValidationError
			{
				Value = new XmlValueValidationError[]
				{
					xmlValueValidationError
				}
			};
		}

		private IDictionary<SyncPropertyDefinition, object> GetChangedProperties(ICollection<SyncPropertyDefinition> properties)
		{
			Dictionary<SyncPropertyDefinition, object> dictionary = new Dictionary<SyncPropertyDefinition, object>(properties.Count);
			foreach (SyncPropertyDefinition syncPropertyDefinition in properties)
			{
				if (this.IsPropertyPresent(syncPropertyDefinition))
				{
					dictionary[syncPropertyDefinition] = this.propertyBag[syncPropertyDefinition];
				}
			}
			return dictionary;
		}

		private void AddLinkValue(string linkAttributeName, bool isDeleted, DirectoryObjectClass targetClass, string targetId)
		{
			SyncPropertyDefinition syncPropertyDefinition;
			if (!SyncSchema.Instance.TryGetLinkedPropertyDefinitionByMsoPropertyName(linkAttributeName, out syncPropertyDefinition))
			{
				return;
			}
			SyncLink syncLink = new SyncLink(targetId, targetClass, isDeleted ? LinkState.Removed : LinkState.Added);
			if (syncPropertyDefinition.IsMultivalued)
			{
				MultiValuedProperty<SyncLink> multiValuedProperty = (MultiValuedProperty<SyncLink>)this.propertyBag[syncPropertyDefinition];
				multiValuedProperty.TryAdd(syncLink);
				return;
			}
			SyncLink syncLink2 = (SyncLink)this.propertyBag[syncPropertyDefinition];
			if (syncLink2 == null || syncLink2.State == LinkState.Removed)
			{
				this[syncPropertyDefinition] = syncLink;
			}
		}

		internal const string MSOTimestampSchemaName = "http://schemas.microsoft.com/online/directoryservices/sync/metadata/2010/01";

		private const string MSOSchemaName = "http://schemas.microsoft.com/online/directoryservices/sync/2008/11";

		private const string XmlSchemaInstancePrefix = "xsi";

		private const string XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

		public static readonly ICollection<ADPropertyDefinition> ForwardSyncProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.RecipientTypeDetails
		};

		public static readonly ICollection<ADPropertyDefinition> BackSyncProperties = new ADPropertyDefinition[]
		{
			SyncObjectSchema.ObjectId,
			ADRecipientSchema.ExternalDirectoryObjectId,
			ADObjectSchema.ObjectClass,
			ADRecipientSchema.RecipientTypeDetails,
			ADRecipientSchema.AttributeMetadata,
			ADRecipientSchema.LastExchangeChangedTime,
			ADRecipientSchema.IsDirSynced,
			ADRecipientSchema.DirSyncAuthorityMetadata,
			ADRecipientSchema.ExcludedFromBackSync,
			ADObjectSchema.WhenChangedUTC,
			ADRecipientSchema.UsnChanged,
			ADRecipientSchema.ConfigurationXML,
			ExtendedOrganizationalUnitSchema.DirSyncStatusAck
		};

		public static readonly ICollection<ADPropertyDefinition> FullSyncLinkPageBackSyncProperties = new ADPropertyDefinition[]
		{
			SyncObjectSchema.ObjectId,
			ADRecipientSchema.ExternalDirectoryObjectId,
			ADObjectSchema.ObjectClass,
			ADRecipientSchema.RecipientTypeDetails
		};

		private static XmlReaderSettings syncObjectXmlSettings;

		private ADPropertyBag propertyBag;

		private bool hasBaseProperties;

		private bool hasLinkedProperties;

		private abstract class BackSyncDataProcessor : IPropertyProcessor
		{
			protected BackSyncDataProcessor(DirectoryObjectClass objectClass, ADPropertyBag propertyBag)
			{
				this.PropertyBag = propertyBag;
				this.isObjectDirsynced = SyncObject.IsDirsyncedObject(propertyBag);
				this.BacksyncShadowProperties = (objectClass == DirectoryObjectClass.User);
			}

			private protected ADPropertyBag PropertyBag { protected get; private set; }

			private protected bool BacksyncShadowProperties { protected get; private set; }

			public abstract void Process<T>(SyncPropertyDefinition propertyDefinition, ref T values) where T : DirectoryProperty, new();

			protected bool ShouldIgnoreProperty(SyncPropertyDefinition propertyDefinition)
			{
				return !propertyDefinition.IsBackSync || (!propertyDefinition.IsCloud && !this.HasShadow(propertyDefinition) && this.isObjectDirsynced);
			}

			protected bool HasShadow(SyncPropertyDefinition definition)
			{
				return this.BacksyncShadowProperties && definition.ShadowProperty != null;
			}

			private readonly bool isObjectDirsynced;
		}

		private class BasePropertyModificationChecker : SyncObject.BackSyncDataProcessor
		{
			public BasePropertyModificationChecker(DirectoryObjectClass objectClass, ADPropertyBag propertyBag) : base(objectClass, propertyBag)
			{
			}

			public bool BasePropertiesModified { get; private set; }

			public override void Process<T>(SyncPropertyDefinition propertyDefinition, ref T values)
			{
				if (base.ShouldIgnoreProperty(propertyDefinition))
				{
					return;
				}
				SyncPropertyDefinition property = propertyDefinition;
				if (base.HasShadow(propertyDefinition))
				{
					property = (SyncPropertyDefinition)propertyDefinition.ShadowProperty;
				}
				this.BasePropertiesModified |= ADDirSyncHelper.ContainsProperty(base.PropertyBag, property);
			}
		}

		private class FwdSyncDataConverter : IPropertyProcessor
		{
			public FwdSyncDataConverter(Action<SyncPropertyDefinition, DirectoryProperty> populate, PropertyBag propertyBag)
			{
				this.populate = populate;
				this.propertyBag = propertyBag;
			}

			public bool BasePropertiesModified { get; private set; }

			public void Process<T>(SyncPropertyDefinition propertyDefinition, ref T values) where T : DirectoryProperty, new()
			{
				this.populate(propertyDefinition, values);
				this.BasePropertiesModified |= ADDirSyncHelper.ContainsProperty(this.propertyBag, propertyDefinition);
			}

			private readonly Action<SyncPropertyDefinition, DirectoryProperty> populate;

			private readonly PropertyBag propertyBag;
		}

		private class BackSyncDataConverter : SyncObject.BackSyncDataProcessor
		{
			public BackSyncDataConverter(DirectoryObjectClass objectClass, ADPropertyBag propertyBag) : base(objectClass, propertyBag)
			{
				if (base.BacksyncShadowProperties)
				{
					this.timeStampDictionary = new Dictionary<string, DateTime>(StringComparer.InvariantCultureIgnoreCase);
					MultiValuedProperty<AttributeMetadata> multiValuedProperty = (MultiValuedProperty<AttributeMetadata>)propertyBag[ADRecipientSchema.AttributeMetadata];
					UserConfigXML userConfigXML = (UserConfigXML)propertyBag[ADRecipientSchema.ConfigurationXML];
					DateTime t = DateTime.MinValue;
					if (userConfigXML != null)
					{
						t = userConfigXML.RelocationLastWriteTime;
					}
					using (MultiValuedProperty<AttributeMetadata>.Enumerator enumerator = multiValuedProperty.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							AttributeMetadata attributeMetadata = enumerator.Current;
							SyncPropertyDefinition syncPropertyDefinition = SyncSchema.Instance.GetADPropDefByLdapDisplayName(attributeMetadata.AttributeName) as SyncPropertyDefinition;
							if (syncPropertyDefinition != null && syncPropertyDefinition.IsShadow)
							{
								this.timeStampDictionary[attributeMetadata.AttributeName] = attributeMetadata.LastWriteTime;
								if (t > attributeMetadata.LastWriteTime && userConfigXML != null && userConfigXML.RelocationShadowPropMetaData != null)
								{
									PropertyMetaData propertyMetaData = userConfigXML.RelocationShadowPropMetaData.FirstOrDefault((PropertyMetaData m) => m.AttributeName == attributeMetadata.AttributeName);
									if (propertyMetaData != null)
									{
										this.timeStampDictionary[attributeMetadata.AttributeName] = propertyMetaData.LastWriteTime;
									}
								}
							}
						}
					}
				}
			}

			public override void Process<T>(SyncPropertyDefinition propertyDefinition, ref T values)
			{
				if (base.ShouldIgnoreProperty(propertyDefinition))
				{
					return;
				}
				SyncPropertyDefinition syncPropertyDefinition = propertyDefinition;
				if (base.HasShadow(propertyDefinition))
				{
					base.PropertyBag.SetField(SyncRecipientSchema.UseShadow, true);
					syncPropertyDefinition = (SyncPropertyDefinition)propertyDefinition.ShadowProperty;
				}
				if (typeof(T) != syncPropertyDefinition.ExternalType)
				{
					return;
				}
				if (!ADDirSyncHelper.ContainsProperty(base.PropertyBag, syncPropertyDefinition))
				{
					values = default(T);
					return;
				}
				if (values == null)
				{
					values = Activator.CreateInstance<T>();
				}
				object value = base.PropertyBag[syncPropertyDefinition];
				if ((syncPropertyDefinition == SyncUserSchema.WindowsLiveID || syncPropertyDefinition == SyncUserSchema.WindowsLiveID.ShadowProperty) && (int)base.PropertyBag[SyncUserSchema.RecipientSoftDeletedStatus] != 0)
				{
					string text = (string)base.PropertyBag[SyncObjectSchema.ObjectId];
					text = text.Replace("-", string.Empty);
					if (!string.IsNullOrEmpty(text))
					{
						value = new SmtpAddress(text + base.PropertyBag[syncPropertyDefinition].ToString());
					}
				}
				IList list = SyncValueConvertor.GetValuesForDirectoryProperty(syncPropertyDefinition, value);
				if (SyncObject.BackSyncDataConverter.IsNullableType(syncPropertyDefinition.Type))
				{
					IList list2 = new ArrayList(list.Count);
					foreach (object obj in list)
					{
						if (obj != null)
						{
							list2.Add(obj);
						}
					}
					list = list2;
				}
				values.SetValues(list);
				if (this.timeStampDictionary != null && syncPropertyDefinition.IsShadow)
				{
					string ldapDisplayName = syncPropertyDefinition.LdapDisplayName;
					if (syncPropertyDefinition.IsCalculated)
					{
						ldapDisplayName = ((ADPropertyDefinition)syncPropertyDefinition.SupportingProperties[0]).LdapDisplayName;
					}
					if (this.timeStampDictionary.ContainsKey(ldapDisplayName))
					{
						values.Timestamp = this.timeStampDictionary[ldapDisplayName];
						values.TimestampSpecified = true;
					}
				}
			}

			private static bool IsNullableType(Type type)
			{
				return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
			}

			private readonly Dictionary<string, DateTime> timeStampDictionary;
		}
	}
}
