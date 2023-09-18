using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;
        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsysnc(BlogPost blogPost)
        {
            await dbContext.BlogPosts.AddAsync(blogPost);
            await dbContext.SaveChangesAsync();
            return blogPost;
        }
        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await dbContext.BlogPosts.Include(x=>x.Categories).ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
           return await dbContext.BlogPosts.Include(x=>x.Categories).FirstOrDefaultAsync(x=>x.Id==id);
        }
        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
           var existingBlogPost= await dbContext.BlogPosts.Include(x=>x.Categories)
                .FirstOrDefaultAsync(x=>x.Id==blogPost.Id);
            if (existingBlogPost is null)
                return null;
            //update blogpost
            dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
            //update category
            existingBlogPost.Categories = blogPost.Categories;
            await dbContext.SaveChangesAsync();
            return blogPost; 
        }
        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogPosts = await dbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
            if (existingBlogPosts is null)
                return null;
            dbContext.BlogPosts.Remove(existingBlogPosts);
            await dbContext.SaveChangesAsync();
            return existingBlogPosts;
        }
    }
}
