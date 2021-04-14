using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.RightsManagement;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	internal delegate object OnProcessDecryption(DecryptionStatus status, TransportDecryptionSetting settings, AgentAsyncState state, Exception exception);
}
