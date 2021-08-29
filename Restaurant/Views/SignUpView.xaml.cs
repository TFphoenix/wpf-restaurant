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
    /// Interaction logic for SignUpView.xaml
    /// </summary>
    public partial class SignUpView : UserControl
    {
        public SignUpView()
        {
            InitializeComponent();
        }

        private void Back_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayRouter.DisplaySignIn();
        }

        public Tuple<string, string> GetPasswords()
        {
            return new Tuple<string, string>(Password1.Password, Password2.Password);
        }

        public void ClearPasswords()
        {
            Password1.Password = "";
            Password2.Password = "";
        }
    }
}
