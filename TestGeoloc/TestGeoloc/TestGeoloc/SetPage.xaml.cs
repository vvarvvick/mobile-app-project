using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace TestGeoloc
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetPage : ContentPage
    {
        public SetPage()
        {
            InitializeComponent();
            But_Time_A_Name.Text = Application.Current.Properties["countA_send"].ToString() + " Min";
            But_Time_S_Name.Text = ((int)Application.Current.Properties["minS_send"] / 60).ToString() + " Min";
        }

        public void But_Tim_A_Clicked(object sender, EventArgs e)
        {
            if (But_Time_A_Name.Text == "10 Min")
            {
                Application.Current.Properties["countA_send"] = 15;
                But_Time_A_Name.Text = "15 Min";
            }
            else if (But_Time_A_Name.Text == "15 Min")
            {
                Application.Current.Properties["countA_send"] = 30;
                But_Time_A_Name.Text = "30 Min";
            }
            else if (But_Time_A_Name.Text == "30 Min")
            {
                Application.Current.Properties["countA_send"] = 60;
                But_Time_A_Name.Text = "60 Min";
            }
            else if (But_Time_A_Name.Text == "60 Min")
            {
                Application.Current.Properties["countA_send"] = 10;
                But_Time_A_Name.Text = "10 Min";
            }
        }

        private void But_Tim_S_Clicked(object sender, EventArgs e)
        {
            if (But_Time_S_Name.Text == "10 Min")
            {
                Application.Current.Properties["minS_send"] = 900;
                But_Time_S_Name.Text = "15 Min";
            }
            else if (But_Time_S_Name.Text == "15 Min")
            {
                Application.Current.Properties["minS_send"] = 1800;
                But_Time_S_Name.Text = "30 Min";
            }
            else if (But_Time_S_Name.Text == "30 Min")
            {
                Application.Current.Properties["minS_send"] = 3600;
                But_Time_S_Name.Text = "60 Min";
            }
            else if (But_Time_S_Name.Text == "60 Min")
            {
                Application.Current.Properties["minS_send"] = 600;
                But_Time_S_Name.Text = "10 Min";
            }
        }

        private void But_Priv_Clicked(object sender, EventArgs e)
        {
            AppInfo.ShowSettingsUI();
        }

        private void But_Reset(object sender, EventArgs e)
        {
            Application.Current.Properties["countA_send"] = 10;
            Application.Current.Properties["minS_send"] = 600;
            File.Delete(Application.Current.Properties["_path_send"].ToString());
        }
    }
}