using AuthApi.Core.Persistence;
using AuthApi.Core.Persistence.Entities;
using AuthApi.Core.Security;
using AuthApi.Modules.Users.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Modules.Users.Services
{
    public interface IUsersService
    {
        Task<List<UserResponseDto>> GetAllAsync();
        Task<UserResponseDto?> GetByIdAsync(int id);
        Task<UserResponseDto> CreateAsync(CreateUserDto dto);
        Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class UsersService : IUsersService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;

        public UsersService(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            return await _context.Users
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

        public async Task<UserResponseDto?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = _passwordService.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Dni = dto.Dni,
                Phone = dto.Phone,
                Address = dto.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserResponseDto> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) throw new KeyNotFoundException($"Usuario con id {id} no encontrado");

            if (dto.FirstName != null) user.FirstName = dto.FirstName;
            if (dto.LastName != null) user.LastName = dto.LastName;
            if (dto.Dni != null) user.Dni = dto.Dni;
            if (dto.Phone != null) user.Phone = dto.Phone;
            if (dto.Address != null) user.Address = dto.Address;
            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

            await _context.SaveChangesAsync();
            return MapToDto(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private static UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Dni = user.Dni,
                Phone = user.Phone,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
