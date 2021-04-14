using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using www.outlook.com.highavailability.ServerLocator.v1;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[DebuggerStepThrough]
public class ServerLocatorClient : ClientBase<ServerLocator>, ServerLocator
{
	public ServerLocatorClient()
	{
	}

	public ServerLocatorClient(string endpointConfigurationName) : base(endpointConfigurationName)
	{
	}

	public ServerLocatorClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ServerLocatorClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
	{
	}

	public ServerLocatorClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
	{
	}

	public event EventHandler<GetVersionCompletedEventArgs> GetVersionCompleted;

	public event EventHandler<GetServerForDatabaseCompletedEventArgs> GetServerForDatabaseCompleted;

	public event EventHandler<GetActiveCopiesForDatabaseAvailabilityGroupCompletedEventArgs> GetActiveCopiesForDatabaseAvailabilityGroupCompleted;

	public event EventHandler<GetActiveCopiesForDatabaseAvailabilityGroupExtendedCompletedEventArgs> GetActiveCopiesForDatabaseAvailabilityGroupExtendedCompleted;

	public ServiceVersion GetVersion()
	{
		return base.Channel.GetVersion();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginGetVersion(AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetVersion(callback, asyncState);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public ServiceVersion EndGetVersion(IAsyncResult result)
	{
		return base.Channel.EndGetVersion(result);
	}

	private IAsyncResult OnBeginGetVersion(object[] inValues, AsyncCallback callback, object asyncState)
	{
		return this.BeginGetVersion(callback, asyncState);
	}

	private object[] OnEndGetVersion(IAsyncResult result)
	{
		ServiceVersion serviceVersion = this.EndGetVersion(result);
		return new object[]
		{
			serviceVersion
		};
	}

	private void OnGetVersionCompleted(object state)
	{
		if (this.GetVersionCompleted != null)
		{
			ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs)state;
			this.GetVersionCompleted(this, new GetVersionCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
		}
	}

	public void GetVersionAsync()
	{
		this.GetVersionAsync(null);
	}

	public void GetVersionAsync(object userState)
	{
		if (this.onBeginGetVersionDelegate == null)
		{
			this.onBeginGetVersionDelegate = new ClientBase<ServerLocator>.BeginOperationDelegate(this.OnBeginGetVersion);
		}
		if (this.onEndGetVersionDelegate == null)
		{
			this.onEndGetVersionDelegate = new ClientBase<ServerLocator>.EndOperationDelegate(this.OnEndGetVersion);
		}
		if (this.onGetVersionCompletedDelegate == null)
		{
			this.onGetVersionCompletedDelegate = new SendOrPostCallback(this.OnGetVersionCompleted);
		}
		base.InvokeAsync(this.onBeginGetVersionDelegate, null, this.onEndGetVersionDelegate, this.onGetVersionCompletedDelegate, userState);
	}

	public DatabaseServerInformation GetServerForDatabase(DatabaseServerInformation database)
	{
		return base.Channel.GetServerForDatabase(database);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginGetServerForDatabase(DatabaseServerInformation database, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetServerForDatabase(database, callback, asyncState);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DatabaseServerInformation EndGetServerForDatabase(IAsyncResult result)
	{
		return base.Channel.EndGetServerForDatabase(result);
	}

	private IAsyncResult OnBeginGetServerForDatabase(object[] inValues, AsyncCallback callback, object asyncState)
	{
		DatabaseServerInformation database = (DatabaseServerInformation)inValues[0];
		return this.BeginGetServerForDatabase(database, callback, asyncState);
	}

	private object[] OnEndGetServerForDatabase(IAsyncResult result)
	{
		DatabaseServerInformation databaseServerInformation = this.EndGetServerForDatabase(result);
		return new object[]
		{
			databaseServerInformation
		};
	}

	private void OnGetServerForDatabaseCompleted(object state)
	{
		if (this.GetServerForDatabaseCompleted != null)
		{
			ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs)state;
			this.GetServerForDatabaseCompleted(this, new GetServerForDatabaseCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
		}
	}

	public void GetServerForDatabaseAsync(DatabaseServerInformation database)
	{
		this.GetServerForDatabaseAsync(database, null);
	}

	public void GetServerForDatabaseAsync(DatabaseServerInformation database, object userState)
	{
		if (this.onBeginGetServerForDatabaseDelegate == null)
		{
			this.onBeginGetServerForDatabaseDelegate = new ClientBase<ServerLocator>.BeginOperationDelegate(this.OnBeginGetServerForDatabase);
		}
		if (this.onEndGetServerForDatabaseDelegate == null)
		{
			this.onEndGetServerForDatabaseDelegate = new ClientBase<ServerLocator>.EndOperationDelegate(this.OnEndGetServerForDatabase);
		}
		if (this.onGetServerForDatabaseCompletedDelegate == null)
		{
			this.onGetServerForDatabaseCompletedDelegate = new SendOrPostCallback(this.OnGetServerForDatabaseCompleted);
		}
		base.InvokeAsync(this.onBeginGetServerForDatabaseDelegate, new object[]
		{
			database
		}, this.onEndGetServerForDatabaseDelegate, this.onGetServerForDatabaseCompletedDelegate, userState);
	}

	public DatabaseServerInformation[] GetActiveCopiesForDatabaseAvailabilityGroup()
	{
		return base.Channel.GetActiveCopiesForDatabaseAvailabilityGroup();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginGetActiveCopiesForDatabaseAvailabilityGroup(AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetActiveCopiesForDatabaseAvailabilityGroup(callback, asyncState);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DatabaseServerInformation[] EndGetActiveCopiesForDatabaseAvailabilityGroup(IAsyncResult result)
	{
		return base.Channel.EndGetActiveCopiesForDatabaseAvailabilityGroup(result);
	}

	private IAsyncResult OnBeginGetActiveCopiesForDatabaseAvailabilityGroup(object[] inValues, AsyncCallback callback, object asyncState)
	{
		return this.BeginGetActiveCopiesForDatabaseAvailabilityGroup(callback, asyncState);
	}

	private object[] OnEndGetActiveCopiesForDatabaseAvailabilityGroup(IAsyncResult result)
	{
		DatabaseServerInformation[] array = this.EndGetActiveCopiesForDatabaseAvailabilityGroup(result);
		return new object[]
		{
			array
		};
	}

	private void OnGetActiveCopiesForDatabaseAvailabilityGroupCompleted(object state)
	{
		if (this.GetActiveCopiesForDatabaseAvailabilityGroupCompleted != null)
		{
			ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs)state;
			this.GetActiveCopiesForDatabaseAvailabilityGroupCompleted(this, new GetActiveCopiesForDatabaseAvailabilityGroupCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
		}
	}

	public void GetActiveCopiesForDatabaseAvailabilityGroupAsync()
	{
		this.GetActiveCopiesForDatabaseAvailabilityGroupAsync(null);
	}

	public void GetActiveCopiesForDatabaseAvailabilityGroupAsync(object userState)
	{
		if (this.onBeginGetActiveCopiesForDatabaseAvailabilityGroupDelegate == null)
		{
			this.onBeginGetActiveCopiesForDatabaseAvailabilityGroupDelegate = new ClientBase<ServerLocator>.BeginOperationDelegate(this.OnBeginGetActiveCopiesForDatabaseAvailabilityGroup);
		}
		if (this.onEndGetActiveCopiesForDatabaseAvailabilityGroupDelegate == null)
		{
			this.onEndGetActiveCopiesForDatabaseAvailabilityGroupDelegate = new ClientBase<ServerLocator>.EndOperationDelegate(this.OnEndGetActiveCopiesForDatabaseAvailabilityGroup);
		}
		if (this.onGetActiveCopiesForDatabaseAvailabilityGroupCompletedDelegate == null)
		{
			this.onGetActiveCopiesForDatabaseAvailabilityGroupCompletedDelegate = new SendOrPostCallback(this.OnGetActiveCopiesForDatabaseAvailabilityGroupCompleted);
		}
		base.InvokeAsync(this.onBeginGetActiveCopiesForDatabaseAvailabilityGroupDelegate, null, this.onEndGetActiveCopiesForDatabaseAvailabilityGroupDelegate, this.onGetActiveCopiesForDatabaseAvailabilityGroupCompletedDelegate, userState);
	}

	public DatabaseServerInformation[] GetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters)
	{
		return base.Channel.GetActiveCopiesForDatabaseAvailabilityGroupExtended(parameters);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters, AsyncCallback callback, object asyncState)
	{
		return base.Channel.BeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(parameters, callback, asyncState);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public DatabaseServerInformation[] EndGetActiveCopiesForDatabaseAvailabilityGroupExtended(IAsyncResult result)
	{
		return base.Channel.EndGetActiveCopiesForDatabaseAvailabilityGroupExtended(result);
	}

	private IAsyncResult OnBeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(object[] inValues, AsyncCallback callback, object asyncState)
	{
		GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters = (GetActiveCopiesForDatabaseAvailabilityGroupParameters)inValues[0];
		return this.BeginGetActiveCopiesForDatabaseAvailabilityGroupExtended(parameters, callback, asyncState);
	}

	private object[] OnEndGetActiveCopiesForDatabaseAvailabilityGroupExtended(IAsyncResult result)
	{
		DatabaseServerInformation[] array = this.EndGetActiveCopiesForDatabaseAvailabilityGroupExtended(result);
		return new object[]
		{
			array
		};
	}

	private void OnGetActiveCopiesForDatabaseAvailabilityGroupExtendedCompleted(object state)
	{
		if (this.GetActiveCopiesForDatabaseAvailabilityGroupExtendedCompleted != null)
		{
			ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs invokeAsyncCompletedEventArgs = (ClientBase<ServerLocator>.InvokeAsyncCompletedEventArgs)state;
			this.GetActiveCopiesForDatabaseAvailabilityGroupExtendedCompleted(this, new GetActiveCopiesForDatabaseAvailabilityGroupExtendedCompletedEventArgs(invokeAsyncCompletedEventArgs.Results, invokeAsyncCompletedEventArgs.Error, invokeAsyncCompletedEventArgs.Cancelled, invokeAsyncCompletedEventArgs.UserState));
		}
	}

	public void GetActiveCopiesForDatabaseAvailabilityGroupExtendedAsync(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters)
	{
		this.GetActiveCopiesForDatabaseAvailabilityGroupExtendedAsync(parameters, null);
	}

	public void GetActiveCopiesForDatabaseAvailabilityGroupExtendedAsync(GetActiveCopiesForDatabaseAvailabilityGroupParameters parameters, object userState)
	{
		if (this.onBeginGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate == null)
		{
			this.onBeginGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate = new ClientBase<ServerLocator>.BeginOperationDelegate(this.OnBeginGetActiveCopiesForDatabaseAvailabilityGroupExtended);
		}
		if (this.onEndGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate == null)
		{
			this.onEndGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate = new ClientBase<ServerLocator>.EndOperationDelegate(this.OnEndGetActiveCopiesForDatabaseAvailabilityGroupExtended);
		}
		if (this.onGetActiveCopiesForDatabaseAvailabilityGroupExtendedCompletedDelegate == null)
		{
			this.onGetActiveCopiesForDatabaseAvailabilityGroupExtendedCompletedDelegate = new SendOrPostCallback(this.OnGetActiveCopiesForDatabaseAvailabilityGroupExtendedCompleted);
		}
		base.InvokeAsync(this.onBeginGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate, new object[]
		{
			parameters
		}, this.onEndGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate, this.onGetActiveCopiesForDatabaseAvailabilityGroupExtendedCompletedDelegate, userState);
	}

	private ClientBase<ServerLocator>.BeginOperationDelegate onBeginGetVersionDelegate;

	private ClientBase<ServerLocator>.EndOperationDelegate onEndGetVersionDelegate;

	private SendOrPostCallback onGetVersionCompletedDelegate;

	private ClientBase<ServerLocator>.BeginOperationDelegate onBeginGetServerForDatabaseDelegate;

	private ClientBase<ServerLocator>.EndOperationDelegate onEndGetServerForDatabaseDelegate;

	private SendOrPostCallback onGetServerForDatabaseCompletedDelegate;

	private ClientBase<ServerLocator>.BeginOperationDelegate onBeginGetActiveCopiesForDatabaseAvailabilityGroupDelegate;

	private ClientBase<ServerLocator>.EndOperationDelegate onEndGetActiveCopiesForDatabaseAvailabilityGroupDelegate;

	private SendOrPostCallback onGetActiveCopiesForDatabaseAvailabilityGroupCompletedDelegate;

	private ClientBase<ServerLocator>.BeginOperationDelegate onBeginGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate;

	private ClientBase<ServerLocator>.EndOperationDelegate onEndGetActiveCopiesForDatabaseAvailabilityGroupExtendedDelegate;

	private SendOrPostCallback onGetActiveCopiesForDatabaseAvailabilityGroupExtendedCompletedDelegate;
}
