using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace ExampleApp.Custom
{
    public class AuthorizationPolicyConvention : IActionModelConvention
    {
        private readonly string _controllerName;
        private readonly string _actionName;
        private readonly IAuthorizeData _attr = new AuthData();

        public AuthorizationPolicyConvention(string controller,
            string action = null,
            string policy = null,
            string roles = null,
            string schemes = null)
        {
            _controllerName = controller;
            _actionName = action;
            _attr.Policy = policy;
            _attr.Roles = roles;
            _attr.AuthenticationSchemes = schemes;
        }

        private class AuthData : IAuthorizeData
        {
            public string AuthenticationSchemes { get; set; }

            public string Policy { get; set; }

            public string Roles { get; set; }
        }

        public void Apply(ActionModel action)
        {
            if (_controllerName == action.Controller.ControllerName && (_actionName == null || _actionName == action.ActionName))
            {
                foreach (var selector in action.Selectors)
                {
                    selector.EndpointMetadata.Add(_attr);
                }
            }
        }
    }
}
