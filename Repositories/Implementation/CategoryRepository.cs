using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<Category> CreateAsync(Category category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            return category;
        }

      

        public async Task<IEnumerable<Category>> GetAllAsync(string? query = null, string? sortBy = null, string? sortDirection = null, int? pageNumber = 1, int? pageSize = 100)
        {
            //Query
            var categories = dbContext.Categories.AsQueryable();

            //Filtering
            if(string.IsNullOrWhiteSpace(query)== false)
                categories= categories.Where(x=>x.Name.Contains(query));
            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (string.Equals(sortBy,"Name", StringComparison.OrdinalIgnoreCase))
                {
                    bool isAsc=string.Equals(sortDirection,"asc", StringComparison.OrdinalIgnoreCase)?true:false;
                    categories= isAsc?categories.OrderBy(x=>x.Name):categories.OrderByDescending(x=>x.Name);
                }
                if (string.Equals(sortBy, "URL", StringComparison.OrdinalIgnoreCase))
                {
                    bool isAsc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;
                    categories = isAsc ? categories.OrderBy(x => x.UrlHandle) : categories.OrderByDescending(x => x.UrlHandle);
                }

            }
            //Pagination
            //PageNumber 1 pageSize 5 - skip 0 , take 5
            //PageNumber 2 pageSize 5 - skip 5 , take 5
            //PageNumber 3 pageSize 5 - skip 10 , take 5

            var skipResults = ( pageNumber - 1 ) * pageSize;

            categories= categories.Skip(skipResults??0).Take(pageSize??100);

            return await categories.ToListAsync();

          //return await dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Category?> UpdateAsync(Category category)
        {
            var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == category.Id);

            if (existingCategory != null)
            {
                dbContext.Entry(existingCategory).CurrentValues.SetValues(category);
                await dbContext.SaveChangesAsync();
                return category;
            }

            return null;
        }
        public async Task<Category?> DeleteAsync(Guid id)
        {
           var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (existingCategory is null)
                return null;
             dbContext.Categories.Remove(existingCategory);
            await dbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<Category?> DeleteBulkAsync(List<Guid> ids)
        {
            //var existingCategory= (dynamic)null;
            Category existingCategory = null;
            foreach (var id in ids)
            {
                existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (existingCategory is null)
                    return null;
                dbContext.Categories.Remove(existingCategory);
            }
            await dbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<int> GetCountAsync()
        {
           return await dbContext.Categories.CountAsync();
        }
    }
}
