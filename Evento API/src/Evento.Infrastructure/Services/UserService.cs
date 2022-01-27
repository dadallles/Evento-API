using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Evento.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly IMapper _mapper;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public UserService(IUserRepository userRepository, IJwtHandler jwtHandler, IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtHandler = jwtHandler;
            _mapper = mapper;
        }

        public async Task<AccountDto> GetAccountAsync(Guid userId)
        {
            Logger.Info("Catch account");
            var user = await _userRepository.GetOrFailAsync(userId);

            return _mapper.Map<AccountDto>(user);
        }

        public async Task RegisterAsync(Guid userId, string email, string name, string password, string role = "user")
        {
            Logger.Info("Register account");
            var user = await _userRepository.GetAsync(email);
            if(user != null)
            {
                throw new Exception($"Urzytkownik z mailem: '{email}' juz istnieje");
            }
            user = new User(userId, role, name, email, password);
            await _userRepository.AddAsync(user);
        }

        public async Task<TokenDto> LoginAsync(string email, string password)
        {
            Logger.Info("Log in account");
            var user = await _userRepository.GetAsync(email);
            if (user == null)
            {
                throw new Exception($"Bledne dane");
            }
            if(user.Password != password)
            {
                throw new Exception($"Bledne dane");
            }
            var jwt = _jwtHandler.CreateToken(user.Id, user.Role);

            return new TokenDto
            {
                Token = jwt.Token,
                Expires = jwt.Expires,
                Role = user.Role
            };
        }

        
    }
}
