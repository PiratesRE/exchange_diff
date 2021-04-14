using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "UpdateCreatePersonaRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class UpdateCreatePersonaRequestBase : BaseRequest
	{
		[XmlElement("PropertyUpdates")]
		[DataMember(Name = "PropertyUpdates", IsRequired = false)]
		public PersonaPropertyUpdate[] PropertyUpdates
		{
			get
			{
				return this.propertyUpdates;
			}
			set
			{
				this.propertyUpdates = value;
			}
		}

		[DataMember(Name = "PersonaId", IsRequired = false)]
		[XmlElement("PersonaId")]
		public ItemId PersonaId
		{
			get
			{
				return this.personaId;
			}
			set
			{
				this.personaId = value;
			}
		}

		[DataMember(Name = "PersonTypeString", IsRequired = false)]
		[XmlElement("PersonTypeString")]
		public string PersonTypeString
		{
			get
			{
				return this.personTypeString;
			}
			set
			{
				this.personTypeString = value;
			}
		}

		[XmlElement("ParentFolderId")]
		[DataMember(Name = "ParentFolderId", IsRequired = false, EmitDefaultValue = false)]
		public TargetFolderId ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
			set
			{
				this.parentFolderId = value;
			}
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		private PersonaPropertyUpdate[] propertyUpdates;

		private ItemId personaId;

		private string personTypeString;

		private TargetFolderId parentFolderId;
	}
}
