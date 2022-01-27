using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Services;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTests.Services
{
    public class UserServiceTests
    {
        [Fact]
        public async Task register_async_should_invoke_add_async_on_user_repository()
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var jwtHandler = new Mock<IJwtHandler>();
            var mapperMock = new Mock<IMapper>();
            var userService = new UserService(userRepositoryMock.Object, jwtHandler.Object, mapperMock.Object);

            await userService.RegisterAsync(Guid.NewGuid(), "test@test.com", "test", "secret");

            userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once());
        }

        [Fact]
        public async Task when_invoking_getasync_it_should_invoke_get_async_on_user_repository()
        {
            var user = new User(Guid.NewGuid(), "user", "test", "test@test.com", "secret");
            var accountDto = new AccountDto
            {
                Id = user.Id,
                Role = user.Role,
                Email = user.Email,
                Name = user.Name
            };
            var userRepositoryMock = new Mock<IUserRepository>();
            var jwtHandler = new Mock<IJwtHandler>();
            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(x => x.Map<AccountDto>(user)).Returns(accountDto);
            var userService = new UserService(userRepositoryMock.Object, jwtHandler.Object, mapperMock.Object);
            userRepositoryMock.Setup(x => x.GetAsync(user.Id)).ReturnsAsync(user);

            var existingAccountDto = await userService.GetAccountAsync(user.Id);

            userRepositoryMock.Verify(x => x.GetAsync(user.Id), Times.Once());
            accountDto.Should().NotBeNull();
            accountDto.Email.Should().BeEquivalentTo(user.Email);
        }

    }
}
