using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMCallDataRecord")]
	public interface IUMCallDataRecordService : IGetListService<UMCallDataRecordFilter, UMCallDataRecordRow>
	{
	}
}
