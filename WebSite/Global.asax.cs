﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using AMud.SignalR;
using Amud.Core;
using Autofac;
using Autofac.Integration.Mvc;

namespace AMud
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			IocConfig.RegisterIoc(new ContainerBuilder());
		}
	}
	public class IocConfig
	{
		public static void RegisterIoc(ContainerBuilder containerBuilder)
		{
			containerBuilder.RegisterControllers();
			containerBuilder.RegisterType<ClientProvider>()
				.As<IClientProvider>()
				.SingleInstance();
			var container = containerBuilder.Build();
			DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
		}
	}
}