﻿using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Orchard.Core.XmlRpc.Controllers
{
    [OrchardFeature("Orchard.RemotePublishing")]
    public class MetaWeblogController : Controller
    {
        private readonly IEnumerable<IXmlRpcHandler> _xmlRpcHandlers;
        private const string ManifestUri = "http://schemas.microsoft.com/wlw/manifest/weblog";

        public MetaWeblogController(
            IEnumerable<IXmlRpcHandler> xmlRpcHandlers,
            ILogger<MetaWeblogController> logger)
        {
            _xmlRpcHandlers = xmlRpcHandlers;
            Logger = logger;
        }

        ILogger Logger { get; }

        [ResponseCache(Duration = 0, NoStore = true)]
        [AllowAnonymous]
        public ActionResult Manifest()
        {
            if (Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug("Manifest requested");
            }

            var options = new XElement(
                XName.Get("options", ManifestUri),
                new XElement(XName.Get("supportsAutoUpdate", ManifestUri), "Yes"),
                new XElement(XName.Get("clientType", ManifestUri), "MetaWeblog"),
                new XElement(XName.Get("supportsKeywords", ManifestUri), "No"),
                new XElement(XName.Get("supportsCategories", ManifestUri), "No"),
                new XElement(XName.Get("supportsFileUpload", ManifestUri), "No"),
                new XElement(XName.Get("supportsCustomDate", ManifestUri), "No"));

            foreach (var handler in _xmlRpcHandlers)
            {
                handler.SetCapabilities(options);
            }

            var doc = new XDocument(
                        new XElement(
                            XName.Get("manifest", ManifestUri),
                            options));

            return Content(doc.ToString(), "text/xml");
        }
    }
}