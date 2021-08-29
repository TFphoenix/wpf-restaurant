using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Restaurant.Models;

namespace Restaurant.Views
{
    /// <summary>
    /// Interaction logic for SigninView.xaml
    /// </summary>
    public partial class SignInView : UserControl
    {
        public SignInView()
        {
            InitializeComponent();
        }

        private void SignUp_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayRouter.DisplaySignUp();
        }

        private void ContinueWithoutUser_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayRouter.SwitchToAppWindow();
        }

        private void SignIn_OnClick(object sender, RoutedEventArgs e)
        {
            SignInVM.Login(Email.Text, Password.Password);
        }
    }
}
