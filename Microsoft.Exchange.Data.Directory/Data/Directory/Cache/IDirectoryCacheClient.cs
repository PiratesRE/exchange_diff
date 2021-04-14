using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[ServiceContract(Namespace = "http://Microsoft.Exchange.Data.Directory.DirectoryCache", ConfigurationName = "Microsoft.Exchange.Data.Directory.Cache.IDirectoryCacheService")]
	internal interface IDirectoryCacheClient : IDirectoryCacheService
	{
		[OperationContract(Name = "GetObject", Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/GetObject", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/GetObjectResponse")]
		GetObjectContext GetObject(DirectoryCacheRequest cacheRequest);

		[OperationContract(Name = "PutObject", Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/PutObject", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/PutObjectResponse")]
		CacheResponseContext PutObject(AddDirectoryCacheRequest cacheRequest);

		[OperationContract(Name = "RemoveObject", Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/RemoveObject", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/RemoveObjectResponse")]
		CacheResponseContext RemoveObject(RemoveDirectoryCacheRequest cacheRequest);

		[OperationContract(Name = "Diagnostic", Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/Diagnostic", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/DiagnosticResponse")]
		void Diagnostic(DiagnosticsCacheRequest cacheRequest);
	}
}
