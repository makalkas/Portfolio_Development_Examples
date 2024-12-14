using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AmetekLabelPrinterApplication.Resources.Services
{
    public class UserService : IUserService
    {
        #region Declarations
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IExceptionService _exceptionService;
        private readonly IConfiguration _configuration;
        #endregion Declarations

        #region Constructors
        public UserService(
            AuthenticationStateProvider AuthenticationStateProvider,
            IExceptionService exceptionService,
            IConfiguration configuration
            )
        {
            _authenticationStateProvider = AuthenticationStateProvider;
            _authenticationStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            _exceptionService = exceptionService;
            _configuration = configuration;
            Task.Run(async () => await GetUserInformationAsync()).Wait();
        }
        #endregion Constructors

        #region Properties
        public string CurrentUser { get; private set; } = string.Empty;

        public bool IsLoggedIn { get; private set; }

        #endregion Properties

        #region Public Methods


        #endregion Public Methods

        #region Private Methods
        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            // Handle the state change
            task.ContinueWith(t =>
            {
                var user = t.Result.User;
                if (user.Identity != null && user.Identity.IsAuthenticated)
                {
                    this.IsLoggedIn = user.Identity?.IsAuthenticated ?? false;
                }
            });
        }

        private async Task GetUserInformationAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            ClaimsPrincipal? user = authState.User;
            if (user != null && user.Identity != null)
            {
                this.CurrentUser = FormatUserName(user.Identity.Name) ?? string.Empty;
                this.IsLoggedIn = user.Identity?.IsAuthenticated ?? false;
            }
        }

        private string FormatUserName(string? userName = "")
        {
            if (string.IsNullOrEmpty(userName)) return string.Empty;
            if (userName.Contains(@"\"))
            {
                string temp = userName.Substring(userName.LastIndexOf(@"\") + 1);
                userName = temp.Trim();
            }

            return userName;
        }
        #endregion Private Methods
    }
}
