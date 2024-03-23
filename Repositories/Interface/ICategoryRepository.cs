﻿using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<IEnumerable<Category>> GetAllAsync(string? query = null,string? sortBy = null, string? sortDirection = null,int? pageNumber=1,int? pageSize=100);
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> UpdateAsync(Category category);
        Task<Category?> DeleteAsync(Guid id);
        //Task<Category?> DeleteBulkAsync(Guid[] ids);
        Task<Category?> DeleteBulkAsync(List<Guid> ids);

        Task<int> GetCountAsync();

    }
}
