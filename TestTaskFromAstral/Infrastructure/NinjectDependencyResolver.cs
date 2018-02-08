using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using System.Web.Mvc;
using Infrastructure.Data.Interfaces;
using Infrastructure.Data.UnitOfWork;
using Infrastructure.Data.Context;
using System.Data.Entity;

namespace TestTaskFromAstral.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            // Здесь размещаются привязки  
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>();
            kernel.Bind<IApplicationContext>().To<ApplicationContext>();
        }
    }
}