using Authy.AccountManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Authy
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(Services service)
        {
            InitializeComponent();
            Title = service.ToString();
        }


        public ProfilePage(Services service, string name, string image)
        {
            InitializeComponent();
            Title = service.ToString();
            ProfileImage.Source = image;
            LoginName.Text = name;
        }
    }
}
