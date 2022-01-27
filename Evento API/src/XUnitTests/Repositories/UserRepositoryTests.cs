using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.Repositories;
using Evento.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTests.Repositories
{
    public class UserRepositoryTests
    {
        [Fact]
        public async Task when_adding_newuser_it_should_be_added_correctly_to_the_collection()
        {
            var user = new User(Guid.NewGuid(), "user", "test", "test@test.com", "secret");
            IUserRepository repository = new UserRepository();

            await repository.AddAsync(user);

            var exestingUser = await repository.GetAsync(user.Id);
            Assert.Equal(user, exestingUser);
        }

      

    }
}
