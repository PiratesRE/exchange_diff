using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabManifestType
	{
		public SlabManifestType(string manifestName)
		{
			this.manifestName = manifestName;
		}

		public string Name
		{
			get
			{
				return this.manifestName;
			}
		}

		public static SlabManifestType Standard
		{
			get
			{
				if (SlabManifestType.standard == null)
				{
					SlabManifestType.standard = new SlabManifestType("slabmanifest.standard");
				}
				return SlabManifestType.standard;
			}
		}

		public static SlabManifestType Pal
		{
			get
			{
				if (SlabManifestType.pal == null)
				{
					SlabManifestType.pal = new SlabManifestType("slabmanifest.pal");
				}
				return SlabManifestType.pal;
			}
		}

		public static SlabManifestType Anonymous
		{
			get
			{
				if (SlabManifestType.anonymous == null)
				{
					SlabManifestType.anonymous = new SlabManifestType("slabmanifest.anonymous");
				}
				return SlabManifestType.anonymous;
			}
		}

		public static SlabManifestType PreBoot
		{
			get
			{
				if (SlabManifestType.preBoot == null)
				{
					SlabManifestType.preBoot = new SlabManifestType("slabmanifest.preboot");
				}
				return SlabManifestType.preBoot;
			}
		}

		public static SlabManifestType GenericMail
		{
			get
			{
				if (SlabManifestType.genericMail == null)
				{
					SlabManifestType.genericMail = new SlabManifestType("slabmanifest.genericmail");
				}
				return SlabManifestType.genericMail;
			}
		}

		public static SlabManifestType SharedHoverCard
		{
			get
			{
				if (SlabManifestType.sharedHoverCard == null)
				{
					SlabManifestType.sharedHoverCard = new SlabManifestType("slabmanifest.standard");
				}
				return SlabManifestType.sharedHoverCard;
			}
		}

		private static SlabManifestType standard;

		private static SlabManifestType pal;

		private static SlabManifestType anonymous;

		private static SlabManifestType preBoot;

		private static SlabManifestType genericMail;

		private static SlabManifestType sharedHoverCard;

		private readonly string manifestName;
	}
}
