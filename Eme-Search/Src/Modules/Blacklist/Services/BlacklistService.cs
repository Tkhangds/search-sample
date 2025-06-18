using AutoMapper;
using Eme_Search.Common;
using Eme_Search.Common.Cache;
using Eme_Search.Database.Models;
using Eme_Search.Infrastructures.Repositories;
using Eme_Search.Infrastructures.Specifications.Impl;
using Eme_Search.Modules.Blacklist.DTOs;
using Eme_Search.Modules.Search.DTOs;
using Pagination.EntityFrameworkCore.Extensions;

namespace Eme_Search.Modules.Blacklist.Services;

public class BlacklistService: IBlacklistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;  
    
    public BlacklistService(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<int> AddBlacklistedBusiness(BlacklistBusinessRequestDto request)
    {
        var business = _mapper.Map<BlacklistBusiness>(request);
        await _unitOfWork.Repository<BlacklistBusiness>().AddAsync(business);
        await _unitOfWork.SaveChangesAsync();

        await _cacheService.ClearAllAsync();
        
        return business.Id;
    }

    public async Task RemoveBlacklistedBusiness(int id)
    {
        var getBusinessSpec = BlacklistBusinessSpecification.GetById(id);
        var business = await _unitOfWork.Repository<BlacklistBusiness>().GetFirstAsync(getBusinessSpec);
        if (business == null)
        {
            throw new KeyNotFoundException($"BlacklistedBusiness with id {id} not found.");
        }

        await _unitOfWork.Repository<BlacklistBusiness>().DeleteAsync(business);
        
        await _cacheService.ClearAllAsync();
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<int> AddBlacklistedCategory(BlacklistCategoryRequestDto request)
    {
        var category = _mapper.Map<BlacklistCategory>(request);
        await _unitOfWork.Repository<BlacklistCategory>().AddAsync(category);
        
        await _cacheService.ClearAllAsync();
        
        await _unitOfWork.SaveChangesAsync();
        
        return category.Id;
    }

    public async Task RemoveBlacklistedCategory(int id)
    {
        var getCategorySpec = BlacklistCategorySpecification.GetById(id);
        var category = await _unitOfWork.Repository<BlacklistCategory>().GetFirstAsync(getCategorySpec);
        if (category == null)
        {
            throw new KeyNotFoundException($"BlacklistedCategory with id {id} not found.");
        }

        await _unitOfWork.Repository<BlacklistCategory>().DeleteAsync(category);
        
        await _cacheService.ClearAllAsync();
        
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Pagination<BlacklistBusinessResponseDto>> GetBlacklistedBusinesses(PaginationDto pagination)
    {
        var getPagedBlacklistBusinessSpec = BlacklistBusinessSpecification.GetPagedBlacklistBusiness(pagination);
        var lists = await _unitOfWork.Repository<BlacklistBusiness>().GetAllAsync(getPagedBlacklistBusinessSpec);
        var totalLists = await _unitOfWork.Repository<BlacklistBusiness>().CountAsync();

        var listDtos = _mapper.Map<BlacklistBusinessResponseDto[]>(lists);

        var currentPage = pagination.Page;
        var currentLimit = pagination.Limit;
        
        return new Pagination<BlacklistBusinessResponseDto>(listDtos, totalLists, currentPage, currentLimit);
    }

    public async Task<Pagination<BlacklistCategoryResponseDto>> GetBlacklistedCategories(PaginationDto pagination)
    {
        var getPagedBlacklistCategorySpec = BlacklistCategorySpecification.GetPagedBlacklistCategory(pagination);
        var lists = await _unitOfWork.Repository<BlacklistCategory>().GetAllAsync(getPagedBlacklistCategorySpec);
        var totalLists = await _unitOfWork.Repository<BlacklistCategory>().CountAsync();

        var listDtos = _mapper.Map<BlacklistCategoryResponseDto[]>(lists);

        var currentPage = pagination.Page;
        var currentLimit = pagination.Limit;

        return new Pagination<BlacklistCategoryResponseDto>(listDtos, totalLists, currentPage, currentLimit);
    }

    public async Task<string[]> FilterCategoryList(string[] categories)
    {
        var blacklistedCategories = (await _unitOfWork.Repository<BlacklistCategory>().GetAllAsync()) 
            .Select(b => b.Alias)
            .ToHashSet();
        
        var filteredCategories = categories
            .Where(c => !blacklistedCategories.Contains(c))
            .ToArray();

        return filteredCategories;
    }

    public async Task<List<StandardSearchResultDto>> FilterSearchResults(List<StandardSearchResultDto> results)
    {
        var blacklistedBusinesses = (await _unitOfWork.Repository<BlacklistBusiness>().GetAllAsync());
        
        var blacklistedAliases = blacklistedBusinesses.Select(b => b.Alias).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var blacklistedCategories = (await _unitOfWork.Repository<BlacklistCategory>().GetAllAsync()) 
            .Select(b => b.Alias)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var filtered = results.Where(r =>
            !blacklistedAliases.Contains(r.Alias) &&
            !r.Categories.Any(c => blacklistedCategories.Contains(c.Alias))
        ).ToList();

        return filtered;
    }
}