#region Using

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;

using log4net;
using MDS;
using MDS.Client;
using MDS.Communication.Messages;

using OMS.Core.Communication;

using System.Diagnostics;

#endregion

namespace HmiExample
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public MainWindow()
        {
            Logger.InfoFormat("{0} application ready",Controller.Instance.ApplicationName);

            /* necessario per il binding in xaml */
            this.DataContext = Controller.Instance.model;    

            InitializeComponent();
        }


        private void btnAddProperty_Click(object sender, RoutedEventArgs e)
        {
            // Create the startup window
            var AddPropertyWnd = new AddProperty();
            // Show the window
            if (AddPropertyWnd.ShowDialog() == true)
            {
                string path = AddPropertyWnd.tbPropertyPath.Text;
                Controller.Instance.SubscribeProperty(new PropertyItem() { Path = path });
            }
        }

        private void btnDeleteProperty_Click(object sender, RoutedEventArgs e)
        {
            if (listviewProperties.SelectedItem != null)
            {
                var prop = listviewProperties.SelectedItem as PropertyItem;

                Controller.Instance.RemoveProperty(prop);
            }
        }

        private void btnPropertySetValue_Click(object sender, RoutedEventArgs e)
        {
            if (listviewProperties.SelectedItem != null)
            {
                var prop = listviewProperties.SelectedItem as PropertyItem;
            }
        }

        // Method to handle the Window.Closing event.
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // disconnettere 
            Controller.Instance.Close();
        }

        private void listviewProperties_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetListButtonsState(listviewProperties.SelectedItem as PropertyItem);
        }

        private void SetListButtonsState(PropertyItem prop)
        {
            if (prop == null)
            {
                btnDeleteProperty.IsEnabled = false;
            }
            else
            {
                btnDeleteProperty.IsEnabled = true;
            }
        }

    }
}


