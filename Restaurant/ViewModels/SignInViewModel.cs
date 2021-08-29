using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Restaurant.Commands;
using Restaurant.Models;
using Restaurant.Models.Services;

namespace Restaurant.ViewModels
{
    public class SignInViewModel : ViewModel
    {
        private string _errorMessage;
        private readonly UserServices _userServices;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public SignInViewModel()
        {
            ErrorMessage = "";
            _userServices = new UserServices();
        }

        public void Login(string email, string password)
        {
            try
            {
                ActiveSession.ConnectedUser = _userServices.LoginMethod(email, password);
                DisplayRouter.SwitchToAppWindow();
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }
    }
}
