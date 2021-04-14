using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "MailboxSearches")]
	public interface IMailboxSearches : IDataSourceService<MailboxSearchFilter, MailboxSearchRow, MailboxSearch, SetMailboxSearchParameters, NewMailboxSearchParameters>, IDataSourceService<MailboxSearchFilter, MailboxSearchRow, MailboxSearch, SetMailboxSearchParameters, NewMailboxSearchParameters, BaseWebServiceParameters>, IEditListService<MailboxSearchFilter, MailboxSearchRow, MailboxSearch, NewMailboxSearchParameters, BaseWebServiceParameters>, IGetListService<MailboxSearchFilter, MailboxSearchRow>, INewObjectService<MailboxSearchRow, NewMailboxSearchParameters>, IRemoveObjectsService<BaseWebServiceParameters>, IEditObjectForListService<MailboxSearch, SetMailboxSearchParameters, MailboxSearchRow>, IGetObjectService<MailboxSearch>, IGetObjectForListService<MailboxSearchRow>
	{
		[OperationContract]
		PowerShellResults<MailboxSearchRow> StartSearch(Identity[] identities, StartMailboxSearchParameters parameters);

		[OperationContract]
		PowerShellResults<MailboxSearchRow> StopSearch(Identity[] identities, BaseWebServiceParameters parameters);
	}
}
