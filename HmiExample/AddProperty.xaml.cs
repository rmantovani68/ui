#region Using
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
using System.Windows.Shapes;
#endregion

namespace HmiExample
{
    /// <summary>
    /// Interaction logic for AddProperty.xaml
    /// </summary>
    public partial class AddProperty : Window
    {
        public AddProperty()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void AddPropertyWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult == true)
            {
                /* controllo le condizioni per validare la property da aggiungere */
                if (!true)
                {
                    MessageBox.Show(string.Format("Poperty Path {0} non corretto", this.tbPropertyPath.Text.Trim()));
                    e.Cancel = true;
                }
            }
        }
    }
}
