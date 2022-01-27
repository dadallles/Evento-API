using System;
using System.Collections.Generic;
using System.Text;

namespace Evento.Core.Domain
{
    public class User : Entity
    {
        private static List<string> _roles = new List<string>
        {
            "user", "admin"
        };

        public string Role { get; protected set; }

        public string Name { get; protected set; }

        public string Email { get; protected set; }

        public string Password { get; protected set; }

        public DateTime CreatedAt { get; protected set; }


        protected User()
        {

        }

        public User(Guid id, string role, string name, string email, string password)
        {
            Id = id;
            SetRole(role);
            SetName(name);
            SetEmail(email);
            SetPassword(password);
            CreatedAt = DateTime.UtcNow;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Imie nie może być puste.");
            }
            Name = name;
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new Exception($"Mail nie może być pusty.");
            }
            Email = email;
        }

        public void SetRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new Exception($"Rola nie moze byc pusta.");
            }
            role = role.ToLowerInvariant();
            if (!_roles.Contains(role))
            {
                throw new Exception($"Uzytkownik nie może miec roli: '{role}'.");
            }
            Role = role;
        }

        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception($"Haslo nie moze byc puste.");
            }
            Password = password;
        }

    }
}
