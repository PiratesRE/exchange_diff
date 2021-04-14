using System;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	internal class MapiSubmitSystemProbeHandler
	{
		private MapiSubmitSystemProbeFactory MonitoringFactory
		{
			get
			{
				if (this.monitoringFactory == null)
				{
					this.monitoringFactory = MapiSubmitSystemProbeFactory.CreateFactory(MapiSubmitSystemProbeFactory.Type.MonitoringSystemProbe);
				}
				return this.monitoringFactory;
			}
		}

		private IMapiMessageSubmitter MapiMessageSubmitter
		{
			get
			{
				if (this.mapiMessageSubmitter == null)
				{
					this.mapiMessageSubmitter = this.MonitoringFactory.MakeMapiMessageSubmitter();
				}
				return this.mapiMessageSubmitter;
			}
		}

		private MapiSubmitSystemProbeHandler()
		{
		}

		public static MapiSubmitSystemProbeHandler GetInstance()
		{
			return MapiSubmitSystemProbeHandler.instance;
		}

		public Guid SendMapiSubmitSystemProbe(string senderEmailAddress, string recipientEmailAddress, out string internetMessageId)
		{
			SendMapiMailDefinition mapiMailDefinition = this.MonitoringFactory.MakeSendMapiMailDefinition(senderEmailAddress, recipientEmailAddress);
			internetMessageId = null;
			string entryId;
			Guid mbxGuid;
			this.MapiMessageSubmitter.SendMapiMessage(mapiMailDefinition, out entryId, out internetMessageId, out mbxGuid);
			return MapiSubmitSystemProbeHandler.ComputeSystemProbeId(entryId, mbxGuid);
		}

		public DeletionResult DeleteMessageFromOutbox(string senderEmailAddress, string internetMessageId)
		{
			DeleteMapiMailDefinition deleteMapiMailDefinition = this.MonitoringFactory.MakeDeleteMapiMailDefinition(senderEmailAddress, internetMessageId);
			return this.MapiMessageSubmitter.DeleteMessageFromOutbox(deleteMapiMailDefinition);
		}

		private static Guid ComputeSystemProbeId(string entryId, Guid mbxGuid)
		{
			if (string.IsNullOrEmpty(entryId))
			{
				throw new ArgumentNullException("entryId");
			}
			if (Guid.Empty == mbxGuid)
			{
				throw new ArgumentNullException("mbxGuid");
			}
			string arg = string.Format("{0:X8}", entryId.GetHashCode());
			string text = mbxGuid.ToString();
			int startIndex = text.IndexOf('-');
			return new Guid(string.Format("{0}{1}", arg, text.Substring(startIndex)));
		}

		private static MapiSubmitSystemProbeHandler instance = new MapiSubmitSystemProbeHandler();

		private MapiSubmitSystemProbeFactory monitoringFactory;

		private IMapiMessageSubmitter mapiMessageSubmitter;
	}
}
