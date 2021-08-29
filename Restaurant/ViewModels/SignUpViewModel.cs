using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Restaurant.Commands;
using Restaurant.Models;
using Restaurant.Models.Services;
using Restaurant.Views;

namespace Restaurant.ViewModels
{
    public class SignUpViewModel : ViewModel
    {
        private user _newUser;
        private readonly UserServices _userServices;

        public user NewUser
        {
            get => _newUser;
            set
            {
                if (_newUser != value)
                {
                    _newUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand CreateUserCommand { get; }

        public SignUpViewModel()
        {
            _userServices = new UserServices();
            NewUser = new user();

            CreateUserCommand = new RelayCommand(CreateUser);
        }

        private void CreateUser(object param)
        {
            var passwords = DisplayRouter.StartupWindowReference.SignUpView.GetPasswords();

            if (String.IsNullOrEmpty(NewUser.first_name) ||
                String.IsNullOrEmpty(NewUser.last_name) ||
                String.IsNullOrEmpty(NewUser.email) ||
                String.IsNullOrEmpty(NewUser.phone) ||
                String.IsNullOrEmpty(NewUser.address) ||
                String.IsNullOrEmpty(passwords.Item1) ||
                String.IsNullOrEmpty(passwords.Item2))
            {
                MessageBox.Show("All fields must be completed", "Can't create new user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!passwords.Item1.Equals(passwords.Item2))
            {
                MessageBox.Show("Passwords don't match", "Can't create new user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NewUser.password = passwords.Item1;

            try
            {
                _userServices.RegisterMethod(NewUser);
            }
            catch
            {
                MessageBox.Show("User already registered on this email address", "Can't create new user", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DisplayRouter.DisplaySignIn();
            MessageBox.Show($"User {NewUser.email} successfully created", "User successfully created", MessageBoxButton.OK, MessageBoxImage.Information);
            NewUser = new user();
            DisplayRouter.StartupWindowReference.SignUpView.ClearPasswords();
        }
    }
}
