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
using Clean.Domain.ContextNameContext.Models.DTOs;
using Clean.Domain.ContextNameContext.Services;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Http;
using Clean.Domain.Common.Models;

namespace Clean.API.Functions.ContextNameContext.TableName.Functions
{
    public class UpdateTableName
    {
        private readonly ITableNameService _TableNameService;
        private readonly ILogger<UpdateTableName> _logger;

        public UpdateTableName(ILogger<UpdateTableName> log, ITableNameService TableNameService)
        {
            _TableNameService = TableNameService;
            _logger = log;
        }

        [FunctionName("UpdateTableName")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "patch", Route = "TableName")] HttpRequest req)
        {
            _logger.LogInformation("Begining TableName update request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync(); 
            TableNameDTO dto;
            try
            {
                dto = JsonConvert.DeserializeObject<TableNameDTO>(requestBody);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Unable to parse TableName: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            try 
            { 
                ResultResponse<TableNameDTO> response = _TableNameService.Update(dto);

                string json = JsonConvert.SerializeObject(response);
                if(response.Successful)
                {
                    _logger.LogInformation($"Updated TableName.");
                    return new OkObjectResult(json);
                }
                else 
                { 
                    _logger.LogInformation($"Unable to update TableName: {string.Join(", ", response.Errors)}");
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

