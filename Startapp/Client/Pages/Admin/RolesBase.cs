using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MultiLanguages.Translator;
using Startapp.Client.Pages.Admin.Components;
using Startapp.Client.Services;
using Startapp.Shared.Helpers;
using Startapp.Shared.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Startapp.Client.Pages.Admin
{
    public class RolesBase : ComponentBase
    {
        [CascadingParameter] Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter] public ILanguageContainerService Translate { get; set; }

        [Inject] public IRoleService RoleService { get; set; }

        public string Message { get; set; }
        

        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public MetaData MetaData { get; set; } = new MetaData();
        private PagingParameters PagingParameters = new PagingParameters();

        public string SearchTerm { get; set; } = "";

        public bool Filtred { get; set; } = false;
        public bool Loading { get; set; } = true;
        public bool DisableSearch { get; set; } = true;

        protected AddRoleDialog AddRoleDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PagingParameters.PageSize = 5;
            Message = string.Empty;
            await GetRolesAsync();
        }
        protected async Task SelectedPage(int page)
        {
            PagingParameters.PageNumber = page;
            await GetRolesAsync();
        }
        protected async void Search()
        {
            PagingParameters.SearchTerm = SearchTerm;
            Filtred = true; DisableSearch = true;
            await GetRolesAsync();
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
            await GetRolesAsync();
        }

        private async Task GetRolesAsync()
        {
            string url = $"api/admin/roles";
            Loading = true;
            Roles.Clear();
            StateHasChanged();
            var pagingResponse = await RoleService.GetPageAsync(PagingParameters, url);
          
            Roles = pagingResponse.Items;
            if (Roles.Count == 0)
            {
                Message = $"{Translate.Keys["SorryNo"]} {Translate.Keys["Roles"]} {Translate.Keys["Found"]} ! ";
            }
            MetaData = pagingResponse.MetaData;
            Loading = false;
            StateHasChanged();

        }

        public async void AddUpdateRole_OnDialogClose()
        {
            await GetRolesAsync();
        }

        protected async Task AddUpdateRole(RoleViewModel role)
        {
            var authenticationState = await AuthenticationStateTask;
            //if (authenticationState.User.Identity.Name != "Kevin")
            //{
            AddRoleDialog.Show(role);
            //}
        }

        protected async Task DeleteRole(RoleViewModel role)
        {
            var authenticationState = await AuthenticationStateTask;
            //if (authenticationState.User.Identity.Name != "Kevin")
            //{
            await RoleService.DeleteAsync(role.Id);
            Roles.Remove(role);
            StateHasChanged();
            //}
        }
    }
}
