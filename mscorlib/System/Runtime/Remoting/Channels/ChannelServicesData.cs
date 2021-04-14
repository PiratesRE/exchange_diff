using System;

namespace System.Runtime.Remoting.Channels
{
	internal class ChannelServicesData
	{
		internal long remoteCalls;

		internal CrossContextChannel xctxmessageSink;

		internal CrossAppDomainChannel xadmessageSink;

		internal bool fRegisterWellKnownChannels;
	}
}
