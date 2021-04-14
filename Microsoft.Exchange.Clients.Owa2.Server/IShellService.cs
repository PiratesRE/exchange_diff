using System;
using System.CodeDom.Compiler;
using System.ServiceModel;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;
using Microsoft.Online.BOX.UI.Shell.AllSettings;

[ServiceContract(ConfigurationName = "IShellService")]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public interface IShellService
{
	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/GetMetaDataShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[OperationContract(Action = "http://tempuri.org/IShellService/GetMetaData", ReplyAction = "http://tempuri.org/IShellService/GetMetaDataResponse")]
	string GetMetaData(BrandInfo brandInfo, string locale, UserInfo userInfo, Options options);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/GetMetaData", ReplyAction = "http://tempuri.org/IShellService/GetMetaDataResponse")]
	IAsyncResult BeginGetMetaData(BrandInfo brandInfo, string locale, UserInfo userInfo, Options options, AsyncCallback callback, object asyncState);

	string EndGetMetaData(IAsyncResult result);

	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/GetNavBarInfoShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[OperationContract(Action = "http://tempuri.org/IShellService/GetNavBarInfo", ReplyAction = "http://tempuri.org/IShellService/GetNavBarInfoResponse")]
	NavBarInfo GetNavBarInfo(NavBarInfoRequest navBarInfoRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/GetNavBarInfo", ReplyAction = "http://tempuri.org/IShellService/GetNavBarInfoResponse")]
	IAsyncResult BeginGetNavBarInfo(NavBarInfoRequest navBarInfoRequest, AsyncCallback callback, object asyncState);

	NavBarInfo EndGetNavBarInfo(IAsyncResult result);

	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/SetYammerEnabledShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[OperationContract(Action = "http://tempuri.org/IShellService/SetYammerEnabled", ReplyAction = "http://tempuri.org/IShellService/SetYammerEnabledResponse")]
	void SetYammerEnabled(SetYammerEnabledRequest setYammerEnabledRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/SetYammerEnabled", ReplyAction = "http://tempuri.org/IShellService/SetYammerEnabledResponse")]
	IAsyncResult BeginSetYammerEnabled(SetYammerEnabledRequest setYammerEnabledRequest, AsyncCallback callback, object asyncState);

	void EndSetYammerEnabled(IAsyncResult result);

	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/GetShellInfoShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[OperationContract(Action = "http://tempuri.org/IShellService/GetShellInfo", ReplyAction = "http://tempuri.org/IShellService/GetShellInfoResponse")]
	ShellInfo GetShellInfo(ShellInfoRequest shellInfoRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/GetShellInfo", ReplyAction = "http://tempuri.org/IShellService/GetShellInfoResponse")]
	IAsyncResult BeginGetShellInfo(ShellInfoRequest shellInfoRequest, AsyncCallback callback, object asyncState);

	ShellInfo EndGetShellInfo(IAsyncResult result);

	[OperationContract(Action = "http://tempuri.org/IShellService/SetUserTheme", ReplyAction = "http://tempuri.org/IShellService/SetUserThemeResponse")]
	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/SetUserThemeShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	void SetUserTheme(SetUserThemeRequest setUserThemeRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/SetUserTheme", ReplyAction = "http://tempuri.org/IShellService/SetUserThemeResponse")]
	IAsyncResult BeginSetUserTheme(SetUserThemeRequest setUserThemeRequest, AsyncCallback callback, object asyncState);

	void EndSetUserTheme(IAsyncResult result);

	[OperationContract(Action = "http://tempuri.org/IShellService/DoNothing", ReplyAction = "http://tempuri.org/IShellService/DoNothingResponse")]
	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/DoNothingShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	void DoNothing(NavBarData ignored0);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/DoNothing", ReplyAction = "http://tempuri.org/IShellService/DoNothingResponse")]
	IAsyncResult BeginDoNothing(NavBarData ignored0, AsyncCallback callback, object asyncState);

	void EndDoNothing(IAsyncResult result);

	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/GetAlertsShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	[OperationContract(Action = "http://tempuri.org/IShellService/GetAlerts", ReplyAction = "http://tempuri.org/IShellService/GetAlertsResponse")]
	Alert[] GetAlerts(GetAlertRequest getAlertRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/GetAlerts", ReplyAction = "http://tempuri.org/IShellService/GetAlertsResponse")]
	IAsyncResult BeginGetAlerts(GetAlertRequest getAlertRequest, AsyncCallback callback, object asyncState);

	Alert[] EndGetAlerts(IAsyncResult result);

	[OperationContract(Action = "http://tempuri.org/IShellService/GetSuiteServiceInfo", ReplyAction = "http://tempuri.org/IShellService/GetSuiteServiceInfoResponse")]
	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/GetSuiteServiceInfoShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	SuiteServiceInfo GetSuiteServiceInfo(GetSuiteServiceInfoRequest getSuiteServiceInfoRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/GetSuiteServiceInfo", ReplyAction = "http://tempuri.org/IShellService/GetSuiteServiceInfoResponse")]
	IAsyncResult BeginGetSuiteServiceInfo(GetSuiteServiceInfoRequest getSuiteServiceInfoRequest, AsyncCallback callback, object asyncState);

	SuiteServiceInfo EndGetSuiteServiceInfo(IAsyncResult result);

	[OperationContract(Action = "http://tempuri.org/IShellService/GetShellSettings", ReplyAction = "http://tempuri.org/IShellService/GetShellSettingsResponse")]
	[FaultContract(typeof(ShellWebServiceFault), Action = "http://tempuri.org/IShellService/GetShellSettingsShellWebServiceFaultFault", Name = "ShellWebServiceFault", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.Shell")]
	ShellSettingsResponse GetShellSettings(ShellSettingsRequest shellSettingsRequest);

	[OperationContract(AsyncPattern = true, Action = "http://tempuri.org/IShellService/GetShellSettings", ReplyAction = "http://tempuri.org/IShellService/GetShellSettingsResponse")]
	IAsyncResult BeginGetShellSettings(ShellSettingsRequest shellSettingsRequest, AsyncCallback callback, object asyncState);

	ShellSettingsResponse EndGetShellSettings(IAsyncResult result);
}
