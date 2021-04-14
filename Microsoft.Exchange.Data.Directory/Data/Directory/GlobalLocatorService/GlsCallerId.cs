using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class GlsCallerId
	{
		internal static GlsEnvironmentType GLSEnvironment
		{
			get
			{
				if (GlsCallerId.glsEnvironmentType == GlsEnvironmentType.NotDefined)
				{
					if (DatacenterRegistry.IsForefrontForOffice())
					{
						string forefrontRegion = DatacenterRegistry.GetForefrontRegion();
						if (forefrontRegion.StartsWith("CN", true, CultureInfo.InvariantCulture))
						{
							GlsCallerId.glsEnvironmentType = GlsEnvironmentType.Gallatin;
						}
						else
						{
							GlsCallerId.glsEnvironmentType = GlsEnvironmentType.Prod;
							if (forefrontRegion.StartsWith("EMEASIP", true, CultureInfo.InvariantCulture))
							{
								GlsCallerId.glsEnvironmentType = GlsEnvironmentType.FFO_PROD_EMEASIP;
							}
						}
					}
					else
					{
						GlsCallerId.glsEnvironmentType = RegistrySettings.ExchangeServerCurrentVersion.GlsEnvironmentType;
					}
				}
				return GlsCallerId.glsEnvironmentType;
			}
		}

		public static GlsCallerId ForwardSyncService
		{
			get
			{
				switch (GlsCallerId.GLSEnvironment)
				{
				case GlsEnvironmentType.Prod:
					return GlsCallerId.forwardSync_Prod;
				case GlsEnvironmentType.Gallatin:
					return GlsCallerId.forwardSync_CN;
				case GlsEnvironmentType.FFO_PROD_EMEASIP:
					return GlsCallerId.forwardSync_EMEASIP;
				default:
					throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
				}
			}
		}

		public static GlsCallerId Transport
		{
			get
			{
				switch (GlsCallerId.GLSEnvironment)
				{
				case GlsEnvironmentType.Prod:
				case GlsEnvironmentType.FFO_PROD_EMEASIP:
					return GlsCallerId.transport_Prod;
				case GlsEnvironmentType.Gallatin:
					return GlsCallerId.transport_CN;
				default:
					throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
				}
			}
		}

		public static GlsCallerId Exchange
		{
			get
			{
				switch (GlsCallerId.GLSEnvironment)
				{
				case GlsEnvironmentType.Prod:
				case GlsEnvironmentType.FFO_PROD_EMEASIP:
					return GlsCallerId.exchange_Prod;
				case GlsEnvironmentType.Gallatin:
					return GlsCallerId.exchange_CN;
				default:
					throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
				}
			}
		}

		public static GlsCallerId FFOAdminUI
		{
			get
			{
				switch (GlsCallerId.GLSEnvironment)
				{
				case GlsEnvironmentType.Prod:
				case GlsEnvironmentType.FFO_PROD_EMEASIP:
					return GlsCallerId.ffoAdminUI_Prod;
				case GlsEnvironmentType.Gallatin:
					return GlsCallerId.ffoAdminUI_CN;
				default:
					throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
				}
			}
		}

		public static GlsCallerId MessageTrace
		{
			get
			{
				switch (GlsCallerId.GLSEnvironment)
				{
				case GlsEnvironmentType.Prod:
				case GlsEnvironmentType.FFO_PROD_EMEASIP:
					return GlsCallerId.messageTrace_Prod;
				case GlsEnvironmentType.Gallatin:
					return GlsCallerId.messageTrace_CN;
				default:
					throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
				}
			}
		}

		public static GlsCallerId FFOVersioning
		{
			get
			{
				switch (GlsCallerId.GLSEnvironment)
				{
				case GlsEnvironmentType.Prod:
					return GlsCallerId.ffoVersioning_Prod;
				case GlsEnvironmentType.Gallatin:
					return GlsCallerId.ffoVersioning_CN;
				case GlsEnvironmentType.FFO_PROD_EMEASIP:
					return GlsCallerId.ffoVersioning_EMEASIP;
				default:
					throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
				}
			}
		}

		public static GlsCallerId O365CE
		{
			get
			{
				GlsEnvironmentType glsenvironment = GlsCallerId.GLSEnvironment;
				if (glsenvironment == GlsEnvironmentType.Prod)
				{
					return GlsCallerId.o365ce;
				}
				throw new ArgumentException("GLS callerID not initialized because of invalid value in GlsCallerId.GLSEnvironment");
			}
		}

		public static GlsCallerId TestOnly
		{
			get
			{
				return GlsCallerId.testOnly;
			}
		}

		public string CallerIdString
		{
			get
			{
				return this.callerIdString;
			}
		}

		public Guid TrackingGuid
		{
			get
			{
				return this.trackingGuid;
			}
		}

		public static bool IsForwardSyncCallerID(GlsCallerId callerID)
		{
			return callerID == GlsCallerId.forwardSync_Prod || callerID == GlsCallerId.forwardSync_EMEASIP || callerID == GlsCallerId.forwardSync_CN;
		}

		private GlsCallerId(string callerIdString, Guid trackingGuid, Namespace defaultNamespace)
		{
			this.callerIdString = callerIdString;
			this.trackingGuid = trackingGuid;
			this.defaultNamespace = defaultNamespace;
		}

		public Namespace DefaultNamespace
		{
			get
			{
				return this.defaultNamespace;
			}
		}

		private const string FFOEMEASIP = "EMEASIP";

		private static GlsEnvironmentType glsEnvironmentType = GlsEnvironmentType.NotDefined;

		private static readonly GlsCallerId forwardSync_Prod = new GlsCallerId("ffoFSS1", new Guid("54b2df1c-9cc0-4933-99e7-5f6ab597bc86"), Namespace.Ffo);

		private static readonly GlsCallerId forwardSync_CN = new GlsCallerId("ffoFSS1-CN", new Guid("54b2df1c-9cc0-4933-99e7-5f6ab597bc86"), Namespace.Ffo);

		private static readonly GlsCallerId forwardSync_EMEASIP = new GlsCallerId("ffoFSS1-EMEASIP", new Guid("54b2df1c-9cc0-4933-99e7-5f6ab597bc86"), Namespace.Ffo);

		private static readonly GlsCallerId ffoVersioning_Prod = new GlsCallerId("FFOVersioning", new Guid("ff8c4923-2a5f-49ed-b586-3a0960379326"), Namespace.Ffo);

		private static readonly GlsCallerId ffoVersioning_CN = new GlsCallerId("FFOVersioning-CN", new Guid("ff8c4923-2a5f-49ed-b586-3a0960379326"), Namespace.Ffo);

		private static readonly GlsCallerId ffoVersioning_EMEASIP = new GlsCallerId("FFOVersioning-EMEASIP", new Guid("ff8c4923-2a5f-49ed-b586-3a0960379326"), Namespace.Ffo);

		private static readonly GlsCallerId transport_Prod = new GlsCallerId("Transport", new Guid("1b1c89c1-44c1-447a-b7dc-ad48a8b8e495"), Namespace.Ffo);

		private static readonly GlsCallerId transport_CN = new GlsCallerId("Transport-CN", new Guid("1b1c89c1-44c1-447a-b7dc-ad48a8b8e495"), Namespace.Ffo);

		private static readonly GlsCallerId exchange_Prod = new GlsCallerId("EXO", new Guid("423b8095-fe9f-4193-89e9-d08341d951ef"), Namespace.Exo);

		private static readonly GlsCallerId exchange_CN = new GlsCallerId("EXO-CN", new Guid("423b8095-fe9f-4193-89e9-d08341d951ef"), Namespace.Exo);

		private static readonly GlsCallerId messageTrace_Prod = new GlsCallerId("MTRT", new Guid("4EDA220A-88C2-42f4-ABD8-5CB6F1A826D8"), Namespace.Ffo);

		private static readonly GlsCallerId messageTrace_CN = new GlsCallerId("MTRT-CN", new Guid("4EDA220A-88C2-42f4-ABD8-5CB6F1A826D8"), Namespace.Ffo);

		private static readonly GlsCallerId ffoAdminUI_Prod = new GlsCallerId("FFOAdminUI", new Guid("72f9956a-cb04-4a16-a149-0f382eb044e4"), Namespace.Ffo);

		private static readonly GlsCallerId ffoAdminUI_CN = new GlsCallerId("FFOAdminUI-CN", new Guid("72f9956a-cb04-4a16-a149-0f382eb044e4"), Namespace.Ffo);

		private static readonly GlsCallerId testOnly = new GlsCallerId("Lin Caller", new Guid("4d5b5b22-200e-4582-a4fe-3e44f5276b82"), Namespace.Ffo);

		private static readonly GlsCallerId o365ce = new GlsCallerId("O365CE", new Guid("fa662ad4-faa2-4f7e-b3ab-2802549b88b8"), Namespace.Ffo);

		private readonly string callerIdString;

		private readonly Namespace defaultNamespace;

		private readonly Guid trackingGuid;
	}
}
