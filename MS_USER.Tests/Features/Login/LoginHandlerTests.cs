using Microsoft.EntityFrameworkCore;
using MS_USER.Data;
using MS_USER.Features.Login;
using MS_USER.Models;

namespace MS_USER.Tests.Features.Login;

/// <summary>
/// Unit test untuk LoginHandler menggunakan xUnit.
///
/// MENGAPA xUnit?
/// - xUnit adalah framework testing paling modern dan populer di ekosistem .NET.
/// - Digunakan secara resmi oleh tim ASP.NET Core di Microsoft.
/// - Lebih ringan dibanding NUnit atau MSTest.
/// - Mendukung constructor injection (tidak perlu [SetUp] seperti NUnit).
/// - Setiap test method berjalan dalam instance class yang baru (isolasi sempurna).
///
/// STRUKTUR TEST (AAA Pattern):
/// - Arrange : Siapkan data dan kondisi awal.
/// - Act     : Jalankan method yang akan diuji.
/// - Assert  : Verifikasi hasil sesuai ekspektasi.
/// </summary>
public class LoginHandlerTests : IDisposable
{
    // In-Memory Database: tidak butuh koneksi SQL Server sungguhan.
    // Setiap test mendapat database bersih karena nama Guid berbeda-beda.
    private readonly AppDbContext _context;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _handler = new LoginHandler(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    // TEST 1 - Login berhasil dengan credential yang benar
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsSuccessResponse()
    {
        // Arrange
        _context.Users.Add(new Users
        {
            Id = 1, Username = "budi", Password = "password123",
            Email = "budi@mail.com", RoleId = 1, RowStatus = 1
        });
        await _context.SaveChangesAsync();

        var command = new LoginCommand { Username = "budi", Password = "password123" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Login berhasil", result.Message);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token!);
    }

    // TEST 2 - Password salah harus ditolak
    [Fact]
    public async Task Handle_WrongPassword_ReturnsFailureResponse()
    {
        // Arrange
        _context.Users.Add(new Users
        {
            Id = 2, Username = "budi", Password = "password123",
            Email = "budi@mail.com", RoleId = 1, RowStatus = 1
        });
        await _context.SaveChangesAsync();

        var command = new LoginCommand { Username = "budi", Password = "passwordSalah" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username atau password salah", result.Message);
        Assert.Null(result.Token);
    }

    // TEST 3 - Username tidak ditemukan (database kosong)
    [Fact]
    public async Task Handle_UserNotFound_ReturnsFailureResponse()
    {
        // Arrange - database kosong
        var command = new LoginCommand { Username = "tidakada", Password = "apapun" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username atau password salah", result.Message);
        Assert.Null(result.Token);
    }

    // TEST 4 - User non-aktif (RowStatus < 0) tidak boleh login
    [Fact]
    public async Task Handle_InactiveUser_ReturnsFailureResponse()
    {
        // Arrange
        _context.Users.Add(new Users
        {
            Id = 4, Username = "nonaktif", Password = "pass123",
            Email = "nonaktif@mail.com", RoleId = 1, RowStatus = -1
        });
        await _context.SaveChangesAsync();

        var command = new LoginCommand { Username = "nonaktif", Password = "pass123" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Username atau password salah", result.Message);
    }

    // TEST 5 - Token mengandung username dan roleId yang benar
    [Fact]
    public async Task Handle_ValidCredentials_TokenContainsUsernameAndRole()
    {
        // Arrange
        _context.Users.Add(new Users
        {
            Id = 5, Username = "admin", Password = "admin123",
            Email = "admin@mail.com", RoleId = 99, RowStatus = 1
        });
        await _context.SaveChangesAsync();

        var command = new LoginCommand { Username = "admin", Password = "admin123" };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Contains("user:admin", result.Token);
        Assert.Contains("role:99", result.Token);
    }

    // TEST 6 - Multiple user, hanya yang cocok yang bisa login
    [Fact]
    public async Task Handle_MultipleUsers_OnlyMatchingUserCanLogin()
    {
        // Arrange
        _context.Users.AddRange(
            new Users { Id = 6, Username = "alice", Password = "alice123", Email = "alice@mail.com", RoleId = 1, RowStatus = 1 },
            new Users { Id = 7, Username = "bob",   Password = "bob456",   Email = "bob@mail.com",   RoleId = 2, RowStatus = 1 }
        );
        await _context.SaveChangesAsync();

        var commandAlice    = new LoginCommand { Username = "alice", Password = "alice123" };
        var commandBobWrong = new LoginCommand { Username = "bob",   Password = "passwordSalah" };

        // Act
        var resultAlice    = await _handler.Handle(commandAlice,    CancellationToken.None);
        var resultBobWrong = await _handler.Handle(commandBobWrong, CancellationToken.None);

        // Assert
        Assert.True(resultAlice.Success,     "Alice harus berhasil login");
        Assert.False(resultBobWrong.Success, "Bob dengan password salah harus gagal");
    }
}
