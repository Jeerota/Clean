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
using System.Collections.Generic;
using System;
using System.Linq;
using System.Web.Http;
using Clean.Domain.Common.Models;
using Clean.Domain.ContextNameContext.Models.DTOs;

namespace Clean.API.Functions.ContextNameContext.TableName.Functions
{
    public class GetTableName
    {
        private readonly ITableNameService _TableNameService;
        private readonly ILogger<GetTableName> _logger;

        public GetTableName(ILogger<GetTableName> log, ITableNameService TableNameService)
        {
            _TableNameService = TableNameService;
            _logger = log;
        }

        [FunctionName("GetTableName")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "TableName")] HttpRequest req)
        {
            _logger.LogInformation("Begining TableName lookup request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TableNameLookupRequest lookupRequest;
            try
            {
                lookupRequest = JsonConvert.DeserializeObject<TableNameLookupRequest>(requestBody);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Unable to parse lookupRequest: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            int? page = req.Query.ContainsKey("page")
                ? int.TryParse(req.Query["page"], out int result) ? result : (int?)null
                : (int?)null;

            try
            {
                if (lookupRequest == null)
                    lookupRequest = new();

                FetchResponse<TableNameDTO> response = _TableNameService.GetFetchResponse(lookupRequest);

                if (response.Records == null)
                {
                    _logger.LogInformation($"No TableName found for parameters: {requestBody}");
                    return new NotFoundResult();
                }
                else
                {
                    _logger.LogInformation($"Found {response.TotalRecords} and returning {response.RecordCount} records matching parameters: {requestBody}");

                    string json = JsonConvert.SerializeObject(response);
                    return new OkObjectResult(json);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error fetching from service: {ex.Message}");
                return new ExceptionResult(ex, false);
            }
        }
    }
}

