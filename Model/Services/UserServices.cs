using Microsoft.EntityFrameworkCore;

namespace Model.Services
{
    public interface IUserServices
    {
        public Task<bool> IsEmailExists(string email);
        public Task<User> GetUserById(int id);
        public Task<User> GetUser(string email, string password);
        public Task<User> AddUser(User oUser);
        public Task<User> UpdateUser(User oUser);
    }

    public class UserServices : IUserServices
    {

        private readonly PostgresDbContext _dbContext;
        public UserServices(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> IsEmailExists(string email)
        {
            return await _dbContext.User.AnyAsync(x => x.Email == email);
        }
        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.User.Where(x => x.Id == id).FirstOrDefaultAsync();
        }
        public async Task<User> GetUser(string email, string password)
        {
            return await _dbContext.User.FirstOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<User> AddUser(User oUser)
        {
            var result = _dbContext.User.Add(oUser);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
        public async Task<User> UpdateUser(User oUser)
        {
            var result = _dbContext.User.Update(oUser);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
