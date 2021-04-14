using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class SyncContactSchema
	{
		public static PropertyTag WorkAddressCity
		{
			get
			{
				return SyncContactSchema.workAddressCity;
			}
		}

		public static PropertyTag LegacyWebPage
		{
			get
			{
				return SyncContactSchema.legacyWebPage;
			}
		}

		public static PropertyTag WorkAddressCountry
		{
			get
			{
				return SyncContactSchema.workAddressCountry;
			}
		}

		public static PropertyTag Email1EmailAddress
		{
			get
			{
				return SyncContactSchema.email1EmailAddress;
			}
		}

		public static PropertyTag Email2EmailAddress
		{
			get
			{
				return SyncContactSchema.email2EmailAddress;
			}
		}

		public static PropertyTag Email3EmailAddress
		{
			get
			{
				return SyncContactSchema.email3EmailAddress;
			}
		}

		public static PropertyTag FileAsStringInternal
		{
			get
			{
				return SyncContactSchema.fileAsStringInternal;
			}
		}

		public static PropertyTag WorkAddressPostalCode
		{
			get
			{
				return SyncContactSchema.workAddressPostalCode;
			}
		}

		public static PropertyTag WorkAddressState
		{
			get
			{
				return SyncContactSchema.workAddressState;
			}
		}

		public static PropertyTag WorkAddressStreet
		{
			get
			{
				return SyncContactSchema.workAddressStreet;
			}
		}

		public static PropertyTag YomiFirstName
		{
			get
			{
				return SyncContactSchema.yomiFirstName;
			}
		}

		public static PropertyTag YomiLastName
		{
			get
			{
				return SyncContactSchema.yomiLastName;
			}
		}

		public static PropertyTag YomiCompany
		{
			get
			{
				return SyncContactSchema.yomiCompany;
			}
		}

		public static PropertyTag DisplayNameFirstLast
		{
			get
			{
				return SyncContactSchema.displayNameFirstLast;
			}
		}

		public static PropertyTag DisplayNameLastFirst
		{
			get
			{
				return SyncContactSchema.displayNameLastFirst;
			}
		}

		internal const string PartnerNetworkIDOutlook = "outlook.com";

		private static readonly PropertyTag workAddressCity = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32768, PropType.String));

		private static readonly PropertyTag legacyWebPage = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32769, PropType.String));

		private static readonly PropertyTag workAddressCountry = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32770, PropType.String));

		private static readonly PropertyTag email1EmailAddress = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32771, PropType.String));

		private static readonly PropertyTag email2EmailAddress = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32772, PropType.String));

		private static readonly PropertyTag email3EmailAddress = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32773, PropType.String));

		private static readonly PropertyTag fileAsStringInternal = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32774, PropType.String));

		private static readonly PropertyTag workAddressPostalCode = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32775, PropType.String));

		private static readonly PropertyTag workAddressState = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32776, PropType.String));

		private static readonly PropertyTag workAddressStreet = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32777, PropType.String));

		private static readonly PropertyTag yomiFirstName = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32778, PropType.String));

		private static readonly PropertyTag yomiLastName = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32779, PropType.String));

		private static readonly PropertyTag yomiCompany = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32780, PropType.String));

		private static readonly PropertyTag displayNameFirstLast = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32781, PropType.String));

		private static readonly PropertyTag displayNameLastFirst = new PropertyTag((uint)PropTagHelper.PropTagFromIdAndType(32782, PropType.String));

		public static readonly Dictionary<PropertyTag, NamedProperty> PropertyTagToNamedProperties = new Dictionary<PropertyTag, NamedProperty>
		{
			{
				SyncContactSchema.WorkAddressCity,
				new NamedProperty(WellKnownPropertySet.Address, 32838U)
			},
			{
				SyncContactSchema.LegacyWebPage,
				new NamedProperty(WellKnownPropertySet.Address, 32811U)
			},
			{
				SyncContactSchema.WorkAddressCountry,
				new NamedProperty(WellKnownPropertySet.Address, 32841U)
			},
			{
				SyncContactSchema.Email1EmailAddress,
				new NamedProperty(WellKnownPropertySet.Address, 32899U)
			},
			{
				SyncContactSchema.Email2EmailAddress,
				new NamedProperty(WellKnownPropertySet.Address, 32915U)
			},
			{
				SyncContactSchema.Email3EmailAddress,
				new NamedProperty(WellKnownPropertySet.Address, 32931U)
			},
			{
				SyncContactSchema.FileAsStringInternal,
				new NamedProperty(WellKnownPropertySet.Address, 32773U)
			},
			{
				SyncContactSchema.WorkAddressPostalCode,
				new NamedProperty(WellKnownPropertySet.Address, 32840U)
			},
			{
				SyncContactSchema.WorkAddressState,
				new NamedProperty(WellKnownPropertySet.Address, 32839U)
			},
			{
				SyncContactSchema.WorkAddressStreet,
				new NamedProperty(WellKnownPropertySet.Address, 32837U)
			},
			{
				SyncContactSchema.YomiFirstName,
				new NamedProperty(WellKnownPropertySet.Address, 32812U)
			},
			{
				SyncContactSchema.YomiLastName,
				new NamedProperty(WellKnownPropertySet.Address, 32813U)
			},
			{
				SyncContactSchema.YomiCompany,
				new NamedProperty(WellKnownPropertySet.Address, 32814U)
			},
			{
				SyncContactSchema.DisplayNameFirstLast,
				new NamedProperty(WellKnownPropertySet.Address, "DisplayNameFirstLast")
			},
			{
				SyncContactSchema.DisplayNameLastFirst,
				new NamedProperty(WellKnownPropertySet.Address, "DisplayNameLastFirst")
			}
		};

		public static readonly List<PropertyTag> AllContactPropertyTags = new List<PropertyTag>
		{
			new PropertyTag(980811807U),
			new PropertyTag(977338432U),
			new PropertyTag(976224287U),
			new PropertyTag(976093215U),
			new PropertyTag(977403968U),
			new PropertyTag(974848031U),
			SyncContactSchema.WorkAddressCity,
			new PropertyTag(973602847U),
			SyncContactSchema.LegacyWebPage,
			SyncContactSchema.WorkAddressCountry,
			new PropertyTag(974651423U),
			SyncContactSchema.Email1EmailAddress,
			SyncContactSchema.Email2EmailAddress,
			SyncContactSchema.Email3EmailAddress,
			new PropertyTag(975437855U),
			SyncContactSchema.FileAsStringInternal,
			new PropertyTag(236781599U),
			new PropertyTag(973471775U),
			new PropertyTag(973078559U),
			new PropertyTag(977535007U),
			new PropertyTag(978911263U),
			new PropertyTag(978976799U),
			new PropertyTag(975503391U),
			new PropertyTag(973668383U),
			new PropertyTag(976158751U),
			new PropertyTag(979042335U),
			new PropertyTag(979107871U),
			new PropertyTag(979173407U),
			new PropertyTag(974913567U),
			new PropertyTag(974520351U),
			new PropertyTag(979304479U),
			new PropertyTag(979370015U),
			new PropertyTag(975044639U),
			new PropertyTag(979435551U),
			new PropertyTag(979501087U),
			new PropertyTag(979566623U),
			new PropertyTag(975241247U),
			SyncContactSchema.WorkAddressPostalCode,
			new PropertyTag(974192671U),
			new PropertyTag(977797151U),
			SyncContactSchema.WorkAddressState,
			SyncContactSchema.WorkAddressStreet,
			new PropertyTag(974585887U),
			SyncContactSchema.YomiFirstName,
			SyncContactSchema.YomiLastName,
			SyncContactSchema.YomiCompany,
			new PropertyTag(974716959U),
			new PropertyTag(974979103U),
			new PropertyTag(980942879U),
			SyncContactSchema.DisplayNameFirstLast,
			SyncContactSchema.DisplayNameLastFirst
		};
	}
}
