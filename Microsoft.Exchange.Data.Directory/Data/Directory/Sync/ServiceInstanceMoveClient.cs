using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "4.0.0.0")]
	public class ServiceInstanceMoveClient : ClientBase<IServiceInstanceMove>, IServiceInstanceMove
	{
		public ServiceInstanceMoveClient()
		{
		}

		public ServiceInstanceMoveClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public ServiceInstanceMoveClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public ServiceInstanceMoveClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public ServiceInstanceMoveClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		StartServiceInstanceMoveTaskResponse IServiceInstanceMove.StartServiceInstanceMoveTask(StartServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.StartServiceInstanceMoveTask(request);
		}

		public ServiceInstanceMoveOperationResult StartServiceInstanceMoveTask(string contextId, string oldServiceInstance, string newServiceInstance, ServiceInstanceMoveOptions options)
		{
			StartServiceInstanceMoveTaskResponse startServiceInstanceMoveTaskResponse = ((IServiceInstanceMove)this).StartServiceInstanceMoveTask(new StartServiceInstanceMoveTaskRequest
			{
				contextId = contextId,
				oldServiceInstance = oldServiceInstance,
				newServiceInstance = newServiceInstance,
				options = options
			});
			return startServiceInstanceMoveTaskResponse.StartServiceInstanceMoveTaskResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<StartServiceInstanceMoveTaskResponse> IServiceInstanceMove.StartServiceInstanceMoveTaskAsync(StartServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.StartServiceInstanceMoveTaskAsync(request);
		}

		public Task<StartServiceInstanceMoveTaskResponse> StartServiceInstanceMoveTaskAsync(string contextId, string oldServiceInstance, string newServiceInstance, ServiceInstanceMoveOptions options)
		{
			return ((IServiceInstanceMove)this).StartServiceInstanceMoveTaskAsync(new StartServiceInstanceMoveTaskRequest
			{
				contextId = contextId,
				oldServiceInstance = oldServiceInstance,
				newServiceInstance = newServiceInstance,
				options = options
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		GetServiceInstanceMoveTaskStatusResponse IServiceInstanceMove.GetServiceInstanceMoveTaskStatus(GetServiceInstanceMoveTaskStatusRequest request)
		{
			return base.Channel.GetServiceInstanceMoveTaskStatus(request);
		}

		public ServiceInstanceMoveOperationResult GetServiceInstanceMoveTaskStatus(ServiceInstanceMoveTask serviceInstanceMoveTask, byte[] lastCookie)
		{
			GetServiceInstanceMoveTaskStatusResponse serviceInstanceMoveTaskStatus = ((IServiceInstanceMove)this).GetServiceInstanceMoveTaskStatus(new GetServiceInstanceMoveTaskStatusRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask,
				lastCookie = lastCookie
			});
			return serviceInstanceMoveTaskStatus.GetServiceInstanceMoveTaskStatusResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<GetServiceInstanceMoveTaskStatusResponse> IServiceInstanceMove.GetServiceInstanceMoveTaskStatusAsync(GetServiceInstanceMoveTaskStatusRequest request)
		{
			return base.Channel.GetServiceInstanceMoveTaskStatusAsync(request);
		}

		public Task<GetServiceInstanceMoveTaskStatusResponse> GetServiceInstanceMoveTaskStatusAsync(ServiceInstanceMoveTask serviceInstanceMoveTask, byte[] lastCookie)
		{
			return ((IServiceInstanceMove)this).GetServiceInstanceMoveTaskStatusAsync(new GetServiceInstanceMoveTaskStatusRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask,
				lastCookie = lastCookie
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		FinalizeServiceInstanceMoveTaskResponse IServiceInstanceMove.FinalizeServiceInstanceMoveTask(FinalizeServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.FinalizeServiceInstanceMoveTask(request);
		}

		public ServiceInstanceMoveOperationResult FinalizeServiceInstanceMoveTask(ServiceInstanceMoveTask serviceInstanceMoveTask, byte[] lastCookie)
		{
			FinalizeServiceInstanceMoveTaskResponse finalizeServiceInstanceMoveTaskResponse = ((IServiceInstanceMove)this).FinalizeServiceInstanceMoveTask(new FinalizeServiceInstanceMoveTaskRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask,
				lastCookie = lastCookie
			});
			return finalizeServiceInstanceMoveTaskResponse.FinalizeServiceInstanceMoveTaskResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<FinalizeServiceInstanceMoveTaskResponse> IServiceInstanceMove.FinalizeServiceInstanceMoveTaskAsync(FinalizeServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.FinalizeServiceInstanceMoveTaskAsync(request);
		}

		public Task<FinalizeServiceInstanceMoveTaskResponse> FinalizeServiceInstanceMoveTaskAsync(ServiceInstanceMoveTask serviceInstanceMoveTask, byte[] lastCookie)
		{
			return ((IServiceInstanceMove)this).FinalizeServiceInstanceMoveTaskAsync(new FinalizeServiceInstanceMoveTaskRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask,
				lastCookie = lastCookie
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		DeleteServiceInstanceMoveTaskResponse IServiceInstanceMove.DeleteServiceInstanceMoveTask(DeleteServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.DeleteServiceInstanceMoveTask(request);
		}

		public ServiceInstanceMoveOperationResult DeleteServiceInstanceMoveTask(ServiceInstanceMoveTask serviceInstanceMoveTask)
		{
			DeleteServiceInstanceMoveTaskResponse deleteServiceInstanceMoveTaskResponse = ((IServiceInstanceMove)this).DeleteServiceInstanceMoveTask(new DeleteServiceInstanceMoveTaskRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask
			});
			return deleteServiceInstanceMoveTaskResponse.DeleteServiceInstanceMoveTaskResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<DeleteServiceInstanceMoveTaskResponse> IServiceInstanceMove.DeleteServiceInstanceMoveTaskAsync(DeleteServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.DeleteServiceInstanceMoveTaskAsync(request);
		}

		public Task<DeleteServiceInstanceMoveTaskResponse> DeleteServiceInstanceMoveTaskAsync(ServiceInstanceMoveTask serviceInstanceMoveTask)
		{
			return ((IServiceInstanceMove)this).DeleteServiceInstanceMoveTaskAsync(new DeleteServiceInstanceMoveTaskRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask
			});
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		RecoverServiceInstanceMoveTaskResponse IServiceInstanceMove.RecoverServiceInstanceMoveTask(RecoverServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.RecoverServiceInstanceMoveTask(request);
		}

		public ServiceInstanceMoveOperationResult RecoverServiceInstanceMoveTask(ServiceInstanceMoveTask serviceInstanceMoveTask)
		{
			RecoverServiceInstanceMoveTaskResponse recoverServiceInstanceMoveTaskResponse = ((IServiceInstanceMove)this).RecoverServiceInstanceMoveTask(new RecoverServiceInstanceMoveTaskRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask
			});
			return recoverServiceInstanceMoveTaskResponse.RecoverServiceInstanceMoveTaskResult;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		Task<RecoverServiceInstanceMoveTaskResponse> IServiceInstanceMove.RecoverServiceInstanceMoveTaskAsync(RecoverServiceInstanceMoveTaskRequest request)
		{
			return base.Channel.RecoverServiceInstanceMoveTaskAsync(request);
		}

		public Task<RecoverServiceInstanceMoveTaskResponse> RecoverServiceInstanceMoveTaskAsync(ServiceInstanceMoveTask serviceInstanceMoveTask)
		{
			return ((IServiceInstanceMove)this).RecoverServiceInstanceMoveTaskAsync(new RecoverServiceInstanceMoveTaskRequest
			{
				serviceInstanceMoveTask = serviceInstanceMoveTask
			});
		}
	}
}
