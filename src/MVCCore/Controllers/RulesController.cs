using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using MVCCore.Services;
using MVCCore.Common;
using MVCCore.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace MVCCore.Controllers
{
    [Route("api/[controller]")]
    [FormatFilter]
    public class RulesController : Controller
    {
        IConfigurationRoot configroot;
        ILogger logger;
        IAzureStorageManager azMgr;
        IAzureJobManager azJobMgr;

        public RulesController(IConfigurationRoot config, ILoggerFactory loggerFactory, IAzureStorageManager azMgr, IAzureJobManager azJobMgr) 
        {
            if (config == null)
                throw new ArgumentNullException("config");

            configroot = config;
            this.azMgr = azMgr;
            this.azJobMgr = azJobMgr;

            logger = loggerFactory.CreateLogger<RulesController>();
        }

       // [Route("[action]/{rulename}")]
        [HttpPost("[action]/{inputruleparam}")]
        public async Task<IActionResult> UploadAndApplyRule(string inputruleparam)
        {
            if (Request.Headers["Content-Type"] != "text/plain")
                return StatusCode((int)HttpStatusCode.UnsupportedMediaType);

            KeyValuePair<string,string> rule;
            string id = string.Empty;

            try
            {
                //Parse Rule name
                rule = await RuleParser.ParseRule(configroot, inputruleparam);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return BadRequest("An error has occured while parsing rule command.");
            }

            try
            {
                if (!string.IsNullOrEmpty(rule.Value) && !string.IsNullOrEmpty(rule.Key))
                {
                    id = await azMgr.UploadtoAzureStorage(Request.Body, Request.ContentType, rule.Key);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return BadRequest("An error has occured while uploading file");
            }

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var jobinfo = new Dictionary<string, KeyValuePair<string, string>>();
                    jobinfo.Add(id+".txt", rule);
                    await azJobMgr.CreateJobInfo(jobinfo);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return BadRequest("An error has occured while creating job information");
            }

            return Ok(new CallbackData { CallbackUrl= "api/rules/GetResults", Id= id });
        }

        [Route("[action]/{id}")]
        public async Task<IActionResult> GetResults(string id)
        {
            try
            {
                var data = await azMgr.DownloadfromAzureStorage(id);
                return Ok(data);
            }
            catch (Exception e)
            {
                logger.LogError(e.ToString());
                return BadRequest("An error has occured while downloading content");
            }
        }

        // GET: api/values
        [HttpGet]
        public IActionResult Get()
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }
       
    }
}
