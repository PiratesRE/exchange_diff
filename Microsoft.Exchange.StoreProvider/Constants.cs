using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Constants
	{
		internal const int Unicode = -2147483648;

		internal const int Associated = 64;

		internal const int Create = 2;

		internal const int ShowSoftDeletes = 2;

		internal const int NoNotifications = 32;

		internal const int ShowConversations = 256;

		internal const int ShowConversationMembers = 512;

		internal const int RetrieveFromIndex = 1024;

		internal const int DocumentIdView = 2048;

		internal const int ExpandedConversationView = 8192;

		internal const int PrereadExtendedProperties = 16384;

		internal const int Modify = 1;

		internal const int DeferredErrors = 8;

		internal const int ReadOnly = 16;

		internal const int BestAccess = 16;

		internal const int ForceUnicode = 1024;

		internal const int BestBody = 8192;

		internal const int ContentAggregation = 1;

		internal const int StandardChainedRpcBufferSize = 98304;

		internal const int MaximumChainedRpcBufferSize = 262144;
	}
}
