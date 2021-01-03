using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FL_Note.Elements
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChooseColorLayout : StackLayout
	{
        public event EventHandler StopChooseColor;
        public event EventHandler ChooseColor;

        public ChooseColorLayout ()
		{
			InitializeComponent ();
		}

        void OnStopChooseColor(object sender, EventArgs e)
        {
            StopChooseColor?.Invoke(this, e);
        }

        void OnChooseColor(object sender, EventArgs e)
        {
            ChooseColor?.Invoke(sender, e);
        }
    }
}