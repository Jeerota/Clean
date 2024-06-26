using Clean.Domain.Common.Models;
using Clean.Domain.ContextNameContext.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Clean.Domain.ContextNameContext.Models.LookupRequests;
using Clean.Domain.ContextNameContext.Models.DTOs;

namespace Clean.Blazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableNameController : ControllerBase
    {
        private readonly ITableNameService _TableNameService;
        private readonly ILogger<TableNameController> _logger;

        public TableNameController(ILogger<TableNameController> log, ITableNameService TableNameService)
        {
            _TableNameService = TableNameService;
            _logger = log;
        }

        [HttpPost("TableName")]
        public async Task<IActionResult> CreateTableName()
        {
            _logger.LogInformation("Begining TableName create request.");

            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            TableNameDTO dto;
            try
            {
                dto = JsonConvert.DeserializeObject<TableNameDTO>(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to parse TableName: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            try
            {
                ResultResponse<TableNameDTO> response = _TableNameService.Create(dto);

                string json = JsonConvert.SerializeObject(response);
                if (response.Successful)
                {
                    _logger.LogInformation($"Created TableName.");
                    return new OkObjectResult(json);
                }
                else
                {
                    _logger.LogInformation($"Unable to create TableName: {string.Join(", ", response.Errors)}");
                    return new BadRequestObjectResult(json);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating from service: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpDelete("TableName")]
        public async Task<IActionResult> DeleteTableName()
        {
            _logger.LogInformation("Begining TableName delete request.");

            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            string[] primaryKey;
            try
            {
                primaryKey = JsonConvert.DeserializeObject<string[]>(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to parse primary key: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            try
            {
                ResultResponse<TableNameDTO> response = _TableNameService.Delete(primaryKey);

                string json = JsonConvert.SerializeObject(response);
                if (response.Successful)
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
            catch (Exception ex)
            {
                _logger.LogError($"Error updating from service: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpGet("TableName")]
        public async Task<IActionResult> GetTableName()
        {
            _logger.LogInformation("Begining TableName lookup request.");

            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            TableNameLookupRequest lookupRequest;
            try
            {
                lookupRequest = JsonConvert.DeserializeObject<TableNameLookupRequest>(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to parse lookupRequest: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            int? page = Request.Query.ContainsKey("page")
                ? int.TryParse(Request.Query["page"], out int result) ? result : (int?)null
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
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching from service: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPatch("TableName")]
        public async Task<IActionResult> UpdateTableName()
        {
            _logger.LogInformation("Begining TableName update request.");

            string requestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            TableNameDTO dto;
            try
            {
                dto = JsonConvert.DeserializeObject<TableNameDTO>(requestBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to parse TableName: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }

            try
            {
                ResultResponse<TableNameDTO> response = _TableNameService.Update(dto);

                string json = JsonConvert.SerializeObject(response);
                if (response.Successful)
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
            catch (Exception ex)
            {
                _logger.LogError($"Error updating from service: {ex.Message}");
                return new BadRequestObjectResult(ex.Message);
            }
        }

    }
}
