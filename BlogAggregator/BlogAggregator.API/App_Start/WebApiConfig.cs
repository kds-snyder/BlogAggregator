using AutoMapper;
using BlogAggregator.Core.Domain;
using BlogAggregator.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BlogAggregator.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Enable cross origin requests to API
            var cors = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
            );
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }           
            );

            // Remove XML format in order to return JSON
            var appXmlType =
                config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault
                                            (t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Initialize AutoMapper
            setUpAutoMapper();
        }

        public static void setUpAutoMapper()
        {
            Mapper.CreateMap<Blog, BlogModel>();
            Mapper.CreateMap<Post, PostModel>();            
        }
    }
}
