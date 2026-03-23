// Interface
public interface IUserService
{
    void GetUser();
}

// Implementation
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger)
    {
        _logger = logger;
    }

    public void GetUser()
    {
        _logger.LogInformation("User fetched");
    }
}

// Consumer
public class UserController
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public void Execute()
    {
        _logger.LogInformation("Inside execute method");
        _userService.GetUser();
    }
}