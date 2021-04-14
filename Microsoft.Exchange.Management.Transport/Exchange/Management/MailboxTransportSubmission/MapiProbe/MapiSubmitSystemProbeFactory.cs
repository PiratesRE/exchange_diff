using System;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	internal abstract class MapiSubmitSystemProbeFactory
	{
		public static MapiSubmitSystemProbeFactory CreateFactory(MapiSubmitSystemProbeFactory.Type type)
		{
			if (type == MapiSubmitSystemProbeFactory.Type.MonitoringSystemProbe)
			{
				return MapiSubmitMonitoringSystemProbeFactory.CreateInstance();
			}
			throw new ArgumentException(string.Format("type {0} is not a valid enum MapiSubmitSystemProbeFactory.Type", type));
		}

		public virtual SendMapiMailDefinition MakeSendMapiMailDefinition(string senderEmailAddress, string recipientEmailAddress)
		{
			if (string.IsNullOrEmpty(senderEmailAddress))
			{
				throw new ArgumentNullException("senderEmailAddress");
			}
			if (string.IsNullOrEmpty(recipientEmailAddress))
			{
				recipientEmailAddress = senderEmailAddress;
			}
			return SendMapiMailDefinition.CreateInstance(this.Subject, "This is a Mapi System Probe message that's Submitted to Store so that Mailbox transport Submission service can process it", "IPM.Note.MapiSubmitSystemProbe", true, true, senderEmailAddress, recipientEmailAddress);
		}

		public virtual DeleteMapiMailDefinition MakeDeleteMapiMailDefinition(string senderEmailAddress, string internetMessageId)
		{
			if (string.IsNullOrEmpty(senderEmailAddress))
			{
				throw new ArgumentException("senderEmailAddress is null or empty");
			}
			return DeleteMapiMailDefinition.CreateInstance("IPM.Note.MapiSubmitSystemProbe", senderEmailAddress, internetMessageId);
		}

		public abstract IMapiMessageSubmitter MakeMapiMessageSubmitter();

		private const string Body = "This is a Mapi System Probe message that's Submitted to Store so that Mailbox transport Submission service can process it";

		private const string MessageClass = "IPM.Note.MapiSubmitSystemProbe";

		private const bool DoNotDeliver = true;

		private const bool DeleteAfterSubmit = true;

		private readonly string Subject = string.Format("MapiSubmitSystemProbe_{0}", Guid.NewGuid());

		public enum Type
		{
			MonitoringSystemProbe
		}
	}
}
