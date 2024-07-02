using Microsoft.EntityFrameworkCore;

namespace Model.Services
{
    public interface IUserServices
    {
        public Task<User> GetUserById(int id);
        public Task<User> AddUser(User oUser);
    }

    public class UserServices : IUserServices
    {

        private readonly PostgresDbContext _dbContext;
        public UserServices(PostgresDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> GetUserById(int id)
        {
            return await _dbContext.User.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<User> AddUser(User oUser)
        {
            var result = _dbContext.User.Add(oUser);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }
    }
}
