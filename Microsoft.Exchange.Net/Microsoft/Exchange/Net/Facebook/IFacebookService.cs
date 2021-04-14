using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.Facebook
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[ServiceContract]
	internal interface IFacebookService
	{
		[OperationContract(AsyncPattern = true)]
		[WebGet(UriTemplate = "/me/friends?access_token={accessToken}&fields={fields}&limit={limit}&offset={offset}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetFriends(string accessToken, string fields, string limit, string offset, AsyncCallback callback, object state);

		FacebookUsersList EndGetFriends(IAsyncResult ar);

		[OperationContract(AsyncPattern = true)]
		[GetUsersOperationBehavior]
		[WebGet(UriTemplate = "?access_token={accessToken}&ids={userIds}&fields={fields}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		IAsyncResult BeginGetUsers(string accessToken, string userIds, string fields, AsyncCallback callback, object state);

		[WebGet(UriTemplate = "/me?access_token={accessToken}&fields={fields}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		FacebookUser GetProfile(string accessToken, string fields);

		FacebookUsersList EndGetUsers(IAsyncResult ar);

		[WebInvoke(Method = "DELETE", UriTemplate = "/me/permissions?access_token={accessToken}")]
		[OperationContract]
		void RemoveApplication(string accessToken);

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "/me/importcontacts?access_token={accessToken}&format={format}&encoding={encoding}&continuous={continuous}&async={async}&source={source}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
		FacebookImportContactsResult ImportContacts(string accessToken, string format, string encoding, bool continuous, bool async, string source, Stream requestBody);
	}
}
