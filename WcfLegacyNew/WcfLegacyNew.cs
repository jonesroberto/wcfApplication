using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;
using WcfLegacy;
using WcfLegacyNew.Listeners;

namespace WcfLegacyNew
{
    internal sealed class WcfLegacyNew : StatelessService
    {
        private readonly IMyLegacyService _myLegacyService;
        
        public WcfLegacyNew(StatelessServiceContext context)
            : base(context)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IMyLegacyService, MyLegacyService>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            _myLegacyService = serviceProvider.GetService<IMyLegacyService>();
        }

         protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
            =>
            new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener((context)=> {
                    var endpointAddress = new EndpointAddress(WcfListener.HttpServiceUri(context, "MyLegacyService.svc"));

                    var r = new WcfCommunicationListener<IMyLegacyService>(
                        serviceContext: context,
                        wcfServiceObject: _myLegacyService,
                        listenerBinding: WcfListener.GetBinding(BindingType.WsHttpBinding),
                        address: endpointAddress);

                    ServiceMetadataBehavior smb = r.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if(smb == null)
                    {
                        smb = new ServiceMetadataBehavior();
                        smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                        smb.HttpGetEnabled = true;
                        smb.HttpGetUrl = endpointAddress.Uri;
                        r.ServiceHost.Description.Behaviors.Add(smb);
                    }

                    return r;
                    },"WebHttpListenerMyLegacyService")
            };

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
