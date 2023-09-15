using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Shipping.Tawsela.Services;

namespace Nop.Plugin.Shipping.Tawsela.Infrastructure
{

    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<TawselaService>().AsSelf().InstancePerLifetimeScope();
        }

        public int Order => 1;
    }
}