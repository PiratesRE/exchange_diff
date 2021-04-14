using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class MesoContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return MesoContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return MesoContainer.MostDerivedClass;
			}
		}

		public int ObjectVersion
		{
			get
			{
				return (int)this.propertyBag[MesoContainerSchema.ObjectVersion];
			}
			internal set
			{
				this.propertyBag[MesoContainerSchema.ObjectVersion] = value;
			}
		}

		public static int DomainPrepVersion
		{
			get
			{
				return MesoContainerSchema.DomainPrepVersion;
			}
		}

		internal const string MESOContainerName = "Microsoft Exchange System Objects";

		private static MesoContainerSchema schema = ObjectSchema.GetInstance<MesoContainerSchema>();

		internal static string MostDerivedClass = "msExchSystemObjectsContainer";
	}
}
