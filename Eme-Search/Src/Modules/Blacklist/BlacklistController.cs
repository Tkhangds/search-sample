using Eme_Search.Common;
using Eme_Search.Modules.Blacklist.DTOs;
using Eme_Search.Modules.Blacklist.Services;
using Eme_Search.Modules.Search.DTOs;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;

namespace Eme_Search.Modules.Blacklist;

public class BlacklistController: ApiController
{
    private readonly IBlacklistService _blacklistService;
    
    public BlacklistController(IBlacklistService blacklistService)
    {
        _blacklistService = blacklistService;
    }
    
    [HttpPost("categories")]
    public async Task<IActionResult> CreateBlacklistedCategory([FromBody] BlacklistCategoryRequestDto request)
    {
        var result =  await _blacklistService.AddBlacklistedCategory(request);
        return CreatedAtAction(nameof(CreateBlacklistedCategory), new { id = result }, null);
    }
    
    [HttpDelete("categories/{id:int}")]
    public async  Task<IActionResult> RemoveBlacklistedCategory([FromRoute] int id)
    {
        await _blacklistService.RemoveBlacklistedCategory(id);
        return NoContent();
    }
    
    [HttpPost("businesses")]
    public async Task<IActionResult> CreateBlacklistedBusiness([FromBody] BlacklistBusinessRequestDto request)
    {
        var result = await _blacklistService.AddBlacklistedBusiness(request);
        return CreatedAtAction(nameof(CreateBlacklistedBusiness), new { id = result }, null);
    }
    
    [HttpDelete("businesses/{id:int}")]
    public async Task<IActionResult> RemoveBlacklistedBusiness([FromRoute] int id)
    {
        await _blacklistService.RemoveBlacklistedBusiness(id);
        return NoContent();
    }
    
    [HttpGet("businesses")]
    public async Task<IActionResult> GetBlacklistedBusinesses([FromQuery] PaginationDto pagination)
    {
        var businesses = await _blacklistService.GetBlacklistedBusinesses(pagination);
        return Ok(businesses);
    }
    
    [HttpGet("categories")]
    public async Task<IActionResult> GetBlacklistedCategories([FromQuery] PaginationDto pagination)
    {
        var categories = await _blacklistService.GetBlacklistedCategories(pagination);
        return Ok(categories);
    }
}