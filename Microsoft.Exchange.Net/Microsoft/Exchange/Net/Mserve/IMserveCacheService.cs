using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Microsoft.Exchange.Net.Mserve
{
	[ServiceContract(ConfigurationName = "Microsoft.Exchange.Net.Mserve.IMserveCacheService")]
	public interface IMserveCacheService
	{
		[Description("Get partner Id/minor partner Id from tenant name/domain name")]
		[WebGet]
		[OperationContract]
		string ReadMserveData(string requestName);

		[WebGet]
		[OperationContract]
		[Description("chunk size")]
		int GetChunkSize();
	}
}
