using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.InfoWorker.Common.Availability.Proxy;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class DummyApplication : Application
	{
		public static FreeBusyQuery[] ConvertBaseToFreeBusyQuery(BaseQuery[] baseQueries)
		{
			throw new NotImplementedException();
		}

		public override ThreadCounter Worker
		{
			get
			{
				return DummyApplication.WorkerThreadCounter;
			}
		}

		public override ThreadCounter IOCompletion
		{
			get
			{
				return DummyApplication.IOCompletionThreadCounter;
			}
		}

		public override int MinimumRequiredVersion
		{
			get
			{
				return 0;
			}
		}

		public override LocalizedString Name
		{
			get
			{
				return Strings.DummyApplicationName;
			}
		}

		private DummyApplication() : base(false)
		{
		}

		public override IService CreateService(WebServiceUri webServiceUri, TargetServerVersion targetVersion, RequestType requestType)
		{
			throw new NotImplementedException();
		}

		public override IAsyncResult BeginProxyWebRequest(IService service, MailboxData[] mailboxArray, AsyncCallback callback, object asyncState)
		{
			throw new NotImplementedException();
		}

		public override void EndProxyWebRequest(ProxyWebRequest proxyWebRequest, QueryList queryList, IService service, IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public override string GetParameterDataString()
		{
			throw new NotImplementedException();
		}

		public override LocalQuery CreateLocalQuery(ClientContext clientContext, DateTime requestCompletionDeadline)
		{
			throw new NotImplementedException();
		}

		public override BaseQueryResult CreateQueryResult(LocalizedException exception)
		{
			throw new NotImplementedException();
		}

		public override BaseQuery CreateFromUnknown(RecipientData recipientData, LocalizedException exception)
		{
			throw new NotImplementedException();
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData)
		{
			throw new NotImplementedException();
		}

		public override BaseQuery CreateFromIndividual(RecipientData recipientData, LocalizedException exception)
		{
			throw new NotImplementedException();
		}

		public override BaseQuery CreateFromGroup(RecipientData recipientData, BaseQuery[] groupMembers, bool groupCapped)
		{
			throw new NotImplementedException();
		}

		public override Offer OfferForExternalSharing
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override bool EnabledInRelationship(OrganizationRelationship organizationRelationship)
		{
			throw new NotImplementedException();
		}

		public override AvailabilityException CreateExceptionForUnsupportedVersion(RecipientData recipient, int serverVersion)
		{
			throw new NotImplementedException();
		}

		private static readonly ThreadCounter WorkerThreadCounter = new ThreadCounter();

		private static readonly ThreadCounter IOCompletionThreadCounter = new ThreadCounter();

		public static readonly DummyApplication Instance = new DummyApplication();
	}
}
