using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.Storage
{
	[XmlRoot(Namespace = "http://schemas.microsoft.com/sharing/2008", ElementName = "SharingMessage")]
	[Serializable]
	public sealed class SharingMessage
	{
		[XmlElement]
		public string DataType { get; set; }

		[XmlElement]
		public SharingMessageInitiator Initiator { get; set; }

		[XmlElement(IsNullable = false)]
		public SharingMessageAction Invitation { get; set; }

		[XmlElement(IsNullable = false)]
		public SharingMessageAction Request { get; set; }

		[XmlElement(IsNullable = false)]
		public SharingMessageAction AcceptOfRequest { get; set; }

		[XmlElement(IsNullable = false)]
		public SharingMessageAction DenyOfRequest { get; set; }

		internal static SharingMessage DeserializeFromStream(Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SharingMessage));
			XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(stream);
			xmlTextReader.WhitespaceHandling = WhitespaceHandling.Significant;
			xmlTextReader.Normalization = true;
			return xmlSerializer.Deserialize(xmlTextReader) as SharingMessage;
		}

		internal ValidationResults Validate()
		{
			if (string.IsNullOrEmpty(this.DataType))
			{
				return new ValidationResults(ValidationResult.Failure, "DataType is required");
			}
			if (this.Initiator == null)
			{
				return new ValidationResults(ValidationResult.Failure, "Initiator is required");
			}
			if (this.Invitation == null && this.Request == null && this.AcceptOfRequest == null && this.DenyOfRequest == null)
			{
				return new ValidationResults(ValidationResult.Failure, "At least one of the following are required: Invitation, Request, AcceptOfRequest or DenyOfRequest");
			}
			if (this.AcceptOfRequest != null && this.DenyOfRequest != null)
			{
				return new ValidationResults(ValidationResult.Failure, "AcceptOfRequest and DenyOfRequest are mutually exclusive");
			}
			ValidationResults validationResults = this.Initiator.Validate();
			if (validationResults.Result != ValidationResult.Success)
			{
				return validationResults;
			}
			var array = new <>f__AnonymousType2<SharingMessageAction, SharingMessageKind>[]
			{
				new
				{
					Action = this.Invitation,
					Kind = SharingMessageKind.Invitation
				},
				new
				{
					Action = this.Request,
					Kind = SharingMessageKind.Request
				},
				new
				{
					Action = this.AcceptOfRequest,
					Kind = SharingMessageKind.AcceptOfRequest
				},
				new
				{
					Action = this.DenyOfRequest,
					Kind = SharingMessageKind.DenyOfRequest
				}
			};
			var array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				var <>f__AnonymousType = array2[i];
				if (<>f__AnonymousType.Action != null)
				{
					validationResults = <>f__AnonymousType.Action.Validate(<>f__AnonymousType.Kind);
					if (validationResults.Result != ValidationResult.Success)
					{
						return validationResults;
					}
				}
			}
			return ValidationResults.Success;
		}

		internal void SerializeToStream(Stream stream)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(SharingMessage));
			xmlSerializer.Serialize(stream, this);
		}
	}
}
