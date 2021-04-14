using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	[ServiceContract(Namespace = "http://Microsoft.Exchange.Data.Directory.DirectoryCache", ConfigurationName = "Microsoft.Exchange.Data.Directory.Cache.IDirectoryCacheService")]
	internal interface IDirectoryCacheService
	{
		[OperationContract(Name = "GetObject", AsyncPattern = true, Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/GetObject", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/GetObjectResponse")]
		IAsyncResult BeginGetObject(DirectoryCacheRequest cacheRequest, AsyncCallback callback, object asyncState);

		GetObjectContext EndGetObject(IAsyncResult result);

		[OperationContract(Name = "PutObject", AsyncPattern = true, Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/PutObject", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/PutObjectResponse")]
		IAsyncResult BeginPutObject(AddDirectoryCacheRequest cacheRequest, AsyncCallback callback, object asyncState);

		CacheResponseContext EndPutObject(IAsyncResult result);

		[OperationContract(Name = "RemoveObject", AsyncPattern = true, Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/RemoveObject", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/RemoveObjectResponse")]
		IAsyncResult BeginRemoveObject(RemoveDirectoryCacheRequest cacheRequest, AsyncCallback callback, object asyncState);

		CacheResponseContext EndRemoveObject(IAsyncResult result);

		[OperationContract(Name = "Diagnostic", AsyncPattern = true, Action = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/Diagnostic", ReplyAction = "http://Microsoft.Exchange.Data.Directory.DirectoryCache/IDirectoryCacheService/DiagnosticResponse")]
		IAsyncResult BeginDiagnostic(DiagnosticsCacheRequest cacheRequest, AsyncCallback callback, object asyncState);

		void EndDiagnostic(IAsyncResult result);
	}
}
