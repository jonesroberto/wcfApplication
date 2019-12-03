using System;
using System.Fabric;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfLegacyNew.Listeners
{
    public static class WcfListener
    {
        public static string HttpServiceUri(StatelessServiceContext context, string name)
        {
            var epc = context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            var uri = $"http://{context.NodeContext.IPAddressOrFQDN}:{epc.Port}/{name}";
            return uri;
        }

        public static Binding GetBinding(BindingType type)
        {
            switch (type)
            {
                case BindingType.WsHttpBinding:
                    return GenerateWsHttpBinding();

                default:
                    throw new ArgumentException("Tipo de binding inexistente.");
            }
        }

        private static WSHttpBinding GenerateWsHttpBinding()
        {
            var wsBinding = new WSHttpBinding
            {
                SendTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10),
                OpenTimeout = TimeSpan.FromMinutes(10),
                CloseTimeout = TimeSpan.FromMinutes(10),
                MaxReceivedMessageSize = int.MaxValue,
                AllowCookies = false,
                BypassProxyOnLocal = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxBufferPoolSize = int.MaxValue,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = Encoding.UTF8,
                UseDefaultWebProxy = true,
            };

            wsBinding.ReaderQuotas.MaxDepth = int.MaxValue;
            wsBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            wsBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            wsBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            wsBinding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            wsBinding.Security.Mode = SecurityMode.None;
            return wsBinding;
        }
    }
}
