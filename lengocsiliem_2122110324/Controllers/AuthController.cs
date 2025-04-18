using lengocsiliem_2122110324.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using lengocsiliem_2122110324.Data;
using lengocsiliem_2122110324.Model;
using Microsoft.AspNetCore.Identity.Data;
using lengocsiliem_2122110324.Request;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public AuthController(IConfiguration configuration, AppDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] lengocsiliem_2122110324.Request.RegisterRequest request)
    {
        // Kiểm tra input
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        // Kiểm tra email đã tồn tại
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == request.Email);
        if (existingUser != null)
        {
            return BadRequest(new { message = "Email already exists." });
        }

        // Mã hóa mật khẩu
        var user = new User
        {
            Name = request.Name,
            Email = request.Email
        };

        var passwordHasher = new PasswordHasher<User>();
        user.Password = passwordHasher.HashPassword(user, request.Password);

        // Gán UserRoles
        user.UserRoles = request.Roles.Select(roleId => new UserRoles
        {
            RoleId = roleId,
            User = user
        }).ToList();

        _context.Users.Add(user);
        _context.SaveChanges();

        var token = GenerateJwtToken(user.Email);

        return Ok(new
        {
            message = "User registered successfully",
            token = token
        });
    }



    [HttpPost("login")]
    public IActionResult Login([FromBody] UserDTO user)
    {
        // Kiểm tra input đơn giản
        if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        // Kiểm tra email và mật khẩu
        var existingUser = _context.Users.FirstOrDefault(u => u.Email == user.Email);
        if (existingUser == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var passwordHasher = new PasswordHasher<UserDTO>();
        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, existingUser.Password, user.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = GenerateJwtToken(existingUser.Email);

        return Ok(new
        {
            message = "Login successful",
            token = token
        });
    }

    private string GenerateJwtToken(string email)
    {
        var user = _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Email == email);

        if (user == null)
            throw new Exception("User not found.");

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, user.Name)
    };

        // Thêm các role vào claims
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
        }

        var jwtKey = _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "lengocsiliem",
            audience: "your-audience",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


}
