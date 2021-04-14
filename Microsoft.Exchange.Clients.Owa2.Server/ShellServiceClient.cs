using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;
using Microsoft.Online.BOX.UI.Shell.AllSettings;

[DebuggerStepThrough]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
public class ShellServiceClient : ClientBase<IShellService>, IShellService
{
	public ShellServiceClient()
	{
	}

	public ShellServiceClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public ShellServiceClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ShellServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ShellServiceClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public string GetMetaData(BrandInfo brandInfo, string locale, UserInfo userInfo, Options options)
	{
		return base.Channel.GetMetaData(brandInfo, locale, userInfo, options);
	}

	public IAsyncResult BeginGetMetaData(BrandInfo brandInfo, string locale, UserInfo userInfo, Options options, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetMetaData(brandInfo, locale, userInfo, options, callback, asyncState);
	}

	public string EndGetMetaData(IAsyncResult result)
	{
		return base.Channel.EndGetMetaData(result);
	}

	public NavBarInfo GetNavBarInfo(NavBarInfoRequest navBarInfoRequest)
	{
		return base.Channel.GetNavBarInfo(navBarInfoRequest);
	}

	public IAsyncResult BeginGetNavBarInfo(NavBarInfoRequest navBarInfoRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetNavBarInfo(navBarInfoRequest, callback, asyncState);
	}

	public NavBarInfo EndGetNavBarInfo(IAsyncResult result)
	{
		return base.Channel.EndGetNavBarInfo(result);
	}

	public void SetYammerEnabled(SetYammerEnabledRequest setYammerEnabledRequest)
	{
		base.Channel.SetYammerEnabled(setYammerEnabledRequest);
	}

	public IAsyncResult BeginSetYammerEnabled(SetYammerEnabledRequest setYammerEnabledRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginSetYammerEnabled(setYammerEnabledRequest, callback, asyncState);
	}

	public void EndSetYammerEnabled(IAsyncResult result)
	{
		base.Channel.EndSetYammerEnabled(result);
	}

	public ShellInfo GetShellInfo(ShellInfoRequest shellInfoRequest)
	{
		return base.Channel.GetShellInfo(shellInfoRequest);
	}

	public IAsyncResult BeginGetShellInfo(ShellInfoRequest shellInfoRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetShellInfo(shellInfoRequest, callback, asyncState);
	}

	public ShellInfo EndGetShellInfo(IAsyncResult result)
	{
		return base.Channel.EndGetShellInfo(result);
	}

	public void SetUserTheme(SetUserThemeRequest setUserThemeRequest)
	{
		base.Channel.SetUserTheme(setUserThemeRequest);
	}

	public IAsyncResult BeginSetUserTheme(SetUserThemeRequest setUserThemeRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginSetUserTheme(setUserThemeRequest, callback, asyncState);
	}

	public void EndSetUserTheme(IAsyncResult result)
	{
		base.Channel.EndSetUserTheme(result);
	}

	public void DoNothing(NavBarData ignored0)
	{
		base.Channel.DoNothing(ignored0);
	}

	public IAsyncResult BeginDoNothing(NavBarData ignored0, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginDoNothing(ignored0, callback, asyncState);
	}

	public void EndDoNothing(IAsyncResult result)
	{
		base.Channel.EndDoNothing(result);
	}

	public Alert[] GetAlerts(GetAlertRequest getAlertRequest)
	{
		return base.Channel.GetAlerts(getAlertRequest);
	}

	public IAsyncResult BeginGetAlerts(GetAlertRequest getAlertRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetAlerts(getAlertRequest, callback, asyncState);
	}

	public Alert[] EndGetAlerts(IAsyncResult result)
	{
		return base.Channel.EndGetAlerts(result);
	}

	public SuiteServiceInfo GetSuiteServiceInfo(GetSuiteServiceInfoRequest getSuiteServiceInfoRequest)
	{
		return base.Channel.GetSuiteServiceInfo(getSuiteServiceInfoRequest);
	}

	public IAsyncResult BeginGetSuiteServiceInfo(GetSuiteServiceInfoRequest getSuiteServiceInfoRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetSuiteServiceInfo(getSuiteServiceInfoRequest, callback, asyncState);
	}

	public SuiteServiceInfo EndGetSuiteServiceInfo(IAsyncResult result)
	{
		return base.Channel.EndGetSuiteServiceInfo(result);
	}

	public ShellSettingsResponse GetShellSettings(ShellSettingsRequest shellSettingsRequest)
	{
		return base.Channel.GetShellSettings(shellSettingsRequest);
	}

	public IAsyncResult BeginGetShellSettings(ShellSettingsRequest shellSettingsRequest, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetShellSettings(shellSettingsRequest, callback, asyncState);
	}

	public ShellSettingsResponse EndGetShellSettings(IAsyncResult result)
	{
		return base.Channel.EndGetShellSettings(result);
	}
}
