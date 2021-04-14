using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Serializable]
	public class MsoRawObject : ConfigurableObject
	{
		private string GetXmlElementString(XmlElement xmlElement)
		{
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter w = new XmlTextWriter(stringWriter);
			xmlElement.WriteTo(w);
			return stringWriter.ToString();
		}

		private string SerializeForRpsDelivery(object obj)
		{
			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
			XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
			xmlSerializer.Serialize(xmlWriter, obj);
			return stringWriter.ToString();
		}

		private MultiValuedProperty<string> CollectCapabilities(XmlValueAssignedPlan[] assignedPlans)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			foreach (XmlValueAssignedPlan xmlValueAssignedPlan in assignedPlans)
			{
				multiValuedProperty.TryAdd(this.GetXmlElementString(xmlValueAssignedPlan.Plan.Capability));
			}
			return multiValuedProperty;
		}

		private MultiValuedProperty<string> CollectErrorDetails(XmlValueValidationError[] validationErrors)
		{
			MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
			foreach (XmlValueValidationError xmlValueValidationError in validationErrors)
			{
				multiValuedProperty.TryAdd(this.GetXmlElementString(xmlValueValidationError.ErrorInfo.ErrorDetail));
			}
			return multiValuedProperty;
		}

		internal MsoRawObject(SyncObjectId externalObjectId, string serviceInstanceId, DirectoryObjectsAndLinks directoryObjectsAndLinks, bool? allLinksCollected, bool populateRawObject) : this()
		{
			this.ExternalObjectId = externalObjectId;
			this.ServiceInstanceId = serviceInstanceId;
			this.SerializedObjectAndLinks = this.SerializeForRpsDelivery(directoryObjectsAndLinks);
			this.MySyncObject = SyncObject.Create(directoryObjectsAndLinks.Objects[0], directoryObjectsAndLinks.Links, Guid.Empty);
			if (populateRawObject)
			{
				this.ObjectAndLinks = directoryObjectsAndLinks;
			}
			if (allLinksCollected != null)
			{
				this.AllLinksCollected = allLinksCollected;
				this.LinksCollected = new int?((directoryObjectsAndLinks.Links == null) ? 0 : directoryObjectsAndLinks.Links.Length);
			}
			switch (externalObjectId.ObjectClass)
			{
			case DirectoryObjectClass.Account:
			{
				Account account = (Account)directoryObjectsAndLinks.Objects[0];
				if (account.DisplayName != null)
				{
					this.DisplayName = account.DisplayName.Value[0];
					return;
				}
				break;
			}
			case DirectoryObjectClass.Company:
			{
				Company company = (Company)directoryObjectsAndLinks.Objects[0];
				if (company.DisplayName != null)
				{
					this.DisplayName = company.DisplayName.Value[0];
				}
				if (company.AssignedPlan != null)
				{
					this.AssignedPlanCapabilities = this.CollectCapabilities(company.AssignedPlan.Value);
					return;
				}
				break;
			}
			case DirectoryObjectClass.Contact:
			{
				Contact contact = (Contact)directoryObjectsAndLinks.Objects[0];
				if (contact.DisplayName != null)
				{
					this.DisplayName = contact.DisplayName.Value[0];
				}
				if (contact.ValidationError != null && contact.ValidationError.Value != null)
				{
					this.ExchangeValidationError = this.CollectErrorDetails(contact.ValidationError.Value);
					return;
				}
				break;
			}
			case DirectoryObjectClass.Device:
			case DirectoryObjectClass.KeyGroup:
			case DirectoryObjectClass.ServicePrincipal:
			case DirectoryObjectClass.SubscribedPlan:
				break;
			case DirectoryObjectClass.ForeignPrincipal:
			{
				ForeignPrincipal foreignPrincipal = (ForeignPrincipal)directoryObjectsAndLinks.Objects[0];
				if (foreignPrincipal.DisplayName != null)
				{
					this.DisplayName = foreignPrincipal.DisplayName.Value[0];
					return;
				}
				break;
			}
			case DirectoryObjectClass.Group:
			{
				Group group = (Group)directoryObjectsAndLinks.Objects[0];
				if (group.DisplayName != null)
				{
					this.DisplayName = group.DisplayName.Value[0];
				}
				if (group.ValidationError != null && group.ValidationError.Value != null)
				{
					this.ExchangeValidationError = this.CollectErrorDetails(group.ValidationError.Value);
				}
				break;
			}
			case DirectoryObjectClass.User:
			{
				User user = (User)directoryObjectsAndLinks.Objects[0];
				if (user.DisplayName != null)
				{
					this.DisplayName = user.DisplayName.Value[0];
				}
				if (user.WindowsLiveNetId != null)
				{
					this.WindowsLiveNetId = new NetID(user.WindowsLiveNetId.Value[0]);
				}
				if (user.AssignedPlan != null)
				{
					this.AssignedPlanCapabilities = this.CollectCapabilities(user.AssignedPlan.Value);
				}
				if (user.ValidationError != null && user.ValidationError.Value != null)
				{
					this.ExchangeValidationError = this.CollectErrorDetails(user.ValidationError.Value);
					return;
				}
				break;
			}
			default:
				return;
			}
		}

		internal MsoRawObject() : base(new SimpleProviderPropertyBag())
		{
			this.propertyBag.SetField(this.propertyBag.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal SyncObject MySyncObject
		{
			get
			{
				return this.mySyncObject;
			}
			private set
			{
				this.mySyncObject = value;
			}
		}

		public void PopulateSyncObjectData()
		{
			this.SyncObjectData = string.Empty;
			SortedDictionary<string, object> sortedDictionary = new SortedDictionary<string, object>();
			foreach (PropertyDefinition propertyDefinition in this.MySyncObject.Schema.AllProperties)
			{
				SyncPropertyDefinition syncPropertyDefinition = propertyDefinition as SyncPropertyDefinition;
				if (syncPropertyDefinition != null && this.MySyncObject.IsPropertyPresent(syncPropertyDefinition))
				{
					object obj = this.MySyncObject[syncPropertyDefinition];
					ISyncProperty syncProperty = obj as ISyncProperty;
					if (syncProperty != null)
					{
						if (!syncProperty.HasValue)
						{
							continue;
						}
						obj = syncProperty.GetValue();
					}
					if (obj != null)
					{
						if (SuppressingPiiContext.NeedPiiSuppression)
						{
							obj = SuppressingPiiProperty.TryRedact(syncPropertyDefinition, obj, null);
						}
						IList list = obj as IList;
						if (list != null)
						{
							StringBuilder stringBuilder = new StringBuilder();
							stringBuilder.Append("(");
							for (int i = 0; i < list.Count; i++)
							{
								if (i == 0)
								{
									stringBuilder.Append(list[i].ToString());
								}
								else
								{
									stringBuilder.AppendFormat(",{0}", list[i].ToString());
								}
							}
							stringBuilder.Append(")");
							obj = stringBuilder.ToString();
						}
						sortedDictionary[propertyDefinition.Name] = obj;
					}
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (string text in sortedDictionary.Keys)
			{
				object obj2 = sortedDictionary[text];
				stringBuilder2.AppendFormat("[{0}]:{1}\r\n", text, obj2.ToString());
			}
			this.SyncObjectData = stringBuilder2.ToString();
		}

		public string SyncObjectData
		{
			get
			{
				return (string)this[MsoRawObjectSchema.SyncObjectData];
			}
			private set
			{
				this[MsoRawObjectSchema.SyncObjectData] = value;
			}
		}

		public SyncObjectId ExternalObjectId
		{
			get
			{
				return (SyncObjectId)this[MsoRawObjectSchema.ExternalObjectId];
			}
			private set
			{
				this[MsoRawObjectSchema.ExternalObjectId] = value;
			}
		}

		public string ServiceInstanceId
		{
			get
			{
				return (string)this[MsoRawObjectSchema.ServiceInstanceId];
			}
			private set
			{
				this[MsoRawObjectSchema.ServiceInstanceId] = value;
			}
		}

		public DirectoryObjectsAndLinks ObjectAndLinks
		{
			get
			{
				return (DirectoryObjectsAndLinks)this[MsoRawObjectSchema.ObjectAndLinks];
			}
			private set
			{
				this[MsoRawObjectSchema.ObjectAndLinks] = value;
			}
		}

		public string SerializedObjectAndLinks
		{
			get
			{
				return (string)this[MsoRawObjectSchema.SerializedObjectAndLinks];
			}
			private set
			{
				this[MsoRawObjectSchema.SerializedObjectAndLinks] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[MsoRawObjectSchema.DisplayName];
			}
			private set
			{
				this[MsoRawObjectSchema.DisplayName] = value;
			}
		}

		public NetID WindowsLiveNetId
		{
			get
			{
				return (NetID)this[MsoRawObjectSchema.WindowsLiveNetId];
			}
			private set
			{
				this[MsoRawObjectSchema.WindowsLiveNetId] = value;
			}
		}

		public bool? AllLinksCollected
		{
			get
			{
				return (bool?)this[MsoRawObjectSchema.AllLinksCollected];
			}
			private set
			{
				this[MsoRawObjectSchema.AllLinksCollected] = value;
			}
		}

		public int? LinksCollected
		{
			get
			{
				return (int?)this[MsoRawObjectSchema.LinksCollected];
			}
			private set
			{
				this[MsoRawObjectSchema.LinksCollected] = value;
			}
		}

		public MultiValuedProperty<string> AssignedPlanCapabilities
		{
			get
			{
				return (MultiValuedProperty<string>)this[MsoRawObjectSchema.AssignedPlanCapabilities];
			}
			private set
			{
				this[MsoRawObjectSchema.AssignedPlanCapabilities] = value;
			}
		}

		public MultiValuedProperty<string> ExchangeValidationError
		{
			get
			{
				return (MultiValuedProperty<string>)this[MsoRawObjectSchema.ExchangeValidationError];
			}
			private set
			{
				this[MsoRawObjectSchema.ExchangeValidationError] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MsoRawObject.schema;
			}
		}

		private static MsoRawObjectSchema schema = ObjectSchema.GetInstance<MsoRawObjectSchema>();

		[NonSerialized]
		private SyncObject mySyncObject;
	}
}
