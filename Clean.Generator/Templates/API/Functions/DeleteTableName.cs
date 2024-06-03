//This file was auto-genearted by the Clean.Generator.//
//Any modifications to this file will be overwritten on the next run of the generator.//

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Clean.Domain.ContextNameContext.Models.LookupRequests;
using Clean.Domain.ContextNameContext.Services;
using Clean.Infrastructure.ContextNameContext.Entities;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Http;
using Clean.Domain.Common.Models;
using Clean.Domain.ContextNameContext.Models.DTOs;

namespace Clean.API.Functions.ContextNameContext.TableName.Functions
{
    public class DeleteTableName
    {
        private readonly ITableNameService _TableNameService;
        private readonly ILogger<DeleteTableName> _logger;

        public DeleteTableName(ILogger<DeleteTableName> log, ITableNameService TableNameService)
        {
            _TableNameService = TableNameService;
            _logger = log;
        }

        [FunctionName("DeleteTableName")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "TableName")] HttpRequest req)
        {
            _logger.LogInformation("Begining TableName delete request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string[] primaryKey;
            try
            {
                primaryKey = JsonConvert.DeserializeObject<string[]>(requestBody);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Unable to parse primary key: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            try 
            { 
                ResultResponse<TableNameDTO> response = _TableNameService.Delete(primaryKey);

                string json = JsonConvert.SerializeObject(response);
                if(response.Successful)
                {
                    _logger.LogInformation($"Deleted TableName: {primaryKey}");
                    return new OkObjectResult(json);
                }
                else 
                { 
                    _logger.LogInformation($"Unable to delete TableName: {string.Join(", ", response.Errors)}");
                    return new BadRequestObjectResult(json);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error updating from service: {ex.Message}");
                return new ExceptionResult(ex, false);
            }
        }
    }
}

