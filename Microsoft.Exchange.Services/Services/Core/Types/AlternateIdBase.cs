using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(AlternatePublicFolderId))]
	[KnownType(typeof(AlternateId))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(AlternatePublicFolderItemId))]
	[XmlInclude(typeof(AlternateId))]
	[XmlType(TypeName = "AlternateIdBaseType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[KnownType(typeof(AlternatePublicFolderId))]
	[KnownType(typeof(AlternatePublicFolderItemId))]
	[Serializable]
	public class AlternateIdBase
	{
		public AlternateIdBase()
		{
		}

		internal AlternateIdBase(IdFormat format)
		{
			this.format = format;
		}

		[XmlAttribute]
		[IgnoreDataMember]
		public IdFormat Format
		{
			get
			{
				return this.format;
			}
			set
			{
				this.format = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "Format", IsRequired = true, Order = 1)]
		public string FormatString
		{
			get
			{
				return this.format.ToString();
			}
			set
			{
				this.format = (IdFormat)Enum.Parse(typeof(IdFormat), value);
			}
		}

		internal static BaseAlternateIdConverter GetIdConverter(IdFormat idFormat)
		{
			return AlternateIdBase.alternateIdMap.Member[idFormat];
		}

		internal virtual CanonicalConvertedId Parse()
		{
			throw new InvalidOperationException();
		}

		internal AlternateIdBase ConvertId(IdFormat destinationFormat)
		{
			CanonicalConvertedId canonicalId = this.Parse();
			BaseAlternateIdConverter idConverter = AlternateIdBase.GetIdConverter(destinationFormat);
			return idConverter.Format(canonicalId);
		}

		private static LazyMember<Dictionary<IdFormat, BaseAlternateIdConverter>> alternateIdMap = new LazyMember<Dictionary<IdFormat, BaseAlternateIdConverter>>(() => new Dictionary<IdFormat, BaseAlternateIdConverter>
		{
			{
				IdFormat.EntryId,
				new EntryIdConverter()
			},
			{
				IdFormat.EwsLegacyId,
				new EwsLegacyIdConverter()
			},
			{
				IdFormat.EwsId,
				new EwsIdConverter()
			},
			{
				IdFormat.HexEntryId,
				new HexEntryIdConverter()
			},
			{
				IdFormat.StoreId,
				new StoreIdConverter()
			},
			{
				IdFormat.OwaId,
				new OwaIdConverter()
			}
		});

		private IdFormat format;
	}
}
