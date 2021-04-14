using System;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	public enum InterceptorAgentConditionType
	{
		Invalid,
		MessageSubject,
		EnvelopeFrom,
		EnvelopeTo,
		MessageId,
		HeaderName,
		HeaderValue,
		SmtpClientHostName,
		ProcessRole,
		ServerVersion,
		TenantId,
		Directionality,
		AccountForest
	}
}
