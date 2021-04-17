using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MultiLanguages.Translator;
using Startapp.Client.Pages.Admin.Components;
using Startapp.Client.Services;
using Startapp.Shared.Helpers;
using Startapp.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Admin
{
    public class UsersBase : ComponentBase
    {
        [CascadingParameter] Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Inject] public IUserService UserService { get; set; }
        [Inject] public IRoleService RoleService { get; set; }

        public string Message { get; set; }
        public List<AppUser> Users { get; set; } = new List<AppUser>();
        public List<AppRole> Roles { get; set; } = new List<AppRole>();
        public MetaData MetaData { get; set; } = new MetaData();
        private PagingParameters PagingParameters = new PagingParameters();

        public string SearchTerm { get; set; } = "";

        public bool Filtred { get; set; } = false;
        public bool Loading { get; set; } = true;
        public bool DisableSearch { get; set; } = true;

        protected AddUserDialog AddUserDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PagingParameters.PageSize = 5;
            Message = string.Empty;
            await GetUsersAsync();
        }
        protected async Task SelectedPage(int page)
        {
            PagingParameters.PageNumber = page;
            await GetUsersAsync();
        }
        protected async void Search()
        {
            PagingParameters.SearchTerm = SearchTerm;
            Filtred = true; DisableSearch = true;
            await GetUsersAsync();
        }
        protected void SearchKeyup(ChangeEventArgs e)
        {
            SearchTerm = e.Value.ToString();

            DisableSearch = string.IsNullOrWhiteSpace(SearchTerm.Trim()) ? true : false;
        }

        protected void EnterKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                Search();
            }
        }

        protected async void Filter()
        {
            SearchTerm = "";
            PagingParameters.SearchTerm = SearchTerm;
            Filtred = false; DisableSearch = true;
            await GetUsersAsync();
        }

        private async Task GetUsersAsync()
        {
            string url = $"api/admin/users";
            Loading = true;
            Users.Clear();
            StateHasChanged();
            var pagingResponse = await UserService.GetPageAsync(PagingParameters, url);
            Roles = await RoleService.GetRolesAsync($"api/admin/all/roles");
            Users = pagingResponse.Items;
            if (Users.Count == 0)
            {
                Message = $"{Translate.Keys["SorryNo"]} {Translate.Keys["Users"]} {Translate.Keys["Found"]} ! ";
            }
            MetaData = pagingResponse.MetaData;
            Loading = false;
            StateHasChanged();

        }

        public async void AddUpdateUser_OnDialogClose()
        {
            await GetUsersAsync();
        }

        protected async Task AddUpdateUser(AppUser user)
        {
            var authenticationState = await AuthenticationStateTask;
            //if (authenticationState.User.Identity.Name != "Kevin")
            //{
                AddUserDialog.Show(user);
            //}
        }

        protected async Task DeleteUser(AppUser user)
        {
            var authenticationState = await AuthenticationStateTask;
            //if (authenticationState.User.Identity.Name != "Kevin")
            //{
                await UserService.DeleteAsync(user.Id);
                Users.Remove(user);
                StateHasChanged();
            //}
        }
    }
}
